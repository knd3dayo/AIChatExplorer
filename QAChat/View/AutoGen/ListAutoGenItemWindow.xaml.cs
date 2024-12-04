using System.Windows;
using QAChat.ViewModel.AutoGen;
using QAChat.ViewModel.Folder;

namespace QAChat.View.AutoGen {
    /// <summary>
    /// ListAutoGenItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListAutoGenItemWindow : Window {
        public ListAutoGenItemWindow() {
            InitializeComponent();
        }

        public static void OpenListAutoGenItemWindow(ContentFolderViewModel rootFolderViewModel , bool selectGroupChatMode = false) {
            ListAutoGenItemWindow listAutoGenItemWindow = new();
            ListAutoGenItemWindowViewModel listAutoGenItemWindowViewModel = new(rootFolderViewModel, selectGroupChatMode);
            listAutoGenItemWindow.DataContext = listAutoGenItemWindowViewModel;
            listAutoGenItemWindow.Show();
        }
    }
}
