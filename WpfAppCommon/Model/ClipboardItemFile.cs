using LiteDB;

namespace WpfAppCommon.Model {
    public class ClipboardItemFile(string fileName) {

        public ObjectId Id { get; set; } = ObjectId.Empty;

        // ファイルパス
        public string FilePath { get; set; } = fileName;
        // フォルダ名
        public string FolderName {
            get {
                return System.IO.Path.GetDirectoryName(FilePath) ?? "";
            }
        }
        // ファイル名
        public string FileName {
            get {
                return System.IO.Path.GetFileName(FilePath);
            }
        }
        
        // 削除
        public void Delete() {
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItemFile(this);
        }
        // 保存
        public void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertItemFile(this);
        }
        // 取得
        public static ClipboardItemFile? GetItems(ObjectId objectId) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(objectId);
        }
    }
}
