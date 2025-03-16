using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;
using PythonAILib.Common;

namespace AIChatExplorer.Model.Folders.Browser {
    public class EdgeBrowseHistoryItem : ContentItemWrapper {

        // コンストラクタ
        public EdgeBrowseHistoryItem(ContentItemEntity item) : base(item) { }

        public EdgeBrowseHistoryItem(ContentFolderEntity folder) : base(folder) { }

        public override EdgeBrowseHistoryItem Copy() {
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
