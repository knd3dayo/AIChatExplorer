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
        public static string SEARCH_CONDITIONS_COLLECTION_NAME = "search_conditions";

        public static string CLIPBOARD_ROOT_FOLDER_NAME = "clipboard";
        public static string SEARCH_ROOT_FOLDER_NAME = "search_folder";
        public static string SEARCH_CONDITION_APPLIED_CONDITION_NAME = "applied_globally";

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
                    var mapper = BsonMapper.Global;
                    mapper.Entity<ClipboardItemFolder>()
                        .Ignore(x => x.Children);
                    mapper.Entity<ClipboardItemFolder>()
                        .Ignore(x => x.Items);
                    mapper.Entity<ClipboardItemFolder>()
                        .Ignore(x => x.SearchCondition);
                    // ClipboardItem
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
            var items = collection.FindAll().Where(x => x.TargetFolder != null && x.TargetFolder.AbsoluteCollectionName == targetFolder.AbsoluteCollectionName);
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
                && (x.RuleAction.Type == AutoProcessItem.ActionType.CopyToFolder
                    || x.RuleAction.Type == AutoProcessItem.ActionType.MoveToFolder
                    )
                    );

            return items;

        }
        // SearchConditionを追加または更新する
        public static void UpsertSearchCondition(SearchCondition condition)
        {
            var collection = GetClipboardDatabase().GetCollection<SearchCondition>(ClipboardDatabaseController.SEARCH_CONDITIONS_COLLECTION_NAME);
            collection.Upsert(condition);
        }
        // SearchConditionを削除する
        public static void DeleteSearchCondition(SearchCondition condition)
        {
            var collection = GetClipboardDatabase().GetCollection<SearchCondition>(ClipboardDatabaseController.SEARCH_CONDITIONS_COLLECTION_NAME);
            collection.Delete(condition.Id);
        }
        // SearchConditionを取得する
        public static ObservableCollection<SearchCondition> GetSearchConditions()
        {
            var collection = GetClipboardDatabase().GetCollection<SearchCondition>(ClipboardDatabaseController.SEARCH_CONDITIONS_COLLECTION_NAME);
            var items = collection.FindAll();
            ObservableCollection<SearchCondition> result = [.. items];
            return result;
        }
        // 名前を指定して検索条件を取得する
        public static SearchCondition? GetSearchCondition(string name)
        {
            var collection = GetClipboardDatabase().GetCollection<SearchCondition>(ClipboardDatabaseController.SEARCH_CONDITIONS_COLLECTION_NAME);
            var item = collection.FindOne(x => x.Name == name);
            return item;
        }
        // --------------------------------------------------------------

        // 親フォルダのAbsoluteCollectionNameを指定して子フォルダのリストを取得する
        public static List<string> GetClipboardItemFolderRelation(string parentCollectionName)
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
            var collection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ClipboardItem>(item.CollectionName);
            // System.Windows.MessageBox.Show(item.CollectionName);
            collection.Delete(item.Id);
        }



        // ClipboardItemFolderをLiteDBに追加または更新する
        public static void UpsertFolder(ClipboardItemFolder folder)
        {
            // ★ IsSearchFolderがTrueの場合はSearchConditionを保存する、そうでない場合はLiteDBに保存しない
            var tmpSearchCondition = folder.SearchCondition;
            if (folder.IsSearchFolder == false)
            {
                folder.SearchCondition = new SearchCondition();
            }

            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolder>(ClipboardDatabaseController.CLIPBOARD_FOLDERS_COLLECTION_NAME);
            collection.Upsert(folder);

            // SearchConditionを再設定
            folder.SearchCondition = tmpSearchCondition;
            // folderがSearchFolderの場合はここで終了
            if (folder.IsSearchFolder)
            {
                return;
            }

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
        public static IEnumerable<ClipboardItem> GetClipboardItemCollection(string collectionName)
        {
            var collection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ClipboardItem>(collectionName);
            // UpdatedAtの降順で取得
            return collection.FindAll().OrderByDescending(x => x.UpdatedAt);
        }

        public static bool Load(ClipboardItemFolder targetFolder)
        {
            if (targetFolder.IsSearchFolder || targetFolder.AbsoluteCollectionName == SEARCH_ROOT_FOLDER_NAME)
            {
                return LoadSearchFolder(targetFolder);
            }
            else
            {
                SearchCondition searchCondition = ClipboardItemFolder.GlobalSearchCondition;
                return LoadNormalFolder(targetFolder, searchCondition);
            }
        }
        public static bool LoadNormalFolder(ClipboardItemFolder targetFolder, SearchCondition searchCondition)
        {
            // LiteDBからClipboardItemFolderを取得
            var folder = ClipboardDatabaseController.GetClipboardItemFolder(targetFolder.AbsoluteCollectionName);
            if (folder == null)
            {
                return false;
            }

            // folderの内容をコピー
            targetFolder.AbsoluteCollectionName = folder.AbsoluteCollectionName;
            targetFolder.DisplayName = folder.DisplayName;
            targetFolder.IsApplyingSearchCondition = folder.IsApplyingSearchCondition;
            targetFolder.IsSearchFolder = folder.IsSearchFolder;

            // 検索条件がNullまたは空でない場合はFilterを実行
            if (searchCondition.IsEmpty() == false)
            {
                // 現在検索中フラグを立てる
                targetFolder.IsApplyingSearchCondition = true;
                Filter(targetFolder, searchCondition);
            }
            else
            {
                // フィルダーを実行しない場合は、Itemsを取得
                // ItemsをLiteDBのAbsoluteCollectionNameから取得
                var clipboardItems = ClipboardDatabaseController.GetClipboardItemCollection(targetFolder.AbsoluteCollectionName);
                // 
                targetFolder.Items.Clear();
                foreach (var item in clipboardItems)
                {
                    targetFolder.Items.Add(item);
                }
                // 現在検索中フラグをクリア
                targetFolder.IsApplyingSearchCondition = false;
            }

            return true;
        }

        public static bool LoadSearchFolder(ClipboardItemFolder targetFolder)
        {
            // LiteDBからClipboardItemFolderを取得
            var folder = ClipboardDatabaseController.GetClipboardItemFolder(targetFolder.AbsoluteCollectionName);
            if (folder == null)
            {
                Tools.Info("フォルダが見つかりませんでした。");
                return false;
            }
            // folderの内容をコピー
            targetFolder.AbsoluteCollectionName = folder.AbsoluteCollectionName;
            targetFolder.DisplayName = folder.DisplayName;
            targetFolder.IsApplyingSearchCondition = folder.IsApplyingSearchCondition;
            targetFolder.SearchCondition = folder.SearchCondition;
            targetFolder.IsSearchFolder = folder.IsSearchFolder;

            // LiteDBから検索対象のフォルダを取得
            var searchFolder = ClipboardDatabaseController.GetClipboardItemFolder(folder.SearchFolderAbsoluteCollectionName);
            if (searchFolder == null)
            {
                // 現在検索中フラグをクリア
                targetFolder.IsApplyingSearchCondition = false;
                Tools.Info("検索対象フォルダが見つかりませんでした。");
            }
            // 検索条件がNullまたは空の場合は何もしない
            else if (targetFolder.SearchCondition.IsEmpty())
            {
                Tools.Info("検索条件が設定されていません。");
                // 現在検索中フラグをクリア
                targetFolder.IsApplyingSearchCondition = false;
            }
            else
            {
                // 検索条件がNullまたは空でない場合はFilterを実行
                // 現在検索中フラグを立てる
                targetFolder.IsApplyingSearchCondition = true;
                Filter(searchFolder, targetFolder.SearchCondition);
                // 検索結果のItemをTargetFolderにコピー
                targetFolder.Items.Clear();
                foreach (var item in searchFolder.Items)
                {
                    targetFolder.Items.Add(item);
                }
                // 現在検索中フラグをたてる
                targetFolder.IsApplyingSearchCondition = true;
            }

            return true;
        }


        public static void LoadFolderTree(ClipboardItemFolder targetFolder)
        {
            // LiteDBから自分が親となっているフォルダを取得
            var childrenNames = ClipboardDatabaseController.GetClipboardItemFolderRelation(targetFolder.AbsoluteCollectionName);
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

        // フォルダツリーを取得する
        public static ClipboardItemFolder? GetFolderTree()
        {
            var rootFolder = ClipboardDatabaseController.GetClipboardItemFolder(ClipboardDatabaseController.CLIPBOARD_ROOT_FOLDER_NAME);
            if (rootFolder == null)
            {
                return null;
            }

            LoadFolderTree(rootFolder);
            return rootFolder;
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

        public static void Filter(ClipboardItemFolder targetFolder, SearchCondition searchCondition)
        {
            if (searchCondition.IsEmpty())
            {
                return;
            }

            var results = ClipboardDatabaseController.GetClipboardItemCollection(targetFolder.AbsoluteCollectionName);
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


            targetFolder.Items.Clear();
            foreach (var item in results)
            {
                targetFolder.Items.Add(item);
            }

        }



    }
}
