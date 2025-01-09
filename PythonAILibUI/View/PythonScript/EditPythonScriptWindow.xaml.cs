using System.Windows;
using PythonAILib.Model.Script;
using QAChat.ViewModel.Script;

namespace QAChat.View.PythonScript {
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
