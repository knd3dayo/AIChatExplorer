using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using WpfCommonApp.Control.StatusMessage;

namespace WpfAppCommon.Control {
    public class MyStatusBarViewModel : ObservableObject {

        // ステータスバーのテキスト
        public StatusText StatusText {
            get {
                return Tools.StatusText;
            }
        }

        // ステータスバーをクリックしたときの処理
        public static SimpleDelegateCommand<object> OpenStatusMessageWindowCommand => new((parameter) => {
            StatusMessageWindow userControl = new StatusMessageWindow();
            StatusMessageWindowViewModel statusMessageWindowViewModel = new StatusMessageWindowViewModel();
            userControl.DataContext = statusMessageWindowViewModel;
            Window window = new() {
                Title = CommonStringResources.Instance.Log,
                Content = userControl
            };

            window.ShowDialog();
        });
        
        // ロード時の処理
        private Window? window;
        public SimpleDelegateCommand<RoutedEventArgs> LoadedCommand => new((routedEventArgs) => {
            UserControl userControl = (UserControl)routedEventArgs.Source;
            Window window = Window.GetWindow(userControl);
            this.window = window;
            OnPropertyChanged(nameof(StatusText));
        });

    }
}
 