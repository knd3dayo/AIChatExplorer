using LiteDB;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Model.Content {
    public class ContentFolder {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        // プロパティ
        // 親フォルダのID
        public ObjectId ParentId { get; set; } = ObjectId.Empty;

        public string Name { get; set; } = "";

        // 参照用のベクトルDBのリストのプロパティ
        private List<VectorDBItem> _referenceVectorDBItems = [];
        public List<VectorDBItem> ReferenceVectorDBItems {
            get {
                return _referenceVectorDBItems;
            }
            set {
                _referenceVectorDBItems = value;
            }
        }

        // フォルダの絶対パス ファイルシステム用
        public virtual string FolderPath { get; } = "";

        //　フォルダ名
        public virtual string FolderName { get; set; } = "";


        // Description
        public virtual string Description { get; set; } = "";


        // フォルダを移動する
        public virtual void MoveTo(ContentFolder toFolder) {
            // 自分自身を移動
            ParentId = toFolder.Id;
            Save();
        }
        // 名前を変更
        public virtual void Rename(string newName) {
            FolderName = newName;
            Save();
        }

        // 保存
        public virtual void Save() {
            throw new NotImplementedException();
        }


    }
}
