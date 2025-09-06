using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Common;
using LibPythonAI.Utils.Common;
using LibUIMain.Utils;

namespace LibUIMain.ViewModel.Common {
    public class MyStatusBarViewModel : ObservableObject {

        // ステータスバーのテキスト
        public StatusText StatusText { get; } = StatusText.Instance;


        // ログファイルを開く
        public static SimpleDelegateCommand<object> OpenStatusMessageWindowCommand => new((parameter) => {
            string logFile = Path.Combine(PythonAILibManager.Instance.ConfigParams.GetAppDataPath(), "log", "log.txt");
            ProcessUtil.OpenFile(logFile);
        });

    }
}
