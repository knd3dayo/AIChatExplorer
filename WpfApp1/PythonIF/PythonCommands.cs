using WpfApp1.Model;
using WpfApp1.View.ScriptView;

namespace WpfApp1.PythonIF
{
    public class PythonCommands
    {


        public static void CreatePythonScriptCommandExecute(object obj)
        {
            EditScriptWindow editScriptWindow = new EditScriptWindow();
            EditScriptWindowViewModel editScriptWindowViewModel = (EditScriptWindowViewModel)editScriptWindow.DataContext;
            editScriptWindowViewModel.ScriptItem = new ScriptItem("", PythonExecutor.LoadPythonScript(PythonExecutor.TemplateScript), ScriptType.Python);
            editScriptWindow.ShowDialog();
        }

        public static void EditPythonScriptCommandExecute(object obj)
        {
            SelectScriptWindow SelectScriptWindow = new SelectScriptWindow();
            SelectScriptWindow.ShowDialog();
        }

        //--------------------------------------------------------------------------------
        // Pythonスクリプトを実行するコマンド
        //--------------------------------------------------------------------------------
        public static void DeleteScriptCommandExecute(object obj)
        {
            if (obj is ScriptItem)
            {
                ScriptItem scriptItem = (ScriptItem)obj;
                PythonExecutor.DeleteScriptItem(scriptItem);
            }
        }

    }
}
