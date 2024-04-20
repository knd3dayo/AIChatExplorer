using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using ClipboardApp.Model;
using ClipboardApp.PythonIF;
using ClipboardApp.Utils;

namespace ClipboardApp.View.ScriptView
{
    public class SelectScriptWindowViewModel : ObservableObject
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
                EditScriptWindow editScriptWindow = new EditScriptWindow();
                // EditScriptWindowのViewModelを取得
                EditScriptWindowViewModel editScriptWindowViewModel = (EditScriptWindowViewModel)editScriptWindow.DataContext;
                editScriptWindowViewModel.ScriptItem = scriptItem;

                editScriptWindow.ShowDialog();
            }

            // ウィンドウを閉じる
            if (obj is SelectScriptWindow selectScriptWindow) {
                selectScriptWindow.Close();
            }

        }
        // キャンセルボタンを押したときの処理
        public SimpleDelegateCommand CloseCommand => new ((parameter) => {
            // ウィンドウを閉じる
            if (parameter is SelectScriptWindow selectScriptWindow) {
                selectScriptWindow.Close();
            }

        });

    }
}
