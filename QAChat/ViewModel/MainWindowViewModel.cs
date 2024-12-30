using QAChat.View.Common;
using QAChat.Model;
using QAChat.ViewModel.QAChatMain;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel {
    public class MainWindowViewModel : QAChatViewModelBase {

        //初期化
        public MainWindowViewModel(QAChatStartupProps props) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.Common.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
            // QAChatControlViewModelを生成
            QAChatControlViewModel = new(props);
        }
        // QAChatControlのViewModel
        public QAChatControlViewModel QAChatControlViewModel { get; set; }

    }
}
