using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;

namespace AIChatExplorer.Model.Folders.Browser {
    public class RecentFilesItem : ContentItemWrapper {

        // コンストラクタ
        public RecentFilesItem() : base() { }

        public RecentFilesItem(ContentFolderEntity folder) : base(folder) { }

        public override RecentFilesItem Copy() {
            return new() { 
                Entity = Entity.Copy(),
                ChatSettings = ChatSettings
            };
        }

        public override async Task UpdateEmbedding() {
            // ベクトルを更新
            await Task.Run(async () => {
                var item = GetFolder().GetMainVectorSearchItem();
                string? vectorDBItemName = item?.VectorDBItemName;
                if (vectorDBItemName == null) {
                    return;
                }
                VectorEmbeddingItem VectorEmbeddingItem = new(Id.ToString(), GetFolder().ContentFolderPath) {
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
