using System.Windows;
using PythonAILib.Model.Chat;
using QAChat.Model;

namespace QAChat.ViewModel.QAChatMain {
    public class EditChatItemWindowViewModel : QAChatViewModelBase {

        public ChatMessage ChatItem { get; set; }

        public EditChatItemWindowViewModel(ChatMessage chatItem) {
            ChatItem = chatItem;
            OnPropertyChanged(nameof(ChatItem));
        }
    }
}
