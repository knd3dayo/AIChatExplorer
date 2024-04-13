using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Model;
using WpfApp1.Utils;

namespace WpfApp1.View.OpenAIView {
    public class ChatItem : ClipboardItem {
        public static string SystemRole = "system";
        public static string AssistantRole = "assistant";
        public static string UserRole = "user";
        public string Role { get; set; } = SystemRole;
        public new string Content { get; set; } = "";
        public ChatItem(string collectionName) : base() {
            CollectionName = collectionName;
        }
        public ChatItem(string collectionName, string role, string text) : this(collectionName) {
            Role = role;
            Content = text;
        }

    }
    public class JSONChatItem {
        [JsonPropertyName("role")]
        public string Role { get; }
        [JsonPropertyName("content")]
        public string Content { get; }
        public JSONChatItem(string role, string content) {
            Role = role;
            Content = content;
        }
    }

    public class OpenAIChatWindowViewModel : ObservableObject {
        public static ChatItem? SelectedItem { get; set; }

        public List<JSONChatItem> JSONChatItems {
            get {
                if (ChatItems == null) {
                    return new List<JSONChatItem>();
                }
                return ChatItems.Select(x => new JSONChatItem(x.Role, x.Content)).ToList();
            }
        }

        public string? LastSendText {
            get {
                // ChatItemsのうち、ユーザー発言の最後のものを取得
                var lastUserChatItem = ChatItems.LastOrDefault(x => x.Role == ChatItem.UserRole);
                return lastUserChatItem?.Content;
            }
        }
        public string? LastResponseText {
            get {
                // ChatItemsのうち、アシスタント発言の最後のものを取得
                var lastAssistantChatItem = ChatItems.LastOrDefault(x => x.Role == ChatItem.AssistantRole);
                return lastAssistantChatItem?.Content;
            }
        }

        public ObservableCollection<ChatItem> ChatItems { get; set; } = new ObservableCollection<ChatItem>();

        public string ChatSessionCollectionName { get; set; } = "chat-session";

        private string inputText = "";
        public string InputText {
            get {
                return inputText;
            }
            set {
                inputText = value;
                OnPropertyChanged("InputText");
            }

        }
        // チャットを送信するコマンド
        public SimpleDelegateCommand SendChatCommand => new SimpleDelegateCommand(SendChatCommandExecute);

        public void SendChatCommandExecute(object parameter) {
            // inputTextをChatItemsに追加
            ChatItems.Add(new ChatItem(ChatSessionCollectionName, ChatItem.UserRole, InputText));
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // OpenAIにチャットを送信してレスポンスを受け取る
                string result = PythonExecutor.PythonFunctions.OpenAIChat(JSONChatItems);
                // レスポンスをChatItemsに追加
                ChatItems.Add(new ChatItem(ChatSessionCollectionName, ChatItem.AssistantRole, result));
                // inputTextをクリア
                InputText = "";
            } catch (Exception e) {
                Tools.ShowMessage($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            }

        }

        // クリアコマンド
        public SimpleDelegateCommand ClearChatCommand => new SimpleDelegateCommand(ClearChatCommandExecute);

        public void ClearChatCommandExecute(object parameter) {
            ChatItems.Clear();
            InputText = "";
        }

        // Closeコマンド
        public SimpleDelegateCommand CloseCommand => new SimpleDelegateCommand(CloseCommandExecute);
        public void CloseCommandExecute(object parameter) {
            OpenAIChatWindow.current?.Close();
        }

    }
}
