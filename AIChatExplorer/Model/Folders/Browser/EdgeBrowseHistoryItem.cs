using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;
using PythonAILib.Common;

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
                    string? vectorDBItemName = GetFolder().GetMainVectorSearchProperty()?.VectorDBItemName;
                    if (vectorDBItemName == null) {
                        return;
                    }
                    VectorEmbedding vectorEmbedding = new(Id.ToString(), GetFolder().Id) {
                        Content = Content,
                        FolderId = GetFolder().Id,
                        Description = Description,
                        SourceType = PythonAILib.Model.VectorDB.VectorSourceType.Web,
                        SourcePath = SourcePath,
                    };
                    await VectorEmbedding.UpdateEmbeddings(vectorDBItemName, vectorEmbedding);
                });
                ContentModified = false;
                DescriptionModified = false;

            }

            ContentItemEntity.SaveItems([Entity]);
        }

    }
}
