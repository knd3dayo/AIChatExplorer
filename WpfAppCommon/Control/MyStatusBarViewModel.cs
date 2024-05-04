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
                if (window == null) {
                    return new StatusText();
                }
                return StatusText.GetStatusText(window);
            }
        }

        // ステータスバーをクリックした時の処理の実装
        public static Action? OpenStatusMessageWindow { get; set; }

        // ステータスバーをクリックしたときの処理
        public static SimpleDelegateCommand OpenStatusMessageWindowCommand => new((parameter) => {
            StatusMessageWindow userControl = new StatusMessageWindow();
            Window window = new() {
                Title = "Status Message",
                Content = userControl
            };
            StatusMessageWindowViewModel statusMessageWindowViewModel = (StatusMessageWindowViewModel)userControl.DataContext;
            statusMessageWindowViewModel.Initialize();
            window.ShowDialog();
        });

        // ロード時の処理
        private Window? window;
        public SimpleDelegateCommand LoadedCommand => new((parameter) => {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            UserControl userControl = (UserControl)routedEventArgs.Source;
            Window window = Window.GetWindow(userControl);
            this.window = window;
            OnPropertyChanged(nameof(StatusText));
        });

    }
}
