using System.Collections.ObjectModel;
using PythonAILib.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Model.ClipboardApp;

namespace QAChat.Control {
    public class QAChatStartupProps {
        public QAChatStartupProps(ClipboardFolder clipboardFolder, ClipboardItem clipboardItem) {
            ClipboardFolder = clipboardFolder;
            ClipboardItem = clipboardItem;
        }
        public ClipboardFolder ClipboardFolder { get; set; }
        public ClipboardItem ClipboardItem { get; set; }


        public Action<VectorDBItemBase> OpenVectorDBItemAction { get; set; } = (vectorDBItem) => { };

        public Action<ObservableCollection<VectorDBItemBase>> SelectVectorDBItemsAction { get; set; } = (vectorDBItems) => { };

        public Action<ObservableCollection<VectorDBItemBase>> SelectFolderAction { get; set; } = (folders) => { };

        public List<VectorDBItemBase> ExternalVectorDBItems {
            get {
                return [.. ClipboardAppVectorDBItem.GetEnabledItems(false)];
            }
        }
        public Action<List<ClipboardItem>> PasteFromClipboardCommandAction{ get; set;} = (items) => { };
    }

}
