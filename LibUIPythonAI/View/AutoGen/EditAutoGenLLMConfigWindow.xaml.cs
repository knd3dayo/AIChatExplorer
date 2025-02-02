using System.Windows;
using PythonAILib.Model.AutoGen;
using LibUIPythonAI.ViewModel.AutoGen;

namespace LibUIPythonAI.View.AutoGen {
    /// <summary>
    /// EditAutoGenLLMConfigWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoGenLLMConfigWindow : Window {
        public EditAutoGenLLMConfigWindow() {
            InitializeComponent();
        }
        public static void OpenWindow(AutoGenLLMConfig autoGenLLMConfig, Action afterUpdate) {
            var window = new EditAutoGenAgentWindow();
            window.DataContext = new EditAutoGenLLMConfigViewModel(autoGenLLMConfig, afterUpdate);
            window.ShowDialog();
        }
    }
}
