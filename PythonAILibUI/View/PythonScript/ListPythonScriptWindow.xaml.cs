
using System.Windows;
using PythonAILib.Model.Script;
using QAChat.ViewModel.Script;

namespace QAChat.View.PythonScript {
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
