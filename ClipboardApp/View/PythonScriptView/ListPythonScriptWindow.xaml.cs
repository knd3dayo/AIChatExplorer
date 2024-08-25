
using System.Windows;
using ClipboardApp.ViewModel;
using WpfAppCommon.Model.ClipboardApp;

namespace ClipboardApp.View.PythonScriptView
{
    /// <summary>
    /// SelectPythonScriptWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListPythonScriptWindow : Window
    {
        public ListPythonScriptWindow()
        {
            InitializeComponent();
        }

        public static void OpenListPythonScriptWindow(ListPythonScriptWindowViewModel.ActionModeEnum mode, Action<ScriptItem> callback) {
            ListPythonScriptWindow listPythonScriptWindow = new();
            ListPythonScriptWindowViewModel listPythonScriptWindowViewModel = (ListPythonScriptWindowViewModel)listPythonScriptWindow.DataContext;
            listPythonScriptWindowViewModel.Initialize(mode, callback);
            listPythonScriptWindow.ShowDialog();
        }
    }
}
