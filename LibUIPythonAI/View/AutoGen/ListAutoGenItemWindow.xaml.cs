using System.Collections.ObjectModel;
using System.Windows;
using LibUIPythonAI.ViewModel.AutoGen;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.View.AutoGen {
    /// <summary>
    /// ListAutoGenItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListAutoGenItemWindow : Window {
        public ListAutoGenItemWindow() {
            InitializeComponent();
        }

        public static void OpenListAutoGenItemWindow(ObservableCollection<ContentFolderViewModel> rootFolderViewModel , bool selectGroupChatMode = false) {
            ListAutoGenItemWindow listAutoGenItemWindow = new();
            ListAutoGenItemWindowViewModel listAutoGenItemWindowViewModel = new(rootFolderViewModel, selectGroupChatMode);
            listAutoGenItemWindow.DataContext = listAutoGenItemWindowViewModel;
            listAutoGenItemWindow.Show();
        }
    }
}
