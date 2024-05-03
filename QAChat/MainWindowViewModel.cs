using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Model;
using QAChat.View.LogWindow;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace QAChat {
    public partial class MainWindowViewModel : MyWindowViewModel {

        // Progress Indicatorの表示状態
        private bool _IsIndeterminate = false;
        public bool IsIndeterminate {
            get {
                return _IsIndeterminate;
            }
            set {
                _IsIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }
        // モード 0:Normal 1:LangChainWithVectorDB
        private int _Mode = (int)OpenAIExecutionModeEnum.Normal;
        public int Mode {
            get {
                return _Mode;
            }
            set {
                _Mode = value;
                OnPropertyChanged(nameof(Mode));
            }
        }
        public static ChatItem? SelectedItem { get; set; }

        public ObservableCollection<ChatItem> ChatItems { get; set; } = [];

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
                OnPropertyChanged(nameof(InputText));
            }

        }
        // プロンプトテンプレート
        private PromptItem? promptTemplate;
        public PromptItem? PromptTemplate {
            get {
                return promptTemplate;

            }
            set {
                promptTemplate = value;
                OnPropertyChanged(nameof(PromptTemplate));
            }
        }

        // プロンプトテンプレートのテキスト
        public string PromptTemplateText {
            get {
                return PromptTemplate?.Prompt ?? "";
            }
        }

        public StringBuilder Log = new();


        //  Dictionaryを引数として、そのキーと値をProperties.Settingsに保存する

        // このプロジェクトからの呼び出しか否か
        private bool isInternalProject = true;
        public bool IsInternalProject {
            get {
                return isInternalProject;
            }
            set {
                isInternalProject = value;
                OnPropertyChanged(nameof(IsInternalProject));
            }
        }

        // チャットを送信するコマンド
        public SimpleDelegateCommand SendChatCommand => new(async (parameter) => {
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // OpenAIにチャットを送信してレスポンスを受け取る
                // PromptTemplateがある場合はPromptTemplateを先頭に追加
                string prompt = "";
                if (string.IsNullOrEmpty(PromptTemplate?.Prompt) == false) {

                    prompt = PromptTemplate?.Prompt ?? "" + "\n---------\n";

                }
                prompt += InputText;

                ChatResult? result = null;
                // プログレスバーを表示
                IsIndeterminate = true;
                // モードがLangChainWithVectorDBの場合はLangChainOpenAIChatでチャットを送信
                if (Mode == (int)OpenAIExecutionModeEnum.RAG) {
                    await Task.Run(() => {
                        result = PythonExecutor.PythonFunctions?.LangChainChat(prompt, ChatItems, VectorDBItem.GetEnabledItems());
                    });
                } else {
                    // モードがNormalの場合はOpenAIChatでチャットを送信
                    await Task.Run(() => {
                        result = PythonExecutor.PythonFunctions?.OpenAIChat(prompt, ChatItems);
                    });
                }

                if (result == null) {
                    Tools.Error("チャットの送信に失敗しました。");
                    return;
                }
                // inputTextをクリア
                InputText = "";
                // リクエストをChatItemsに追加
                ChatItems.Add(new ChatItem(ChatItem.UserRole, prompt));
                // レスポンスをChatItemsに追加. inputTextはOpenAIChat or LangChainChatの中で追加される
                ChatItems.Add(new ChatItem(ChatItem.AssistantRole, result.Response, result.ReferencedFilePath));

                // verboseがある場合はログに追加
                if (!string.IsNullOrEmpty(result.Verbose)) {
                    Log.AppendLine(result.Verbose);
                }

            } catch (Exception e) {
                Tools.Error($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });

        // クリアコマンド
        public SimpleDelegateCommand ClearChatCommand => new((parameter) => {
            ChatItems.Clear();
            InputText = "";
        });

        // Closeコマンド
        public SimpleDelegateCommand CloseCommand => new((parameter) => {
            if (parameter is Window window) {
                window.Close();
            }
        });

        // 設定画面を開くコマンド
        public SimpleDelegateCommand SettingCommand => new((parameter) => {
            SettingWindow settingWindow = new();
            settingWindow.ShowDialog();
        }

        );

        // ログ画面を開くコマンド
        public SimpleDelegateCommand LogWindowCommand => new((parameter) => {
            LogWindow logWindow = new();
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
        public SimpleDelegateCommand PromptTemplateCommand => new((parameter) => {
            ListPromptTemplateWindow promptTemplateWindow = new();
            ListPromptTemplateWindowViewModel promptTemplateWindowViewModel = (ListPromptTemplateWindowViewModel)promptTemplateWindow.DataContext;
            promptTemplateWindowViewModel.Initialize(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                PromptTemplate = promptTemplateWindowViewModel.PromptItem;

            });
            promptTemplateWindow.ShowDialog();
        });

    }
}
