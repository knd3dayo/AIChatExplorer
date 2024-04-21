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
using QAChat.View.LogWindow;

namespace QAChat {
    public class MainWindowViewModel : ObservableObject {
        public MainWindowViewModel() {
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

        public StringBuilder Log = new  StringBuilder();


        //  Dictionaryを引数として、そのキーと値をProperties.Settingsに保存する

        public void SaveSettings(Dictionary<string, string> settings) {
            Properties.Settings.Default.AzureOpenAI = bool.Parse(settings["AzureOpenAI"]);
            Properties.Settings.Default.OpenAIKey = settings["OpenAIKey"];
            Properties.Settings.Default.OpenAICompletionModel = settings["OpenAICompletionModel"];
            Properties.Settings.Default.OpenAIEmbeddingModel = settings["OpenAIEmbeddingModel"];
            Properties.Settings.Default.AzureOpenAIEndpoint = settings["AzureOpenAIEndpoint"];
            Properties.Settings.Default.OpenAIBaseURL = settings["OpenAIBaseURL"];
            Properties.Settings.Default.VectorDBURL = settings["VectorDBURL"];
            Properties.Settings.Default.SourceDocumentURL = settings["SourceDocumentURL"];
            Properties.Settings.Default.PythonDllPath = settings["PythonDllPath"];

            Properties.Settings.Default.Save();
        }
        // このプロジェクトからの呼び出しか否か
        private bool isInternalProject = true;
        public bool IsInternalProject {
            get {
                return isInternalProject;
            }
            set {
                isInternalProject = value;
                OnPropertyChanged("IsInternalProject");
            }
        }


        // チャットを送信するコマンド
        public SimpleDelegateCommand SendChatCommand => new SimpleDelegateCommand(SendChatCommandExecute);

        public void SendChatCommandExecute(object parameter) {
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // PythonExecutorが初期化されていない場合は初期化
                if (!PythonExecutor.Initialized) {
                    PythonExecutor.Init();
                }
                // OpenAIにチャットを送信してレスポンスを受け取る
                ChatResult? result = PythonExecutor.PythonNetFunctions?.OpenAIChat(InputText, ChatItems);
                if (result == null) {
                    return;
                }
                // inputTextをChatItemsに追加
                ChatItems.Add(new ChatItem(ChatItem.UserRole, InputText));
                // レスポンスをChatItemsに追加
                ChatItems.Add(new ChatItem(ChatItem.AssistantRole, result.Response, result.ReferencedFilePath));

                // verboseがある場合はログに追加
                if (!string.IsNullOrEmpty(result.Verbose)) {
                    Log.AppendLine(result.Verbose);
                }

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

        // ログ画面を開くコマンド
        public SimpleDelegateCommand LogWindowCommand => new SimpleDelegateCommand((parameter) => {
            LogWindow logWindow = new LogWindow();
            LogWindowViewModel logWindowViewModel = (LogWindowViewModel)logWindow.DataContext;
            logWindowViewModel.LogText = Log.ToString();
            logWindow.ShowDialog();
        });
    }
}
