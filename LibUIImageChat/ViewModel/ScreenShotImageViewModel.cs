using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.Image;
using LibUIMain.Utils;

namespace LibUIImageChat.ViewModel {
    public class ScreenShotImageViewModel : ObservableObject {

        public ImageChatWindowViewModel MainWindowViewModel { get; set; }
        public ScreenShotImage ScreenShotImage { get; set; }

        public ScreenShotImageViewModel(ImageChatWindowViewModel mainWindowViewModel, ScreenShotImage screenShotImage) {
            MainWindowViewModel = mainWindowViewModel;
            ScreenShotImage = screenShotImage;
        }

        public SimpleDelegateCommand<ScreenShotImageViewModel> OpenSelectedImageFileCommand => new((image) => {
            MainWindowViewModel.OpenSelectedImageFileCommand.Execute(image);
        });

        public SimpleDelegateCommand<ScreenShotImageViewModel> RemoveSelectedImageFileCommand => new((image) => {
            MainWindowViewModel.RemoveSelectedImageFileCommand.Execute(image);
        });
    }
}
