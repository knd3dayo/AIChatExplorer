using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using ClipboardApp.Factory;
using LiteDB;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Prompt;
using PythonAILib.PythonIF;
using QAChat.Resource;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folder {
    public partial class ClipboardFolder : ContentFolder {

        public override void Save() {
            Save<ClipboardFolder, ClipboardItem>();
        }
        // 削除
        public override void Delete() {
            Delete<ClipboardFolder, ClipboardItem>();
        }

        //--------------------------------------------------------------------------------
        // コンストラクタ
        public ClipboardFolder() { }

        protected ClipboardFolder(ClipboardFolder? parent, string folderName) {

            ParentId = parent?.Id ?? ObjectId.Empty;
            FolderName = folderName;
            // 親フォルダがnullの場合は、FolderTypeをNormalに設定
            FolderType = parent?.FolderType ?? FolderTypeEnum.Normal;
            // 親フォルダのAutoProcessEnabledを継承
            IsAutoProcessEnabled = parent?.IsAutoProcessEnabled ?? true;

        }

        // フォルダの種類
        public FolderTypeEnum FolderType { get; set; } = FolderTypeEnum.Normal;

        // AutoProcessを有効にするかどうか
        public bool IsAutoProcessEnabled { get; set; } = true;

        // AutoProcessRuleのIdのリスト
        public List<ObjectId> AutoProcessRuleIds { get; set; } = [];

        // フォルダの絶対パス
        public override string FolderPath {
            get {
                ClipboardFolder? parent = (ClipboardFolder?)ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>().FindById(ParentId);
                if (parent == null) {
                    return FolderName;
                }
                return $"{parent.FolderPath}/{FolderName}";
            }
        }

        // アイテム LiteDBには保存しない。
        [BsonIgnore]
        public override List<T> GetItems<T>() {
            List<ContentItem> _items = [];
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // 通常のフォルダの場合で、GlobalSearchConditionが設定されている場合
            if (FolderManager.GlobalSearchCondition.SearchCondition != null && FolderManager.GlobalSearchCondition.SearchCondition.IsEmpty() == false) {
                _items = [.. SearchItems(FolderManager.GlobalSearchCondition.SearchCondition).OrderByDescending(x => x.UpdatedAt)];

            } else {
                // 通常のフォルダの場合で、GlobalSearchConditionが設定されていない場合
                _items = [.. ClipboardDatabaseController.GetItemCollection<ClipboardItem>().Find(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt)];
            }

            return _items.Cast<T>().ToList();
        }

        // アイテムを追加する処理
        public override void AddItem(ContentItem item) {
            if (item is not ClipboardItem) {
                LogWrapper.Error("Item is not ClipboardItem");
                return;
            }
            ClipboardItem clipboardItem = (ClipboardItem)item;
            base.AddItem(clipboardItem);

            // 自動処理を適用
            if (IsAutoProcessEnabled) {
                LogWrapper.Info(CommonStringResources.Instance.ApplyAutoProcessing);
                ContentItem? result = clipboardItem.ApplyAutoProcess();
                if (result == null) {
                    // 自動処理で削除または移動された場合は何もしない
                    LogWrapper.Info(CommonStringResources.Instance.ItemsDeletedOrMovedByAutoProcessing);
                    return;
                }
                result.Save();
                LogWrapper.Info(CommonStringResources.Instance.AutoProcessingApplied);
            }
        }
        public override ClipboardFolder CreateChild(string folderName) {
            ClipboardFolder child = new(this, folderName);
            return child;
        }


        #region マージ
        #endregion

        #region エクスポート/インポート


        //exportしたJSONファイルをインポート
        public override void ImportItemsFromJson(string json) {
            JsonNode? node = JsonNode.Parse(json);
            if (node == null) {
                LogWrapper.Error(CommonStringResources.Instance.FailedToParseJSONString);
                return;
            }
            JsonArray? jsonArray = node as JsonArray;
            if (jsonArray == null) {
                LogWrapper.Error(CommonStringResources.Instance.FailedToParseJSONString);
                return;
            }

            foreach (JsonObject? jsonValue in jsonArray.Cast<JsonObject?>()) {
                if (jsonValue == null) {
                    continue;
                }
                string jsonString = jsonValue.ToString();
                ClipboardItem? item = ClipboardItem.FromJson<ClipboardItem>(jsonString);

                if (item == null) {
                    continue;
                }
                item.CollectionId = Id;
                //保存
                item.Save();
            }
        }

        public override void ImportFromExcel(string fileName, List<ExportImportItem> items, bool executeAutoProcess) {

            // PythonNetの処理を呼び出す。
            CommonDataTable data = PythonExecutor.PythonAIFunctions.ImportFromExcel(fileName);
            if (data == null) {
                return;
            }
            bool importTitle = items.FirstOrDefault(x => x.Name == "Title")?.IsChecked ?? false;
            bool importText = items.FirstOrDefault(x => x.Name == "Text")?.IsChecked ?? false;

            foreach (var row in data.Rows) {
                ClipboardItem item = new(Id);
                // importTitleと、importTextがTrueの場合は、row[0]をTitle、row[1]をContentに設定。Row.Countが足りない場合は空文字を設定
                if (importTitle && importText) {
                    item.Description = row.Count > 0 ? row[0] : "";
                    item.Content = row.Count > 1 ? row[1] : "";
                } else if (importTitle) {
                    item.Description = row.Count > 0 ? row[0] : "";
                } else if (importText) {
                    item.Content = row.Count > 0 ? row[0] : "";
                }
                item.Save();
                if (executeAutoProcess) {
                    // システム共通自動処理を適用s
                    ProcessClipboardItem(item, (processedItem) => {
                        // 自動処理後のアイテムを保存
                        item.Save();
                    });
                }
            }
        }
        #endregion

        #region システムのクリップボードへ貼り付けられたアイテムに関連する処理
        public virtual void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ClipboardItem> _afterClipboardChanged) {

            // Get the cut/copied text.
            List<ClipboardItem> items = CreateClipboardItem(this, e);

            foreach (var item in items) {
                // Process clipboard clipboardItem
                ProcessClipboardItem(item, _afterClipboardChanged);
            }
        }
        #endregion


        /// Process clipboard item 
        /// <summary>
        /// Process clipboard item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_afterClipboardChanged"></param>
        public static void ProcessClipboardItem(ClipboardItem item, Action<ClipboardItem> _afterClipboardChanged) {

            // Execute in a separate thread
            Task.Run(() => {
                StatusText statusText = Tools.StatusText;
                MainUITask.Run(() => {
                    statusText.InProgressText = CommonStringResources.Instance.AutoProcessing;
                    statusText.IsInProgress = true;
                });
                try {
                    // Apply automatic processing
                    Task<ClipboardItem?> updatedItemTask = ApplyAutoAction(item);
                    if (updatedItemTask.Result == null) {
                        // If the item is ignored, return
                        return;
                    }
                    // Notify the completion of processing
                    _afterClipboardChanged(updatedItemTask.Result);

                } catch (Exception ex) {
                    LogWrapper.Error($"{CommonStringResources.Instance.AddItemFailed}\n{ex.Message}\n{ex.StackTrace}");

                } finally {
                    MainUITask.Run(() => {
                        statusText.IsInProgress = false;
                    });
                }
            });
        }
        /// <summary>
        /// Apply automatic processing
        /// </summary>
        /// <param name="item"></param>
        /// <param name="image"></param>
        private static async Task<ClipboardItem?> ApplyAutoAction(ClipboardItem item) {
            // ★TODO Implement processing based on automatic processing rules.
            // 指定した行数以下のテキストアイテムは無視
            int lineCount = item.Content.Split('\n').Length;
            if (item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text && lineCount <= ClipboardAppConfig.Instance.IgnoreLineCount) {
                return null;
            }

            // ★TODO Implement processing based on automatic processing rules.
            // If AutoMergeItemsBySourceApplicationTitle is set, automatically merge items
            if (ClipboardAppConfig.Instance.AutoMergeItemsBySourceApplicationTitle) {
                LogWrapper.Info(CommonStringResources.Instance.AutoMerge);
                FolderManager.RootFolder.MergeItemsBySourceApplicationTitleCommandExecute(item);
            }
            // If AutoFileExtract is set, extract files
            if (ClipboardAppConfig.Instance.AutoFileExtract && item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files) {
                string text = PythonExecutor.PythonAIFunctions.ExtractFileToText(item.FilePath);
                item.Content += "\n" + text;
            }
            if (item.IsImage() && item.Image != null) {
                // ★TODO Implement processing based on automatic processing rules.
                // If AutoExtractImageWithPyOCR is set, perform OCR
                if (ClipboardAppConfig.Instance.AutoExtractImageWithPyOCR) {
                    string extractImageText = PythonExecutor.PythonMiscFunctions.ExtractTextFromImage(item.Image, ClipboardAppConfig.Instance.TesseractExePath);
                    item.Content += "\n" + extractImageText;
                    LogWrapper.Info(CommonStringResources.Instance.OCR);

                } else if (ClipboardAppConfig.Instance.AutoExtractImageWithOpenAI) {

                    LogWrapper.Info(CommonStringResources.Instance.AutoExtractImageText);
                    item.ExtractImageWithOpenAI();
                }
            }

            // ★TODO Implement processing based on automatic processing rules.
            var task1 = Task.Run(() => {
                // If AUTO_TAG is set, automatically set the tags
                if (ClipboardAppConfig.Instance.AutoTag) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoSetTag);
                    ClipboardItem.CreateAutoTags(item);
                }
            });
            var task2 = Task.Run(() => {
                // If AUTO_DESCRIPTION is set, automatically set the DisplayText
                if (ClipboardAppConfig.Instance.AutoDescription) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoSetTitle);
                    ClipboardItem.CreateAutoTitle(item);

                } else if (ClipboardAppConfig.Instance.AutoDescriptionWithOpenAI) {

                    LogWrapper.Info(CommonStringResources.Instance.AutoSetTitle);
                    item.CreateAutoTitleWithOpenAI();
                }
            });
            var task3 = Task.Run(() => {
                // 背景情報
                if (ClipboardAppConfig.Instance.AutoBackgroundInfo) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoSetBackgroundInfo);
                    item.CreateAutoBackgroundInfo();
                }
            });
            var task4 = Task.Run(() => {
                // サマリー
                if (ClipboardAppConfig.Instance.AutoSummary) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoCreateSummary);
                    item.CreateChatResult(SystemDefinedPromptNames.SummaryGeneration.ToString());
                }
            });
            var task5 = Task.Run(() => {
                // Tasks
                if (ClipboardAppConfig.Instance.AutoGenerateTasks) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoCreateTaskList);
                    item.CreateChatResult(SystemDefinedPromptNames.TasksGeneration.ToString());
                }
            });
            var task6 = Task.Run(() => {
                // Tasks
                if (ClipboardAppConfig.Instance.AutoDocumentReliabilityCheck) {
                    LogWrapper.Info(CommonStringResources.Instance.AutoCheckDocumentReliability);
                    item.CheckDocumentReliability();
                }
            });

            await Task.WhenAll(task1, task2, task3, task4, task5, task6);

            return item;
        }


        /// <summary>
        /// Set application information from ClipboardChangedEventArgs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sender"></param>
        public static void SetApplicationInfo(ClipboardItem item, ClipboardChangedEventArgs sender) {
            item.SourceApplicationName = sender.SourceApplication.Name;
            item.SourceApplicationTitle = sender.SourceApplication.Title;
            item.SourceApplicationID = sender.SourceApplication.ID;
            item.SourceApplicationPath = sender.SourceApplication.Path;
        }

        /// Create ContentItem
        public static List<ClipboardItem> CreateClipboardItem(
            ClipboardFolder clipboardFolder, ClipboardChangedEventArgs e) {

            List<ClipboardItem> result = [];

            PythonAILib.Model.File.ContentTypes.ContentItemTypes contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text;
            if (e.ContentType == WK.Libraries.SharpClipboardNS.SharpClipboard.ContentTypes.Text) {
                contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text;
            } else if (e.ContentType == WK.Libraries.SharpClipboardNS.SharpClipboard.ContentTypes.Files) {
                contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files;
            } else if (e.ContentType == WK.Libraries.SharpClipboardNS.SharpClipboard.ContentTypes.Image) {
                contentTypes = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Image;
            } else if (e.ContentType == WK.Libraries.SharpClipboardNS.SharpClipboard.ContentTypes.Other) {
                return result;
            } else {
                return result;
            }

            // If ContentType is Text, set text data
            if (contentTypes == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text) {
                ClipboardItem item = new(clipboardFolder.Id) {
                    ContentType = contentTypes
                };
                SetApplicationInfo(item, e);
                item.Content = (string)e.Content;
                result.Add(item);
                return result;
            }

            // If ContentType is BitmapImage, set image data
            if (contentTypes == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Image) {
                ClipboardItem item = new(clipboardFolder.Id) {
                    ContentType = contentTypes
                };
                SetApplicationInfo(item, e);
                System.Drawing.Image image = (System.Drawing.Image)e.Content;
                // byte
                item.CachedBase64String = PythonAILib.Model.File.ContentTypes.GetBase64StringFromImage(image);
                result.Add(item);
                return result;
            }

            // If ContentType is Files, set file data
            if (contentTypes == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files) {
                string[] files = (string[])e.Content;

                // Get the cut/copied file/files.
                for (int i = 0; i < files.Length; i++) {
                    ClipboardItem item = new(clipboardFolder.Id) {
                        ContentType = contentTypes
                    };
                    SetApplicationInfo(item, e);
                    item.FilePath = files[i];
                    item.LastModified = new System.IO.FileInfo(item.FilePath).LastWriteTime.Ticks;
                    result.Add(item);
                }
                return result;
            }
            return result;
        }


        // フォルダ内のアイテムをJSON形式でExport
        public void ExportItemsToJson(string fileName) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            string jsonString = System.Text.Json.JsonSerializer.Serialize(GetItems<ClipboardItem>, options);

            System.IO.File.WriteAllText(fileName, jsonString);

        }

    }
}

