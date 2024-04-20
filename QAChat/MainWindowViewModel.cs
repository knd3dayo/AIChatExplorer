using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Model;
using QAChat.PythonIF;
using QAChat.Utils;

namespace QAChat {
    public class MainWindowViewModel : ObservableObject {
        public MainWindowViewModel() {
            // Pythonの初期化
            PythonExecutor.Init();
        }
        public static ChatItem? SelectedItem { get; set; }

        public ObservableCollection<ChatItem> ChatItems { get; set; } = new ObservableCollection<ChatItem>();

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
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // OpenAIにチャットを送信してレスポンスを受け取る
                ChatResult? result = PythonExecutor.PythonNetFunctions?.OpenAIChat(InputText, ChatItems);
                if (result == null) {
                    return;
                }
                // inputTextをChatItemsに追加
                ChatItems.Add(new ChatItem(ChatItem.UserRole, InputText));
                // レスポンスをChatItemsに追加
                ChatItems.Add(new ChatItem(ChatItem.AssistantRole, result.Response, result.ReferencedFilePath));
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
            if (parameter is Window window) {
                window.Close();
            }
        }

        // 設定画面を開くコマンド
        public SimpleDelegateCommand SettingCommand => new SimpleDelegateCommand((parameter) => {
             SettingWindow settingWindow = new SettingWindow();
             settingWindow.ShowDialog();
        }
        
        );
    }
}
