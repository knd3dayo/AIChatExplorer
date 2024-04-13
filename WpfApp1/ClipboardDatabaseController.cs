using System.Collections.ObjectModel;
using LiteDB;
using Python.Runtime;
using WpfApp1.Model;
using WpfApp1.Utils;

namespace WpfApp1
{
    public class ClipboardDatabaseController
    {
        public static string CLIPBOARD_FOLDER_RELATION_NAME = "folder_relation";
        public static string CLIPBOARD_FOLDERS_COLLECTION_NAME = "folders";
        public static string SCRIPT_COLLECTION_NAME = "scripts";
        public static string AUTO_PROCESS_RULES_COLLECTION_NAME = "auto_process_rules";
        
        public static string SEARCH_CONDITION_RULES_COLLECTION_NAME = "search_condition_rules";
        public static string SEARCH_CONDITION_APPLIED_CONDITION_NAME = "applied_globally";

        public static string CLIPBOARD_ROOT_FOLDER_NAME = "clipboard";
        public static string SEARCH_ROOT_FOLDER_NAME = "search_folder";

        private static LiteDatabase? db;
        // static
        public static string ConcatenatePath(string parentPath, string childPath)
        {
            if (string.IsNullOrEmpty(parentPath))
                return childPath;
            if (string.IsNullOrEmpty(childPath))
                return parentPath;
            return parentPath + "_" + childPath;
        }

        // フォルダーを削除する
        public static void DeleteFolder(ClipboardItemFolder folder)
        {
            // folderの子フォルダを再帰的に削除
            foreach (var i in folder.Children)
            { 
                ClipboardItemFolder child = GetClipboardItemFolder(i.AbsoluteCollectionName);
                DeleteFolder(child);
            }

            // folderを削除
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolder>(folder.AbsoluteCollectionName);
            collection.DeleteAll();
            // folderと親フォルダの関係を削除
            var parentCollection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(ClipboardDatabaseController.CLIPBOARD_FOLDER_RELATION_NAME);
            foreach (var i in parentCollection.FindAll().Where(x => x.ChildCollectionName == folder.AbsoluteCollectionName))
            {
                if (i.ChildCollectionName == folder.AbsoluteCollectionName)
                {
                    parentCollection.Delete(i.Id);
                }
            }
            // CLIPBOARD_FOLDERS_COLLECTION_NAMEから削除
            var foldersCollection = GetClipboardDatabase().GetCollection<ClipboardItemFolder>(ClipboardDatabaseController.CLIPBOARD_FOLDERS_COLLECTION_NAME);
            foldersCollection.Delete(folder.Id);

        }
        public static LiteDatabase GetClipboardDatabase(){
            if (db == null)
            {
                try
                {
                    db = new LiteDatabase("clipboard.db");

                    // BSonMapperの設定
                    // ClipboardItemFolderのChildren, Items, SearchConditionを無視する
                    var mapper = BsonMapper.Global;
                    mapper.Entity<ClipboardItemFolder>()
                        .Ignore(x => x.Children);
                    mapper.Entity<ClipboardItemFolder>()
                        .Ignore(x => x.Items);
                }

                catch (System.Exception e)
                {
                    Tools.Error("データベースのオープンに失敗しました。" + e.Message);
                    // データベースのオープンに失敗した場合は終了
                    System.Environment.Exit(1);
                }
            }
            return db;
        }
        // AbsoluteCollectionNameを指定してClipboardItemFolderを取得する
        public static ClipboardItemFolder GetClipboardItemFolder(string collectionName)
        {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolder>(ClipboardDatabaseController.CLIPBOARD_FOLDERS_COLLECTION_NAME);
            var item = collection.FindOne(x => x.AbsoluteCollectionName == collectionName);

            return item;
        }

        // 指定したTargetFolderを持つAutoProcessRuleを取得する
        public static ObservableCollection<AutoProcessRule> GetAutoProcessRules(ClipboardItemFolder? targetFolder)
        {
            if (targetFolder == null)
            {
                return GetAllAutoProcessRules();
            }
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(ClipboardDatabaseController.AUTO_PROCESS_RULES_COLLECTION_NAME);
            var items = collection.FindAll()
                .Where(x => x.TargetFolder != null && x.TargetFolder.AbsoluteCollectionName == targetFolder.AbsoluteCollectionName)
                .OrderBy(x => x.RuleName);

            ObservableCollection<AutoProcessRule> result = [.. items];
            return result;
        }
        // すべてのAutoProcessRuleを取得する
        public static ObservableCollection<AutoProcessRule> GetAllAutoProcessRules()
        {
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(ClipboardDatabaseController.AUTO_PROCESS_RULES_COLLECTION_NAME);
            var items = collection.FindAll();
            ObservableCollection<AutoProcessRule> result = [.. items];
            return result;
        }

        // AutoProcessRuleを追加または更新する
        public static void UpsertAutoProcessRule(AutoProcessRule rule)
        {
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(ClipboardDatabaseController.AUTO_PROCESS_RULES_COLLECTION_NAME);
            collection.Upsert(rule);
        }
        // AutoProcessRuleを削除する
        public static void DeleteAutoProcessRule(AutoProcessRule rule)
        {
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(ClipboardDatabaseController.AUTO_PROCESS_RULES_COLLECTION_NAME);
            collection.Delete(rule.Id);
        }

        // TypeがCopyTo または MoveToのルールをLiteDBから取得する。
        public static IEnumerable<AutoProcessRule> GetCopyToMoveToRules()
        {
            var collection = GetClipboardDatabase().GetCollection<AutoProcessRule>(ClipboardDatabaseController.AUTO_PROCESS_RULES_COLLECTION_NAME);
            var items = collection.FindAll().Where(
                x => x.RuleAction != null
                && (x.RuleAction.Name== AutoProcessItem.ActionName.CopyToFolder.Name
                    || x.RuleAction.Name == AutoProcessItem.ActionName.MoveToFolder.Name));
            return items;

        }
        // SearchConditionを追加または更新する
        public static void UpsertSearchConditionRule(SearchConditionRule conditionRule)
        {
            var collection = GetClipboardDatabase().GetCollection<SearchConditionRule>(ClipboardDatabaseController.SEARCH_CONDITION_RULES_COLLECTION_NAME);
            collection.Upsert(conditionRule);
        }
        // SearchConditionを削除する
        public static void DeleteSearchConditionRule(SearchConditionRule conditionRule)
        {
            var collection = GetClipboardDatabase().GetCollection<SearchCondition>(ClipboardDatabaseController.SEARCH_CONDITION_RULES_COLLECTION_NAME);
            collection.Delete(conditionRule.Id);
        }
        // SearchConditionを取得する
        public static ObservableCollection<SearchConditionRule> GetSearchConditionRules()
        {
            var collection = GetClipboardDatabase().GetCollection<SearchConditionRule>(ClipboardDatabaseController.SEARCH_CONDITION_RULES_COLLECTION_NAME);
            var items = collection.FindAll();
            ObservableCollection<SearchConditionRule> result = [.. items];
            return result;
        }
        // 名前を指定して検索条件を取得する
        public static SearchConditionRule? GetSearchConditionRule(string name)
        {
            var collection = GetClipboardDatabase().GetCollection<SearchConditionRule>(ClipboardDatabaseController.SEARCH_CONDITION_RULES_COLLECTION_NAME);
            var item = collection.FindOne(x => x.Name == name);
            return item;
        }
        // 指定したAbsoluteCollectionNameに対応する検索条件を取得する
        public static SearchConditionRule? GetSearchConditionRuleByCollectionName(string collectionName) {
            var collection = GetClipboardDatabase().GetCollection<SearchConditionRule>(ClipboardDatabaseController.SEARCH_CONDITION_RULES_COLLECTION_NAME);
            var item = collection.FindOne(x => x.SearchFolder != null && x.SearchFolder.AbsoluteCollectionName == collectionName);
            return item;
        }
        // --------------------------------------------------------------

        // 親フォルダのAbsoluteCollectionNameを指定して子フォルダのリストを取得する
        public static List<string> GetClipboardItemFolderChildRelations(string parentCollectionName)
        {
            List<string> result = new List<string>();
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(ClipboardDatabaseController.CLIPBOARD_FOLDER_RELATION_NAME);

            var items = collection.FindAll().Where(x => x.ParentCollectionName == parentCollectionName);
            foreach (var i in items)
            {
                result.Add(i.ChildCollectionName);
            }
            return result;
        }
        // 子フォルダのAbsoluteCollectionNameを指定して親フォルダを取得する。
        public static string GetClipboardItemFolderParentRelation(string childCollectionName) {
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(ClipboardDatabaseController.CLIPBOARD_FOLDER_RELATION_NAME);
            var item = collection.FindOne(x => x.ChildCollectionName == childCollectionName);
            return item.ParentCollectionName;
        }

        // 第1引数にフォルダパスをして、第2引数に子フォルダのパスの第1引数のフォルダパスが第2引数の祖先フォルダかどうかを返す
        public static bool IsAncestorFolder(string parentPath, string childPath) {
            if (string.IsNullOrEmpty(parentPath) || string.IsNullOrEmpty(childPath)) {
                return false;
            }
            if (childPath.StartsWith(parentPath) == false) {
                return false;
            }
            return true;
        }

        // ClipboardItemをLiteDBに追加または更新する
        public static void UpsertItem(ClipboardItem item)
        {
            // 更新日時を設定
            item.UpdatedAt = DateTime.Now;
            var collection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ClipboardItem>(item.CollectionName);
            collection.Upsert(item);
        }
        // アイテムをDBから削除する
        public static void DeleteItem(ClipboardItem item)
        {
            if (item.Id == null) {
                return;
            }
            var collection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ClipboardItem>(item.CollectionName);
            // System.Windows.MessageBox.Show(item.CollectionName);
            collection.Delete(item.Id);
        }
        // アイテムをDBから削除する
        public static void DeleteItems(List<ClipboardItem> items) {
            foreach (var item in items) {
                DeleteItem(item);
            }
        }



        // ClipboardItemFolderをLiteDBに追加または更新する
        public static void UpsertFolder(ClipboardItemFolder folder)
        {

            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolder>(ClipboardDatabaseController.CLIPBOARD_FOLDERS_COLLECTION_NAME);
            collection.Upsert(folder);

        }

        // 親フォルダと子フォルダを指定してClipboardItemFolderRelationを追加または更新する
        public static void UpsertFolderRelation(ClipboardItemFolder parent , ClipboardItemFolder child)
        {
            // parentと、childの関係をLiteDBに追加または更新
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(ClipboardDatabaseController.CLIPBOARD_FOLDER_RELATION_NAME);
            ClipboardItemFolderRelation relation = new ClipboardItemFolderRelation(parent.AbsoluteCollectionName, child.AbsoluteCollectionName);
            collection.Upsert(relation);

        }
        // ClipboardItemのリストをLiteDBから取得するObservableCollectionOn
        public static IEnumerable<ClipboardItem> GetClipboardItems(string collectionName, SearchCondition? searchCondition)
        {
            var collection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ClipboardItem>(collectionName);
            if (searchCondition == null || searchCondition.IsEmpty()) {
                return collection.FindAll().OrderByDescending(x => x.UpdatedAt);
            }
            else {
                return Filter(collection, searchCondition).OrderByDescending(x => x.UpdatedAt);
            }
        }

        // ClipboardItemを検索する。
        public static IEnumerable<ClipboardItem> SearchClipboardItems(SearchConditionRule searchConditionRule) {
            // 結果を格納するIEnumerable<ClipboardItem>を作成
            IEnumerable<ClipboardItem> result = new List<ClipboardItem>();
            // TargetFolderがない場合は何もしない
            if (searchConditionRule.TargetFolder == null) {
                return result;
            }
            // サブフォルダを含むかどうか
            bool includeSubFolder = searchConditionRule.IsIncludeSubFolder;
            // サブフォルダを含まない場合は、対象フォルダのみ検索
            var collection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ClipboardItem>(searchConditionRule.TargetFolder.AbsoluteCollectionName);
            // Filterの結果を結果に追加
            result = Filter(collection, searchConditionRule.SearchCondition);
            // サブフォルダを含む場合は、対象フォルダとそのサブフォルダを検索
            if (includeSubFolder) {
                // 対象フォルダの子フォルダを取得
                var folders = GetChildFolders(searchConditionRule.TargetFolder.AbsoluteCollectionName);
                foreach (var folder in folders) {
                    // サブフォルダのアイテムを取得
                    var subCollection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ClipboardItem>(folder);
                    // Filterの結果を結果に追加
                    result = result.Concat(Filter(subCollection, searchConditionRule.SearchCondition));
                }
            }
            return result;

        }

        // 指定したフォルダの子フォルダのAbsoluteCollectionNameを再帰的に取得する
        public static List<string> GetChildFolders(string parentCollectionName) {
            List<string> result = new List<string>();
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(ClipboardDatabaseController.CLIPBOARD_FOLDER_RELATION_NAME);
            var items = collection.FindAll().Where(x => x.ParentCollectionName == parentCollectionName);
            foreach (var i in items) {
                result.Add(i.ChildCollectionName);
                result.AddRange(GetChildFolders(i.ChildCollectionName));
            }
            return result;
        }


        private static void LoadFolderTree(ClipboardItemFolder targetFolder)
        {
            // LiteDBから自分が親となっているフォルダを取得
            var childrenNames = ClipboardDatabaseController.GetClipboardItemFolderChildRelations(targetFolder.AbsoluteCollectionName);
            // Childrenをクリア
            targetFolder.Children.Clear();
            // childrenNamesからClipboardItemFolderを取得する
            foreach (var childName in childrenNames)
            {
                var child = ClipboardDatabaseController.GetClipboardItemFolder(childName);
                if (child != null)
                {
                    targetFolder.Children.Add(child);
                    // childを親としたフォルダツリーを再帰的に読み込む
                    LoadFolderTree(child);
                }
            }
        }

        public static void DeleteItems(ClipboardItemFolder targetFolder)
        {
            foreach (var item in targetFolder.Items)
            {
                ClipboardDatabaseController.DeleteItem(item);
            }
            targetFolder.Items.Clear();
            ClipboardDatabaseController.UpsertFolder(targetFolder);

        }

        public static IEnumerable<ClipboardItem> Filter(ILiteCollection<ClipboardItem> liteCollection, SearchCondition searchCondition)
        {
            if (searchCondition.IsEmpty())
            {
                return liteCollection.FindAll();
            }

            var results = liteCollection.FindAll();
            // SearchConditionの内容に従ってフィルタリング
            if (string.IsNullOrEmpty(searchCondition.Description) == false)
            {
                if (searchCondition.ExcludeDescription)
                {
                    results = results.Where(x => x.Description.Contains(searchCondition.Description) == false);
                }
                else
                {
                    results = results.Where(x => x.Description.Contains(searchCondition.Description));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Content) == false)
            {
                if (searchCondition.ExcludeContent)
                {
                    results = results.Where(x => x.Content.Contains(searchCondition.Content) == false);
                }
                else
                {
                    results = results.Where(x => x.Content.Contains(searchCondition.Content));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Tags) == false)
            {
                if (searchCondition.ExcludeTags)
                {
                    results = results.Where(x => x.Tags.Contains(searchCondition.Tags) == false);
                }
                else
                {
                    results = results.Where(x => x.Tags.Contains(searchCondition.Tags));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationName) == false)
            {
                if (searchCondition.ExcludeSourceApplicationName)
                {
                    results = results.Where(x => x.SourceApplicationName.Contains(searchCondition.SourceApplicationName) == false);
                }
                else
                {
                    results = results.Where(x => x.SourceApplicationName.Contains(searchCondition.SourceApplicationName));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationTitle) == false)
            {
                if (searchCondition.ExcludeSourceApplicationTitle)
                {
                    results = results.Where(x => x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle) == false);
                }
                else
                {
                    results = results.Where(x => x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle));
                }
            }
            if (searchCondition.EnableStartTime)
            {
                results = results.Where(x => x.CreatedAt > searchCondition.StartTime);
            }
            if (searchCondition.EnableEndTime)
            {
                results = results.Where(x => x.CreatedAt < searchCondition.EndTime);
            }
            results = results.OrderByDescending(x => x.UpdatedAt);

            return results;

        }
        public static List<ScriptItem> GetScriptItems() {
            var collection = GetClipboardDatabase().GetCollection<ScriptItem>(SCRIPT_COLLECTION_NAME);
            var items = collection.FindAll();
            return items.ToList();
        }


    }
}
