using System.Collections.ObjectModel;
using System.Windows;
using LibUIMain.ViewModel.Item;
using LibUIMain.ViewModel.Folder;
using LibUIMergeChat.ViewModel;

namespace LibUIMergeChat.View {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MergeChatWindow : Window {
        public MergeChatWindow() {
            InitializeComponent();
        }

        public static void OpenWindow(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems) {
            LibUIMergeChat.View.MergeChatWindow openAIChatWindow = new();
            MergeChatWindowViewModel mainWindowViewModel = new(folderViewModel, selectedItems);
            openAIChatWindow.DataContext = mainWindowViewModel;

            openAIChatWindow.Show();
        }
    }
}