using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Model;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;
using WpfAppCommon.View.QAChat;

namespace WpfAppCommon.Control.QAChat {
    public class QAChatControlViewModel : ObservableObject{
        //初期化
        public void Initialize(ClipboardFolder? clipboardFolder, ClipboardItem? clipboardItem, Action<object>? PromptTemplateCommandExecute) {
            // クリップボードアイテムを設定
            ClipboardItem = clipboardItem;
            // クリップボードフォルダを設定
            ClipboardFolder = clipboardFolder;
            // VectorDBItemsを設定
            VectorDBItems = [.. VectorDBItem.GetEnabledItemsWithSystemCommonVectorDBCollectionName(ClipboardFolder?.Id.ToString(), ClipboardFolder?.Description)];

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

        // ClipboardItemを開くアクション
        public Action<ClipboardItem> OpenClipboardItemAction { get; set; } = (item) => { };

        // VectorDBItemを開くアクション
        public Action<VectorDBItem> OpenVectorDBItemAction { get; set; } = (item) => { };

        // 選択中のフォルダの全てのClipboardItem
        public ObservableCollection<ClipboardItem> ClipboardItems { get; set; } = new();


        public ClipboardItem? ClipboardItem { get; set; }

        public ClipboardFolder? ClipboardFolder { get; set; }

        public ChatController ChatController { get; set; } = new();
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

        public int Mode {
            get {
                return (int)ChatController.ChatMode;
            }
            set {
                ChatController.ChatMode = (OpenAIExecutionModeEnum)value;
                OnPropertyChanged(nameof(Mode));
            }
        }

        private ObservableCollection<VectorDBItem> vectorDBItems = [];
        public ObservableCollection<VectorDBItem> VectorDBItems {
            get {
                return vectorDBItems;
            }
            set {
                vectorDBItems = value;
                OnPropertyChanged(nameof(VectorDBItems));
            }
        }

        public static ChatItem? SelectedItem { get; set; }

        public ObservableCollection<ChatItem> ChatItems {
            get {
                return [.. ChatController.ChatItems];
            }
            set {
                ChatController.ChatItems = [.. value];
                OnPropertyChanged(nameof(ChatItems));
            }
        }

        public string InputText {
            get {
                return ChatController.ContentText;
            }
            set {
                ChatController.ContentText = value;
                OnPropertyChanged(nameof(InputText));
            }

        }
        // プロンプトの文字列
        public string PromptText {
            get {
                return ChatController.PromptTemplateText;
            }
            set {
                ChatController.PromptTemplateText = value;
                OnPropertyChanged(nameof(PromptText));
            }
        }

        // ContextItems
        public ObservableCollection<ClipboardItem> ContextItems {
            get {
                return [.. ChatController.ContextItems];
            }
            set {
                ChatController.ContextItems = [.. value];
                OnPropertyChanged(nameof(ContextItems));
            }
        }
        // SelectedContextItem
        private ClipboardItem? _SelectedContextItem = null;
        public ClipboardItem? SelectedContextItem {
            get {
                return _SelectedContextItem;
            }
            set {
                _SelectedContextItem = value;
                OnPropertyChanged(nameof(SelectedContextItem));
            }
        }
        // SelectedVectorDBItem
        private VectorDBItem? _SelectedVectorDBItem = null;
        public VectorDBItem? SelectedVectorDBItem {
            get {
                return _SelectedVectorDBItem;
            }
            set {
                _SelectedVectorDBItem = value;
                OnPropertyChanged(nameof(SelectedVectorDBItem));
            }
        }


        public string PreviewText {
            get {
                return ChatController.CreateOpenAIRequestJSON();
            }
        }

        private readonly TextSelector TextSelector = new();

        private bool _IsDrawerOpen = false;
        public bool IsDrawerOpen {
            get {
                return _IsDrawerOpen;
            }
            set {
                _IsDrawerOpen = value;
                OnPropertyChanged(nameof(IsDrawerOpen));
            }
        }

        public Visibility VectorDBItemVisibility {
            get {
                return ChatController.ChatMode == OpenAIExecutionModeEnum.RAG ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // チャットを送信するコマンド
        public SimpleDelegateCommand<object> SendChatCommand => new(async (parameter) => {
            // OpenAIにチャットを送信してレスポンスを受け取る
            try {
                // OpenAIにチャットを送信してレスポンスを受け取る
                // PromptTemplateがある場合はPromptTemplateを先頭に追加
                string prompt = PreviewText;

                ChatResult? result = null;
                // プログレスバーを表示
                IsIndeterminate = true;

                // Python処理機能の初期化
                PythonExecutor.Init(ClipboardAppConfig.PythonDllPath);

                await Task.Run(() => {
                    // LangChainChat用。VectorDBItemの有効なアイテムを設定。
                    ChatController.VectorDBItems = VectorDBItems;
                    // OpenAIChat or LangChainChatを実行
                    result = ChatController.ExecuteChat();
                });

                if (result == null) {
                    Tools.Error("チャットの送信に失敗しました。");
                    return;
                }
                // inputTextをクリア
                InputText = "";
                OnPropertyChanged(nameof(ChatItems));

                // ClipboardItemがある場合は、結果をClipboardItemに設定
                if (ClipboardItem != null) {
                    ChatController.SetChatItems(ClipboardItem);
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
        public SimpleDelegateCommand<RoutedEventArgs> ModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            ChatController.ChatMode = (OpenAIExecutionModeEnum)index;
            // ModeがRAGの場合は、VectorDBItemを取得
            if (ChatController.ChatMode == OpenAIExecutionModeEnum.RAG) {
                VectorDBItems = [.. VectorDBItem.GetEnabledItemsWithSystemCommonVectorDBCollectionName(ClipboardFolder?.Id.ToString(), ClipboardFolder?.Description)];
            }
            // VectorDBItemVisibilityを更新
            OnPropertyChanged(nameof(VectorDBItemVisibility));

        });
        // Tabが変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> TabSelectionChangedCommand => new((routedEventArgs) => {
            if (routedEventArgs.OriginalSource is TabControl tabControl) {
                // タブが変更されたときの処理
                if (tabControl.SelectedIndex == 1) {
                    // PreviewTextを更新
                    OnPropertyChanged(nameof(PreviewText));
                }
            }
        });

        // 追加コンテキスト情報が変更されたときの処理
        public SimpleDelegateCommand<RoutedEventArgs> AdditionalContextSelectionChangedCommand => new((routedEventArgs) => {

            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            // 0の場合はコンテキスト情報をクリア
            if (index == 0) {
                ChatController.ContextItems.Clear();

            } else if (index == 1) {
                // ClipboardItemを選択
                SetContentTextFromClipboardItemsAction();
            } else if (index == 2) {
                // SearchWindowを表示
                ShowSearchWindowAction();
            }
            OnPropertyChanged(nameof(ContextItems));
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

        // チャットアイテムを編集するコマンド
        public SimpleDelegateCommand<ChatItem>  OpenChatItemCommand => new((chatItem) => {
            EditChatItemWindow.OpenEditChatItemWindow(chatItem);
        });

        // 選択したクリップボードアイテムを開くコマンド

        public SimpleDelegateCommand<object> OpenClipboardItemCommand => new((parameter) => {
            if (SelectedContextItem != null) {
                OpenClipboardItemAction(SelectedContextItem);
            }
        });

        // 選択したクリップボードアイテムをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveClipboardItemCommand => new((parameter) => {
            if (SelectedContextItem != null) {
                ChatController.ContextItems.Remove(SelectedContextItem);
            }
            OnPropertyChanged(nameof(ContextItems));
        });

        // 選択したVectorDBItemの編集画面を開くコマンド
        public SimpleDelegateCommand<object> OpenVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorDBItem != null) {
                OpenVectorDBItemAction(SelectedVectorDBItem);
            }
        });

        // 選択したVectorDBItemをリストから削除するコマンド
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorDBItem != null) {
                VectorDBItems.Remove(SelectedVectorDBItem);
            }
            OnPropertyChanged(nameof(VectorDBItems));
        });

    }

}
