using System.Windows;
using PythonAILib.Model.AutoGen;
using QAChat.ViewModel.AutoGen;

namespace QAChat.View.AutoGen {
    /// <summary>
    /// EditAutoGenNormalWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoGenNormalChatWindow : Window {
        public EditAutoGenNormalChatWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(AutoGenNormalChat autoGenNormalChat, Action afterUpdate) {
            var window = new EditAutoGenNormalChatWindow();
            window.DataContext = new EditAutoGenNormalChatViewModel(autoGenNormalChat, afterUpdate);
            window.ShowDialog();
        }
    }
}
