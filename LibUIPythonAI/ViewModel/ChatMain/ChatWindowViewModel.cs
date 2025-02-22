using LibPythonAI.Utils.Common;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel;

namespace LibUIPythonAI.ViewModel.ChatMain {
    public class ChatWindowViewModel : ChatViewModelBase {

        //初期化
        public ChatWindowViewModel(QAChatStartupProps props) {
            // PythonAILibのLogWrapperのログ出力設定
            LogWrapper.SetActions(new LogWrapperAction());
            // ChatControlViewModelを生成
            ChatControlViewModel = new(props);
        }
        // QAChatControlのViewModel
        public ChatControlViewModel ChatControlViewModel { get; set; }

    }
}
