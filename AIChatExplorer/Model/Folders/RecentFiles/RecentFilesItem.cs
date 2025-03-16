using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;

namespace AIChatExplorer.Model.Folders.Browser {
    public class RecentFilesItem : ContentItemWrapper {

        // コンストラクタ
        public RecentFilesItem(ContentItemEntity item) : base(item) { }

        public RecentFilesItem(ContentFolderEntity folder) : base(folder) { }

        public override RecentFilesItem Copy() {
            return new(Entity.Copy());
        }
        public override void Save() {
            if (ContentModified || DescriptionModified) {
                // ベクトルを更新
                Task.Run(() => {
                    VectorDBProperty.UpdateEmbeddings([GetFolder().GetMainVectorSearchProperty()]);
                });
            }

            ContentItemEntity.SaveItems([Entity]);
        }

    }
}
