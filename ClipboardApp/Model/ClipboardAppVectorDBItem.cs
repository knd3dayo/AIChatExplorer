using System.IO;
using ClipboardApp.Factory;
using PythonAILib.Model.VectorDB;
using WpfAppCommon.Model;

namespace ClipboardApp.Model {

    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class ClipboardAppVectorDBItem : VectorDBItem {

        // システム共通のベクトルDB
        public static VectorDBItem SystemCommonVectorDB {
            get {
                IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
                return dbController.GetSystemVectorDBItem();
            }
        }


        public ClipboardAppVectorDBItem() { }

        // Get
        public static IEnumerable<VectorDBItem> GetItems(bool includeSystemVectorDB) {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // GetItemsメソッドを呼び出して取得
            IEnumerable<VectorDBItem> items = dbController.GetVectorDBItems();
            if (!includeSystemVectorDB) {
                items = items.Where(item => !item.IsSystem && item.Name != SystemCommonVectorDBName);
            }

            return items;
        }
        public static IEnumerable<VectorDBItem> GetEnabledItems(bool includeSystemVectorDB) {
            return GetItems(includeSystemVectorDB).Where(item => item.IsEnabled);
        }

        public static VectorDBItem GetFolderVectorDBItem(ClipboardFolder folder) {
            // SystemCommonVectorDBからコピーを作成
            ClipboardAppVectorDBItem item = new() {
                Id = SystemCommonVectorDB.Id,
                Name = folder.FolderName,
                CollectionName = folder.Id.ToString(),
                // ★TODO デスクリプションはLangChainのToolのDescriptionに使用するが、適切なものでない場合はToolsが選択されないためとりあえず空文字に設定する。
                // Description = folder.FolderName,
                Description = "",
                Type = SystemCommonVectorDB.Type,
                VectorDBURL = SystemCommonVectorDB.VectorDBURL,
                DocStoreURL = SystemCommonVectorDB.DocStoreURL,
                IsUseMultiVectorRetriever = SystemCommonVectorDB.IsUseMultiVectorRetriever,
                IsEnabled = SystemCommonVectorDB.IsEnabled,
                IsSystem = SystemCommonVectorDB.IsSystem
            };
            return item;
        }

    }
}
