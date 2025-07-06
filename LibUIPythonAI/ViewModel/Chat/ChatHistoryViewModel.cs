using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.Chat;
using LibPythonAI.PythonIF.Request;

namespace LibUIPythonAI.ViewModel.Chat {
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
