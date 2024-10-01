using System.Windows;
using PythonAILib.Model.Chat;
using QAChat.Model;

namespace QAChat.ViewModel.QAChatMain {
    public class EditChatItemWindowViewModel : QAChatViewModelBase {

        public ChatHistoryItem ChatItem { get; set; }

        public EditChatItemWindowViewModel(ChatHistoryItem chatItem) {
            ChatItem = chatItem;
            OnPropertyChanged(nameof(ChatItem));
        }

        public TextWrapping TextWrapping {
            get {
                if (QAChatManager.Instance == null) {
                    return TextWrapping.NoWrap;
                }
                return QAChatManager.Instance.ConfigParams.GetTextWrapping();
            }
        }

    }
}
