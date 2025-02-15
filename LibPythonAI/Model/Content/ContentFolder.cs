using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Folder;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Model.Content {
    public class ContentFolder {


        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        // フォルダの種類
        public FolderTypeEnum FolderType { get; set; } = FolderTypeEnum.Normal;

        // プロパティf
        // 親フォルダのID
        public ObjectId ParentId { get; set; } = ObjectId.Empty;

        // ルートフォルダか否か
        public bool IsRootFolder { get; set; } = false;

        public List<VectorDBProperty> ReferenceVectorSearchProperties { get; set; } = [];

        // AutoProcessを有効にするかどうか
        public bool IsAutoProcessEnabled { get; set; } = false;


        //　フォルダ名
        public virtual string FolderName { get; set; } = "";
        // Description
        public virtual string Description { get; set; } = "";

        // 拡張プロパティ
        public Dictionary<string, object> ExtendedProperties { get; set; } = new();


        // フォルダの絶対パス ファイルシステム用
        public virtual string FolderPath {
            get {
                // 親フォルダを取得
                var parentFolder = GetParent();
                if (parentFolder == null) {
                    return FolderName;
                }
                return $"{parentFolder.FolderPath}/{FolderName}";
            }
        }

        public virtual ContentFolder CreateChild(string folderName) {
            ContentFolder child = new() {
                FolderName = folderName,
                ParentId = Id,
                FolderType = FolderType,
            };
            return child;
        }

        // 親フォルダ
        public virtual void Save() {
            Save<ContentFolder, ContentItem>();
        }
        // 削除
        public virtual void Delete() {

            Delete<ContentFolder, ContentItem>();
        }

        // 親フォルダ
        public virtual ContentFolder? GetParent() {
            return GetParent<ContentFolder>();
        }

        public virtual List<T> GetChildren<T>() where T : ContentFolder {

            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            IEnumerable<T> folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders.Cast<T>().ToList();
        }

        public virtual List<T> GetItems<T>() where T : ContentItem {
            List<T> _items = [];
            var collection = PythonAILibManager.Instance.DataFactory.GetItemCollection<T>();
            var items = collection.Find(x => x.CollectionId == Id);
            foreach (var item in items) {
                _items.Add(item);
            }
            return _items.Cast<T>().ToList();
        }

        protected T? GetParent<T>() where T : ContentFolder {
            if (ParentId == ObjectId.Empty) {
                return null;
            }
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            return collection.FindById(ParentId);
        }


        // Idを指定してフォルダを取得
        public static T? GetFolderById<T>(ObjectId id) where T : ContentFolder {
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            return collection.FindById(id);
        }


        // 保存
        protected void Save<T1, T2>() where T1 : ContentFolder where T2 : ContentItem {

            IDataFactory dataFactory = PythonAILibManager.Instance.DataFactory;
            dataFactory.GetFolderCollection<T1>().Upsert((T1)this);
            // ItemsのIsReferenceVectorDBItemsSyncedをFalseに設定
            foreach (var item in GetItems<T2>()) {
                item.IsReferenceVectorDBItemsSynced = false;
                item.Save(false);
            }
        }
        protected void Delete<T1, T2>() where T1 : ContentFolder where T2 : ContentItem {
            DeleteFolder<T1, T2>((T1)this);
        }

        // フォルダを削除
        protected void DeleteFolder<T1, T2>(T1 folder) where T1 : ContentFolder where T2 : ContentItem {
            // folderの子フォルダを再帰的に削除
            foreach (var child in folder.GetChildren<T1>()) {
                if (child != null) {
                    DeleteFolder<T1, T2>(child);
                }
            }
            // folderのアイテムを削除
            var items = PythonAILibManager.Instance.DataFactory.GetItemCollection<T2>().Find(x => x.CollectionId == folder.Id);
            foreach (var item in items) {
                item.Delete();
            }

            // folderを削除
            var folderCollection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T1>();
            folderCollection.Delete(folder.Id);
        }


    }
}
