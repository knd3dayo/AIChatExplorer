using System.Windows;
using LibPythonAI.Model.Chat;
using LibUIMain.ViewModel.Chat;

namespace LibUIMain.View.Chat {
    /// <summary>
    /// EditChatItem.xaml の相互作用ロジック
    /// </summary>
    public partial class EditChatItemWindow : Window {
        public EditChatItemWindow() {
            InitializeComponent();
        }

        public static void OpenEditChatItemWindow(ChatMessage chatItem) {
            var window = new EditChatItemWindow {
                DataContext = new EditChatItemWindowViewModel(chatItem)
            };
            window.Show();
        }

    }

}
