using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using ClipboardApp.Model;
using ClipboardApp.PythonIF;
using ClipboardApp.Utils;

namespace ClipboardApp.View.PythonScriptView
{
    public class SelectPythonScriptWindowViewModel : ObservableObject
    {
        public static ObservableCollection<ScriptItem> ScriptItems { get; } = PythonExecutor.ScriptItems;

        // Scriptを削除したときの処理
        public static SimpleDelegateCommand DeleteScriptCommandExecute => new ((parameter) => {
            if (parameter is ScriptItem scriptItem) {
                PythonExecutor.DeleteScriptItem(scriptItem);
                ScriptItems.Remove(scriptItem);
            }
        });

        // OKボタンを押したときの処理
        public static void SelectScriptCommandExecute(object obj)
        {
            if (obj is ScriptItem)
            {
                ScriptItem scriptItem = (ScriptItem)obj;
                // EditScriptWindowを開く
                EditPythonScriptWindow editScriptWindow = new EditPythonScriptWindow();
                // EditScriptWindowのViewModelを取得
                EditPythonScriptWindowViewModel editScriptWindowViewModel = (EditPythonScriptWindowViewModel)editScriptWindow.DataContext;
                editScriptWindowViewModel.ScriptItem = scriptItem;

                editScriptWindow.ShowDialog();
            }

            // ウィンドウを閉じる
            if (obj is SelectPythonScriptWindow selectScriptWindow) {
                selectScriptWindow.Close();
            }

        }
        // キャンセルボタンを押したときの処理
        public SimpleDelegateCommand CloseCommand => new ((parameter) => {
            // ウィンドウを閉じる
            if (parameter is SelectPythonScriptWindow selectScriptWindow) {
                selectScriptWindow.Close();
            }

        });

    }
}
