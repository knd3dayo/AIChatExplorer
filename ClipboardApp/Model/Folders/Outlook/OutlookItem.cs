using ClipboardApp.Model.Folders.FileSystem;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;

namespace ClipboardApp.Model.Folders.Outlook {
    public class OutlookItem : ContentItemWrapper {

        // EntryIDの名前
        public const string EntryIDName = "EntryID";

        // コンストラクタ
        public OutlookItem(ContentItemEntity item) : base(item) { }

        public OutlookItem(ContentFolderEntity folder) : base(folder) { }


        public OutlookItem(ContentFolderEntity folder, string entryID) : base(folder) {
            EntryID = entryID;
        }

        public override FileSystemItem Copy() {
            return new(Entity.Copy());
        }

        public override void Save() {
            if (ContentModified || DescriptionModified) {
                // ベクトルを更新
                Task.Run(() => {
                    VectorDBProperty.UpdateEmbeddings([GetFolder().GetMainVectorSearchProperty()]);
                });
            }

            ContentItemEntity.SaveItems([Entity]);
        }

        public string EntryID {
            get {
                if (Entity.ExtendedProperties.TryGetValue(EntryIDName, out var path)) {
                    return (string)path;
                } else {
                    return "";
                }
            }
            set {
                Entity.ExtendedProperties[EntryIDName] = value;
            }
        }

    }
}
