using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Chat;

namespace LibUIAutoGenChat.ViewModel.Chat {
    public class AutoGenChatWindowViewModel : CommonViewModelBase {

        //初期化
        public AutoGenChatWindowViewModel(QAChatStartupPropsBase props) {
            // PythonAILibのLogWrapperのログ出力設定
            LogWrapper.SetActions(new LogWrapperAction());
            // ChatControlViewModelを生成
            ChatControlViewModel = new(props);
        }
        // QAChatControlのViewModel
        public AutoGenChatControlViewModel ChatControlViewModel { get; set; }

    }
}
