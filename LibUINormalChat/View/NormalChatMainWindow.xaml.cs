using System.Collections.ObjectModel;
using System.Windows;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.ViewModel.Folder;
using LibUINormalChat.ViewModel;
using LibUIPythonAI.ViewModel.Chat;

namespace LibUINormalChat.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class NormalChatWindow : Window {
        public NormalChatWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(QAChatStartupProps qAChatStartupProps) {
            LibUINormalChat.View.NormalChatWindow openAIChatWindow = new();
            NormalChatWindowViewModel mainWindowViewModel = new(qAChatStartupProps);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}