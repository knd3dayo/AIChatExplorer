using System.Windows;
using PythonAILib.Model.AutoGen;
using QAChat.ViewModel.AutoGen;

namespace QAChat.View.AutoGen {
    /// <summary>
    /// EditAutoGenNestedWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoGenNestedChatWindow : Window {
        public EditAutoGenNestedChatWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(AutoGenNestedChat autoGenNestedChat, Action afterUpdate) {
            var window = new EditAutoGenNestedChatWindow();
            window.DataContext = new EditAutoGenNestedChatViewModel(autoGenNestedChat, afterUpdate);
            window.ShowDialog();
        }
    }
}
