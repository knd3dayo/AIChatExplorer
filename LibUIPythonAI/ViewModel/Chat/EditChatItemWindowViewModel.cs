using PythonAILib.Model.Chat;

namespace LibUIPythonAI.ViewModel.Chat {
    public class EditChatItemWindowViewModel : ChatViewModelBase {

        public ChatMessage ChatItem { get; set; }

        public EditChatItemWindowViewModel(ChatMessage chatItem) {
            ChatItem = chatItem;
            OnPropertyChanged(nameof(ChatItem));
        }
    }
}
