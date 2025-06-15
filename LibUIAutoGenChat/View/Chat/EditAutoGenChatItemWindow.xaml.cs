using System.Windows;
using LibPythonAI.Model.Chat;
using LibUIAutoGenChat.ViewModel.Chat;

namespace LibUIAutoGenChat.View.Chat {
    /// <summary>
    /// EditChatItem.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoGenChatItemWindow : Window {
        public EditAutoGenChatItemWindow() {
            InitializeComponent();
        }

        public static void OpenEditChatItemWindow(ChatMessage chatItem) {
            var window = new EditAutoGenChatItemWindow();
            window.DataContext = new EditAutoGenChatItemWindowViewModel(chatItem);
            window.Show();
        }

    }

}
