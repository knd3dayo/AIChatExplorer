using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using QAChat.Control.StatusMessage;

namespace QAChat.Control.StatusBar {
    public class MyStatusBarViewModel : ObservableObject {

        // 文字列リソース
        public CommonStringResources StringResources { get; } = CommonStringResources.Instance;
        // ステータスバーのテキスト
        public StatusText StatusText { get; set; } = Tools.StatusText;

        // ステータスメッセージのログ画面を表示する。
        public static SimpleDelegateCommand<object> OpenStatusMessageWindowCommand => new((parameter) => {
            StatusMessageWindow userControl = new StatusMessageWindow();
            StatusMessageWindowViewModel statusMessageWindowViewModel = new StatusMessageWindowViewModel();
            userControl.DataContext = statusMessageWindowViewModel;
            Window window = new() {
                Title = CommonStringResources.Instance.Log,
                Content = userControl,
                Width = 800,
                Height = 450
            };

            window.ShowDialog();
        });
        // 統計情報のログ画面を表示する。
        public static SimpleDelegateCommand<object> OpenStatisticsWindowCommand => new((parameter) => {
            StatusMessageWindow userControl = new StatusMessageWindow();
            StatisticsMessageWindowViewModel statisticsWindowViewModel = new StatisticsMessageWindowViewModel();
            userControl.DataContext = statisticsWindowViewModel;
            Window window = new() {
                Title = CommonStringResources.Instance.Statistics,
                Content = userControl,
                Width = 800,
                Height = 450
            };

            window.ShowDialog();
        });

        // ロード時の処理
        public SimpleDelegateCommand<RoutedEventArgs> LoadedCommand => new((routedEventArgs) => {
            UserControl userControl = (UserControl)routedEventArgs.Source;
            Window window = Window.GetWindow(userControl);
            OnPropertyChanged(nameof(StatusText));
        });

    }
}
