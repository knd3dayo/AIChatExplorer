using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.VectorDB;
using QAChat.Control;
using QAChat.View.PromptTemplateWindow;
using QAChat.ViewModel.PromptTemplateWindow;
using WpfAppCommon.Model;

namespace QAChat.ViewModel.QAChatMain {

    public partial class QAChatControlViewModel : ObservableObject {
        //初期化
        public QAChatControlViewModel(QAChatStartupProps props) {

            QAChatStartupProps = props;

            // VectorDBItemsを設定 ClipboardFolderのベクトルDBを取得
            VectorDBItems.Add(props.ContentItem.GetVectorDBItem());

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
        public ObservableCollection<ContentItem> ClipboardItems { get; set; } = new();


        public Chat ChatController { get; set; } = new();

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

        public static ChatHistoryItem? SelectedItem { get; set; }

        public ObservableCollection<ChatHistoryItem> ChatHistory {
            get {
                return [.. ChatController.ChatHistory];
            }
            set {
                ChatController.ChatHistory = [.. value];
                OnPropertyChanged(nameof(ChatHistory));
            }

        }

        private ObservableCollection<VectorDBItem> _vectorDBItemBases = [];
        public ObservableCollection<VectorDBItem> VectorDBItems {
            get {
                return _vectorDBItemBases;
            }
            set {
                _vectorDBItemBases = value;
                OnPropertyChanged(nameof(VectorDBItems));
            }
        }

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
        private ContentItem? _SelectedContextItem = null;
        public ContentItem? SelectedContextItem {
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
                PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILib.Resource.PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
                return ChatController.CreateOpenAIRequestJSON(libManager.ConfigParams.GetOpenAIProperties());
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

        // 外部ベクトルDBのDrawer表示状態
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

        public TextWrapping TextWrapping {
            get {
                if (QAChatManager.Instance == null) {
                    return TextWrapping.NoWrap;
                }
                return QAChatManager.Instance.ConfigParams.GetTextWrapping();
            }
        }
    }

}
