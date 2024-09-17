using System.Collections.ObjectModel;
using PythonAILib.Model;
using QAChat.Control;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel {
    public class MainWindowViewModel : MyWindowViewModel {

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
