using System.IO;
using ClipboardApp.Factory;
using PythonAILib.Model.Abstract;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using WpfAppCommon.Model;

namespace ClipboardApp.Model
{

    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class ClipboardAppVectorDBItem : VectorDBItemBase {

        // システム共通のベクトルDBの名前
        public static string SystemCommonVectorDBName = "SystemCommonVectorDB";
        // システム共通のベクトルDB
        public static VectorDBItemBase SystemCommonVectorDB {
            get {
                // DBからベクトルDBを取得
                var item = GetItems(true).FirstOrDefault(item => item.Name == SystemCommonVectorDBName);
                if (item == null) {
                    string docDBPath = Path.Combine(ClipboardAppConfig.AppDataFolder, "clipboard_doc_store.db");
                    string vectorDBPath = Path.Combine(ClipboardAppConfig.AppDataFolder, "clipboard_vector_db.db");
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

        // Save
        public override void Save() {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // UpsertItemメソッドを呼び出して保存
            dbController.UpsertVectorDBItem(this);

        }

        // Delete
        public override void Delete() {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // DeleteItemメソッドを呼び出して削除
            dbController.DeleteVectorDBItem(this);
        }
        // Get
        public static IEnumerable<VectorDBItemBase> GetItems(bool includeSystemVectorDB) {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // GetItemsメソッドを呼び出して取得
            IEnumerable<VectorDBItemBase> items = dbController.GetVectorDBItems();
            if (!includeSystemVectorDB) {
                items = items.Where(item => !item.IsSystem && item.Name != SystemCommonVectorDBName);
            }

            return items;
        }
        public static IEnumerable<VectorDBItemBase> GetEnabledItems(bool includeSystemVectorDB) {
            return GetItems(includeSystemVectorDB).Where(item => item.IsEnabled);
        }

        public static VectorDBItemBase GetFolderVectorDBItem(ClipboardFolder folder) {
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

        // GetItemById
        public static VectorDBItemBase? GetItemById(LiteDB.ObjectId id) {
            return GetItems(true).FirstOrDefault(item => item.Id == id);
        }

        public override void UpdateIndex(ContentInfo contentInfo) {
            UpdateIndex(contentInfo, ClipboardAppConfig.CreateOpenAIProperties());
        }

        public override void DeleteIndex(ContentInfo contentInfo) {
            DeleteIndex(contentInfo, ClipboardAppConfig.CreateOpenAIProperties());
        }

        public override void UpdateIndex(ImageInfo imageInfo) {
            // CollectionNameの設定
            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(ClipboardAppConfig.CreateOpenAIProperties(), imageInfo, this);
        }

        public override void DeleteIndex(ImageInfo imageInfo) {

            PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(ClipboardAppConfig.CreateOpenAIProperties(), imageInfo, this);
        }

    }
}
