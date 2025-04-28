using System.Windows;
using LibUIAutoGenChat.ViewModel.Chat;
using PythonAILib.Model.Chat;

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
