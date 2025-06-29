using System.Collections.ObjectModel;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Chat;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;

namespace LibUINormalChat.ViewModel {
    public class NormalChatWindowViewModel : CommonViewModelBase {

        //初期化
        public NormalChatWindowViewModel(QAChatStartupProps qAChatStartupProps) {
            // PythonAILibのLogWrapperのログ出力設定
            LogWrapper.SetActions(new LogWrapperAction());

            RelatedItemsPanelViewModel relatedItemsPanelViewModel = new(CommonViewModelProperties.UpdateIndeterminate);

            // ChatControlViewModelを生成
            NormalChatControlViewModel = new(relatedItemsPanelViewModel, qAChatStartupProps);

        }

        public NormalChatControlViewModel NormalChatControlViewModel { get; set; }

    }
}
