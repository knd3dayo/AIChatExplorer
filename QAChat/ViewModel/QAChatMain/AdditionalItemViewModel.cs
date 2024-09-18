using PythonAILib.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.QAChatMain {
    public class AdditionalItemViewModel {


        public QAChatControlViewModel QAChatControlViewModel { get; set; }
        public ContentItemBase ClipboardItem { get; set; }

        public AdditionalItemViewModel(QAChatControlViewModel qaChatControlViewModel, ContentItemBase clipboardItem) {
            QAChatControlViewModel = qaChatControlViewModel;
            ClipboardItem = clipboardItem;
        }

        // RemoveSelectedItemCommand
        public SimpleDelegateCommand<object> RemoveSelectedItemCommand => new((parameter) => {
            QAChatControlViewModel.ChatController.AdditionalItems.Remove(ClipboardItem);
        });

        // OpenSelectedItemCommand
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            QAChatControlViewModel.QAChatStartupProps?.OpenSelectedItemCommand(ClipboardItem);
        });
    }
}
