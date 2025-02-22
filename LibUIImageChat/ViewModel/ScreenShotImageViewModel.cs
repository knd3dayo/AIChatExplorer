using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Utils;
using PythonAILib.Model.Image;

namespace LibUIImageChat.ViewModel {
    public class ScreenShotImageViewModel : ObservableObject {

        public ImageChatMainWindowViewModel MainWindowViewModel { get; set; }
        public ScreenShotImage ScreenShotImage { get; set; }

        public ScreenShotImageViewModel(ImageChatMainWindowViewModel mainWindowViewModel, ScreenShotImage screenShotImage) {
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
