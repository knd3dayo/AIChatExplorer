using System.IO;
using ClipboardApp.Model;
using LiteDB;
using PythonAILib.Model;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.Utils;

namespace ClipboardApp.Factory.Default {
    public class DefaultClipboardDBController : IClipboardDBController {
        public static readonly string CLIPBOARD_FOLDERS_COLLECTION_NAME = "folders";
        public static readonly string CLIPBOARD_ROOT_FOLDERS_COLLECTION_NAME = "root_folders";

        public static readonly string SCRIPT_COLLECTION_NAME = "scripts";
        public static readonly string AUTO_PROCESS_RULES_COLLECTION_NAME = "auto_process_rules";
        public static readonly string TAG_COLLECTION_NAME = "tags";

        public static readonly string SEARCH_CONDITION_RULES_COLLECTION_NAME = "search_condition_rules";
        public static readonly string SEARCH_CONDITION_APPLIED_CONDITION_NAME = "applied_globally";
        public static readonly string CHAT_SESSION_COLLECTION_NAME = "chat_session";

        public static readonly string CLIPBOARD_ITEM_COLLECTION_NAME = "clipboard_item";
        public static readonly string CLIPBOARD_IMAGE_COLLECTION_NAME = "clipboard_image";
        public static readonly string CLIPBOARD_FILE_COLLECTION_NAME = "clipboard_file";


        public const string PromptTemplateCollectionName = "PromptTemplate";

        // ClipboardRAGSourceItem
        public const string RAGSourceItemCollectionName = "RAGSourceItem";
        // VectorDBItem
        public const string VectorDBItemCollectionName = "VectorDBItem";

        private static LiteDatabase? db;

        // --- static method ----------------------------------------------

        public static LiteDatabase GetClipboardDatabase() {
            if (db == null) {
                try {
                    /// AppDataフォルダーパスを取得
                    string appDataPath = ClipboardAppConfig.Instance.AppDataFolder;
                    // データベースファイルのパスを作成
                    string dbPath = Path.Combine(appDataPath, "clipboard.db");
                    db = new LiteDatabase(dbPath);

                    // WpfAppCommon.Model.ClipboardItemをClipboardApp.Model.ClipboardItemに変更
                    /*** クラスの場所変更時の暫定的な処理
                    var collection = db.GetCollection(CLIPBOARD_ITEM_COLLECTION_NAME);
                    foreach (var item in collection.FindAll()) {
                        string typeString = item["_type"];
                        if (typeString == "WpfAppCommon.Model.ClipboardApp.ClipboardItem, WpfAppCommon") {
                            item["_type"] = "ClipboardApp.Model.ClipboardItem, ClipboardApp";
                            collection.Update(item);
                        }
                        var fileItems = item["ClipboardItemFiles"];
                        if (fileItems != null && fileItems.AsArray != null) {
                            foreach (var fileItem in fileItems.AsArray) {
                                string? fileTypeString = fileItem["_type"];
                                if (fileTypeString == null) {
                                    continue;
                                }
                                if (fileTypeString.Contains("WpfAppCommon.Model.ClipboardApp.ClipboardItemFile, WpfAppCommon")) {
                                    string newTypeString = fileTypeString.Replace("WpfAppCommon.Model.ClipboardApp.ClipboardItemFile, WpfAppCommon", "ClipboardApp.Model.ClipboardItemFile, ClipboardApp");
                                    fileItem["_type"] = newTypeString;
                                    collection.Update(item);
                                }
                            }
                        }

                    }
                    // ClipboardItemFile
                    var fileCollection = db.GetCollection(CLIPBOARD_FILE_COLLECTION_NAME);
                    foreach (var item in fileCollection.FindAll()) {
                        string typeString = item["_type"];
                        if (typeString == "WpfAppCommon.Model.ClipboardApp.ClipboardItemFile, WpfAppCommon") {
                            item["_type"] = "ClipboardApp.Model.ClipboardItemFile, ClipboardApp";
                            fileCollection.Update(item);
                        }
                    }
                    ***/

                    // BSonMapperの設定
                    // ClipboardItemFolderのChildren, Items, SearchConditionを無視する
                    var mapper = BsonMapper.Global;
                    mapper.Entity<ClipboardFolder>()
                        .Ignore(x => x.Items);
                    mapper.Entity<ClipboardFolder>()
                        .Ignore(x => x.Children);

                } catch (Exception e) {
                    throw new Exception("データベースのオープンに失敗しました。" + e.Message);
                }
            }
            return db;
        }

        #region フォルダー関連
        //---- フォルダー関連 ----------------------------------------------
        // フォルダーを削除する
        public void DeleteFolder(ClipboardFolder folder) {
            // folderの子フォルダを再帰的に削除
            foreach (var child in folder.Children) {
                if (child != null) {
                    DeleteFolder(child);
                }
            }

            // Itemsを削除
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(CLIPBOARD_ITEM_COLLECTION_NAME);
            collection.FindAll().Where(x => x.FolderObjectId == folder.Id).ToList().ForEach(x => DeleteItem(x));
            // folderを削除
            var folderCollection = GetClipboardDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            folderCollection.Delete(folder.Id);

        }

        public ClipboardFolder? GetFolder(ObjectId? objectId) {
            if (objectId == null) return null;
            ClipboardFolder? result = null;
            var collection = GetClipboardDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            var item = collection.FindById(objectId);
            if (item != null) {
                result = item;
            }
            return result;
        }

        public ClipboardFolder? GetRootFolderByType(ClipboardFolder.FolderTypeEnum folderType) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardFolder.RootFolderInfo>(CLIPBOARD_ROOT_FOLDERS_COLLECTION_NAME);
            // Debug FolderName = folderNameのアイテムが複数ある時はエラー
            var items = collection.FindAll().Where(x => x.FolderType == folderType);
            if (items.Count() > 1) {
                throw new Exception("RootFolderInfoに同じFolderNameが複数存在します。");
            }
            // itemsの最初の要素を取得
            var item = items.FirstOrDefault();
            return GetFolder(item?.FolderId);
        }

        public List<ClipboardFolder> GetFoldersByParentId(ObjectId? objectId) {
            List<ClipboardFolder> result = [];

            // objectIdがnullの場合は、空のリストを返す
            if (objectId == null) {
                return result;
            }
            // objectIdに対応するフォルダをすべて取得して返す
            var collection = GetClipboardDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            var items = collection.FindAll().Where(x => x.ParentId == objectId);
            foreach (var i in items) {
                result.Add(i);
            }
            return result;
        }


        // 親フォルダのCollectionNameを指定して子フォルダのリストを取得する
        private IEnumerable<ClipboardFolder> GetChildFolders(ObjectId folderObjectId) {
            List<ClipboardFolder> result = [];
            // 親フォルダを取得
            var parent = GetFolder(folderObjectId);
            if (parent == null) {
                return result;
            }
            foreach (var child in parent.Children) {
                // nullの場合は、次のループへ
                if (child == null) {
                    continue;
                }
                result.Add(child);
            }
            return result;
        }

        // ClipboardItemFolderをLiteDBに追加または更新する
        public void UpsertFolder(ClipboardFolder folder) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            // フォルダの親フォルダのIdをチェック
            if (folder.ParentId == null || folder.ParentId == ObjectId.Empty) {
                // 親フォルダのIDが存在しない場合は、ルートフォルダか否かをチェックする。GetRootFolderを呼び出す
                var rootFolder = GetRootFolderByType(folder.FolderType);
                // ルートフォルダが存在しない場合は、新規作成
                if (rootFolder == null) {
                    var rootFolderInfoCollection = GetClipboardDatabase().GetCollection<ClipboardFolder.RootFolderInfo>(CLIPBOARD_ROOT_FOLDERS_COLLECTION_NAME);
                    var rootFolderInfo = new ClipboardFolder.RootFolderInfo {
                        FolderName = folder.FolderName,
                        Id = ObjectId.NewObjectId(),
                        FolderType = folder.FolderType
                    };
                    rootFolderInfo.FolderId = rootFolderInfo.Id;
                    rootFolderInfoCollection.Upsert(rootFolderInfo);

                    // ルートフォルダの作成
                    folder.Id = rootFolderInfo.FolderId;
                    collection.Upsert(folder);

                } else {
                    //　ルートフォルダの更新。folderにRootFolder.FolderIdを設定してUpsert 
                    folder.Id = rootFolder.Id;
                    collection.Upsert(folder);
                }

            } else {
                // 親フォルダが存在する場合は、ParentIdとFolderNameが位置するフォルダを取得する
                var childFolders = GetChildFolders(folder.ParentId);
                var targetFolder = childFolders.FirstOrDefault(x => x.FolderName == folder.FolderName);
                // 存在しない場合は、新規作成
                if (targetFolder == null) {
                    collection.Upsert(folder);
                } else {
                    // 存在する場合は、folderのIdを設定してUpsert
                    folder.Id = targetFolder.Id;
                    collection.Upsert(folder);
                }
            }

        }
        # endregion

        //---- 自動処理関連 ----------------------------------------------
        // 指定したTargetFolderを持つAutoProcessRuleを取得する
        public List<AutoProcessRule> GetAutoProcessRules(ClipboardFolder targetFolder) {
            List<AutoProcessRule> result = [];
            if (targetFolder == null) {
                return result;
            }
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            // targetFolderのAutoProcessRuleIdsに対応するAutoProcessRuleを取得
            foreach (var i in targetFolder.AutoProcessRuleIds) {
                var item = collection.FindById(i);
                if (item != null) {
                    result.Add(item);
                }
            }

            return result;
        }

        // すべてのAutoProcessRuleを取得する
        public IEnumerable<AutoProcessRule> GetAllAutoProcessRules() {
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            return collection.FindAll().OrderBy(x => x.Priority);
        }

        // AutoProcessRuleを追加または更新する
        public void UpsertAutoProcessRule(AutoProcessRule rule) {
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            collection.Upsert(rule);
        }
        // AutoProcessRuleを削除する
        public void DeleteAutoProcessRule(AutoProcessRule rule) {
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            collection.Delete(rule.Id);
        }

        // TypeがCopyTo または MoveToのルールをLiteDBから取得する。
        public IEnumerable<AutoProcessRule> GetCopyToMoveToRules() {
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            var items = collection.FindAll().Where(
                x => x.RuleAction != null
                && (x.RuleAction.Name == SystemAutoProcessItem.TypeEnum.CopyToFolder.ToString()
                    || x.RuleAction.Name == SystemAutoProcessItem.TypeEnum.MoveToFolder.ToString()));
            return items;

        }
        // SearchConditionを追加または更新する
        public void UpsertSearchRule(SearchRule conditionRule) {
            var collection = GetClipboardDatabase().GetCollection<SearchRule>(SEARCH_CONDITION_RULES_COLLECTION_NAME);
            collection.Upsert(conditionRule);
        }
        // 名前を指定して検索条件を取得する
        public SearchRule? GetSearchRule(string name) {
            var collection = GetClipboardDatabase().GetCollection<SearchRule>(SEARCH_CONDITION_RULES_COLLECTION_NAME);
            var item = collection.FindOne(x => x.Name == name);
            return item;
        }
        // 指定したCollectionNameに対応する検索条件を取得する
        public SearchRule? GetSearchRuleByFolder(ClipboardFolder folder) {
            var collection = GetClipboardDatabase().GetCollection<SearchRule>(SEARCH_CONDITION_RULES_COLLECTION_NAME);
            var item = collection.FindOne(x => x.SearchFolder != null && x.SearchFolder.Id == folder.Id);
            return item;
        }

        #region クリップボードアイテム関連
        // --- クリップボードアイテム関連 ----------------------------------------------
        // ClipboardItemを取得する。
        public ContentItemBase? GetItem(ContentItemBase item) {
            if (item is not ClipboardItem) {
                // Incorrect argument type error
                throw new Exception("Incorrect argument type.");
            }
            ClipboardItem clipboardItem = (ClipboardItem)item;
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(CLIPBOARD_ITEM_COLLECTION_NAME);
            var result = collection.FindById(clipboardItem.Id);
            return result;
        }

        // ClipboardItemをLiteDBに追加または更新する
        public void UpsertItem(ContentItemBase item, bool updateModifiedTime = true) {
            if (item is not ClipboardItem) {
                // Incorrect argument type error
                throw new Exception("Incorrect argument type.");
            }
            ClipboardItem clipboardItem = (ClipboardItem)item;

            // 更新日時を設定
            if (updateModifiedTime) {
                item.UpdatedAt = DateTime.Now;
            }
            // ファイルがある場合は、追加または更新
            foreach (var file in clipboardItem.ClipboardItemFiles) {
                UpsertAttachedItem((ClipboardItemFile)file);
            }
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(CLIPBOARD_ITEM_COLLECTION_NAME);
            collection.Upsert(clipboardItem);
        }


        // アイテムをDBから削除する
        public void DeleteItem(ContentItemBase item) {
            if (item is not ClipboardItem) {
                // Incorrect argument type error
                throw new Exception("Incorrect argument type.");
            }
            ClipboardItem clipboardItem = (ClipboardItem)item;
            if (clipboardItem.Id == null) {
                return;
            }
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(CLIPBOARD_ITEM_COLLECTION_NAME);
            // System.Windows.MessageBox.Show(item.CollectionName);
            collection.Delete(clipboardItem.Id);
        }

        // ClipboardItemのリストをLiteDBから取得する
        public IEnumerable<ClipboardItem> GetItems(ClipboardFolder folder) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(CLIPBOARD_ITEM_COLLECTION_NAME);
            var items = collection.FindAll().Where(x => x.FolderObjectId == folder.Id).OrderByDescending(x => x.UpdatedAt);
            return items;

        }

        // ClipboardItemを検索する。
        public IEnumerable<ClipboardItem> SearchItems(ClipboardFolder folder, SearchCondition searchCondition) {
            // 結果を格納するIEnumerable<ContentItem>を作成
            IEnumerable<ClipboardItem> result = [];
            // フォルダが存在しない場合は、結果を返す
            if (folder == null) {
                return result;
            }
            // 検索条件が空の場合は、結果を返す
            if (searchCondition.IsEmpty()) {
                return result;
            }

            // folder内のアイテムを保持するコレクションを取得
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(CLIPBOARD_ITEM_COLLECTION_NAME);
            var clipboardItems = collection.FindAll().Where(x => x.FolderObjectId == folder.Id);
            // Filterの結果を結果に追加
            result = Filter(clipboardItems, searchCondition);

            // サブフォルダを含む場合は、対象フォルダとそのサブフォルダを検索
            if (searchCondition.IsIncludeSubFolder) {
                // 対象フォルダの子フォルダを取得
                var childFolders = GetChildFolders(folder.Id);
                foreach (var childFolder in childFolders) {
                    // サブフォルダのアイテムを検索
                    var subFolderResult = SearchItems(childFolder, searchCondition);
                    // Filterの結果を結果に追加
                    result = result.Concat(subFolderResult);
                }
            }
            return result;

        }


        public void DeleteItems(ClipboardFolder targetFolder) {
            foreach (var item in targetFolder.Items) {
                if (item.IsPinned == false) {
                    DeleteItem(item);
                }
                targetFolder.Items.Clear();
                UpsertFolder(targetFolder);

            }
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
        // --- ScriptItem関連 ----------------------------------------------

        public IEnumerable<ScriptItem> GetScriptItems() {
            var collection = GetClipboardDatabase().GetCollection<ScriptItem>(SCRIPT_COLLECTION_NAME);
            var items = collection.FindAll();
            return items.ToList();
        }

        // --- TagItem関連 ----------------------------------------------
        // タグを取得する
        public IEnumerable<TagItem> GetTagList() {
            var collection = GetClipboardDatabase().GetCollection<TagItem>(TAG_COLLECTION_NAME);
            var items = collection.FindAll();
            return items;
        }

        // 名前を指定してタグを検索する
        public IEnumerable<TagItem> SearchTag(TagItem tag) {
            var collection = GetClipboardDatabase().GetCollection<TagItem>(TAG_COLLECTION_NAME);
            var tags = collection.FindAll().Where(x => x.Tag != null && x.Tag.Contains(tag.Tag));
            return tags;

        }
        public IEnumerable<TagItem> FilterTag(string tag, bool exclude) {
            if (string.IsNullOrEmpty(tag)) {
                return GetTagList();
            }
            if (exclude) {
                return GetTagList().Where(x => x.Tag.Contains(tag) == false);
            } else {
                return GetTagList().Where(x => x.Tag.Contains(tag));
            }

        }
        // タグを削除する
        public void DeleteTag(TagItem tag) {
            if (tag.Id == null) {
                return;
            }

            var collection = GetClipboardDatabase().GetCollection<TagItem>(TAG_COLLECTION_NAME);
            collection.Delete(tag.Id);
        }

        // タグを追加する
        public void UpsertTag(TagItem tag) {
            // すでに存在するかチェック
            var tags = SearchTag(tag);
            foreach (var i in tags) {
                if (i.Tag == tag.Tag) {
                    return;
                }
            }
            var collection = GetClipboardDatabase().GetCollection<TagItem>(TAG_COLLECTION_NAME);
            collection.Insert(tag);
        }

        // --- PromptItem関連 ----------------------------------------------

        // create
        public PromptItem CreatePromptItem() {
            return new ClipboardPromptItem();
        }

        public void UpsertPromptTemplate(PromptItem item) {
            if (item is not ClipboardPromptItem promptItem) {
                throw new Exception("Incorrect argument type.");
            }
            var db = GetClipboardDatabase();

            var col = db.GetCollection<ClipboardPromptItem>(PromptTemplateCollectionName);
            col.Upsert(promptItem);
        }

        // プロンプトテンプレートを取得する
        public PromptItem GetPromptTemplate(ObjectId objectId) {
            var col = GetClipboardDatabase().GetCollection<ClipboardPromptItem>(PromptTemplateCollectionName);
            return col.FindById(objectId);
        }
        // プロンプトテンプレートを名前で取得する
        public PromptItem? GetPromptTemplateByName(string name) {
            var col = GetClipboardDatabase().GetCollection<ClipboardPromptItem>(PromptTemplateCollectionName);
            return col.FindOne(x => x.Name == name);
        }
        // システム定義のPromptItemを取得する
        public PromptItem? GetSystemPromptTemplateByName(string name) {
            var col = GetClipboardDatabase().GetCollection<ClipboardPromptItem>(PromptTemplateCollectionName);
            var item = col.FindOne(x => x.Name == name);
            if (item != null &&
                (item.PromptTemplateType == PromptItem.PromptTemplateTypeEnum.SystemDefined
                    || item.PromptTemplateType == PromptItem.PromptTemplateTypeEnum.ModifiedSystemDefined)) {
                return item;
            }
            return null;
        }


        // 引数として渡されたプロンプトテンプレートを削除する
        public void DeletePromptTemplate(PromptItem item) {
            if (item is not ClipboardPromptItem promptItem) {
                throw new Exception("Incorrect argument type.");
            }

            var col = GetClipboardDatabase().GetCollection<ClipboardPromptItem>(PromptTemplateCollectionName);
            col.Delete(promptItem.Id);
        }

        // プロンプトテンプレートを全て取得する
        public ICollection<PromptItem> GetAllPromptTemplates() {
            ICollection<PromptItem> collation = [];
            var col = GetClipboardDatabase().GetCollection<ClipboardPromptItem>(PromptTemplateCollectionName);
            foreach (var item in col.FindAll()) {
                collation.Add(item);
            }
            return collation;
        }
        // プロンプトテンプレートを全て削除する
        public static void DeleteAllPromptTemplates() {
            var col = GetClipboardDatabase().GetCollection<ClipboardPromptItem>(PromptTemplateCollectionName);
            col.DeleteAll();
        }

        //----  ClipboardRAGSourceItem
        // create
        public RAGSourceItem CreateRAGSourceItem() {
            return new ClipboardRAGSourceItem();
        }
        // update
        public void UpsertRAGSourceItem(RAGSourceItem item) {
            if (item is not ClipboardRAGSourceItem ragItem) {
                throw new Exception("Incorrect argument type.");
            }
            // RAGSourceItemコレクションに、itemを追加または更新
            var collection = GetClipboardDatabase().GetCollection<ClipboardRAGSourceItem>(RAGSourceItemCollectionName);
            collection.Upsert(ragItem);

        }
        // delete
        public void DeleteRAGSourceItem(RAGSourceItem item) {
            if (item is not ClipboardRAGSourceItem ragItem) {
                throw new Exception("Incorrect argument type.");
            }
            // RAGSourceItemコレクションから、itemを削除
            var collection = GetClipboardDatabase().GetCollection<ClipboardRAGSourceItem>(RAGSourceItemCollectionName);
            collection.Delete(ragItem.Id);
        }

        // get
        public IEnumerable<RAGSourceItem> GetRAGSourceItems() {
            // RAGSourceItemコレクションから、すべてのアイテムを取得
            var collection = GetClipboardDatabase().GetCollection<ClipboardRAGSourceItem>(RAGSourceItemCollectionName);
            return collection.FindAll();
        }
        //----  ClipboardRAGSourceItem
        // update
        public void UpsertVectorDBItem(VectorDBItem item) {
            if (item is not ClipboardAppVectorDBItem clipboardAppVectorDB) {
                return;
            }
            // VectorDBItemコレクションに、itemを追加または更新
            var collection = GetClipboardDatabase().GetCollection<ClipboardAppVectorDBItem>(VectorDBItemCollectionName);
            collection.Upsert(clipboardAppVectorDB);

        }
        // delete
        public void DeleteVectorDBItem(VectorDBItem item) {
            // VectorDBItemコレクションから、itemを削除
            var collection = GetClipboardDatabase().GetCollection<ClipboardAppVectorDBItem>(VectorDBItemCollectionName);
            collection.Delete(item.Id);
        }
        // get
        public IEnumerable<VectorDBItem> GetVectorDBItems() {
            // VectorDBItemコレクションから、すべてのアイテムを取得
            var collection = GetClipboardDatabase().GetCollection<ClipboardAppVectorDBItem>(VectorDBItemCollectionName);
            return collection.FindAll();
        }

        public void UpsertAttachedItem(ContentAttachedItem item) {
            if (item is not ClipboardItemFile clipboardItemFile) {
                // Incorrect argument type error
                throw new Exception("Incorrect argument type.");
            }

            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFile>(CLIPBOARD_FILE_COLLECTION_NAME);
            collection.Upsert(clipboardItemFile);
        }
        public void DeleteAttachedItem(ContentAttachedItem item) {
            if (item is not ClipboardItemFile clipboardItemFile) {
                // Incorrect argument type error
                throw new Exception("Incorrect argument type.");
            }
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFile>(CLIPBOARD_FILE_COLLECTION_NAME);
            collection.Delete(clipboardItemFile.Id);
        }
        public ContentAttachedItem? GetAttachedItem(ObjectId id) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFile>(CLIPBOARD_FILE_COLLECTION_NAME);
            var item = collection.FindById(id);
            return item;
        }


        // -- VectorDBItem
        public VectorDBItem CreateVectorDBItem() {
            return new ClipboardAppVectorDBItem();
        }

    }
}
