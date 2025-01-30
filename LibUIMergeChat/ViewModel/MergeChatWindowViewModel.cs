using QAChat.Model;
using QAChat.ViewModel;
using WpfAppCommon.Utils;

namespace MergeChat.ViewModel {
    public class MergeChatWindowViewModel : QAChatViewModelBase {

        //初期化
        public MergeChatWindowViewModel(QAChatStartupProps props) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.Common.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
            // QAChatControlViewModelを生成
            MergeChatControlViewModel = new(props);
        }
        // QAChatControlのViewModel
        public MergeChatControlViewModel MergeChatControlViewModel { get; set; }

    }
}
