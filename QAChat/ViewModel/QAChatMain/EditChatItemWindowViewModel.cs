using PythonAILib.Model.Chat;
using WpfAppCommon.Model;

namespace QAChat.ViewModel.QAChatMain {
    public class EditChatItemWindowViewModel : MyWindowViewModel {

        public ChatIHistorytem ChatItem { get; set; }

        public EditChatItemWindowViewModel(ChatIHistorytem chatItem) {
            ChatItem = chatItem;
            OnPropertyChanged(nameof(ChatItem));
        }
    }
}
