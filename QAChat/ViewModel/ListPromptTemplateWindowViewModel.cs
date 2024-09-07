using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using PythonAILib.Model;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon;
using WpfAppCommon.Factory;
using WpfAppCommon.Model;
using WpfAppCommon.Model.QAChat;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel
{
    public class ListPromptTemplateWindowViewModel : MyWindowViewModel {

        // 初期化
        public ListPromptTemplateWindowViewModel(ActionModeEum actionMode, Action<PromptItemViewModel, OpenAIExecutionModeEnum> afterUpdate, Func<PromptItemBase> createPromptItemFunction) {
            // PromptItemsを更新
            ReloadCommand.Execute();
            AfterSelect = afterUpdate;
            // ActionModeを設定
            ActionMode = actionMode;
            // CreatePromptItemFunctionを設定
            CreatePromptItemFunction = createPromptItemFunction;

            // SelectButtonTextを更新
            OnPropertyChanged(nameof(SelectButtonText));
            // 実行モードの場合は、実行/選択ボタンを表示する
            OnPropertyChanged(nameof(ExecButtonVisibility));
            // 実行モードの場合は、Modeを表示する
            OnPropertyChanged(nameof(ModeVisibility));

        }
        // PromptItemを作成する処理
        public Func<PromptItemBase> CreatePromptItemFunction { get; set; } 

        // プロンプトテンプレートの一覧
        public ObservableCollection<PromptItemViewModel> PromptItems { get; set; } = [];
        // 選択中の自動処理ルール
        private static PromptItemViewModel? _selectedPromptItem;
        public static PromptItemViewModel? SelectedPromptItem {
            get => _selectedPromptItem;
            set {
                _selectedPromptItem = value;
            }
        }

        //システム用のプロンプトテンプレートを表示するか否か
        public bool _IsShowSystemPromptItems = false;
        public bool IsShowSystemPromptItems {
            get {
                return _IsShowSystemPromptItems;
            }
            set {
                _IsShowSystemPromptItems = value;
                OnPropertyChanged(nameof(IsShowSystemPromptItems));
                ReloadCommand.Execute();
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

        public SimpleDelegateCommand<object> ReloadCommand => new((parameter) => {
            // PromptItemsを更新
            PromptItems.Clear();
            IClipboardDBController clipboardDBController = ClipboardAppFactory.Instance.GetClipboardDBController();
            foreach (var item in clipboardDBController.GetAllPromptTemplates()) {
                // システム用のプロンプトテンプレートを表示しない場合は、システム用のプロンプトテンプレートを表示しない
                if (!IsShowSystemPromptItems && 
                    ( item.PromptTemplateType == PromptItemBase.PromptTemplateTypeEnum.SystemDefined ||
                       item.PromptTemplateType == PromptItemBase.PromptTemplateTypeEnum.ModifiedSystemDefined )) {
                    continue;
                }
                PromptItemViewModel itemViewModel = new (item);
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
            PromptItemBase item = CreatePromptItemFunction();
            PromptItemViewModel itemViewModel = new PromptItemViewModel(item);
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
            PromptItemBase? item = SelectedPromptItem?.PromptItem;
            if (item == null) {
                LogWrapper.Error(StringResources.NoPromptTemplateSelected);
                return;
            }
            if (MessageBox.Show($"{item.Name}{StringResources.ConfirmDelete}",StringResources.Confirm, MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            PromptItems.Remove(itemViewModel);
            // PromptItemを保存
            item.Save();

            OnPropertyChanged(nameof(PromptItems));
        }
    }
}
