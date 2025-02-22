using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Common;
using WpfAppCommon.Model;

namespace LibUIPythonAI.ViewModel.Common {
    public class MyStatusBarViewModel : ObservableObject {

        // ステータスバーのテキスト
        public StatusText StatusText { get; } = StatusText.Instance;


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
