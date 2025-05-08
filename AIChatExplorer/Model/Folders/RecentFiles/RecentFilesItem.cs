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
                Task.Run(() => {
                    string? vectorDBItemName = GetFolder().GetMainVectorSearchProperty()?.VectorDBItemName;
                    if (vectorDBItemName == null) {
                        return;
                    }
                    VectorDBEmbedding vectorDBEmbedding = new(Id.ToString(), GetFolder().Id) {
                        Content = Content,
                        Description = Description,
                        SourceType = PythonAILib.Model.VectorDB.VectorSourceType.File,
                        SourcePath = SourcePath,
                    };
                    VectorDBEmbedding.UpdateEmbeddings(vectorDBItemName, vectorDBEmbedding);
                });
                ContentModified = false;
                DescriptionModified = false;
            }

            ContentItemEntity.SaveItems([Entity]);
        }

    }
}
