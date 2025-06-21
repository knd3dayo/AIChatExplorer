using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.Model.Main;

namespace AIChatExplorer.Model.Folders.ClipboardHistory {
    public  class ClipboardHistoryFolder : ApplicationFolder {

        // コンストラクタ
        public ClipboardHistoryFolder() : base() {
            FolderTypeString = FolderManager.CLIPBOARD_HISTORY_ROOT_FOLDER_NAME_EN;
        }
    }
}
