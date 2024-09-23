using System.Windows;
using PythonAILib.Model.Chat;
using QAChat.ViewModel;
using QAChat.ViewModel.QAChatMain;

namespace QAChat.View.EditChatItemWindow
{
    /// <summary>
    /// EditChatItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditChatItemWindow : Window {
        public EditChatItemWindow() {
            InitializeComponent();
        }

        public static void OpenEditChatItemWindow(ChatHistoryItem chatItem) {
            var window = new EditChatItemWindow();
            window.DataContext = new EditChatItemWindowViewModel(chatItem);
            window.Show();
        }

    }

}
