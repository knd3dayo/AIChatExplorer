using LibPythonAI.Utils.Common;
using LibUIMain.Resource;
using LibUIMain.Utils;

namespace LibUIMain.ViewModel.Chat {
    public class ChatWindowViewModel : CommonViewModelBase {

        //初期化
        public ChatWindowViewModel(QAChatStartupPropsBase props) {
            // PythonAILibのLogWrapperのログ出力設定
            LogWrapper.SetActions(new LogWrapperAction());
            // ChatControlViewModelを生成
            ChatControlViewModel = new(props);
        }
        // QAChatControlのViewModel
        public ChatControlViewModel ChatControlViewModel { get; set; }

    }
}
