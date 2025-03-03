using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.ExportImport;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.PromptTemplate;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Prompt;

namespace LibUIPythonAI.ViewModel.PromptTemplate {
    public class ListPromptTemplateWindowViewModel : ChatViewModelBase {
        // 初期化

        public enum ActionModeEum {
            Edit,
            Select,
            Exec
        }

        public ListPromptTemplateWindowViewModel(ActionModeEum actionMode, Action<PromptItemViewModel, OpenAIExecutionModeEnum> afterUpdate) {
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
        public string SelectButtonText {
            get {
                // ActionModeがExecの場合は、"実行"、それ以外は"選択"
                return ActionMode == ActionModeEum.Exec ? StringResources.Execute : StringResources.Select;
            }
        }


        private Action<PromptItemViewModel, OpenAIExecutionModeEnum> AfterSelect { get; set; } = (promptItemViewModel, mode) => { };

        public SimpleDelegateCommand<object> ReloadCommand => new((parameter) => {
            IDataFactory clipboardDBController = PythonAILibManager.Instance?.DataFactory ?? throw new NullReferenceException();

            // PromptItemsを更新
            PromptItems.Clear();
            foreach (var item in clipboardDBController.GetPromptCollection<PromptItem>().FindAll()) {
                // システム用のプロンプトテンプレートを表示しない場合は、システム用のプロンプトテンプレートを表示しない
                if (!IsShowSystemPromptItems &&
                    (item.PromptTemplateType == PromptTemplateTypeEnum.SystemDefined ||
                       item.PromptTemplateType == PromptTemplateTypeEnum.ModifiedSystemDefined)) {
                    continue;
                }
                PromptItemViewModel itemViewModel = new(item);
                PromptItems.Add(itemViewModel);
            }
            OnPropertyChanged(nameof(PromptItems));

        });

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
            IDataFactory clipboardDBController = PythonAILibManager.Instance?.DataFactory ?? throw new NullReferenceException();
            PromptItem item = new();

            PromptItemViewModel itemViewModel = new(item);
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
            // ChatMode からOpenAIExecutionModeEnumに変換
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
            if (MessageBox.Show($"{item.Name}{StringResources.ConfirmDelete}", StringResources.Confirm, MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            PromptItems.Remove(itemViewModel);
            // PromptItemを保存
            item.Delete();

            OnPropertyChanged(nameof(PromptItems));
        }

        // Excelへエクスポートする処理

        public SimpleDelegateCommand<object> ExportToExcelCommand => new((obj) => {
            string SelectedFileName = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-PromptTemplate"   + ".xlsx";

            //ファイルダイアログを表示


            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFilePlease,
                DefaultFileName = SelectedFileName,
                InitialDirectory = @".",
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                SelectedFileName = dialog.FileName;

                Task.Run(() => {
                    ImportExportUtil.ExportPromptItemsToExcel(SelectedFileName, PromptItems.Select( x => x.PromptItem).ToList());
                });
            }
        });

        public SimpleDelegateCommand<object> ImportFromExcelCommand => new((obj) => {
            string SelectedFileName = "";
            //ファイルダイアログを表示
            using var dialog = new CommonOpenFileDialog() {
                Title = CommonStringResources.Instance.SelectFilePlease,
                InitialDirectory = @".",
                DefaultExtension = ".xlsx",
            };
            var window = Application.Current.Windows.OfType<Window>().SingleOrDefault(w => w.IsActive);
            if (dialog.ShowDialog(window) != CommonFileDialogResult.Ok) {
                return;
            } else {
                SelectedFileName = dialog.FileName;
                Task.Run(() => {
                    ImportExportUtil.ImportPromptItemsFromExcel(SelectedFileName);
                    ReloadCommand.Execute();
                });
            }
        });
    }
}
