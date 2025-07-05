using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;

namespace AIChatExplorer.Model.Folders.Browser {
    public class EdgeBrowseHistoryItem : ContentItemWrapper {

        // コンストラクタ
        public EdgeBrowseHistoryItem() : base() { }

        public EdgeBrowseHistoryItem(ContentFolderEntity folder) : base(folder) { }

        public override EdgeBrowseHistoryItem Copy() {
            return new() { Entity = Entity.Copy() };
        }
        public override void Save() {
            if (ContentModified || DescriptionModified) {
                // ベクトルを更新
                Task.Run(async () => {
                    var item = await GetFolder().GetMainVectorSearchItem();
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
                ContentModified = false;
                DescriptionModified = false;

            }

            ContentItemEntity.SaveItems([Entity]);
        }

    }
}
