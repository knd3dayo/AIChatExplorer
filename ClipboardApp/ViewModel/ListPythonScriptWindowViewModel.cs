using System.Collections.ObjectModel;
using System.Windows;
using ClipboardApp.Factory;
using ClipboardApp.Model.Script;
using ClipboardApp.View.PythonScriptView;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel
{
    public class ListPythonScriptWindowViewModel : ClipboardAppViewModelBase {

        public enum ActionModeEnum {
            Edit,
            Select,
            Exec
        }
        private ActionModeEnum ActionMode { get; set; } = ActionModeEnum.Edit;

        public ObservableCollection<ScriptItem> ScriptItems { get; } = ScriptItem.ScriptItems;

        private ScriptItem? _selectedScriptItem;

        public ScriptItem? SelectedScriptItem {
            get {
                return _selectedScriptItem;
            }
            set {
                _selectedScriptItem = value;
                OnPropertyChanged(nameof(ScriptItem));
            }
        }

        private Action<ScriptItem> afterSelect = (scriptItem) => { };

        // ExecモードまたはSelectモード時は、実行ボタンを表示する。
        public Visibility ExecButtonVisibility {
            get {
                return ActionMode == ActionModeEnum.Exec || ActionMode == ActionModeEnum.Select ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        // Selectボタンのテキスト Selectモード時は「選択」、Execモード時は「実行」
        public string SelectButtonText {
            get {
                return ActionMode == ActionModeEnum.Exec ? StringResources.Execute : StringResources.Select;
            }
        }


        public ListPythonScriptWindowViewModel(ActionModeEnum actionModeEnum, Action<ScriptItem> afterSelect) {
            ActionMode = actionModeEnum;
            this.afterSelect = afterSelect;

            ScriptItems.Clear();
            foreach (var item in ClipboardAppFactory.Instance.GetClipboardDBController().GetScriptItems()) {
                ScriptItems.Add(item);
            }
            OnPropertyChanged(nameof(ScriptItems));
            OnPropertyChanged(nameof(ExecButtonVisibility));
        }


        // Scriptを新規作成するときの処理
        public static SimpleDelegateCommand<object> CreateScriptItemCommand => new(PythonCommands.CreateScriptCommandExecute);

        // Scriptを編集するときの処理
        public SimpleDelegateCommand<object> EditScriptItemCommand => new((parameter) => {
            if (SelectedScriptItem == null) {
                LogWrapper.Error(StringResources.SelectScript);
                return;
            }
            PythonCommands.EditScriptItemCommandExecute(SelectedScriptItem);
        });
        // Scriptを削除したときの処理
        public SimpleDelegateCommand<object> DeleteScriptCommand => new((parameter) => {
            if (SelectedScriptItem == null) {
                LogWrapper.Error(StringResources.SelectScript);
                return;
            }
            ScriptItem.DeleteScriptItem(SelectedScriptItem);
            ScriptItems.Remove(SelectedScriptItem);
            OnPropertyChanged(nameof(ScriptItems));
        });

        // 選択ボタンを押したときの処理
        public SimpleDelegateCommand<object> SelectScriptItemCommand => new((parameter) => {
            if (_selectedScriptItem == null) {
                LogWrapper.Error(StringResources.SelectScript);
                return;
            }
            // Actionを実行
            afterSelect(_selectedScriptItem);
            // ウィンドウを閉じる
            if (parameter is ListPythonScriptWindow selectScriptWindow) {
                selectScriptWindow.Close();

            }
        });

    }
}
