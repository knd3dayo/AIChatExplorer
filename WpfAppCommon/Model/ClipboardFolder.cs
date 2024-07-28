using System.IO;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Windows;
using LiteDB;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using WK.Libraries.SharpClipboardNS;
using WpfAppCommon.Factory;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace WpfAppCommon.Model {
    public class ClipboardFolder {

        public enum FolderTypeEnum {
            Normal,
            Search,
            ImageCheck,
            Chat,
        }

        public class RootFolderInfo {

            public string FolderName { get; set; } = "";
            public LiteDB.ObjectId Id { get; set; } = ObjectId.Empty;

            public LiteDB.ObjectId FolderId { get; set; } = ObjectId.Empty;

        }

        public static readonly string CLIPBOARD_ROOT_FOLDER_NAME = "クリップボード";
        public static readonly string SEARCH_ROOT_FOLDER_NAME = "検索フォルダ";
        public static readonly string CHAT_ROOT_FOLDER_NAME = "チャット履歴";
        public static readonly string IMAGECHECK_ROOT_FOLDER_NAME = "画像チェック";


        //--------------------------------------------------------------------------------
        // コンストラクタ
        public ClipboardFolder() {
        }

        protected ClipboardFolder(ClipboardFolder? parent, string folderName) {

            ParentId = parent?.Id ?? ObjectId.Empty;
            FolderName = folderName;
            // 親フォルダがnullの場合は、FolderTypeをNormalに設定
            FolderType = parent?.FolderType ?? FolderTypeEnum.Normal;
            // 親フォルダのAutoProcessEnabledを継承
            IsAutoProcessEnabled = parent?.IsAutoProcessEnabled ?? true;

        }

        // アプリ共通の検索条件
        public static SearchRule GlobalSearchCondition { get; set; } = new();

        //--------------------------------------------------------------------------------
        public static ClipboardFolder RootFolder {
            get {
                ClipboardFolder? rootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolder(CLIPBOARD_ROOT_FOLDER_NAME);
                if (rootFolder == null) {
                    rootFolder = new() {
                        FolderName = CLIPBOARD_ROOT_FOLDER_NAME,
                        IsRootFolder = true,
                        IsAutoProcessEnabled = true
                    };
                    rootFolder.Save();
                }
                // 既にRootFolder作成済みの環境のための措置
                rootFolder.IsRootFolder = true;
                return rootFolder;
            }
        }

        public static ClipboardFolder SearchRootFolder {
            get {
                ClipboardFolder? searchRootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolder(SEARCH_ROOT_FOLDER_NAME);
                if (searchRootFolder == null) {
                    searchRootFolder = new ClipboardFolder {
                        FolderName = SEARCH_ROOT_FOLDER_NAME,
                        FolderType = FolderTypeEnum.Search,
                        IsRootFolder = true,
                        // 自動処理を無効にする
                        IsAutoProcessEnabled = false
                    };
                    searchRootFolder.Save();
                }
                // 既にSearchRootFolder作成済みの環境のための措置
                searchRootFolder.IsRootFolder = true;
                searchRootFolder.FolderType = FolderTypeEnum.Search;
                return searchRootFolder;
            }
        }


        public static ClipboardFolder ImageCheckRootFolder {
            get {
                ClipboardFolder? searchRootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolder(IMAGECHECK_ROOT_FOLDER_NAME);
                if (searchRootFolder == null) {
                    searchRootFolder = new ClipboardFolder {
                        FolderName = IMAGECHECK_ROOT_FOLDER_NAME,
                        FolderType = FolderTypeEnum.ImageCheck,
                        IsRootFolder = true,
                        // 自動処理を無効にする
                        IsAutoProcessEnabled = false

                    };
                    searchRootFolder.Save();
                }
                // 既にSearchRootFolder作成済みの環境のための措置
                searchRootFolder.IsRootFolder = true;
                return searchRootFolder;
            }
        }

        public static ClipboardFolder ChatRootFolder {
            get {
                ClipboardFolder? chatRootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolder(CHAT_ROOT_FOLDER_NAME);
                if (chatRootFolder == null) {
                    chatRootFolder = new ClipboardFolder {
                        FolderName = CHAT_ROOT_FOLDER_NAME,
                        FolderType = FolderTypeEnum.Chat,
                        IsRootFolder = true,
                        // 自動処理を無効にする
                        IsAutoProcessEnabled = false
                    };
                    chatRootFolder.Save();
                }
                // 既にSearchRootFolder作成済みの環境のための措置
                chatRootFolder.IsRootFolder = true;
                return chatRootFolder;
            }
        }



        // プロパティ
        // LiteDBのID
        public ObjectId Id { get; set; } = ObjectId.Empty;

        // 親フォルダのID
        public ObjectId ParentId { get; set; } = ObjectId.Empty;


        // フォルダの種類
        public FolderTypeEnum FolderType { get; set; } = FolderTypeEnum.Normal;

        // ルートフォルダか否か
        public bool IsRootFolder { get; set; } = false;

        // AutoProcessを有効にするかどうか
        public bool IsAutoProcessEnabled { get; set; } = true;


        // フォルダの絶対パス ファイルシステム用
        public string FolderPath {
            get {
                ClipboardFolder? parent = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(ParentId);
                if (parent == null) {
                    return FolderName;
                }
                return ConcatenateFileSystemPath(parent.FolderPath, FolderName);
            }
        }

        //　フォルダ名
        public string FolderName { get; set; } = "";


        // Description
        public string Description { get; set; } = "";


        public List<ClipboardFolder> Children {
            get {
                // DBからParentIDが自分のIDのものを取得
                return ClipboardAppFactory.Instance.GetClipboardDBController().GetFoldersByParentId(Id);
            }
        }

        // アイテム BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public List<ClipboardItem> Items {
            get {
                if (FolderType == FolderTypeEnum.Search) {
                    return GetSearchFolderItems();
                }
                return GetNormalFolderItems();
            }
        }

        private List<ClipboardItem> GetNormalFolderItems() {
            List<ClipboardItem> _items = [];
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // 通常のフォルダの場合で、GlobalSearchConditionが設定されている場合
            if (GlobalSearchCondition.SearchCondition != null && GlobalSearchCondition.SearchCondition.IsEmpty() == false) {
                _items = [.. ClipboardDatabaseController.SearchItems(this, GlobalSearchCondition.SearchCondition)];

            } else {
                // 通常のフォルダの場合で、GlobalSearchConditionが設定されていない場合
                _items = [.. ClipboardDatabaseController.GetItems(this)];
            }
            return _items;
        }

        private List<ClipboardItem> GetSearchFolderItems() {
            List<ClipboardItem> _items = [];
            // このフォルダが通常フォルダの場合は、GlobalSearchConditionを適用して取得,
            // 検索フォルダの場合は、SearchConditionを適用して取得
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // フォルダに検索条件が設定されている場合
            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(this);
            if (searchConditionRule != null && searchConditionRule.TargetFolder != null) {
                // 検索対象フォルダのアイテムを検索する。
                _items = [.. ClipboardDatabaseController.SearchItems(searchConditionRule.TargetFolder, searchConditionRule.SearchCondition)];

            }
            // 検索対象フォルダパスがない場合は何もしない。
            return _items;
        }

        // AutoProcessRuleのIdのリスト
        public List<ObjectId> AutoProcessRuleIds { get; set; } = [];

        // 子フォルダ BSonMapper.GlobalでIgnore設定しているので、LiteDBには保存されない
        public void DeleteChild(ClipboardFolder child) {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.DeleteFolder(child);
        }

        public ClipboardFolder CreateChild(string folderName) {
            ClipboardFolder child = new(this, folderName);
            return child;
        }

        //--------------------------------------------------------------------------------
        // 自分自身を保存
        public void Save() {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.UpsertFolder(this);
        }
        // Delete
        public void Delete() {
            IClipboardDBController ClipboardDatabaseController = ClipboardAppFactory.Instance.GetClipboardDBController();
            ClipboardDatabaseController.DeleteFolder(this);
        }

        // アイテムを追加する処理
        public ClipboardItem AddItem(ClipboardItem item) {
            // 検索フォルダの場合は何もしない
            if (FolderType == FolderTypeEnum.Search) {
                return item;
            }

            // CollectionNameを設定
            item.FolderObjectId = this.Id;

            // 自動処理を適用
            ClipboardItem? result = item;
            if (IsAutoProcessEnabled) {
                result = ApplyAutoProcess(item);
            }

            if (result == null) {
                // 自動処理で削除または移動された場合は何もしない
                LogWrapper.Info("自動処理でアイテムが削除または移動されました");
                return item;
            }
            // 保存
            result.Save();
            // Itemsに追加
            Items.Add(result);
            // 通知
            LogWrapper.Info("アイテムを追加しました");
            return item;
        }

        // ClipboardItemを削除
        public void DeleteItem(ClipboardItem item) {
            // 検索フォルダの場合は何もしない
            if (FolderType == FolderTypeEnum.Search) {
                return;
            }

            // LiteDBに保存
            item.Delete();
        }

        // 自動処理を適用する処理
        public ClipboardItem? ApplyAutoProcess(ClipboardItem clipboardItem) {
            // 検索フォルダの場合は何もしない
            if (FolderType == FolderTypeEnum.Search) {
                return clipboardItem;
            }

            ClipboardItem? result = clipboardItem;
            // AutoProcessRulesを取得
            var AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(this);
            foreach (var rule in AutoProcessRules) {
                LogWrapper.Info("自動処理を適用します " + rule.GetDescriptionString());
                result = rule.RunAction(result);
                // resultがNullの場合は処理を中断
                if (result == null) {
                    LogWrapper.Info("自動処理でアイテムが削除されました");
                    return null;
                }
            }
            return result;
        }

        // フォルダ内のアイテムをJSON形式でExport
        public void ExportItemsToJson(string directoryPath) {
            JsonArray jsonArray = [];
            foreach (ClipboardItem item in Items) {
                jsonArray.Add(ClipboardItem.ToJson(item));
            }
            string jsonString = jsonArray.ToString();
            string fileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + this.Id.ToString() + ".json";

            File.WriteAllText(Path.Combine(directoryPath, fileName), jsonString);

        }

        //exportしたJSONファイルをインポート
        public void ImportItemsFromJson(string json, Action<ActionMessage> action) {
            JsonNode? node = JsonNode.Parse(json);
            if (node == null) {
                action(ActionMessage.Error("JSON文字列をパースできませんでした"));
                return;
            }
            JsonArray? jsonArray = node as JsonArray;
            if (jsonArray == null) {
                action(ActionMessage.Error("JSON文字列をパースできませんでした"));
                return;
            }

            // Itemsをクリア
            Items.Clear();

            foreach (JsonValue? jsonValue in jsonArray.Cast<JsonValue?>()) {
                if (jsonValue == null) {
                    continue;
                }
                string jsonString = jsonValue.ToString();
                ClipboardItem? item = ClipboardItem.FromJson(jsonString, action);
                if (item == null) {
                    continue;
                }
                // Itemsに追加
                Items.Add(item);
                //保存
                item.Save();
            }

        }

        // 指定されたフォルダの中のSourceApplicationTitleが一致するアイテムをマージするコマンド
        public void MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItem newItem) {
            // 検索フォルダの場合は何もしない
            if (FolderType == FolderTypeEnum.Search) {
                return;
            }

            // SourceApplicationNameが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationName)) {
                return;
            }
            // NewItemのSourceApplicationTitleが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationTitle)) {
                return;
            }
            if (Items.Count == 0) {
                return;
            }
            List<ClipboardItem> sameTitleItems = [];
            // マージ先のアイテムのうち、SourceApplicationTitleとSourceApplicationNameが一致するアイテムを取得
            foreach (var item in Items) {
                if (newItem.SourceApplicationTitle == item.SourceApplicationTitle
                    && newItem.SourceApplicationName == item.SourceApplicationName) {
                    // TypeがTextのアイテムのみマージ
                    if (item.ContentType == ClipboardContentTypes.Text) {
                        sameTitleItems.Add(item);
                    }
                }
            }
            // mergeFromItemsが空の場合は、newItemをそのまま返す。
            if (sameTitleItems.Count == 0) {
                return;
            }
            // マージ元のアイテムをマージ先(更新時間が一番古いもの)のアイテムにマージ
            ClipboardItem mergeToItem = Items.Last();
            // sameTitleItemsの1から最後までをマージ元のアイテムとする
            sameTitleItems.RemoveAt(sameTitleItems.Count - 1);

            // sameTitleItemsに、newItemを追加
            sameTitleItems.Insert(0, newItem);
            // マージ元のアイテムをマージ先のアイテムにマージ

            mergeToItem.MergeItems(sameTitleItems, false, Tools.DefaultAction);
            // newItemにマージしたアイテムをコピー
            mergeToItem.CopyTo(newItem);
            // マージしたアイテムを削除
            foreach (var mergedItem in sameTitleItems) {
                DeleteItem(mergedItem);
            }
            // mergedItemを削除
            DeleteItem(mergeToItem);
        }
        // 指定されたフォルダの全アイテムをマージするコマンド
        public void MergeItems(ClipboardItem item) {
            // 検索フォルダの場合は何もしない
            if (FolderType == FolderTypeEnum.Search) {
                return;
            }
            if (Items.Count == 0) {
                return;
            }

            // マージ元のアイテム
            List<ClipboardItem> mergedFromItems = [];
            for (int i = Items.Count - 1; i > 0; i--) {
                // TypeがTextのアイテムのみマージ
                if (Items[i].ContentType == ClipboardContentTypes.Text) {
                    mergedFromItems.Add(Items[i]);
                }
            }
            // 先頭に引数のアイテムを追加
            mergedFromItems.Insert(0, item);
            // mergeToItemを取得(更新時間が一番古いアイテム)
            ClipboardItem mergeToItem = mergedFromItems.Last();
            // mergedFromItemsから、mergeToItemを削除
            mergedFromItems.RemoveAt(mergedFromItems.Count - 1);

            // マージ元のアイテムをマージ先のアイテムにマージ
            mergeToItem.MergeItems(mergedFromItems, false, Tools.DefaultAction);

            // マージ先アイテムを、newItemにコピー
            mergeToItem.CopyTo(item);

            // マージしたアイテムを削除
            foreach (var mergedItem in mergedFromItems) {
                DeleteItem(mergedItem);
            }
            // マージ先アイテムを削除
            DeleteItem(mergeToItem);

        }


        //------------

        // 親フォルダのパスと子フォルダ名を連結する。ファイルシステム用
        private string ConcatenateFileSystemPath(string parentPath, string childPath) {
            if (string.IsNullOrEmpty(parentPath))
                return childPath;
            if (string.IsNullOrEmpty(childPath))
                return parentPath;
            return Path.Combine(parentPath, childPath);
        }


        // 親フォルダを取得する
        public ClipboardFolder? GetParentFolder() {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(ParentId);
        }

        // 指定したFilePath名のフォルダを取得する。
        public static ClipboardFolder? GetRFolder(string filePath, bool create = false) {
            string[] pathList = filePath.Split(Path.DirectorySeparatorChar);
            ClipboardFolder? rootFolder = ClipboardAppFactory.Instance.GetClipboardDBController().GetRootFolder(pathList[0]);
            // rootFolderがnullの場合は、Nullを返す
            if (rootFolder == null) {
                return null;
            }
            // pathListの1から最後までを取得
            for (int i = 1; i < pathList.Length; i++) {
                string folderName = pathList[i];
                // 子フォルダのリストを取得
                List<ClipboardFolder> children = rootFolder.Children;
                // folderNameと一致するフォルダを取得
                ClipboardFolder? folder = children.FirstOrDefault(x => x.FolderName == folderName);
                if (folder == null) {
                    if (create) {
                        folder = rootFolder.CreateChild(folderName);
                        rootFolder.Save();
                        folder.Save();
                    } else {
                        return null;
                    }
                }
                rootFolder = folder;
            }
            return rootFolder;
        }

        public static ClipboardFolder GetAnotherTreeFolder(ClipboardFolder folder, string anotherTreeRoot, bool create = false) {
            string path = folder.FolderPath;
            // path先頭を取得
            string rootPath = path.Split(Path.DirectorySeparatorChar)[0];
            // pathの先頭を正規表現でAnotherTreeRootに置換.
            string replacedPath = Regex.Replace(path, "^" + rootPath, anotherTreeRoot);
            ClipboardFolder? chatFolder = GetRFolder(replacedPath, create);
            // chatFolderがnullの場合は、ChatRootFolderを返す
            if (chatFolder == null) {
                return ChatRootFolder;
            }
            return chatFolder;

        }

        // SystemCommonVectorDBを取得する。
        public VectorDBItem GetVectorDBItem() {
            return ClipboardAppVectorDBItem.GetFolderVectorDBItem(this);
        }

        #region システムのクリップボードへ貼り付けられたアイテムに関連する処理
        public void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ClipboardItem> _afterClipboardChanged) {

            // Get the cut/copied text.
            ClipboardItem? item = CreateClipboardItem(this, e);

            if (item == null) {
                return;
            }

            ProcessClipboardItem(item, _afterClipboardChanged);

        }

        /// Process clipboard item 
        /// <summary>
        /// Process clipboard item
        /// </summary>
        /// <param name="item"></param>
        /// <param name="_afterClipboardChanged"></param>
        public static void ProcessClipboardItem(ClipboardItem item, Action<ClipboardItem> _afterClipboardChanged) {

            // Execute in a separate thread
            Task.Run(() => {
                string oldReadyText = Tools.StatusText.ReadyText;
                Application.Current.Dispatcher.Invoke(() => {
                    Tools.StatusText.ReadyText = CommonStringResources.Instance.AutoProcessing;
                });
                try {
                    // Apply automatic processing
                    ClipboardItem? updatedItem = ApplyAutoAction(item);
                    if (updatedItem == null) {
                        // If the item is ignored, return
                        return;
                    }
                    // Notify the completion of processing
                    _afterClipboardChanged(updatedItem);

                } catch (Exception ex) {
                    LogWrapper.Error($"{CommonStringResources.Instance.AddItemFailed}\n{ex.Message}\n{ex.StackTrace}");
                } finally {
                    Application.Current.Dispatcher.Invoke(() => {
                        Tools.StatusText.ReadyText = oldReadyText;
                    });
                }
            });
        }


        /// Create ClipboardItem
        public static ClipboardItem? CreateClipboardItem(
            ClipboardFolder clipboardFolder, ClipboardChangedEventArgs e) {

            ClipboardContentTypes contentTypes = ClipboardContentTypes.Text;
            if (e.ContentType == SharpClipboard.ContentTypes.Text) {
                contentTypes = ClipboardContentTypes.Text;
            } else if (e.ContentType == SharpClipboard.ContentTypes.Files) {
                contentTypes = ClipboardContentTypes.Files;
            } else if (e.ContentType == SharpClipboard.ContentTypes.Image) {
                contentTypes = ClipboardContentTypes.Image;
            } else if ( e.ContentType == SharpClipboard.ContentTypes.Other) {
                return null;
            } else {
                return null;
            }

            ClipboardItem item = new(clipboardFolder.Id) {
                ContentType = contentTypes
            };
            SetApplicationInfo(item, e);
            // If ContentType is Text, set text data
            if (contentTypes == ClipboardContentTypes.Text) {
                item.Content = (string)e.Content;
            }
            // If ContentType is Image, set image data
            if (contentTypes == ClipboardContentTypes.Image) {
                System.Drawing.Image image = (System.Drawing.Image)e.Content;
                ClipboardItemImage imageItem = ClipboardItemImage.Create(item, image);
                item.ClipboardItemImages.Add(imageItem);
            }
            // If ContentType is Files, set file data
            else if (contentTypes == ClipboardContentTypes.Files) {
                string[] files = (string[])e.Content;

                // Get the cut/copied file/files.
                for (int i = 0; i < files.Length; i++) {
                    ClipboardItemFile clipboardItemFile = ClipboardItemFile.Create(item, files[i]);
                    item.ClipboardItemFiles.Add(clipboardItemFile);
                }
            }
            return item;

        }

        /// <summary>
        /// Set application information from ClipboardChangedEventArgs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sender"></param>
        private static void SetApplicationInfo(ClipboardItem item, ClipboardChangedEventArgs sender) {
            item.SourceApplicationName = sender.SourceApplication.Name;
            item.SourceApplicationTitle = sender.SourceApplication.Title;
            item.SourceApplicationID = sender.SourceApplication.ID;
            item.SourceApplicationPath = sender.SourceApplication.Path;
        }

        /// <summary>
        /// Apply automatic processing
        /// </summary>
        /// <param name="item"></param>
        /// <param name="image"></param>
        private static ClipboardItem? ApplyAutoAction(ClipboardItem item) {
            // ★TODO Implement processing based on automatic processing rules.
            // 指定した行数以下のテキストアイテムは無視
            int lineCount = item.Content.Split('\n').Length;
            if (item.ContentType == ClipboardContentTypes.Text && lineCount <= ClipboardAppConfig.IgnoreLineCount) {
                return null;
            }

            // ★TODO Implement processing based on automatic processing rules.
            // If AutoMergeItemsBySourceApplicationTitle is set, automatically merge items
            if (ClipboardAppConfig.AutoMergeItemsBySourceApplicationTitle) {
                LogWrapper.Info(CommonStringResources.Instance.AutoMerge);
                ClipboardFolder.RootFolder.MergeItemsBySourceApplicationTitleCommandExecute(item);
            }
            // If AutoFileExtract is set, extract files
            if (ClipboardAppConfig.AutoFileExtract && item.ContentType == ClipboardContentTypes.Files && item.ClipboardItemFiles != null) {
                LogWrapper.Info(CommonStringResources.Instance.ExecuteAutoFileExtract);
                foreach (var fileItem in item.ClipboardItemFiles) {
                    string text = PythonExecutor.PythonFunctions.ExtractText(fileItem.FilePath);
                    item.Content += "\n" + text;
                }
            }
            // ★TODO Implement processing based on automatic processing rules.
            // If AutoExtractImageWithPyOCR is set, perform OCR
            if (ClipboardAppConfig.AutoExtractImageWithPyOCR) {
                foreach (var imageItem in item.ClipboardItemImages) {
                    if (imageItem.Image == null) {
                        continue;
                    }
                    string extractImageText = PythonExecutor.PythonFunctions.ExtractTextFromImage(imageItem.Image, ClipboardAppConfig.TesseractExePath);
                    item.Content += "\n" + extractImageText;
                }
                LogWrapper.Info(CommonStringResources.Instance.OCR);

            } else if (ClipboardAppConfig.AutoExtractImageWithOpenAI) {

                LogWrapper.Info(CommonStringResources.Instance.AutoExtractImageText);
                ClipboardItem.ExtractImageWithOpenAI(item);
            }


            // ★TODO Implement processing based on automatic processing rules.
            // If AUTO_TAG is set, automatically set the tags
            if (ClipboardAppConfig.AutoTag) {
                LogWrapper.Info(CommonStringResources.Instance.AutoSetTag);
                ClipboardItem.CreateAutoTags(item);
            }
            // If AUTO_DESCRIPTION is set, automatically set the Description
            if (ClipboardAppConfig.AutoDescription) {
                LogWrapper.Info(CommonStringResources.Instance.AutoSetTitle);
                ClipboardItem.CreateAutoTitle(item);

            } else if (ClipboardAppConfig.AutoDescriptionWithOpenAI) {

                LogWrapper.Info(CommonStringResources.Instance.AutoSetTitle);
                ClipboardItem.CreateAutoTitleWithOpenAI(item);
            }
            // 背景情報
            if (ClipboardAppConfig.AutoBackgroundInfo) {
                LogWrapper.Info(CommonStringResources.Instance.AutoSetBackgroundInfo);
                ClipboardItem.CreateAutoBackgroundInfo(item);
            }
            return item;
        }

        #endregion


    }
}

