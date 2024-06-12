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
        public void Initialize(ClipboardFolder? clipboardFolder, ClipboardItem? clipboardItem, Action<object>? PromptTemplateCommandExecute) {
            // クリップボードアイテムを設定
            ClipboardItem = clipboardItem;
            // クリップボードフォルダを設定
            ClipboardFolder = clipboardFolder;

            // InputTextを設定
            InputText = clipboardItem?.Content ?? "";
            // ClipboardItemがある場合は、ChatItemsを設定
            if (ClipboardItem != null) {
                ChatItems.Clear();
                foreach (var chatItem in ClipboardItem.ChatItems) {
                    ChatItems.Add(chatItem);
                }
            }
            // PromptTemplateCommandExecuteを設定
            if (PromptTemplateCommandExecute != null) {
                this.PromptTemplateCommandExecute = PromptTemplateCommandExecute;
            }
        }

        // CollectionName
        private string? _CollectionName = null;
        public string? CollectionName {
            get {
                return _CollectionName;
            }
            set {
                _CollectionName = value;
                OnPropertyChanged(nameof(CollectionName));
            }
        }
        // SearchWindowを表示するAction
        public Action ShowSearchWindowAction { get; set; } = () => { };

        // ClipboardItemを選択するアクション
        public Action SetContentTextFromClipboardItemsAction { get; set; } = () => { };

        // 選択中のフォルダの全てのClipboardItem
        public ObservableCollection<ClipboardItem> ClipboardItems { get; set; } = new();


        public ClipboardItem? ClipboardItem { get; set; }

        public ClipboardFolder? ClipboardFolder { get; set; }

        public Action<object> PromptTemplateCommandExecute { get; set; } = (parameter) => { };

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
        private int _Mode = (int)OpenAIExecutionModeEnum.RAG;
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
                UpdatePreviewText();
            }

        }
        private readonly TextSelector TextSelector = new();

        // プロンプトの文字列
        private string promptText = "";
        public string PromptText {
            get {
                return promptText;
            }
            set {
                promptText = value;
                OnPropertyChanged(nameof(PromptText));

                UpdatePreviewText();
            }
        }

        // ContextText
        private string contextText = "";
        public string ContextText {
            get {
                return contextText;
            }
            set {
                contextText = value;
                UpdatePreviewText();
                OnPropertyChanged(nameof(ContextText));
            }
        }


        // PreviewText プロンプトテンプレート + コンテキスト情報 +入力テキスト
        private string _PreviewText = "";
        public string PreviewText {
            get {
                return _PreviewText;
            }
            set {
                _PreviewText = value;
                OnPropertyChanged(nameof(PreviewText));
            }
        }

        private void UpdatePreviewText() {
            string prompt = "";
            if (string.IsNullOrEmpty(PromptText) == false) {
                prompt = PromptText + "\n---------以下は本文です------\n";
            }
            prompt += InputText;

            if (string.IsNullOrEmpty(ContextText) == false) {
                prompt += "\n---------以下は本文の背景情報です--------\n";
                prompt += ContextText;
            }


            PreviewText = prompt;
            OnPropertyChanged(nameof(PreviewText));
        }
        
        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // OpenAIにチャットを送信してレスポンスを受け取る
                // PromptTemplateがある場合はPromptTemplateを先頭に追加
                string prompt = PreviewText;

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
                        // VectorDBItemの有効なアイテムを取得してLangChainChatを実行
                        IEnumerable<VectorDBItem> enabledItems = VectorDBItem.GetEnabledItemsWithSystemCommonVectorDBCollectionName(ClipboardFolder?.Id.ToString(), ClipboardFolder?.Description);
                        result = PythonExecutor.PythonFunctions?.LangChainChat(enabledItems, prompt, ChatItems);
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

            } catch (Exception e) {
                Tools.Error($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });

        // クリアコマンド
        public SimpleDelegateCommand<object> ClearChatCommand => new((parameter) => {
            ChatItems.Clear();
            InputText = "";
        });

        // モードが変更されたときの処理
        public SimpleDelegateCommand<object> ModeSelectionChangedCommand => new((parameter) => {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // クリア処理
            ChatItems.Clear();
            // InputText = "";

        });
        // 追加コンテキスト情報が変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> AdditionalContextSelectionChangedCommand => new((routedEventArgs) => {

            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            // 0の場合はコンテキスト情報をクリア
            if (index == 0) {
                ContextText = "";
            } else if (index == 1) {
                // ClipboardItemを選択
                SetContentTextFromClipboardItemsAction();
            } else if (index == 2) {
                // SearchWindowを表示
                ShowSearchWindowAction();
            }
            OnPropertyChanged(nameof(PreviewText));
        });

        // プロンプトテンプレート画面を開くコマンド
        public SimpleDelegateCommand<object> PromptTemplateCommand => new((parameter) => { 

            PromptTemplateCommandExecute(parameter);
        });

        // Closeコマンド
        public SimpleDelegateCommand<Window?> CloseCommand => new((window) => {

            window?.Close();
        });

        // Ctrl + Aを一回をしたら行選択、二回をしたら全選択
        public SimpleDelegateCommand<TextBox> SelectTextCommand => new((textBox) => {

            // テキスト選択
            TextSelector.SelectText(textBox);
            return;
        });

        // 選択中のテキストをプロセスとして実行
        public SimpleDelegateCommand<TextBox> ExecuteSelectedTextCommand => new((textbox) => {

            // 選択中のテキストをプロセスとして実行
            TextSelector.ExecuteSelectedText(textbox);
        });

    }
}
