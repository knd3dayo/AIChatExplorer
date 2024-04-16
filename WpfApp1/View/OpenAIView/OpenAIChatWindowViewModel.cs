using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Model;
using WpfApp1.Utils;

namespace WpfApp1.View.OpenAIView {

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
            ChatItems.Add(new ChatItem(ChatItem.UserRole, InputText));
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // OpenAIにチャットを送信してレスポンスを受け取る
                string result = AutoProcessCommand.ChatCommandExecute(JSONChatItems);
                // レスポンスをChatItemsに追加
                ChatItems.Add(new ChatItem(ChatItem.AssistantRole, result));
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
