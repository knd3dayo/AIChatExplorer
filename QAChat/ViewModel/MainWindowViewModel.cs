using System.Collections.ObjectModel;
using PythonAILib.Model;
using QAChat.Control;
using QAChat.View.PromptTemplateWindow;
using QAChat.ViewModel.QAChatMain;
using WpfAppCommon.Utils;
using QAChat.Model;

namespace QAChat.ViewModel {
    public class MainWindowViewModel : QAChatViewModelBase {

        //初期化
        public MainWindowViewModel(QAChatStartupProps props) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
            // QAChatControlViewModelを生成
            QAChatControlViewModel = new(props);
        }
        // QAChatControlのViewModel
        public QAChatControlViewModel QAChatControlViewModel { get; set; }

    }
}
