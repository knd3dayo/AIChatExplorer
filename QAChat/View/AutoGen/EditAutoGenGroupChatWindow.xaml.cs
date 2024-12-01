using System.Windows;
using PythonAILib.Model.AutoGen;
using QAChat.ViewModel.AutoGen;

namespace QAChat.View.AutoGen {
    /// <summary>
    /// EditAutoGenGroupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoGenGroupChatWindow : Window {
        public EditAutoGenGroupChatWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(AutoGenGroupChat autoGenGroupChat, Action afterUpdate) {
            var window = new EditAutoGenGroupChatWindow();
            window.DataContext = new EditAutoGenGroupChatViewModel(autoGenGroupChat, afterUpdate);
            window.ShowDialog();
        }
    }
}
