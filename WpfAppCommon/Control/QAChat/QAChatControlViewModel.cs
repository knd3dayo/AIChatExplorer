using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using QAChat.Model;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using WpfAppCommon.View.QAChat;

namespace WpfAppCommon.Control.QAChat {
    public partial class QAChatControlViewModel : ObservableObject{
        //初期化
        public void Initialize(ClipboardItem? clipboardItem, Action<object>? PromptTemplateCommandExecute) {
            if (clipboardItem == null) {
                // ClipboardItemが存在しない場合はチャット履歴フォルダに保存
                ClipboardItem = new(ClipboardFolder.ChatRootFolder.Id);
                ClipboardFolder = ClipboardFolder.ChatRootFolder;

            } else {
                // クリップボードアイテムを設定
                ClipboardItem = clipboardItem;
                // クリップボードフォルダを設定
                ClipboardFolder = clipboardItem.GetFolder();
                // チャット履歴用のItemの設定
                // チャット履歴を保存する。チャット履歴に同一階層のフォルダを作成して、Itemをコピーする。
                ClipboardFolder chatFolder = ClipboardFolder.GetAnotherTreeFolder(ClipboardFolder, ClipboardFolder.CHAT_ROOT_FOLDER_NAME, true);
                ChatHistoryItem = new(chatFolder.Id);
            }
            // VectorDBItemsを設定
            VectorDBItems = [.. ClipboardFolder?.GetVectorDBItems()];

            // InputTextを設定
            InputText = clipboardItem?.Content ?? "";
            // ClipboardItemがある場合は、ChatItemsを設定
            if (ClipboardItem != null) {
                ChatHistory = [.. ClipboardItem.ChatItems];
            }
            // PromptTemplateCommandExecuteを設定
            if (PromptTemplateCommandExecute != null) {
                this.PromptTemplateCommandExecute = PromptTemplateCommandExecute;
            }

        }

        // チャット履歴用のItem
        public ClipboardItem? ChatHistoryItem { get; set; }

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

        public ChatRequest ChatController { get; set; } = new(ClipboardAppConfig.CreateOpenAIProperties());
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

        public ObservableCollection<ChatItem> ChatHistory {
            get {
                return [.. ChatController.ChatHistory];
            }
            set {
                ChatController.ChatHistory = [.. value];
                OnPropertyChanged(nameof(ChatHistory));
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
        private ObservableCollection<ClipboardItem> _AdditionalTextItems = new();
        public ObservableCollection<ClipboardItem> AdditionalTextItems {
            get {

                return _AdditionalTextItems;
            }
            set {
                _AdditionalTextItems = value;
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
        private ObservableCollection<ClipboardItem> _AdditionalImageItems = new();
        public ObservableCollection<ClipboardItem> AdditionalImageItems {
            get {
                return _AdditionalImageItems;
            }
            set {
                _AdditionalImageItems = value;
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
                UpdateChatRequestAdditionalItems();
                return ChatController.CreateOpenAIRequestJSON();
            }
        }

        public string PreviewText {
            get {
                UpdateChatRequestAdditionalItems();
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

        // ChatControllerにAdditionalTextItemとAdditionalImageItemを設定
        private void UpdateChatRequestAdditionalItems() {
            ChatController.AdditionalTextItems = AdditionalTextItems.Select(x => x.Content).ToList();
            foreach (var item in AdditionalImageItems) {
                foreach (var image in item.ClipboardItemImages) {
                    ChatController.AdditionalImageURLs.Add(ChatRequest.CreateImageURLBase64String(image.ImageBase64));
                }

            }
        }
    }

}
