using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace WpfAppCommon.Model {
    public class ClipboardItemFile(string fileName) {

        public ObjectId Id { get; set; } = ObjectId.Empty;

        // ファイル名
        public string FileName { get; set; } = fileName;

        // ファイルからテキスト抽出した結果
        public string ExtractedText { get; set; } = String.Empty;

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
