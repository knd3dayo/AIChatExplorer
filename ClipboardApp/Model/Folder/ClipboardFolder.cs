using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using ClipboardApp.Common;
using ClipboardApp.Factory;
using ClipboardApp.Model.Item;
using LiteDB;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.Folder;
using PythonAILib.PythonIF;
using LibUIPythonAI.Resource;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folder {
    public partial class ClipboardFolder : ContentFolder {


        //--------------------------------------------------------------------------------
        // コンストラクタ
        public ClipboardFolder() {
            IsAutoProcessEnabled = true;
        }

        protected ClipboardFolder(ClipboardFolder? parent, string folderName) {

            ParentId = parent?.Id ?? ObjectId.Empty;
            FolderName = folderName;
            // 親フォルダがnullの場合は、FolderTypeをNormalに設定
            FolderType = parent?.FolderType ?? FolderTypeEnum.Normal;

            IsAutoProcessEnabled = true;

        }

        public override void Save() {
            Save<ClipboardFolder, ClipboardItem>();
        }
        // 削除
        public override void Delete() {
            Delete<ClipboardFolder, ClipboardItem>();
        }

        // 親フォルダ
        public override ContentFolder? GetParent() {
            return GetParent<ClipboardFolder>();
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
        public override void AddItem(ContentItem item, bool applyGlobalAutoAction = false , Action<ContentItem>? afterUpdate = null) {
            base.AddItem(item, applyGlobalAutoAction, afterUpdate);

            // 自動処理を適用
            if (IsAutoProcessEnabled) {
                LogWrapper.Info(CommonStringResources.Instance.ApplyAutoProcessing);
                ContentItem? result = AutoProcessRuleController.ApplyFolderAutoAction(item);
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
                    ClipboardController.ProcessClipboardItem(item, (processedItem) => {
                        // 自動処理後のアイテムを保存
                        item.Save();
                    });
                }
            }
        }
        #endregion

        #region システムのクリップボードへ貼り付けられたアイテムに関連する処理
        public virtual void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ContentItem> _afterClipboardChanged) {

            // Get the cut/copied text.
            List<ClipboardItem> items = CreateClipboardItem(this, e);

            foreach (var item in items) {
                // Process clipboard clipboardItem
                ClipboardController.ProcessClipboardItem(item, _afterClipboardChanged);
            }
        }
        #endregion


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

