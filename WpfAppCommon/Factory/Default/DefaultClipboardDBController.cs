using System.IO;
using LiteDB;
using PythonAILib.Model;
using QAChat.Model;
using WpfAppCommon.Model;

namespace WpfAppCommon.Factory.Default {
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

        // RAGSourceItem
        public const string RAGSourceItemCollectionName = "RAGSourceItem";
        // VectorDBItem
        public const string VectorDBItemCollectionName = "VectorDBItem";

        private static LiteDatabase? db;

        // --- static method ----------------------------------------------

        public static LiteDatabase GetClipboardDatabase() {
            if (db == null) {
                try {
                    /// AppDataフォルダーパスを取得
                    string appDataPath = ClipboardAppConfig.AppDataFolder;
                    // データベースファイルのパスを作成
                    string dbPath = Path.Combine(appDataPath, "clipboard.db");
                    db = new LiteDatabase(dbPath);

                    // BSonMapperの設定
                    // ClipboardItemFolderのChildren, Items, SearchConditionを無視する
                    var mapper = BsonMapper.Global;
                    mapper.Entity<ClipboardFolder>()
                        .Ignore(x => x.Items);
                    mapper.Entity<ClipboardFolder>()
                        .Ignore(x => x.Children);
                    mapper.Entity<ClipboardItem>()
                        .Ignore(x => x.ClipboardItemImages);

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
        public ClipboardFolder? GetRootFolder(string folderName) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardFolder.RootFolderInfo>(CLIPBOARD_ROOT_FOLDERS_COLLECTION_NAME);
            // Debug FolderName = folderNameのアイテムが複数ある時はエラー
            var items = collection.FindAll().Where(x => x.FolderName == folderName);
            if (items.Count() > 1) {
                throw new Exception("RootFolderInfoに同じFolderNameが複数存在します。");
            }

            ClipboardFolder.RootFolderInfo? item = collection.FindOne(x => x.FolderName == folderName);
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
                var rootFolder = GetRootFolder(folder.FolderName);
                // ルートフォルダが存在しない場合は、新規作成
                if (rootFolder == null) {
                    var rootFolderInfoCollection = GetClipboardDatabase().GetCollection<ClipboardFolder.RootFolderInfo>(CLIPBOARD_ROOT_FOLDERS_COLLECTION_NAME);
                    var rootFolderInfo = new ClipboardFolder.RootFolderInfo();
                    rootFolderInfo.FolderName = folder.FolderName;
                    rootFolderInfo.Id = ObjectId.NewObjectId();
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
        public ClipboardItem? GetItem(ObjectId id) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(CLIPBOARD_ITEM_COLLECTION_NAME);
            var item = collection.FindById(id);
            return item;
        }

        // ClipboardItemをLiteDBに追加または更新する
        public void UpsertItem(ClipboardItem item, bool updateModifiedTime = true) {
            // 更新日時を設定
            if (updateModifiedTime) {
                item.UpdatedAt = DateTime.Now;
            }
            // 画像イメージがある場合は、追加または更新
            foreach (var image in item.ClipboardItemImages) {
                UpsertItemImage(image);
            }
            // ファイルがある場合は、追加または更新
            foreach (var file in item.ClipboardItemFiles) {
                UpsertItemFile(file);
            }
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(CLIPBOARD_ITEM_COLLECTION_NAME);
            collection.Upsert(item);
        }
        // アイテムをDBから削除する
        public void DeleteItem(ClipboardItem item) {
            if (item.Id == null) {
                return;
            }
            // 画像イメージがある場合は、削除
            foreach (var image in item.ClipboardItemImages) {
                DeleteItemImage(image);
            }
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(CLIPBOARD_ITEM_COLLECTION_NAME);
            // System.Windows.MessageBox.Show(item.CollectionName);
            collection.Delete(item.Id);
        }
        // ClipboardItemのリストをLiteDBから取得する
        public IEnumerable<ClipboardItem> GetItems(ClipboardFolder folder) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(CLIPBOARD_ITEM_COLLECTION_NAME);
            var items = collection.FindAll().Where(x => x.FolderObjectId == folder.Id).OrderByDescending(x => x.UpdatedAt);
            return items;

        }

        // ClipboardItemを検索する。
        public IEnumerable<ClipboardItem> SearchItems(ClipboardFolder folder, SearchCondition searchCondition) {
            // 結果を格納するIEnumerable<ClipboardItem>を作成
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

        public void UpsertPromptTemplate(PromptItem promptItem) {
            var db = GetClipboardDatabase();

            var col = db.GetCollection<PromptItem>(PromptTemplateCollectionName);
            col.Upsert(promptItem);
        }
        // プロンプトテンプレートを取得する
        public PromptItem GetPromptTemplate(ObjectId objectId) {
            var col = GetClipboardDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            return col.FindById(objectId);
        }


        // 引数として渡されたプロンプトテンプレートを削除する
        public void DeletePromptTemplate(PromptItem promptItem) {
            var col = GetClipboardDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            col.Delete(promptItem.Id);
        }

        // プロンプトテンプレートを全て取得する
        public ICollection<PromptItem> GetAllPromptTemplates() {
            ICollection<PromptItem> collation = [];
            var col = GetClipboardDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            foreach (var item in col.FindAll()) {
                collation.Add(item);
            }
            return collation;
        }
        // プロンプトテンプレートを全て削除する
        public static void DeleteAllPromptTemplates() {
            var col = GetClipboardDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            col.DeleteAll();
        }

        //----  RAGSourceItem
        // update
        public void UpsertRAGSourceItem(RAGSourceItem item) {
            // RAGSourceItemコレクションに、itemを追加または更新
            var collection = GetClipboardDatabase().GetCollection<RAGSourceItem>(RAGSourceItemCollectionName);
            collection.Upsert(item);

        }
        // delete
        public void DeleteRAGSourceItem(RAGSourceItem item) {
            // RAGSourceItemコレクションから、itemを削除
            var collection = GetClipboardDatabase().GetCollection<RAGSourceItem>(RAGSourceItemCollectionName);
            collection.Delete(item.Id);
        }

        // get
        public IEnumerable<RAGSourceItem> GetRAGSourceItems() {
            // RAGSourceItemコレクションから、すべてのアイテムを取得
            var collection = GetClipboardDatabase().GetCollection<RAGSourceItem>(RAGSourceItemCollectionName);
            return collection.FindAll();
        }
        //----  RAGSourceItem
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

        //-- ClipboardItemImage
        // ClipboardItemImageを追加または更新する
        public void UpsertItemImage(ClipboardItemImage item) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemImage>(CLIPBOARD_IMAGE_COLLECTION_NAME);
            collection.Upsert(item);
        }
        // ClipboardItemImageを削除する
        public void DeleteItemImage(ClipboardItemImage item) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemImage>(CLIPBOARD_IMAGE_COLLECTION_NAME);
            collection.Delete(item.Id);
        }

        // 指定したIDに対応するClipboardItemImageを取得する
        public ClipboardItemImage? GetItemImage(ObjectId id) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemImage>(CLIPBOARD_IMAGE_COLLECTION_NAME);
            var item = collection.FindById(id);
            return item;
        }

        public void UpsertItemFile(ClipboardItemFile item) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFile>(CLIPBOARD_FILE_COLLECTION_NAME);
            collection.Upsert(item);
        }
        public void DeleteItemFile(ClipboardItemFile item) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFile>(CLIPBOARD_FILE_COLLECTION_NAME);
            collection.Delete(item.Id);
        }
        public ClipboardItemFile? GetItemFile(ObjectId id) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFile>(CLIPBOARD_FILE_COLLECTION_NAME);
            var item = collection.FindById(id);
            return item;
        }


    }
}
