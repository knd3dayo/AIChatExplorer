using AIChatExplorer.Model.Folders.FileSystem;
using LibMain.Data;
using LibMain.Model.Content;
using LibMain.Model.VectorDB;

namespace AIChatExplorer.Model.Folders.Outlook {
    public class OutlookItem : ContentItem {

        // EntryIDの名前
        public const string EntryIDName = "EntryID";

        public static OutlookItem Create(ContentFolderEntity folder, string entryID){
            OutlookItem item = new ();
            item.FolderId = folder.Id;
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
                var folder = await GetFolderAsync();
                var item = await folder.GetMainVectorSearchItem();
                string? vectorDBItemName = item?.VectorDBItemName;
                if (vectorDBItemName == null) {
                    return;
                }
                var contentFolderPath = await folder.GetContentFolderPath();
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
