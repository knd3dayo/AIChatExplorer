using System.Collections.ObjectModel;
using LiteDB;
using Python.Runtime;

namespace WpfApp1
{
    public class ClipboardDatabaseController
    {
        public static string CLIPBOARD_FOLDER_RELATION_NAME = "folder_relation";
        public static string CLIPBOARD_ROOT_FOLDER_NAME = "clipboard";
        public static string CLIPBOARD_FOLDERS_COLLECTION_NAME = "folders";
        public static string SCRIPT_COLLECTION_NAME = "scripts";

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
                db = new LiteDatabase("clipboard.db");
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

        // ClipboardItemFolderをLiteDBに追加または更新する
        public static void UpsertFolder(ClipboardItemFolder folder)
        {
            // ★ folderのChildrenのItemおよびChildrenはLiteDBに保存しない。Load時に取得する。
            var tmpChildren = folder.Children;
            folder.Children = new ObservableCollection<ClipboardItemFolder>();
            // ★ folderのItemsはCLIPBOARD_FOLDERS_COLLECTION_NAMEに保存しない。AbsoluteCollectionNameコレクションに保存する。
            var tmpItems = folder.Items;
            folder.Items = new ObservableCollection<ClipboardItem>();

            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolder>(ClipboardDatabaseController.CLIPBOARD_FOLDERS_COLLECTION_NAME);
            collection.Upsert(folder);
            // AbsoluteCollectionNameのコレクションを取得
            var itemsCollection = GetClipboardDatabase().GetCollection<ClipboardItem>(folder.AbsoluteCollectionName);

            // folderのItemsを保存
            foreach (var i in tmpItems)
            {
                itemsCollection.Upsert(i);
            }
            // folderのItemsを再設定
            folder.Items = tmpItems;

            // folderのChildrenを再設定
            folder.Children = tmpChildren;

        }

        // 親フォルダと子フォルダを指定してClipboardItemFolderRelationを追加または更新する
        public static void UpsertFolderRelation(ClipboardItemFolder parent , ClipboardItemFolder child)
        {
            // parentと、childの関係をLiteDBに追加または更新
            var collection = GetClipboardDatabase().GetCollection<ClipboardItemFolderRelation>(ClipboardDatabaseController.CLIPBOARD_FOLDER_RELATION_NAME);
            ClipboardItemFolderRelation relation = new ClipboardItemFolderRelation(parent.AbsoluteCollectionName, child.AbsoluteCollectionName);
            collection.Upsert(relation);

        }

        public static bool Load(ClipboardItemFolder targetFolder)
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
            targetFolder.AlwaysApplySearchCondition = folder.AlwaysApplySearchCondition;
            targetFolder.IsApplyingSearchCondition = folder.IsApplyingSearchCondition;
            targetFolder.SearchCondition.CopyFrom(folder.SearchCondition);

            // ItemsをLiteDBのAbsoluteCollectionNameから取得
            var clipboardItems = ClipboardController.GetClipboardItemCollection(targetFolder.AbsoluteCollectionName);
            targetFolder.Items.Clear();
            foreach (var item in clipboardItems)
            {
                targetFolder.Items.Add(item);
            }
            // 常に検索条件を適用する場合はFilterを実行
            if (targetFolder.AlwaysApplySearchCondition)
            {
                // 現在検索中フラグを立てる
                targetFolder.IsApplyingSearchCondition = true;
                Filter(targetFolder, targetFolder.SearchCondition);
            }
            else
            {
                // 現在検索中フラグをクリア
                targetFolder.IsApplyingSearchCondition = false;
            }

            // ClipboardItemFolderRelationから子フォルダを取得
            LoadFolderTree(targetFolder);

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

        public static void DeleteItems(ClipboardItemFolder targetFolder)
        {
            foreach (var item in targetFolder.Items)
            {
                ClipboardController.DeleteItem(item);
            }
            targetFolder.Items.Clear();
            ClipboardDatabaseController.UpsertFolder(targetFolder);

        }

        public static void Filter(ClipboardItemFolder targetFolder, SearchCondition searchCondition)
        {
            targetFolder.SearchCondition.CopyFrom(searchCondition);

            targetFolder.Items.Clear();

            var results = ClipboardController.GetClipboardItemCollection(targetFolder.AbsoluteCollectionName);
            // SearchConditionの内容に従ってフィルタリング
            if (string.IsNullOrEmpty(targetFolder.SearchCondition.Description) == false)
            {
                results = results.Where(x => x.Description != null && x.Description.Contains(targetFolder.SearchCondition.Description));
            }
            if (string.IsNullOrEmpty(targetFolder.SearchCondition.Content) == false)
            {
                results = results.Where(x => x.Content != null && x.Content.Contains(targetFolder.SearchCondition.Content));
            }
            if (string.IsNullOrEmpty(targetFolder.SearchCondition.Tags) == false)
            {
                results = results.Where(x => x.Tags != null && x.Tags.Contains(targetFolder.SearchCondition.Tags));
            }
            if (string.IsNullOrEmpty(targetFolder.SearchCondition.SourceApplicationName) == false)
            {
                results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(targetFolder.SearchCondition.SourceApplicationName));
            }
            if (string.IsNullOrEmpty(targetFolder.SearchCondition.SourceApplicationTitle) == false)
            {
                results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(targetFolder.SearchCondition.SourceApplicationTitle));
            }
            if (targetFolder.SearchCondition.EnableStartTime)
            {
                results = results.Where(x => x.CreatedAt > targetFolder.SearchCondition.StartTime);
            }
            if (targetFolder.SearchCondition.EnableEndTime)
            {
                results = results.Where(x => x.CreatedAt < targetFolder.SearchCondition.EndTime);
            }
            results = results.OrderByDescending(x => x.UpdatedAt);

            foreach (var item in results)
            {
                targetFolder.Items.Add(item);
            }

        }



    }
}
