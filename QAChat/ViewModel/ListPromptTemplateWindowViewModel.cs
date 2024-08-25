using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon;
using WpfAppCommon.Factory;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel {
    public class ListPromptTemplateWindowViewModel : MyWindowViewModel {

        // プロンプトテンプレートの一覧
        public ObservableCollection<PromptItemViewModel> PromptItems { get; set; } = new ObservableCollection<PromptItemViewModel>();
        // 選択中の自動処理ルール
        private static PromptItemViewModel? _selectedPromptItem;
        public static PromptItemViewModel? SelectedPromptItem {
            get => _selectedPromptItem;
            set {
                _selectedPromptItem = value;
            }
        }

        public enum ActionModeEum {
            Edit,
            Select,
            Exec
        }
        private ActionModeEum ActionMode { get; set; } = ActionModeEum.Edit;
        // モード
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
        // 実行/選択ボタンの表示
        public Visibility ExecButtonVisibility {
            get {
                // ActionModeがExecまたはSelectの場合は、Visible
                return ActionMode == ActionModeEum.Exec || ActionMode == ActionModeEum.Select ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        // Modeの表示
        public Visibility ModeVisibility {
            get {
                // ActionModeがExecの場合は、Visible
                return ActionMode == ActionModeEum.Exec ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private Action<PromptItemViewModel, OpenAIExecutionModeEnum> AfterSelect { get; set; } = (promptItemViewModel, mode) => { };
        // 初期化
        public void Initialize(ActionModeEum actionMode, Action<PromptItemViewModel, OpenAIExecutionModeEnum> afterUpdate) {
            // PromptItemsを更新
            ReloadCommand.Execute();
            AfterSelect = afterUpdate;
            // ActionModeを設定
            ActionMode = actionMode;

            // SelectButtonTextを更新
            OnPropertyChanged(nameof(SelectButtonText));
            // 実行モードの場合は、実行/選択ボタンを表示する
            OnPropertyChanged(nameof(ExecButtonVisibility));
            // 実行モードの場合は、Modeを表示する
            OnPropertyChanged(nameof(ModeVisibility));

        }

        public SimpleDelegateCommand<object> ReloadCommand => new((parameter) => {
            // PromptItemsを更新
            PromptItems.Clear();
            IClipboardDBController clipboardDBController = ClipboardAppFactory.Instance.GetClipboardDBController();
            foreach (var item in clipboardDBController.GetAllPromptTemplates()) {
                PromptItemViewModel itemViewModel = new PromptItemViewModel(item);
                PromptItems.Add(itemViewModel);
            }
            OnPropertyChanged(nameof(PromptItems));

        });


        public string SelectButtonText {
            get {
                // ActionModeがExecの場合は、"実行"、それ以外は"選択"
                return ActionMode == ActionModeEum.Exec ? StringResources.Execute :StringResources.Select;
            }
        }

        public SimpleDelegateCommand<object> EditPromptItemCommand => new((parameter) => {
            if (SelectedPromptItem == null) {
                LogWrapper.Error(StringResources.NoPromptTemplateSelected);
                return;
            }
            EditPromptItemWindow.OpenEditPromptItemWindow(SelectedPromptItem, (PromptItemViewModel) => {
                // PromptItemsを更新
                ReloadCommand.Execute();
            });
        });

        // プロンプトテンプレート処理を追加する処理
        public SimpleDelegateCommand<object> AddPromptItemCommand => new((parameter) => {
            PromptItemViewModel itemViewModel = new PromptItemViewModel(new PromptItem());
            EditPromptItemWindow.OpenEditPromptItemWindow(itemViewModel, (PromptItemViewModel) => {
                // PromptItemsを更新
                ReloadCommand.Execute();
            });
        });

        // プロンプトテンプレートを選択する処理
        public SimpleDelegateCommand<Window> SelectPromptItemCommand => new((window) => {
            // 選択されていない場合はメッセージを表示
            if (SelectedPromptItem == null) {
                LogWrapper.Error(StringResources.NoPromptTemplateSelected);
                return;
            }
            // Mode からOpenAIExecutionModeEnumに変換
            OpenAIExecutionModeEnum mode = (OpenAIExecutionModeEnum)Mode;
            AfterSelect(SelectedPromptItem, mode);

            // Windowを閉じる
            window.Close();
        });

        // プロンプトテンプレートを削除する処理
        public SimpleDelegateCommand<object> DeletePromptItemCommand => new(DeletePromptItemCommandExecute);
        public void DeletePromptItemCommandExecute(object parameter) {
            PromptItemViewModel? itemViewModel = SelectedPromptItem;
            if (itemViewModel == null) {
                LogWrapper.Error(StringResources.NoPromptTemplateSelected);
                return;
            }
            PromptItem? item = SelectedPromptItem?.PromptItem;
            if (item == null) {
                LogWrapper.Error(StringResources.NoPromptTemplateSelected);
                return;
            }
            if (MessageBox.Show($"{item.Name}{StringResources.ConfirmDelete}",StringResources.Confirm, MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            PromptItems.Remove(itemViewModel);
            // LiteDBを更新
            ClipboardAppFactory.Instance.GetClipboardDBController().DeletePromptTemplate(item);
            OnPropertyChanged(nameof(PromptItems));
        }
    }
}
