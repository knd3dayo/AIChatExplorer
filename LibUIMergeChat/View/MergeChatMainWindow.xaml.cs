using System.Collections.ObjectModel;
using System.Windows;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.ViewModel.Folder;
using LibUIMergeChat.ViewModel;

namespace LibUIMergeChat.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MergeChatMainWindow : Window {
        public MergeChatMainWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems) {
            LibUIMergeChat.View.MergeChatMainWindow openAIChatWindow = new();
            MergeChatWindowViewModel mainWindowViewModel = new(folderViewModel, selectedItems);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}