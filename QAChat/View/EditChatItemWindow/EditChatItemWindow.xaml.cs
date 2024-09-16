using System.Windows;
using PythonAILib.Model.Chat;
using QAChat.ViewModel;

namespace QAChat.View.EditChatItemWindow
{
    /// <summary>
    /// EditChatItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditChatItemWindow : Window {
        public EditChatItemWindow() {
            InitializeComponent();
        }

        public static void OpenEditChatItemWindow(ChatIHistorytem chatItem) {
            var window = new EditChatItemWindow();
            window.DataContext = new EditChatItemWindowViewModel(chatItem);
            window.Show();
        }

    }

}
