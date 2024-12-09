using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Resource;
using QAChat.View.Common;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.Common {
    public class MyStatusBarViewModel : ObservableObject {

        // ステータスバーのテキスト
        public StatusText StatusText { get; set; } = Tools.StatusText;


        // ステータスメッセージのログ画面を表示する。
        public static SimpleDelegateCommand<object> OpenStatusMessageWindowCommand => new((parameter) => {
            StatusMessageWindowViewModel statusMessageWindowViewModel = new();
            string title = CommonStringResources.Instance.Log;
            StatusMessageWindow.OpenStatusMessageWindow(title, statusMessageWindowViewModel);

        });
        // 統計情報のログ画面を表示する。
        public static SimpleDelegateCommand<object> OpenStatisticsWindowCommand => new((parameter) => {
            StatusMessageWindow.OpenStatusMessageWindow(CommonStringResources.Instance.Statistics, new StatisticsMessageWindowViewModel());
        });

    }
}
