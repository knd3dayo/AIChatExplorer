using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;

namespace LibUIPythonAI.ViewModel.PromptTemplate {
    public class EditPromptItemWindowViewModel : CommonViewModelBase {


        // 初期化
        public EditPromptItemWindowViewModel(PromptItemViewModel itemViewModel, Action<PromptItemViewModel> afterUpdate) {
            ItemViewModel = itemViewModel;
            Name = ItemViewModel.PromptItem.Name ?? "";
            Description = ItemViewModel.Description ?? "";
            Prompt = ItemViewModel.Content ?? "";
            PromptResultToInput = !string.IsNullOrEmpty(ItemViewModel.PromptItem.PromptInputName);
            SelectedInputItem = InputTypeItems.FirstOrDefault(x => x.Name == ItemViewModel.PromptItem.PromptInputName);
            AfterUpdate = afterUpdate;
            OnPropertyChanged(nameof(PromptResultTypeIndex));
            OnPropertyChanged(nameof(PromptOutputTypeIndex));
            OnPropertyChanged(nameof(ChatModeIndex));
            OnPropertyChanged(nameof(InputTypeItems));

            Task.Run(async () => {
                // InputTypeItemsの初期化
                InputTypeItems = await LoadInputTypeItemsAsync();
                OnPropertyChanged(nameof(InputTypeItems));
            });

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
        // PromptResultToInput
        private bool promptResultToInput = false;
        public bool PromptResultToInput {
            get {
                return promptResultToInput;
            }
            set {
                promptResultToInput = value;
                OnPropertyChanged(nameof(PromptResultToInput));
                OnPropertyChanged(nameof(InputTypeItemsVisibility));
            }
        }
        // InputTypeItemsVisibility
        public Visibility InputTypeItemsVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(PromptResultToInput);

        // InputTypeItems
        public ObservableCollection<PromptItem> InputTypeItems { get; private set; } = [];

        private async Task<ObservableCollection<PromptItem>> LoadInputTypeItemsAsync() {
            // DBからプロンプトテンプレートを取得し、選択させる
            List<PromptItem> promptItems = await PromptItem.GetPromptItems();
            promptItems = promptItems.Where(x => x.PromptResultType == PromptResultTypeEnum.TextContent).ToList();
            return [.. promptItems];
        }


        // SelectedInputTypeItem
        public PromptItem? SelectedInputItem { get; set; }


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
        // ChatModeIndex
        public int ChatModeIndex {
            get {
                if (ItemViewModel == null) {
                    return 0;
                }
                return (int)ItemViewModel.PromptItem.ChatMode;
            }
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.PromptItem.ChatMode = (OpenAIExecutionModeEnum)value;
                OnPropertyChanged(nameof(ChatModeIndex));
            }
        }
        // SplitModeIndex
        public int SplitModeIndex {
            get {
                if (ItemViewModel == null) {
                    return 0;
                }
                return (int)ItemViewModel.PromptItem.SplitMode;
            }
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.PromptItem.SplitMode = (SplitModeEnum)value;
                OnPropertyChanged(nameof(SplitModeIndex));
            }
        }
        // RAGModeIndex
        public int RAGModeIndex {
            get {
                if (ItemViewModel == null) {
                    return 0;
                }
                return (int)ItemViewModel.PromptItem.RAGMode;
            }
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.PromptItem.RAGMode = (RAGModeEnum)value;
                OnPropertyChanged(nameof(RAGModeIndex));
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

        public SimpleDelegateCommand<RoutedEventArgs> ChatModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            ChatModeIndex = index;
        });

        public SimpleDelegateCommand<RoutedEventArgs> SplitModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            SplitModeIndex = index;
        });

        // RAGModeSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> RAGModeSelectionChangedCommand => new((routedEventArgs) => {
            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択されたComboBoxItemのIndexを取得
            int index = comboBox.SelectedIndex;
            RAGModeIndex = index;
        });

        private Action<PromptItemViewModel> AfterUpdate { get; set; } = (promtItem) => { };


        // OKボタンのコマンド
        public SimpleDelegateCommand<Window> OKButtonCommand => new(async (window) => {
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
            promptItem.ChatMode = (OpenAIExecutionModeEnum)ChatModeIndex;
            promptItem.SplitMode = (SplitModeEnum)SplitModeIndex;

            // 入力がプロンプト結果の場合はPromptInputNameを設定
            if (PromptResultToInput) {
                // PromptResultToInputが選択されている場合は、PromptInputNameを設定
                promptItem.PromptInputName = SelectedInputItem?.Name ?? "";
            }

            // Nameが空の場合はエラーメッセージを表示
            if (string.IsNullOrEmpty(Name)) {
                LogWrapper.Error(CommonStringResources.Instance.EnterName);
                return;
            }
            // Descriptionが空の場合はエラーメッセージを表示
            if (string.IsNullOrEmpty(Description)) {
                LogWrapper.Error(CommonStringResources.Instance.EnterDescription);
                return;
            }

            promptItem.SaveAsync();

            await PromptItem.LoadItemsAsync();

            AfterUpdate(ItemViewModel);

            // ウィンドウを閉じる
            window.Close();
        });

    }
}
