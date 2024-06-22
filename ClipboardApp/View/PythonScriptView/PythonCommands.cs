using ClipboardApp.View.ClipboardItemView;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.PythonScriptView.PythonScriptView {
    public class PythonCommands {


        public static void CreateScriptCommandExecute(object obj) {
            EditPythonScriptWindow editScriptWindow = new EditPythonScriptWindow();
            EditPythonScriptWindowViewModel editScriptWindowViewModel = (EditPythonScriptWindowViewModel)editScriptWindow.DataContext;
            editScriptWindowViewModel.ScriptItem = new ScriptItem("", "", PythonExecutor.LoadPythonScript(PythonExecutor.TemplateScript), ScriptType.Python);
            editScriptWindow.ShowDialog();
        }

        public static void EditScriptItemCommandExecute(object obj) {
            if (obj is not ScriptItem scriptItem) {
                LogWrapper.Error("スクリプトを選択してください");
                return;
            }
            EditPythonScriptWindow editScriptWindow = new EditPythonScriptWindow();
            EditPythonScriptWindowViewModel editScriptWindowViewModel = (EditPythonScriptWindowViewModel)editScriptWindow.DataContext;
            editScriptWindowViewModel.ScriptItem = scriptItem;
            editScriptWindow.ShowDialog();
        }

        // スクリプト一覧画面を編集モードで開くコマンド
        public static void OpenListPythonScriptWindowCommandExecute(object obj) {
            ListPythonScriptWindow SelectScriptWindow = new ListPythonScriptWindow();
            ListPythonScriptWindowViewModel SelectScriptWindowViewModel = (ListPythonScriptWindowViewModel)SelectScriptWindow.DataContext;
            SelectScriptWindowViewModel.Initialize(ListPythonScriptWindowViewModel.ActionModeEnum.Edit, (scriptItem) => { });
            SelectScriptWindow.ShowDialog();
        }
        // スクリプト一覧画面を実行モードで開くコマンド
        public static void OpenListPythonScriptWindowExecCommandExecute(Action<ScriptItem> action) {
            ListPythonScriptWindow SelectScriptWindow = new ListPythonScriptWindow();
            ListPythonScriptWindowViewModel SelectScriptWindowViewModel = (ListPythonScriptWindowViewModel)SelectScriptWindow.DataContext;
            SelectScriptWindowViewModel.Initialize(ListPythonScriptWindowViewModel.ActionModeEnum.Exec, action);
            SelectScriptWindow.ShowDialog();
        }

        public static void DeleteScriptCommandExecute(object obj) {
            if (obj is ScriptItem) {
                ScriptItem scriptItem = (ScriptItem)obj;
                ScriptItem.DeleteScriptItem(scriptItem);
            }
        }
 

    }
}
