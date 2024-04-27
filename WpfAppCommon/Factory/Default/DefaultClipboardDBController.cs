using System.Collections.ObjectModel;
using LiteDB;
using QAChat.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Factory.Default {
    public class DefaultClipboardDBController : IClipboardDBController {
        public static string CLIPBOARD_FOLDER_RELATION_NAME = "folder_relation";
        public static string CLIPBOARD_FOLDERS_COLLECTION_NAME = "folders";
        public static string SCRIPT_COLLECTION_NAME = "scripts";
        public static string AUTO_PROCESS_RULES_COLLECTION_NAME = "auto_process_rules";
        public static string TAG_COLLECTION_NAME = "tags";

        public static string SEARCH_CONDITION_RULES_COLLECTION_NAME = "search_condition_rules";
        public static string SEARCH_CONDITION_APPLIED_CONDITION_NAME = "applied_globally";
        public static string CHAT_SESSION_COLLECTION_NAME = "chat_session";

        public static string CLIPBOARD_ROOT_FOLDER_NAME = "clipboard";
        public static string SEARCH_ROOT_FOLDER_NAME = "search_folder";

        public const string PromptTemplateCollectionName = "PromptTemplate";

        private static LiteDatabase? db;
        // static

        // フォルダーを削除する
        public void DeleteFolder(ClipboardFolder folder) {
            // folderの子フォルダを再帰的に削除
            foreach (var i in folder.Children) {
                ClipboardFolder child = GetFolder(i.AbsoluteCollectionName);
                DeleteFolder(child);
            }

            // folderを削除
            var collection = GetClipboardDatabase().GetCollection<ClipboardFolder>(folder.AbsoluteCollectionName);
            collection.DeleteAll();
            // folderと親フォルダの関係を削除
            var parentCollection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(CLIPBOARD_FOLDER_RELATION_NAME);
            foreach (var i in parentCollection.FindAll().Where(x => x.ChildCollectionName == folder.AbsoluteCollectionName)) {
                if (i.ChildCollectionName == folder.AbsoluteCollectionName) {
                    parentCollection.Delete(i.Id);
                }
            }
            // CLIPBOARD_FOLDERS_COLLECTION_NAMEから削除
            var foldersCollection = GetClipboardDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            foldersCollection.Delete(folder.Id);

        }
        public static LiteDatabase GetClipboardDatabase() {
            if (db == null) {
                try {
                    db = new LiteDatabase("clipboard.db");

                    // BSonMapperの設定
                    // ClipboardItemFolderのChildren, Items, SearchConditionを無視する
                    var mapper = BsonMapper.Global;
                    mapper.Entity<ClipboardFolder>()
                        .Ignore(x => x.Children);
                    mapper.Entity<ClipboardFolder>()
                        .Ignore(x => x.Items);
                } catch (Exception e) {
                    Tools.Error("データベースのオープンに失敗しました。" + e.Message);
                    // データベースのオープンに失敗した場合は終了
                    Environment.Exit(1);
                }
            }
            return db;
        }
        // AbsoluteCollectionNameを指定してClipboardItemFolderを取得する
        public ClipboardFolder GetFolder(string collectionName) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            var item = collection.FindOne(x => x.AbsoluteCollectionName == collectionName);

            return item;
        }
        // RootFolderを取得する
        public ClipboardFolder GetRootFolder() {
            ClipboardFolder rootFolder = GetFolder(CLIPBOARD_ROOT_FOLDER_NAME);
            if (rootFolder == null) {
                rootFolder = new(CLIPBOARD_ROOT_FOLDER_NAME, "クリップボード");
                UpsertFolder(rootFolder);
            }
            return rootFolder;
        }

        public ClipboardFolder GetSearchRootFolder() {
            ClipboardFolder searchRootFolder = GetFolder(SEARCH_ROOT_FOLDER_NAME);
            if (searchRootFolder == null) {
                searchRootFolder = new(SEARCH_ROOT_FOLDER_NAME, "検索フォルダ");
                UpsertFolder(searchRootFolder);
            }
            return searchRootFolder;
        }

        // 指定したTargetFolderを持つAutoProcessRuleを取得する
        public IEnumerable<AutoProcessRule> GetAutoProcessRules(ClipboardFolder? targetFolder) {
            if (targetFolder == null) {
                return GetAllAutoProcessRules();
            }
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            var items = collection.FindAll()
                .Where(x => x.TargetFolder != null && x.TargetFolder.AbsoluteCollectionName == targetFolder.AbsoluteCollectionName)
                .OrderBy(x => x.RuleName);

            ObservableCollection<AutoProcessRule> result = [.. items];
            return result;
        }
        // すべてのAutoProcessRuleを取得する
        public IEnumerable<AutoProcessRule> GetAllAutoProcessRules() {
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(AUTO_PROCESS_RULES_COLLECTION_NAME);
            return collection.FindAll();
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
                && (x.RuleAction.Name == AutoProcessItem.AutoProcessActionName.CopyToFolder.Name
                    || x.RuleAction.Name == AutoProcessItem.AutoProcessActionName.MoveToFolder.Name));
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
        // 指定したAbsoluteCollectionNameに対応する検索条件を取得する
        public SearchRule? GetSearchRuleByFolderName(string collectionName) {
            var collection = GetClipboardDatabase().GetCollection<SearchRule>(SEARCH_CONDITION_RULES_COLLECTION_NAME);
            var item = collection.FindOne(x => x.SearchFolder != null && x.SearchFolder.AbsoluteCollectionName == collectionName);
            return item;
        }
        // --------------------------------------------------------------

        // 親フォルダのAbsoluteCollectionNameを指定して子フォルダのリストを取得する
        public IEnumerable<string> GetFolderRelations(string parentCollectionName) {
            List<string> result = new List<string>();
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(CLIPBOARD_FOLDER_RELATION_NAME);

            var items = collection.FindAll().Where(x => x.ParentCollectionName == parentCollectionName);
            foreach (var i in items) {
                result.Add(i.ChildCollectionName);
            }
            return result;
        }
        // 子フォルダのAbsoluteCollectionNameを指定して親フォルダを取得する。
        public string GetClipboardItemFolderParentRelation(string childCollectionName) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(CLIPBOARD_FOLDER_RELATION_NAME);
            var item = collection.FindOne(x => x.ChildCollectionName == childCollectionName);
            return item.ParentCollectionName;
        }

        // ClipboardItemをLiteDBに追加または更新する
        public void UpsertItem(ClipboardItem item, bool updateModifiedTime = true) {
            // 更新日時を設定
            if (updateModifiedTime) {
                item.UpdatedAt = DateTime.Now;
            }
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(item.CollectionName);
            collection.Upsert(item);
        }
        // アイテムをDBから削除する
        public void DeleteItem(ClipboardItem item) {
            if (item.Id == null) {
                return;
            }
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(item.CollectionName);
            // System.Windows.MessageBox.Show(item.CollectionName);
            collection.Delete(item.Id);
        }



        // ClipboardItemFolderをLiteDBに追加または更新する
        public void UpsertFolder(ClipboardFolder folder) {

            var collection = GetClipboardDatabase().GetCollection<ClipboardFolder>(CLIPBOARD_FOLDERS_COLLECTION_NAME);
            collection.Upsert(folder);

        }

        // 親フォルダと子フォルダを指定してClipboardItemFolderRelationを追加または更新する
        public void UpsertFolderRelation(ClipboardFolder parent, ClipboardFolder child) {
            // parentと、childの関係をLiteDBに追加または更新
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(CLIPBOARD_FOLDER_RELATION_NAME);
            ClipboardItemFolderRelation relation = new ClipboardItemFolderRelation(parent.AbsoluteCollectionName, child.AbsoluteCollectionName);
            collection.Upsert(relation);

        }
        // ClipboardItemのリストをLiteDBから取得する
        public IEnumerable<ClipboardItem> GetItems(string collectionName) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(collectionName);
            return collection.FindAll().OrderByDescending(x => x.UpdatedAt);
        }

        // ClipboardItemを検索する。
        public IEnumerable<ClipboardItem> SearchItems(string collectionName, SearchCondition searchCondition) {
            // 結果を格納するIEnumerable<ClipboardItem>を作成
            IEnumerable<ClipboardItem> result = new List<ClipboardItem>();
            // collectionNameのコレクションを取得
            var folder = GetFolder(collectionName);
            // フォルダが存在しない場合は、結果を返す
            if (folder == null) {
                return result;
            }
            // 検索条件が空の場合は、結果を返す
            if (searchCondition.IsEmpty()) {
                return result;
            }

            // folder内のアイテムを保持するコレクションを取得
            var collection = GetClipboardDatabase().GetCollection<ClipboardItem>(folder.AbsoluteCollectionName);
            // Filterの結果を結果に追加
            result = Filter(collection, searchCondition);

            // サブフォルダを含む場合は、対象フォルダとそのサブフォルダを検索
            if (searchCondition.IsIncludeSubFolder) {
                // 対象フォルダの子フォルダを取得
                var childFolders = GetChildFolders(folder.AbsoluteCollectionName);
                foreach (var childFolder in childFolders) {
                    // サブフォルダのアイテムを検索
                    var subFolderResult = SearchItems(childFolder, searchCondition);
                    // Filterの結果を結果に追加
                    result = result.Concat(subFolderResult);
                }
            }
            return result;

        }

        // 指定したフォルダの子フォルダのAbsoluteCollectionNameを再帰的に取得する
        public List<string> GetChildFolders(string parentCollectionName) {
            List<string> result = new List<string>();
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(CLIPBOARD_FOLDER_RELATION_NAME);
            var items = collection.FindAll().Where(x => x.ParentCollectionName == parentCollectionName);
            foreach (var i in items) {
                result.Add(i.ChildCollectionName);
                result.AddRange(GetChildFolders(i.ChildCollectionName));
            }
            return result;
        }


        private void LoadFolderTree(ClipboardFolder targetFolder) {
            // LiteDBから自分が親となっているフォルダを取得
            var childrenNames = GetFolderRelations(targetFolder.AbsoluteCollectionName);
            // Childrenをクリア
            targetFolder.Children.Clear();
            // childrenNamesからClipboardItemFolderを取得する
            foreach (var childName in childrenNames) {
                var child = GetFolder(childName);
                if (child != null) {
                    targetFolder.Children.Add(child);
                    // childを親としたフォルダツリーを再帰的に読み込む
                    LoadFolderTree(child);
                }
            }
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

        public IEnumerable<ClipboardItem> Filter(ILiteCollection<ClipboardItem> liteCollection, SearchCondition searchCondition) {
            if (searchCondition.IsEmpty()) {
                return liteCollection.FindAll();
            }

            var results = liteCollection.FindAll();
            // SearchConditionの内容に従ってフィルタリング
            if (string.IsNullOrEmpty(searchCondition.Description) == false) {
                if (searchCondition.ExcludeDescription) {
                    results = results.Where(x => x.Description.Contains(searchCondition.Description) == false);
                } else {
                    results = results.Where(x => x.Description.Contains(searchCondition.Description));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Content) == false) {
                if (searchCondition.ExcludeContent) {
                    results = results.Where(x => x.Content.Contains(searchCondition.Content) == false);
                } else {
                    results = results.Where(x => x.Content.Contains(searchCondition.Content));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Tags) == false) {
                if (searchCondition.ExcludeTags) {
                    results = results.Where(x => x.Tags.Contains(searchCondition.Tags) == false);
                } else {
                    results = results.Where(x => x.Tags.Contains(searchCondition.Tags));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationName) == false) {
                if (searchCondition.ExcludeSourceApplicationName) {
                    results = results.Where(x => x.SourceApplicationName.Contains(searchCondition.SourceApplicationName) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationName.Contains(searchCondition.SourceApplicationName));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationTitle) == false) {
                if (searchCondition.ExcludeSourceApplicationTitle) {
                    results = results.Where(x => x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle));
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

        public IEnumerable<ScriptItem> GetScriptItems() {
            var collection = GetClipboardDatabase().GetCollection<ScriptItem>(SCRIPT_COLLECTION_NAME);
            var items = collection.FindAll();
            return items.ToList();
        }

        // タグを取得する
        public IEnumerable<TagItem> GetTagList() {
            var collection = GetClipboardDatabase().GetCollection<TagItem>(TAG_COLLECTION_NAME);
            var items = collection.FindAll();
            return items;
        }

        // 名前を指定してタグを検索する
        public IEnumerable<TagItem> SearchTag(TagItem tag) {
            var collection = GetClipboardDatabase().GetCollection<TagItem>(TAG_COLLECTION_NAME);
            var tags = collection.FindAll().Where(x => x.Tag.Contains(tag.Tag));
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
            var tags = SearchTag(tag);
            var collection = GetClipboardDatabase().GetCollection<TagItem>(TAG_COLLECTION_NAME);
            foreach (var i in tags) {
                collection.Delete(i.Id);
            }
        }

        // タグを追加する
        public void InsertTag(TagItem tag) {
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

        public void UpsertPromptTemplate(PromptItem promptItem) {
            var db = GetClipboardDatabase();

            var col = db.GetCollection<PromptItem>(PromptTemplateCollectionName);
            col.Upsert(promptItem);
        }
        // プロンプトテンプレートを取得する
        public static PromptItem? GetPromptTemplate(string name) {
            var col = GetClipboardDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            return col.FindOne(x => x.Name == name);
        }
        // 引数として渡されたプロンプトテンプレートを削除する
        public void DeletePromptTemplate(PromptItem promptItem) {
            var col = GetClipboardDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            col.Delete(promptItem.Id);
        }

        // プロンプトテンプレートを全て取得する
        public ICollection<PromptItem> GetAllPromptTemplates() {
            ICollection<PromptItem> collation = new List<PromptItem>();
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

    }
}
