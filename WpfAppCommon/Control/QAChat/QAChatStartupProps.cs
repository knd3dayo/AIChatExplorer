using System.Collections.ObjectModel;
using PythonAILib.Model;
using WpfAppCommon.Model;

namespace WpfAppCommon.Control.QAChat {
    public class QAChatStartupProps {
        public QAChatStartupProps(ClipboardFolder clipboardFolder, ClipboardItem clipboardItem, bool isStartFromInternalApp) {
            ClipboardFolder = clipboardFolder;
            ClipboardItem = clipboardItem;
            IsStartFromInternalApp = isStartFromInternalApp;
        }
        public ClipboardFolder ClipboardFolder { get; set; }
        public ClipboardItem ClipboardItem { get; set; }

        // 内部アプリから起動されたか否か
        public bool IsStartFromInternalApp { get; set; }

        public Action<VectorDBItem> OpenVectorDBItemAction { get; set; } = (vectorDBItem) => { };

        public Action<ObservableCollection<VectorDBItem>> SelectVectorDBItemsAction { get; set; } = (vectorDBItems) => { };

        public Action<ObservableCollection<VectorDBItem>> SelectFolderAction { get; set; } = (folders) => { };

        public Func<List<ClipboardItemImage>> GetSelectedClipboardItemImageFunction { get; set; } = () => { return []; };
    }

}
