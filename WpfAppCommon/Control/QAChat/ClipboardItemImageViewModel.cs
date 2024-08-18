using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Control.QAChat {
    public class ClipboardItemImageViewModel : ObservableObject {
        public ClipboardItemImage ClipboardItemImage { get; set; }
        public QAChatControlViewModel QAChatControlViewModel { get; set; }

        public ClipboardItemImageViewModel(QAChatControlViewModel qAChatControlViewModel, ClipboardItemImage clipboardItemImage) {
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
    public class ScreenShotImageViewModel : ObservableObject {


        public ScreenShotImage ScreenShotImage { get; set; }
        public QAChatControlViewModel QAChatControlViewModel { get; set; }

        // コンストラクタ
        public ScreenShotImageViewModel(QAChatControlViewModel qAChatControlViewModel, ScreenShotImage screenShotImage) {
            QAChatControlViewModel = qAChatControlViewModel;
            ScreenShotImage = screenShotImage;
        }

        // OpenSelectedImageFileCommand  選択した画像ファイルを開くコマンド
        public SimpleDelegateCommand<object> OpenSelectedImageFileCommand => new((parameter) => {
            if (File.Exists(this.ScreenShotImage.ImagePath)) {
                ProcessUtil.OpenFile(this.ScreenShotImage.ImagePath);
            } else {
                LogWrapper.Error(CommonStringResources.Instance.FileDoesNotExist);
            }
        });

        // RemoveSelectedImageFileCommand  選択した画像ファイルをScreenShotImageのリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveSelectedImageFileCommand => new((parameter) => {
            QAChatControlViewModel.ImageFiles.Remove(this);
        });
    }

}
