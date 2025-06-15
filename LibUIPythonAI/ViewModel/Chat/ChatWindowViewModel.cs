using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;

namespace LibUIPythonAI.ViewModel.Chat {
    public class ChatWindowViewModel : CommonViewModelBase {

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
