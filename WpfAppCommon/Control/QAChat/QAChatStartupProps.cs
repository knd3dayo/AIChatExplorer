using System.Collections.ObjectModel;
using PythonAILib.Model;
using WpfAppCommon.Model;

namespace WpfAppCommon.Control.QAChat {
    public class QAChatStartupProps {
        public QAChatStartupProps(ClipboardFolder clipboardFolder, ClipboardItem clipboardItem) {
            ClipboardFolder = clipboardFolder;
            ClipboardItem = clipboardItem;
        }
        public ClipboardFolder ClipboardFolder { get; set; }
        public ClipboardItem ClipboardItem { get; set; }


        public Action<VectorDBItem> OpenVectorDBItemAction { get; set; } = (vectorDBItem) => { };

        public Action<ObservableCollection<VectorDBItem>> SelectVectorDBItemsAction { get; set; } = (vectorDBItems) => { };

        public Action<ObservableCollection<VectorDBItem>> SelectFolderAction { get; set; } = (folders) => { };

        public Func<List<ClipboardItemImage>> GetSelectedClipboardItemImageFunction { get; set; } = () => { return []; };
    }

}
