using System.Windows;
using LibPythonAI.Model.Chat;
using LibUIPythonAI.ViewModel.Chat;

namespace LibUIPythonAI.View.Chat {
    /// <summary>
    /// EditChatItem.xaml の相互作用ロジック
    /// </summary>
    public partial class EditChatItemWindow : Window {
        public EditChatItemWindow() {
            InitializeComponent();
        }

        public static void OpenEditChatItemWindow(ChatMessage chatItem) {
            var window = new EditChatItemWindow();
            window.DataContext = new EditChatItemWindowViewModel(chatItem);
            window.Show();
        }

    }

}
