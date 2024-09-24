using System.IO;
using ClipboardApp.Factory;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using WpfAppCommon.Model;

namespace ClipboardApp.Model
{

    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class ClipboardAppVectorDBItem : VectorDBItem {

        // システム共通のベクトルDB
        public static VectorDBItem SystemCommonVectorDB {
            get {
                // DBからベクトルDBを取得
                var item = GetItems(true).FirstOrDefault(item => item.Name == SystemCommonVectorDBName);
                if (item == null) {
                    string docDBPath = Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "clipboard_doc_store.db");
                    string vectorDBPath = Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "clipboard_vector_db.db");
                    item = new ClipboardAppVectorDBItem() {
                        Id = LiteDB.ObjectId.Empty,
                        Name = SystemCommonVectorDBName,
                        Description = CommonStringResources.Instance.GeneralVectorDBForSearchingPastDocumentsBasedOnUserQuestions,
                        Type = VectorDBTypeEnum.Chroma,
                        VectorDBURL = vectorDBPath,
                        DocStoreURL = $"sqlite:///{docDBPath}",
                        IsUseMultiVectorRetriever = true,
                        IsEnabled = true,
                        IsSystem = true
                    };
                    item.Save();
                }
                // IsSystemフラグ導入前のバージョンへの対応
                item.IsSystem = true;
                return item;

            }
        }


        public ClipboardAppVectorDBItem() {
        }

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
