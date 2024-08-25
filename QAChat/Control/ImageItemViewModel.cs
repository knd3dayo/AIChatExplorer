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


        // OpenSelectedImageFileCommand  選択した画像ファイルを開くコマンド
        public SimpleDelegateCommand<object> OpenSelectedImageItemCommand => new((parameter) => {
            ProcessUtil.OpenBitmapImage(ClipboardItemImage.BitmapImage);
        });

        // RemoveSelectedImageFileCommand  選択した画像ファイルをScreenShotImageのリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveSelectedImageItemCommand => new((parameter) => {
            QAChatControlViewModel.ImageItems.Remove(this);
        });

    }

}
