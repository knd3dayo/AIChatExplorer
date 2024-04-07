using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiteDB;
using WpfApp1.Model;
using WpfApp1.Utils;

namespace WpfApp1.View.ScriptView
{
    public class SelectScriptWindowViewModel : ObservableObject
    {
        public static ObservableCollection<ScriptItem> ScriptItems { get; } = PythonExecutor.ScriptItems;



        // Scriptを削除したときの処理
        public static SimpleDelegateCommand DeleteScriptCommand => new SimpleDelegateCommand(DeleteScriptCommandExecute);

        public static void DeleteScriptCommandExecute(object obj)
        {
            if (obj is ScriptItem)
            {
                ScriptItem scriptItemm = (ScriptItem)obj;
                PythonExecutor.DeleteScriptItem(scriptItemm);
                ScriptItems.Remove(scriptItemm);
            }
        }

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
            SelectScriptWindow.Current?.Close();

        }
        // キャンセルボタンを押したときの処理
        public SimpleDelegateCommand CloseCommand => new SimpleDelegateCommand(CloseCommandExecute);
        private void CloseCommandExecute(object obj)
        {
            // ウィンドウを閉じる
            SelectScriptWindow.Current?.Close();
        }


    }
}
