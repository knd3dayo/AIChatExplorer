using System.Windows;
using PythonAILib.Model.AutoGen;
using QAChat.ViewModel.AutoGen;

namespace QAChat.View.AutoGen {
    /// <summary>
    /// EditAutoGenAgentWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoGenAgentWindow : Window {
        public EditAutoGenAgentWindow() {
            InitializeComponent();
        }
        public static void OpenWindow(AutoGenAgent autoGenAgent, Action afterUpdate) {
            var window = new EditAutoGenAgentWindow();
            window.DataContext = new EditAutoGenAgentViewModel(autoGenAgent, afterUpdate);
            window.ShowDialog();
        }
    }

}
