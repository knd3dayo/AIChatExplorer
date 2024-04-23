using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Model;
using QAChat.PythonIF;
using QAChat.Utils;
using QAChat.View.LogWindow;
using QAChat.View.PromptTemplateWindow;

namespace QAChat {
    public class MainWindowViewModel : ObservableObject {

        // Progress Indicatorの表示状態
        private bool _IsIndeterminate = false;
        public bool IsIndeterminate {
            get {
                return _IsIndeterminate;
            }
            set {
                _IsIndeterminate = value;
                OnPropertyChanged("IsIndeterminate");
            }
        }
        // モードのEnum
        public enum ModeEnum {
            Normal = 0,
            LangChainWithVectorDB = 1,
        }
        // モード
        private int _Mode = (int)ModeEnum.Normal;
        public int Mode {
            get {
                return _Mode;
            }
            set {
                _Mode = value;
                OnPropertyChanged(nameof(Mode));
            }
        }
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
        // プロンプトテンプレート
        private PromptItem? promptTemplate;
        public PromptItem? PromptTemplate {
            get {
                return promptTemplate;

            }
            set{
                promptTemplate = value;
                OnPropertyChanged("PromptTemplate");
            }
         }

        // プロンプトテンプレートのテキスト
        public string PromptTemplateText {
            get {
                return PromptTemplate?.Prompt ?? "";
            }
        }

        public StringBuilder Log = new  StringBuilder();


        //  Dictionaryを引数として、そのキーと値をProperties.Settingsに保存する

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

        // SaveSettings
        public void SaveSettings(Dictionary<string, string> settings) {
            QAChatProperties.SaveSettings(settings);
        }


        // チャットを送信するコマンド
        public SimpleDelegateCommand SendChatCommand => new SimpleDelegateCommand(SendChatCommandExecute);

        public async void SendChatCommandExecute(object parameter) {
            IsIndeterminate = true;
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // PythonExecutorが初期化されていない場合は初期化
                if (!PythonExecutor.Initialized) {
                    // プロジェクトが内部プロジェクトでない場合はPythonEngineはすでに初期化済み
                    PythonExecutor.Init(!IsInternalProject);
                }
                // OpenAIにチャットを送信してレスポンスを受け取る
                // PromptTemplateがある場合はPromptTemplateを先頭に追加
                string prompt = "";
                if (string.IsNullOrEmpty(PromptTemplate?.Prompt) == false) {

                    prompt = PromptTemplate?.Prompt ?? "" + "\n---------\n";

                }
                prompt += InputText;

                // inputTextをChatItemsに追加
                ChatItems.Add(new ChatItem(ChatItem.UserRole, prompt));

                ChatResult? result = null;
                // モードがLangChainWithVectorDBの場合はLangChainOpenAIChatでチャットを送信
                if (Mode == (int)ModeEnum.LangChainWithVectorDB) {
                    await Task.Run(() => {
                        result = PythonExecutor.PythonNetFunctions?.LangChainOpenAIChat(prompt, ChatItems);
                    });
                } else {
                    // モードがNormalの場合はOpenAIChatでチャットを送信
                    await Task.Run(() => {
                        result = PythonExecutor.PythonNetFunctions?.OpenAIChat(prompt, ChatItems);
                    });
                }

                if (result == null) {
                    Tools.ShowMessage("チャットの送信に失敗しました。");
                    return;
                }
                // inputTextをクリア
                InputText = "";
                // レスポンスをChatItemsに追加
                ChatItems.Add(new ChatItem(ChatItem.AssistantRole, result.Response, result.ReferencedFilePath));

                // verboseがある場合はログに追加
                if (!string.IsNullOrEmpty(result.Verbose)) {
                    Log.AppendLine(result.Verbose);
                }

            } catch (Exception e) {
                Tools.ShowMessage($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            }finally {
                IsIndeterminate = false;
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

        // モードが変更されたときの処理
        public SimpleDelegateCommand ModeSelectionChangedCommand => new((parameter) => {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // クリア処理
            ChatItems.Clear();
            InputText = "";

        });

        // プロンプトテンプレート画面を開くコマンド
        public SimpleDelegateCommand PromptTemplateCommand => new SimpleDelegateCommand((parameter) => {
            ListPromptTemplateWindow promptTemplateWindow = new ListPromptTemplateWindow();
            ListPromptTemplateWindowViewModel promptTemplateWindowViewModel = (ListPromptTemplateWindowViewModel)promptTemplateWindow.DataContext;
            promptTemplateWindowViewModel.Initialize((promptTemplateWindowViewModel) => {
                PromptTemplate = promptTemplateWindowViewModel.PromptItem;

            });
            promptTemplateWindow.ShowDialog();
        });

    }
}
