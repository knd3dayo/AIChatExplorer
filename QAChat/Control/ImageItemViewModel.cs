using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model;
using QAChat.ViewModel;
using WpfAppCommon.Utils;

namespace QAChat.Control {
    public class ImageItemViewModel : ObservableObject {
        public ImageItemBase ClipboardItemImage { get; set; }
        public QAChatControlViewModel QAChatControlViewModel { get; set; }

        public ImageItemViewModel(QAChatControlViewModel qAChatControlViewModel, ImageItemBase clipboardItemImage) {
            QAChatControlViewModel = qAChatControlViewModel;
            ClipboardItemImage = clipboardItemImage;
        }

    }

}
