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
        public Action<Action<List<ClipboardItem>>> ShowSearchWindowAction { get; set; } = (afterSelect) => { };

        // ClipboardItemを選択するアクション
        public Action<Action<List<ClipboardItem>>> SetContentTextFromClipboardItemsAction { get; set; } = (afterSelect) => { };

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

        // AdditionalTextItems
        public ObservableCollection<ClipboardItem> AdditionalTextItems {
            get {
                return [.. ChatController.AdditionalTextItems];
            }
            set {
                ChatController.AdditionalTextItems = [.. value];
                OnPropertyChanged(nameof(AdditionalTextItems));
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

        // AdditionalTextItems
        public ObservableCollection<ClipboardItem> AdditionalImageItems {
            get {
                return [.. ChatController.AdditionalImageItems];
            }
            set {
                ChatController.AdditionalImageItems = [.. value];
                OnPropertyChanged(nameof(AdditionalImageItems));
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


        public string PreviewJson {
            get {
                return ChatController.CreateOpenAIRequestJSON();
            }
        }

        public string PreviewText {
            get {
                return ChatController.CreatePromptText();
            }
        }

        private readonly TextSelector TextSelector = new();

        private bool _IsDrawerOpen = true;
        public bool IsDrawerOpen {
            get {
                return _IsDrawerOpen;
            }
            set {
                _IsDrawerOpen = value;
                OnPropertyChanged(nameof(IsDrawerOpen));
            }
        }
        // 追加コンテキスト情報用のDrawer表示状態
        private bool _IsAdditionalContextDrawerOpen = false;
        public bool IsAdditionalContextDrawerOpen {
            get {
                return _IsAdditionalContextDrawerOpen;
            }
            set {
                _IsAdditionalContextDrawerOpen = value;
                OnPropertyChanged(nameof(IsAdditionalContextDrawerOpen));
            }
        }
        // ベクトルDBのDrawer表示状態
        private bool _IsVectorDBDrawerOpen = false;
        public bool IsVectorDBDrawerOpen {
            get {
                return _IsVectorDBDrawerOpen;
            }
            set {
                _IsVectorDBDrawerOpen = value;
                OnPropertyChanged(nameof(IsVectorDBDrawerOpen));
            }
        }
        // 追加画像情報用のDrawer表示状態
        private bool _IsAdditionalImageDrawerOpen = false;
        public bool IsAdditionalImageDrawerOpen {
            get {
                return _IsAdditionalImageDrawerOpen;
            }
            set {
                _IsAdditionalImageDrawerOpen = value;
                OnPropertyChanged(nameof(IsAdditionalImageDrawerOpen));
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
                    LogWrapper.Error("チャットの送信に失敗しました。");
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
                LogWrapper.Error($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            } finally {
                IsIndeterminate = false;
            }

        });

        // クリアコマンド
        public SimpleDelegateCommand<object> ClearChatCommand => new((parameter) => {
            ChatItems = [];

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
                    // プレビュータブが選択された場合、プレビューテキストを更新
                    OnPropertyChanged(nameof(PreviewText));
                } 
                if (tabControl.SelectedIndex == 2) {
                    // プレビュー(JSON)タブが選択された場合、プレビューJSONを更新
                    OnPropertyChanged(nameof(PreviewJson));
                }


            }
        });

        // 追加テキストのコンテキストメニュー
        // クリア処理
        public SimpleDelegateCommand<object> AdditionalTextClearCommand => new((parameter) => {
            ChatController.AdditionalTextItems.Clear();
            OnPropertyChanged(nameof(AdditionalTextItems));
            OnPropertyChanged(nameof(PreviewJson));
        });
        // 選択したフォルダのアイテムを追加

        public SimpleDelegateCommand<object> AdditionalTextAddFromFolderCommand => new((parameter) => {
            // ClipboardItemを選択
            SetContentTextFromClipboardItemsAction((List<ClipboardItem> selectedItems) => {
                AdditionalTextItems = [.. selectedItems];
            });
            OnPropertyChanged(nameof(AdditionalTextItems));
            OnPropertyChanged(nameof(PreviewJson));
        });
        // 検索結果のアイテムを追加
        public SimpleDelegateCommand<object> AdditionalTextAddFromSearchCommand => new((parameter) => {
            // SearchWindowを表示
            ShowSearchWindowAction((List<ClipboardItem> selectedItems) => {
                AdditionalTextItems = [.. selectedItems];
            });
            OnPropertyChanged(nameof(AdditionalTextItems));
            OnPropertyChanged(nameof(PreviewJson));
        });


        // 追加テキストのコンテキストメニュー
        // クリア処理
        public SimpleDelegateCommand<object> AdditionalImageClearCommand => new((parameter) => {
            ChatController.AdditionalImageItems.Clear();
            OnPropertyChanged(nameof(AdditionalImageItems));
            OnPropertyChanged(nameof(PreviewJson));
        });
        // 選択したフォルダのアイテムを追加

        public SimpleDelegateCommand<object> AdditionalImageAddFromFolderCommand => new((parameter) => {
            // ClipboardItemを選択
            SetContentTextFromClipboardItemsAction((List<ClipboardItem> selectedItems) => {
                // SelectedItemsのうち、ClipboardItemImagesがあるものを追加
                foreach (var item in selectedItems) {
                    if (item.ClipboardItemImages.Count != 0) {
                        ChatController.AdditionalImageItems.Add(item);
                    }
                }
            });
            OnPropertyChanged(nameof(AdditionalImageItems));
            OnPropertyChanged(nameof(PreviewJson));
        });
        // 検索結果のアイテムを追加
        public SimpleDelegateCommand<object> AdditionalImageAddFromSearchCommand => new((parameter) => {
            // SearchWindowを表示
            ShowSearchWindowAction((List<ClipboardItem> selectedItems) => {
                // SelectedItemsのうち、ClipboardItemImagesがあるものを追加
                foreach (var item in selectedItems) {
                    if (item.ClipboardItemImages.Count != 0) {
                        ChatController.AdditionalImageItems.Add(item);
                    }
                }
            });
            OnPropertyChanged(nameof(AdditionalImageItems));
            OnPropertyChanged(nameof(PreviewJson));
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
                ChatController.AdditionalTextItems.Remove(SelectedContextItem);
            }
            OnPropertyChanged(nameof(AdditionalTextItems));
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
