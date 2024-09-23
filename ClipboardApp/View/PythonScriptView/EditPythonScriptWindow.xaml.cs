using System.Windows;
using ClipboardApp.Model;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.PythonScriptView {
    /// <summary>
    /// NewScriptWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditPythonScriptWindow : Window {
        public EditPythonScriptWindow() {
            InitializeComponent();
        }


        public static void OpenEditPythonScriptWindow(ScriptItem scriptItem) {
            EditPythonScriptWindow editScriptWindow = new() {
                DataContext = new EditPythonScriptWindowViewModel(scriptItem)
            };
            editScriptWindow.ShowDialog();
        }
    }


}
