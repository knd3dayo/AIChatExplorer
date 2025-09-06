using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LibMain.Model.Chat;
using LibMain.PythonIF.Request;

namespace LibUIMain.ViewModel.Chat {
    public class ChatHistoryViewModel : ObservableObject {

        public ChatHistoryViewModel(ChatRequest chatRequest) {
            ChatRequest = chatRequest;
        }

        public ChatRequest ChatRequest { get; }


        public ObservableCollection<ChatMessage> ChatHistory {
            get => [.. ChatRequest.ChatHistory ];
        }

        public void ClearChatHistory() {
            ChatRequest.ChatHistory.Clear();
            OnPropertyChanged(nameof(ChatHistory));
        }
    }
}
