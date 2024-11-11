using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Model.Content {
    public class ContentFolder {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        // プロパティ
        // 親フォルダのID
        public ObjectId ParentId { get; set; } = ObjectId.Empty;

        public string Name { get; set; } = "";

        // 参照用のベクトルDBのリストのプロパティ
        private List<VectorDBItem> _referenceVectorDBItems = [];
        public List<VectorDBItem> ReferenceVectorDBItems {
            get {
                return _referenceVectorDBItems;
            }
            set {
                _referenceVectorDBItems = value;
            }
        }

        // フォルダの絶対パス ファイルシステム用
        public virtual string FolderPath { get; } = "";

        //　フォルダ名
        public virtual string FolderName { get; set; } = "";


        // Description
        public virtual string Description { get; set; } = "";

        // 子フォルダ
        public virtual List<T> GetChildren<T>() where T : ContentFolder {
            // DBからParentIDが自分のIDのものを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            var folders = collection.FindAll().Where(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders.Cast<T>().ToList();
        }

        // フォルダを削除
        public virtual void DeleteFolder<T1, T2>(T1 folder) where T1 : ContentFolder where T2 : ContentItem {
            // folderの子フォルダを再帰的に削除
            foreach (var child in folder.GetChildren<T1>()) {
                if (child != null) {
                    DeleteFolder<T1, T2>(child);
                }
            }
            // folderのアイテムを削除
            var items = PythonAILibManager.Instance.DataFactory.GetItemCollection<T2>().FindAll().Where(x => x.CollectionId == folder.Id);
            foreach (var item in items) {
                item.Delete();
            }
            // folderを削除
            var folderCollection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T1>();
            folderCollection.Delete(folder.Id);

        }

        // フォルダを移動する
        public virtual void MoveTo(ContentFolder toFolder) {
            // 自分自身を移動
            ParentId = toFolder.Id;
            Save();
        }
        // 名前を変更
        public virtual void Rename(string newName) {
            FolderName = newName;
            Save();
        }

        // 保存
        public virtual void Save() {
            throw new NotImplementedException();
        }

        // 削除
        public virtual void Delete() {
            throw new NotImplementedException();
        }


    }
}
