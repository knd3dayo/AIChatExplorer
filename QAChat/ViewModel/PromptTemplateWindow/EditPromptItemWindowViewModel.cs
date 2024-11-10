using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Prompt;
using QAChat.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.PromptTemplateWindow {
    public class EditPromptItemWindowViewModel : QAChatViewModelBase {

        // 文字列リソース
        public string StringResourcesTitle => StringResources.EditPrompt;
        public string StringResourcesName => StringResources.Name;
        public string StringResourcesDescription => StringResources.Description;
        // 自動処理時の設定
        public string StringResourcesAutoProcessSetting => StringResources.AutoProcessSetting;
        // Chatタイプ
        public string StringResourcesChatType => StringResources.ChatType;
        // 出力形式
        public string StringResourcesOutputType => StringResources.OutputType;

        // 文字列
        public string StringResourcesStringType => StringResources.StringType;
        // リスト
        public string StringResourcesListType => StringResources.ListType;
        // テーブル
        public string StringResourcesTableType => StringResources.TableType;
        // 出力先
        public string StringResourcesOutputDestination => StringResources.OutputDestination;
        // 新規タブ
        public string StringResourcesNewTab => StringResources.NewTab;
        // 本文を上書き
        public string StringResourcesOverwriteContent => StringResources.OverwriteContent;
        // タイトルを上書き
        public string StringResourcesOverwriteTitle => StringResources.OverwriteTitle;
        // OK
        public string StringResourcesOK => StringResources.OK;
        // キャンセル
        public string StringResourcesCancel => StringResources.Cancel;

        // 初期化
        public EditPromptItemWindowViewModel(PromptItemViewModel itemViewModel, Action<PromptItemViewModel> afterUpdate) {
            ItemViewModel = itemViewModel;
            Name = ItemViewModel.PromptItem.Name ?? "";
            Description = ItemViewModel.Description ?? "";
            Prompt = ItemViewModel.Content ?? "";
            AfterUpdate = afterUpdate;
            OnPropertyChanged(nameof(PromptResultTypeIndex));
            OnPropertyChanged(nameof(PromptOutputTypeIndex));
            OnPropertyChanged(nameof(ChatTypeIndex));

        }

        private PromptItemViewModel? itemViewModel;
        public PromptItemViewModel? ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                OnPropertyChanged(nameof(ItemViewModel));
            }
        }
        // 名前
        public string Name {
            get => ItemViewModel?.PromptItem?.Name ?? "";
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.PromptItem.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Description {
            get => ItemViewModel?.Description ?? "";
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        public string Prompt {
            get => ItemViewModel?.Content ?? "";
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.Content = value;
                OnPropertyChanged(nameof(Prompt));
            }
        }


        public int PromptResultTypeIndex {
            get {
                if (ItemViewModel == null) {
                    return 0;
                }
                return (int)ItemViewModel.PromptItem.PromptResultType;
            }
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.PromptItem.PromptResultType = (PromptResultTypeEnum)value;
                OnPropertyChanged(nameof(PromptResultTypeIndex));
            }
        }

        public int PromptOutputTypeIndex {
            get {
                if (ItemViewModel == null) {
                    return 0;
                }
                return (int)ItemViewModel.PromptItem.PromptOutputType;
            }
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.PromptItem.PromptOutputType = (PromptOutputTypeEnum)value;
                OnPropertyChanged(nameof(PromptOutputTypeIndex));
            }
        }
        // ChatTypeIndex
        public int ChatTypeIndex {
            get {
                if (ItemViewModel == null) {
                    return 0;
                }
                return (int)ItemViewModel.PromptItem.ChatType;
            }
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.PromptItem.ChatType = (OpenAIExecutionModeEnum)value;
                OnPropertyChanged(nameof(ChatTypeIndex));
            }
        }

        public SimpleDelegateCommand<RoutedEventArgs> PromptResultTypeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            PromptResultTypeIndex = index;
        });

        public SimpleDelegateCommand<RoutedEventArgs> PromptOutputTypeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            PromptOutputTypeIndex = index;
        });

        public SimpleDelegateCommand<RoutedEventArgs> ChatTypeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            ChatTypeIndex = index;
        });

        private Action<PromptItemViewModel> AfterUpdate { get; set; } = (promtItem) => { };
        public TextWrapping TextWrapping {
            get {
                if (QAChatManager.Instance == null) {
                    return TextWrapping.NoWrap;
                }
                return QAChatManager.Instance.ConfigParams.GetTextWrapping();
            }
        }

        // OKボタンのコマンド
        public SimpleDelegateCommand<Window> OKButtonCommand => new((window) => {
            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            if (ItemViewModel.PromptItem == null) {
                return;
            }
            PromptItem promptItem = ItemViewModel.PromptItem;
            promptItem.Description = Description;
            promptItem.Prompt = Prompt;
            promptItem.Name = Name;
            promptItem.PromptResultType = (PromptResultTypeEnum)PromptResultTypeIndex;
            promptItem.PromptOutputType = (PromptOutputTypeEnum)PromptOutputTypeIndex;
            promptItem.ChatType = (OpenAIExecutionModeEnum)ChatTypeIndex;

            // Nameが空の場合はエラーメッセージを表示
            if (string.IsNullOrEmpty(Name)) {
                LogWrapper.Error(StringResources.EnterName);
                return;
            }
            // Descriptionが空の場合はエラーメッセージを表示
            if (string.IsNullOrEmpty(Description)) {
                LogWrapper.Error(StringResources.EnterDescription);
                return;
            }
            // PromptItemBaseを更新
            promptItem.Save();

            AfterUpdate(ItemViewModel);

            // ウィンドウを閉じる
            window.Close();
        });

    }
}
