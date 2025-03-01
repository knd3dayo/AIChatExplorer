using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Folder;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Model.Content {
    public class ContentFolder {


        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        // フォルダの種類
        public FolderTypeEnum FolderType { get; set; } = FolderTypeEnum.Normal;

        // フォルダの種類の文字列
        public string FolderTypeString { get; set; } = FolderTypeEnum.Normal.Name;

        // プロパティf
        // 親フォルダのID
        public ObjectId ParentId { get; set; } = ObjectId.Empty;

        // ルートフォルダか否か
        public bool IsRootFolder { get; set; } = false;

        //　フォルダ名
        public virtual string FolderName { get; set; } = "";

        // Description
        public virtual string Description { get; set; } = "";

        // 拡張プロパティ
        public Dictionary<string, object> ExtendedProperties { get; set; } = new();

        //　OS上のフォルダ名
        public virtual string ContentOutputFolderPrefix { get; set; } = "";

        // -- EntityFramework未対応
        public List<VectorDBProperty> ReferenceVectorSearchProperties { get; set; } = [];

        // AutoProcessを有効にするかどうか
        public bool IsAutoProcessEnabled { get; set; } = false;

        // アプリケーション内でのフォルダのパス
        [BsonIgnore]
        public virtual string ContentFolderPath {
            get {
                // 親フォルダを取得
                var parentFolder = GetParent();
                if (parentFolder == null) {
                    return FolderName;
                }
                return $"{parentFolder.ContentFolderPath}/{FolderName}";
            }
        }
        // OS上のフォルダのパス
        [BsonIgnore]
        public virtual string ContentOutputFolderPath {
            get {
                string osFolderName;
                // 親フォルダを取得
                var parentFolder = GetParent();
                if (parentFolder == null) {
                    osFolderName =  ContentOutputFolderPrefix;
                } else {
                    // FolderNameに:、\、/が含まれている場合は文字を削除
                    string modifiedFolderName = FolderName;
                    if (FolderName.Contains(':') || FolderName.Contains('\\') || FolderName.Contains('/')) {
                        modifiedFolderName = FolderName.Replace(":", "").Replace("\\", "").Replace("/", "");
                    }

                    osFolderName = $"{parentFolder.ContentOutputFolderPath}{System.IO.Path.DirectorySeparatorChar}{modifiedFolderName}";
                }
                return osFolderName;
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

        public virtual IEnumerable<T> GetChildren<T>() where T : ContentFolder {

            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            IEnumerable<T> folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders;
        }

        public virtual IEnumerable<T> GetItems<T>() where T : ContentItem {
            List<T> _items = [];
            var collection = PythonAILibManager.Instance.DataFactory.GetItemCollection<T>();
            var items = collection.Find(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt);
            return items;
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
            /**
            // OS上のフォルダが存在しない場合は作成。上位のフォルダも再帰的に作成
            if (!System.IO.Directory.Exists(ContentOutputFolderPath)) {
                System.IO.Directory.CreateDirectory(ContentOutputFolderPath);
            }
            **/

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
            // OS上のフォルダを削除
            /**
            if (System.IO.Directory.Exists(ContentOutputFolderPath)) {
                System.IO.Directory.Delete(ContentOutputFolderPath, true);
            }
            **/
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
