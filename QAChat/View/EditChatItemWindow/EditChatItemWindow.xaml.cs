using System.Windows;
using PythonAILib.Model;
using QAChat.ViewModel;

namespace QAChat.View.EditChatItemWindow {
    /// <summary>
    /// EditChatItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditChatItemWindow : Window {
        public EditChatItemWindow() {
            InitializeComponent();
        }

        public static void OpenEditChatItemWindow(ChatIHistorytem chatItem) {
            var window = new EditChatItemWindow();
            EditChatItemWindowViewModel model = (EditChatItemWindowViewModel)window.DataContext;
            model.Initialize(chatItem);
            window.Show();
        }

    }

}
