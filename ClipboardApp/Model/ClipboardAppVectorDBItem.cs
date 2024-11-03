using ClipboardApp.Factory;
using ClipboardApp.Model.Folder;
using PythonAILib.Model.VectorDB;

namespace ClipboardApp.Model {

    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class ClipboardAppVectorDBItem : VectorDBItem {

        public ClipboardAppVectorDBItem() { }


        public static VectorDBItem GetFolderVectorDBItem(ClipboardFolder folder) {
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            VectorDBItem systemVectorItem = dbController.GetMainVectorDBItem();
            // systemVectorItemからコピーを作成

            ClipboardAppVectorDBItem item = new() {
                Id = systemVectorItem.Id,
                Name = folder.FolderName,
                CollectionName = folder.Id.ToString(),
                // ★TODO デスクリプションはLangChainのToolのDescriptionに使用するが、適切なものでない場合はToolsが選択されないためとりあえず空文字に設定する。
                // Description = folder.FolderName,
                Description = "",
                Type = systemVectorItem.Type,
                VectorDBURL = systemVectorItem.VectorDBURL,
                DocStoreURL = systemVectorItem.DocStoreURL,
                IsUseMultiVectorRetriever = systemVectorItem.IsUseMultiVectorRetriever,
                IsEnabled = systemVectorItem.IsEnabled,
                IsSystem = systemVectorItem.IsSystem
            };
            return item;
        }

    }
}
