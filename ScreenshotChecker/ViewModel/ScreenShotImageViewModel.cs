using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ImageChat.ViewModel {
    public class ScreenShotImageViewModel :ObservableObject{

        public MainWindowViewModel MainWindowViewModel { get; set; }
        public ScreenShotImage ScreenShotImage { get; set; }

        public ScreenShotImageViewModel(MainWindowViewModel mainWindowViewModel, ScreenShotImage screenShotImage) {
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
