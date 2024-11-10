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
            VectorDBItem systemVectorItem = VectorDBItem.SystemCommonVectorDB;
            // NameとDescriptionとCollectionNameを設定する
            systemVectorItem.Name = folder.Name;
            systemVectorItem.Description = folder.Description;
            systemVectorItem.CollectionName = folder.Id.ToString();

            return systemVectorItem;
        }

    }
}
