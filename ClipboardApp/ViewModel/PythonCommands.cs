using ClipboardApp.Model.Script;
using ClipboardApp.View.PythonScriptView;
using PythonAILib.PythonIF;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel
{
    public class PythonCommands {
        public static void CreateScriptCommandExecute(object obj) {

            EditPythonScriptWindow.OpenEditPythonScriptWindow(new ScriptItem("", "", PythonExecutor.LoadPythonScript(PythonExecutor.TemplateScript), ScriptType.Python));
        }

        public static void EditScriptItemCommandExecute(object obj) {
            if (obj is not ScriptItem scriptItem) {
                LogWrapper.Error(CommonStringResources.Instance.SelectScript);
                return;
            }
            EditPythonScriptWindow.OpenEditPythonScriptWindow(scriptItem);
        }

        // スクリプト一覧画面を編集モードで開くコマンド
        public static void OpenListPythonScriptWindowCommandExecute(object obj) {
            ListPythonScriptWindow.OpenListPythonScriptWindow(ListPythonScriptWindowViewModel.ActionModeEnum.Edit, (scriptItem) => { });
        }

        // スクリプト一覧画面を実行モードで開くコマンド
        public static void OpenListPythonScriptWindowExecCommandExecute(Action<ScriptItem> action) {
            ListPythonScriptWindow.OpenListPythonScriptWindow(ListPythonScriptWindowViewModel.ActionModeEnum.Exec, action);
        }

        public static void DeleteScriptCommandExecute(object obj) {
            if (obj is ScriptItem) {
                ScriptItem scriptItem = (ScriptItem)obj;
                ScriptItem.DeleteScriptItem(scriptItem);
            }
        }


    }
}
