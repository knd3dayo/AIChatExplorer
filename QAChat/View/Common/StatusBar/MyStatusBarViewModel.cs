using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Resource;
using QAChat.View.Common.StatusMessage;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.View.Common.StatusBar {
    public class MyStatusBarViewModel : ObservableObject {

        // ステータスバーのテキスト
        public StatusText StatusText { get; set; } = Tools.StatusText;

        public CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;


        // ステータスメッセージのログ画面を表示する。
        public static SimpleDelegateCommand<object> OpenStatusMessageWindowCommand => new((parameter) => {
            StatusMessageWindowViewModel statusMessageWindowViewModel = new();
            StatusMessageWindow userControl = new() {
                DataContext = statusMessageWindowViewModel
            };
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
            // UserControl userControl = (UserControl)routedEventArgs.Source;
            // Window window = Window.GetWindow(userControl);
            StringResources = CommonStringResources.Instance;
            string statistics = CommonStringResources.Instance.Statistics;
            OnPropertyChanged(nameof(StringResources));

        });

    }
}
