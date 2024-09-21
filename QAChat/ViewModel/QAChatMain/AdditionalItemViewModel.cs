using PythonAILib.Model;
using WpfAppCommon.Utils;
using QAChat.ViewModel.ContentItemPanel;
namespace QAChat.ViewModel.QAChatMain {
    public class AdditionalItemViewModel: ContentItemPanelViewModel {


        public QAChatControlViewModel QAChatControlViewModel { get; set; }
        
        public AdditionalItemViewModel(QAChatControlViewModel qaChatControlViewModel, ContentItemBase clipboardItem) : base(clipboardItem) {
            QAChatControlViewModel = qaChatControlViewModel;
            ContentItem = clipboardItem;
        }

        public override void OpenContentItem() {
            QAChatControlViewModel.QAChatStartupProps?.OpenSelectedItemCommand(ContentItem);
        }
        // 削除
        public override void RemoveContentItem() {
            QAChatControlViewModel.ChatController.AdditionalItems.Remove(ContentItem);
        }

    }
}
