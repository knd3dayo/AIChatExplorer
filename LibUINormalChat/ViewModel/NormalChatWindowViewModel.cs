using LibMain.Utils.Common;
using LibUIMain.Resource;
using LibUIMain.Utils;
using LibUIMain.ViewModel.Chat;

namespace LibUINormalChat.ViewModel {
    public class NormalChatWindowViewModel : CommonViewModelBase {

        //初期化
        public NormalChatWindowViewModel(QAChatStartupPropsBase qAChatStartupProps) {
            // PythonAILibのLogWrapperのログ出力設定
            LogWrapper.SetActions(new LogWrapperAction());


            // ChatControlViewModelを生成
            NormalChatControlViewModel = new(qAChatStartupProps);

        }

        public NormalChatControlViewModel NormalChatControlViewModel { get; set; }

    }
}
