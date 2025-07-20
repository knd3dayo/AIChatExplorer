using AIChatExplorer.Model.Folders.FileSystem;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;

namespace AIChatExplorer.Model.Folders.Outlook {
    public class OutlookItem : ContentItemWrapper {

        // EntryIDの名前
        public const string EntryIDName = "EntryID";

        // コンストラクタ
        public OutlookItem() : base() { }

        public OutlookItem(ContentFolderEntity folder) : base(folder) { }


        public OutlookItem(ContentFolderEntity folder, string entryID) : base(folder) {
            EntryID = entryID;
        }

        public override OutlookItem Copy() {
            return new() { 
                Entity = Entity.Copy(),
                ChatSettings = ChatSettings
            };
        }

        public override void Save() {
            if (ContentModified || DescriptionModified) {
                // ベクトルを更新
                Task.Run(async () => {
                    var item = GetFolder().GetMainVectorSearchItem();
                    string? vectorDBItemName = item?.VectorDBItemName;
                    if (vectorDBItemName == null) {
                        return;
                    }
                    VectorEmbeddingItem VectorEmbeddingItem = new(Id.ToString(), GetFolder().ContentFolderPath) {
                        Content = Content,
                        Description = Description,
                        SourceType = VectorSourceType.Mail,
                        SourcePath = SourcePath,
                    };
                    VectorEmbeddingItem.UpdateEmbeddings(vectorDBItemName, VectorEmbeddingItem);
                });
                ContentModified = false;
                DescriptionModified = false;
            }

            ContentItemEntity.SaveItems([Entity]);
        }

        public string EntryID {
            get {
                if (Entity.ExtendedProperties.TryGetValue(EntryIDName, out var path) && path != null) {
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
