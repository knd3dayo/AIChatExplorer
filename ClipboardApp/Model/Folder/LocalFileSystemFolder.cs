using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folder {
    public class LocalFileSystemFolder : ClipboardFolder {

        // Save
        public override void Save() {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }
        // Delete
        public override void Delete() {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }
        // DeleteChild
        public override void DeleteChild(ClipboardFolder child) {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }
        // CreateChild
        public override ClipboardFolder CreateChild(string folderName) {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }
        // AddItem
        public override ClipboardItem AddItem(ClipboardItem item) {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }
        // DeleteItem
        public override void DeleteItem(ClipboardItem item) {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }

        // ProcessClipboardItem
        public override void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ClipboardItem> _afterClipboardChanged) {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }

    }
}
