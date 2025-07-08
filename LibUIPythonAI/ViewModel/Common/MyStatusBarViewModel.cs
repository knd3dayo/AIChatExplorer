using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Common;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Common;

namespace LibUIPythonAI.ViewModel.Common {
    public class MyStatusBarViewModel : ObservableObject {

        // ステータスバーのテキスト
        public StatusText StatusText { get; } = StatusText.Instance;


        // ログファイルを開く
        public static SimpleDelegateCommand<object> OpenStatusMessageWindowCommand => new((parameter) => {
            string logFile = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetAppDataPath(), "log", "log.txt");
            ProcessUtil.OpenFile(logFile);
        });
        // 統計情報のログ画面を表示する。
        public static SimpleDelegateCommand<object> OpenStatisticsWindowCommand => new((parameter) => {
            StatisticsMessageWindow.OpenStatusMessageWindow(CommonStringResources.Instance.Statistics, new StatisticsMessageWindowViewModel());
        });

    }
}
