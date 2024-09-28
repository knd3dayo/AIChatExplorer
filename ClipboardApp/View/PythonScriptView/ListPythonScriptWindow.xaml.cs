
using System.Windows;
using ClipboardApp.Model.Script;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.PythonScriptView
{
    /// <summary>
    /// SelectPythonScriptWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListPythonScriptWindow : Window {
        public ListPythonScriptWindow() {
            InitializeComponent();
        }

        public static void OpenListPythonScriptWindow(ListPythonScriptWindowViewModel.ActionModeEnum mode, Action<ScriptItem> callback) {
            ListPythonScriptWindow listPythonScriptWindow = new();
            ListPythonScriptWindowViewModel listPythonScriptWindowViewModel = new(mode, callback);
            listPythonScriptWindow.DataContext = listPythonScriptWindowViewModel;
            listPythonScriptWindow.ShowDialog();
        }
    }
}
