using AIChatExplorer.Model.Folders.FileSystem;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;

namespace AIChatExplorer.Model.Folders.Outlook {
    public class OutlookItem : ContentItemWrapper {

        // EntryIDの名前
        public const string EntryIDName = "EntryID";

        public static async Task<OutlookItem> Create(ContentFolderEntity folder, string entryID){
            OutlookItem item = new ();
            item.Folder = await ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(folder.Id);
            item.EntryID = entryID;

            return item;
        }

        public override OutlookItem Copy() {
            return new() { 
                Entity = Entity.Copy(),
                ChatSettings = ChatSettings
            };
        }

        public override async Task UpdateEmbeddingAsync() {
            // ベクトルを更新
            await Task.Run(async () => {
                var item = await Folder.GetMainVectorSearchItem();
                string? vectorDBItemName = item?.VectorDBItemName;
                if (vectorDBItemName == null) {
                    return;
                }
                var contentFolderPath = await Folder.GetContentFolderPath();
                VectorEmbeddingItem VectorEmbeddingItem = new(Id.ToString(), contentFolderPath) {
                    Content = Content,
                    Description = Description,
                    SourceType = VectorSourceType.Mail,
                    SourcePath = SourcePath,
                };
                VectorEmbeddingItem.UpdateEmbeddings(vectorDBItemName, VectorEmbeddingItem);
            });
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
