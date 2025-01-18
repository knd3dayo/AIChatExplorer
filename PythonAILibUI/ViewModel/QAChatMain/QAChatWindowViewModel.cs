using QAChat.Model;
using QAChat.ViewModel;
using QAChat.ViewModel.QAChatMain;
using WpfAppCommon.Utils;

namespace PythonAILibUI.ViewModel.QAChatMain {
    public class QAChatWindowViewModel : QAChatViewModelBase {

        //初期化
        public QAChatWindowViewModel(QAChatStartupProps props) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.Common.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
            // QAChatControlViewModelを生成
            QAChatControlViewModel = new(props);
        }
        // QAChatControlのViewModel
        public QAChatControlViewModel QAChatControlViewModel { get; set; }

    }
}
