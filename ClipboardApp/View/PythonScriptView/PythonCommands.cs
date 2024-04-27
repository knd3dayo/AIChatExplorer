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
                Tools.Error("スクリプトを選択してください");
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
            SelectScriptWindowViewModel.InitializeEdit();
            SelectScriptWindow.ShowDialog();
        }
        // スクリプト一覧画面を実行モードで開くコマンド
        public static void OpenListPythonScriptWindowExecCommandExecute(ClipboardItemViewModel? itemViewModel) {
            if (itemViewModel == null) {
                Tools.Error("スクリプトを実行するアイテムを選択してください");
                return;
            }
            ListPythonScriptWindow SelectScriptWindow = new ListPythonScriptWindow();
            ListPythonScriptWindowViewModel SelectScriptWindowViewModel = (ListPythonScriptWindowViewModel)SelectScriptWindow.DataContext;
            SelectScriptWindowViewModel.InitializeExec(itemViewModel);
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
