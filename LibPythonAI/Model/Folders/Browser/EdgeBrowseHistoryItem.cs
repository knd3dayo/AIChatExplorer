using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;

namespace AIChatExplorer.Model.Folders.Browser {
    public class EdgeBrowseHistoryItem : ContentItemWrapper {

        // コンストラクタ
        public EdgeBrowseHistoryItem() : base() { }

        public EdgeBrowseHistoryItem(ContentFolderEntity folder) : base(folder) { }

        public override EdgeBrowseHistoryItem Copy() {
            return new() { 
                Entity = Entity.Copy(),
                ChatSettings = ChatSettings
            };
        }

        public override async Task UpdateEmbedding() {
            // ベクトルを更新
            await Task.Run(() => {
                var item = GetFolder().GetMainVectorSearchItem();
                string? vectorDBItemName = item?.VectorDBItemName;
                if (vectorDBItemName == null) {
                    return;
                }
                VectorEmbeddingItem VectorEmbeddingItem = new(Id.ToString(), GetFolder().ContentFolderPath) {
                    Content = Content,
                    Description = Description,
                    SourceType = VectorSourceType.Web,
                    SourcePath = SourcePath,
                };
                VectorEmbeddingItem.UpdateEmbeddings(vectorDBItemName, VectorEmbeddingItem);
            });

        }

    }
}
