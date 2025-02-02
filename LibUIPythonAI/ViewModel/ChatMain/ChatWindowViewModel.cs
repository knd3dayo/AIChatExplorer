using LibUIPythonAI.ViewModel;
using WpfAppCommon.Utils;

namespace LibUIPythonAI.ViewModel.ChatMain {
    public class ChatWindowViewModel : ChatViewModelBase {

        //初期化
        public ChatWindowViewModel(QAChatStartupProps props) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.Common.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
            // ChatControlViewModelを生成
            ChatControlViewModel = new(props);
        }
        // QAChatControlのViewModel
        public ChatControlViewModel ChatControlViewModel { get; set; }

    }
}
