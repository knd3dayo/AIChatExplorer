using QAChat.Model;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.PythonScriptView.PythonScriptView {
    public class PythonCommands {


        public static void CreatePythonScriptCommandExecute(object obj) {
            EditPythonScriptWindow editScriptWindow = new EditPythonScriptWindow();
            EditPythonScriptWindowViewModel editScriptWindowViewModel = (EditPythonScriptWindowViewModel)editScriptWindow.DataContext;
            editScriptWindowViewModel.ScriptItem = new ScriptItem("", PythonExecutor.LoadPythonScript(PythonExecutor.TemplateScript), ScriptType.Python);
            editScriptWindow.ShowDialog();
        }

        public static void EditPythonScriptCommandExecute(object obj) {
            SelectPythonScriptWindow SelectScriptWindow = new SelectPythonScriptWindow();
            SelectScriptWindow.ShowDialog();
        }

        //--------------------------------------------------------------------------------
        // Pythonスクリプトを実行するコマンド
        //--------------------------------------------------------------------------------
        public static void DeleteScriptCommandExecute(object obj) {
            if (obj is ScriptItem) {
                ScriptItem scriptItem = (ScriptItem)obj;
                ScriptItem.DeleteScriptItem(scriptItem);
            }
        }
 

    }
}
