using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;

namespace AIChatExplorer.Model.Folders.Browser {
    public class RecentFilesItem : ContentItemWrapper {

        
        public override RecentFilesItem Copy() {
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
                    SourceType = VectorSourceType.File,
                    SourcePath = SourcePath,
                };
                VectorEmbeddingItem.UpdateEmbeddings(vectorDBItemName, VectorEmbeddingItem);
            });
        }
    }
}
