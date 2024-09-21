using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model;
using PythonAILib.Model.Abstract;
using PythonAILib.Model.Chat;
using QAChat.Control;
using QAChat.View.PromptTemplateWindow;
using QAChat.ViewModel.PromptTemplateWindow;
using WpfAppCommon.Model;

namespace QAChat.ViewModel.QAChatMain {

    public partial class QAChatControlViewModel : ObservableObject {
        //初期化
        public QAChatControlViewModel(QAChatStartupProps props) {

            QAChatStartupProps = props;

            // SystemVectorDBItemsを設定 ClipboardFolderのベクトルDBを取得
            SystemVectorDBItems.Add(props.ContentItem.GetVectorDBItem());

            // ExternalVectorDBItemsを設定 ClipboardVectorDBItemのEnabledがTrueのものを取得
            ExternalVectorDBItems = [.. QAChatStartupProps.ExternalVectorDBItems];

            // InputTextを設定
            InputText = QAChatStartupProps.ContentItem?.Content ?? "";
            // ClipboardItemがある場合は、ChatItemsを設定
            if (QAChatStartupProps.ContentItem != null) {
                ChatHistory = [.. QAChatStartupProps.ContentItem.ChatItems];
            }

        }

        public CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;

        public QAChatStartupProps QAChatStartupProps { get; set; }


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

        // 選択中のフォルダの全てのClipboardItem
        public ObservableCollection<ContentItemBase> ClipboardItems { get; set; } = new();


        public Chat ChatController { get; set; } = new(ClipboardAppConfig.CreateOpenAIProperties());

        private void PromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;
            });
        }

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

        public static ChatIHistorytem? SelectedItem { get; set; }

        public ObservableCollection<ChatIHistorytem> ChatHistory {
            get {
                return [.. ChatController.ChatHistory];
            }
            set {
                ChatController.ChatHistory = [.. value];
                OnPropertyChanged(nameof(ChatHistory));
            }

        }

        private ObservableCollection<VectorDBItemBase> _externalVectorDBItems = [];
        public ObservableCollection<VectorDBItemBase> ExternalVectorDBItems {
            get {
                return _externalVectorDBItems;
            }
            set {
                _externalVectorDBItems = value;
                OnPropertyChanged(nameof(ExternalVectorDBItems));
            }
        }

        // SelectedExternalVectorDBItem
        private VectorDBItemBase? _SelectedExternalVectorDBItem = null;
        public VectorDBItemBase? SelectedExternalVectorDBItem {
            get {
                return _SelectedExternalVectorDBItem;
            }
            set {
                _SelectedExternalVectorDBItem = value;
                OnPropertyChanged(nameof(SelectedExternalVectorDBItem));
            }
        }

        // ベクトルDB(フォルダ)
        private ObservableCollection<VectorDBItemBase> _SystemVectorDBItems = [];
        public ObservableCollection<VectorDBItemBase> SystemVectorDBItems {
            get {
                return _SystemVectorDBItems;
            }
            set {
                _SystemVectorDBItems = value;
                OnPropertyChanged(nameof(SystemVectorDBItems));
            }
        }
        // ベクトルDB(フォルダ)の選択中のアイテム
        private VectorDBItemBase? _SelectedSystemVectorDBItem = null;
        public VectorDBItemBase? SelectedSystemVectorDBItem {
            get {
                return _SelectedSystemVectorDBItem;
            }
            set {
                _SelectedSystemVectorDBItem = value;
                OnPropertyChanged(nameof(SelectedSystemVectorDBItem));
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

        // AdditionalItems
        public ObservableCollection<AdditionalItemViewModel> AdditionalItems {
            get {
                ObservableCollection<AdditionalItemViewModel> items = [];
                foreach (var item in ChatController.AdditionalItems) {
                    items.Add(new(this, item));
                }
                return items;
            }
        }

        // SelectedContextItem
        private ContentItemBase? _SelectedContextItem = null;
        public ContentItemBase? SelectedContextItem {
            get {
                return _SelectedContextItem;
            }
            set {
                _SelectedContextItem = value;
                OnPropertyChanged(nameof(SelectedContextItem));
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

        // 左側メニューのDrawer表示状態
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

        // システムベクトルDBのDrawer表示状態
        private bool _IsSystemVectorDBDrawerOpen = false;
        public bool IsSystemVectorDBDrawerOpen {
            get {
                return _IsSystemVectorDBDrawerOpen;
            }
            set {
                _IsSystemVectorDBDrawerOpen = value;
                OnPropertyChanged(nameof(IsSystemVectorDBDrawerOpen));
            }
        }

        // 外部ベクトルDBのDrawer表示状態
        private bool _IsExternalVectorDBDrawerOpen = false;
        public bool IsExternalVectorDBDrawerOpen {
            get {
                return _IsExternalVectorDBDrawerOpen;
            }
            set {
                _IsExternalVectorDBDrawerOpen = value;
                OnPropertyChanged(nameof(IsExternalVectorDBDrawerOpen));
            }
        }

        // 画像アイテム用のDrawer表示状態
        private bool _IsAdditionalItemDrawerOpen = false;
        public bool IsAdditionalItemDrawerOpen {
            get {
                return _IsAdditionalItemDrawerOpen;
            }
            set {
                _IsAdditionalItemDrawerOpen = value;
                OnPropertyChanged(nameof(IsAdditionalItemDrawerOpen));
            }
        }

        //
        public Visibility VectorDBItemVisibility {
            get {
                if (ChatController.ChatMode == OpenAIExecutionModeEnum.Normal) {
                    return Visibility.Collapsed;
                } else {
                    return Visibility.Visible;
                }
            }
        }
    }

}
