using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;

namespace AIChatExplorer.Model.Folders.Browser {
    public class RecentFilesItem : ContentItemWrapper {

        // コンストラクタ
        public RecentFilesItem() : base() { }

        public RecentFilesItem(ContentFolderEntity folder) : base(folder) { }

        public override RecentFilesItem Copy() {
            return new() { Entity = Entity.Copy() };
        }
        public override void Save() {
            if (ContentModified || DescriptionModified) {
                // ベクトルを更新
                Task.Run(async () => {
                    string? vectorDBItemName = GetFolder().GetMainVectorSearchItem()?.VectorDBItemName;
                    if (vectorDBItemName == null) {
                        return;
                    }
                    VectorEmbeddingItem VectorEmbeddingItem = new(Id.ToString(), GetFolder().Id) {
                        Content = Content,
                        Description = Description,
                        SourceType = VectorSourceType.File,
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
