using System.Windows;
using LibPythonAI.Model.AutoGen;
using LibUIPythonAI.ViewModel.AutoGen;

namespace LibUIPythonAI.View.AutoGen {
    /// <summary>
    /// EditAutoGenToolWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoGenToolWindow : Window {
        public EditAutoGenToolWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(AutoGenTool autoGenTool, Action afterUpdate) {
            var window = new EditAutoGenToolWindow();
            window.DataContext = new EditAutoGenToolViewModel(autoGenTool, afterUpdate);
            window.ShowDialog();
        }
    }
}
