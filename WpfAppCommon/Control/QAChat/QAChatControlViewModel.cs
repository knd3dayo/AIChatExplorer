using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Model;
using WpfAppCommon.Control.Settings;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Control.QAChat {
    public class QAChatControlViewModel : ObservableObject{
        //初期化
        public void Initialize(ClipboardItem? clipboardItem) {
            // クリップボードアイテムを設定
            ClipboardItem = clipboardItem;
            // InputTextを設定
            InputText = clipboardItem?.Content ?? "";
            // ClipboardItemがある場合は、ChatItemsを設定
            if (ClipboardItem != null) {
                ChatItems.Clear();
                foreach (var chatItem in ClipboardItem.ChatItems) {
                    ChatItems.Add(chatItem);
                }
            }
        }

        public ClipboardItem? ClipboardItem { get; set; }

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
        private readonly TextSelector TextSelector = new();

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

                // 初回実行時の処理
                if (ChatItems.Count == 0) {
                    // ClipboardItemのContentTypeがImageの場合は、Base64を取得してPromptに追加
                    if (ClipboardItem?.ContentType == ClipboardContentTypes.Image && ClipboardItem.ClipboardItemImage != null) {
                        prompt += ChatItem.GenerateImageVContent(prompt, ClipboardItem.ClipboardItemImage.ImageBase64);
                    }
                }

                ChatResult? result = null;
                // プログレスバーを表示
                IsIndeterminate = true;

                // Python処理機能の初期化
                PythonExecutor.Init(ClipboardAppConfig.PythonDllPath);

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

                // ClipboardItemがある場合は、結果をClipboardItemに設定
                if (ClipboardItem != null) {
                    ClipboardItem.ChatItems.Clear();
                    ClipboardItem.ChatItems.AddRange(ChatItems);
                }

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

        // モードが変更されたときの処理
        public SimpleDelegateCommand ModeSelectionChangedCommand => new((parameter) => {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // クリア処理
            ChatItems.Clear();
            InputText = "";

        });

        // プロンプトテンプレート画面を開くコマンド
        public Action<object> PromptTemplateCommandExecute { get; set; } = (parameter) => { };
        public SimpleDelegateCommand PromptTemplateCommand => new((parameter) => { 
            PromptTemplateCommandExecute(parameter);
        });

        // Closeコマンド
        public SimpleDelegateCommand CloseCommand => new((parameter) => {
            if (parameter is Window window) {
                window.Close();
            }
        });

        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand SelectTextCommand => new((parameter) => {

            if (parameter is not TextBox textBox) {
                return;
            }

            // テキスト選択
            TextSelector.SelectText(textBox);
            return;
        });
        // 選択中のテキストをプロセスとして実行
        public SimpleDelegateCommand ExecuteSelectedTextCommand => new((parameter) => {

            if (parameter is not TextBox textbox) {
                return;
            }

            // 選択中のテキストをプロセスとして実行
            TextSelector.ExecuteSelectedText(textbox);

        });


    }
}
