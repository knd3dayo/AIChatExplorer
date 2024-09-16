using System.Windows.Controls;
using PythonAILib.Model.Chat;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel {
    public class EditChatItemWindowViewModel : MyWindowViewModel {

        public ChatIHistorytem ChatItem { get; set; }

        public EditChatItemWindowViewModel(ChatIHistorytem chatItem) {
            ChatItem = chatItem;
            OnPropertyChanged(nameof(ChatItem));
        }
    }
}
