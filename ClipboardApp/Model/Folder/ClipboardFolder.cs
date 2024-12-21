using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using ClipboardApp.Factory;
using ClipboardApp.Model.Search;
using LiteDB;
using NetOffice.OutlookApi;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.PythonIF;
using QAChat.Resource;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folder
{
    public partial class ClipboardFolder : ContentFolder {


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
                return Tools.ConcatenateFileSystemPath(parent.FolderPath, FolderName);
            }
        }

        // アイテム LiteDBには保存しない。
        [BsonIgnore]
        public override List<T> GetItems<T>() {
            List<ClipboardItem> _items = [];
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

        public override ClipboardFolder CreateChild(string folderName) {
            ClipboardFolder child = new(this, folderName);
            return child;
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
                ClipboardItem? result = clipboardItem.ApplyAutoProcess();
                if (result == null) {
                    // 自動処理で削除または移動された場合は何もしない
                    LogWrapper.Info(CommonStringResources.Instance.ItemsDeletedOrMovedByAutoProcessing);
                    return;
                }
                result.Save();
                LogWrapper.Info(CommonStringResources.Instance.AutoProcessingApplied);
            }
        }

        #region 検索
        // ClipboardItemを検索する。
        public IEnumerable<ClipboardItem> SearchItems(SearchCondition searchCondition) {
            // 結果を格納するIEnumerable<ContentItem>を作成
            IEnumerable<ClipboardItem> result = [];
            // 検索条件が空の場合は、結果を返す
            if (searchCondition.IsEmpty()) {
                return result;
            }

            // folder内のアイテムを保持するコレクションを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetItemCollection<ClipboardItem>();
            var clipboardItems = collection.Find(x => x.CollectionId == this.Id).OrderByDescending(x => x.UpdatedAt);
            // Filterの結果を結果に追加
            result = Filter(clipboardItems, searchCondition);

            // サブフォルダを含む場合は、対象フォルダとそのサブフォルダを検索
            if (searchCondition.IsIncludeSubFolder) {
                // 対象フォルダの子フォルダを取得
                var folderCollection = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>();
                var childFolders = folderCollection.Find(x => x.ParentId == this.Id).OrderBy(x => x.FolderName);
                foreach (var childFolder in childFolders) {
                    // サブフォルダのアイテムを検索
                    var subFolderResult = childFolder.SearchItems(searchCondition);
                    // Filterの結果を結果に追加
                    result = result.Concat(subFolderResult);
                }
            }
            return result;
        }

        public IEnumerable<ClipboardItem> Filter(IEnumerable<ClipboardItem> liteCollection, SearchCondition searchCondition) {
            if (searchCondition.IsEmpty()) {
                return liteCollection;
            }

            var results = liteCollection;
            // SearchConditionの内容に従ってフィルタリング
            if (string.IsNullOrEmpty(searchCondition.Description) == false) {
                if (searchCondition.ExcludeDescription) {
                    results = results.Where(x => x.Description != null && x.Description.Contains(searchCondition.Description) == false);
                } else {
                    results = results.Where(x => x.Description != null && x.Description.Contains(searchCondition.Description));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Content) == false) {
                if (searchCondition.ExcludeContent) {
                    results = results.Where(x => x.Content != null && x.Content.Contains(searchCondition.Content) == false);
                } else {
                    results = results.Where(x => x.Content != null && x.Content.Contains(searchCondition.Content));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Tags) == false) {
                if (searchCondition.ExcludeTags) {
                    results = results.Where(x => x.Tags != null && x.Tags.Contains(searchCondition.Tags) == false);
                } else {
                    results = results.Where(x => x.Tags != null && x.Tags.Contains(searchCondition.Tags));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationName) == false) {
                if (searchCondition.ExcludeSourceApplicationName) {
                    results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(searchCondition.SourceApplicationName) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(searchCondition.SourceApplicationName));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationTitle) == false) {
                if (searchCondition.ExcludeSourceApplicationTitle) {
                    results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle));
                }
            }
            if (searchCondition.EnableStartTime) {
                results = results.Where(x => x.CreatedAt > searchCondition.StartTime);
            }
            if (searchCondition.EnableEndTime) {
                results = results.Where(x => x.CreatedAt < searchCondition.EndTime);
            }
            results = results.OrderByDescending(x => x.UpdatedAt);

            return results;
        }
        #endregion

        #region マージ
        // 指定されたフォルダの中のSourceApplicationTitleが一致するアイテムをマージするコマンド
        public void MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItem newItem) {
            // SourceApplicationNameが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationName)) {
                return;
            }
            // NewItemのSourceApplicationTitleが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationTitle)) {
                return;
            }
            List<ClipboardItem> items = GetItems<ClipboardItem>();
            if (items.Count == 0) {
                return;
            }
            List<ClipboardItem> sameTitleItems = [];
            // マージ先のアイテムのうち、SourceApplicationTitleとSourceApplicationNameが一致するアイテムを取得
            foreach (var item in items) {
                if (newItem.SourceApplicationTitle == item.SourceApplicationTitle
                    && newItem.SourceApplicationName == item.SourceApplicationName) {
                    // TypeがTextのアイテムのみマージ
                    if (item.ContentType == PythonAILib.Model.File.ContentTypes.ContentItemTypes.Text) {
                        sameTitleItems.Add(item);
                    }
                }
            }
            newItem.MergeItems(sameTitleItems);
        }

        // 指定されたフォルダの全アイテムをマージするコマンド
        public void MergeItems(ClipboardItem item) {
            item.MergeItems(GetItems< ClipboardItem>());

        }
        #endregion

        #region エクスポート/インポート
        // フォルダ内のアイテムをJSON形式でExport
        public void ExportItemsToJson(string fileName) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            string jsonString = System.Text.Json.JsonSerializer.Serialize((Func<List<ClipboardItem>>)GetItems<ClipboardItem>, options);

            File.WriteAllText(fileName, jsonString);

        }

        //exportしたJSONファイルをインポート
        public void ImportItemsFromJson(string json) {
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
                ClipboardItem? item = ClipboardItem.FromJson(jsonString);

                if (item == null) {
                    continue;
                }
                item.CollectionId = Id;
                //保存
                item.Save();
            }
        }

        // --- Export/Import
        public void ExportToExcel(string fileName, List<ExportImportItem> items) {
            // PythonNetの処理を呼び出す。
            List<List<string>> data = [];
            // ClipboardItemのリスト要素毎に処理を行う
            foreach (var clipboardItem in GetItems< ClipboardItem>()) {
                List<string> row = [];
                bool exportTitle = items.FirstOrDefault(x => x.Name == "Title")?.IsChecked ?? false;
                if (exportTitle) {
                    row.Add(clipboardItem.Description);
                }
                bool exportText = items.FirstOrDefault(x => x.Name == "Text")?.IsChecked ?? false;
                if (exportText) {
                    row.Add(clipboardItem.Content);
                }
                // PromptItemのリスト要素毎に処理を行う
                foreach (var promptItem in items.Where(x => x.IsPromptItem)) {
                    if (promptItem.IsChecked) {
                        string promptResult = clipboardItem.PromptChatResult.GetTextContent(promptItem.Name);
                        row.Add(promptResult);
                    }
                }

                data.Add(row);
            }
            CommonDataTable dataTable = new(data);

            PythonExecutor.PythonAIFunctions.ExportToExcel(fileName, dataTable);

        }

        public void ImportFromExcel(string fileName, List<ExportImportItem> items, bool executeAutoProcess) {

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
                    // システム共通自動処理を適用
                    FolderManager.ProcessClipboardItem(item, (processedItem) => {
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
            List<ClipboardItem> items = FolderManager.CreateClipboardItem(this, e);

            foreach (var item in items) {
                // Process clipboard clipboardItem
                FolderManager.ProcessClipboardItem(item, _afterClipboardChanged);
            }
        }
        #endregion
    }
}

