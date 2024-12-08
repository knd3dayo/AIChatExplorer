using System.Windows;
using PythonAILib.Model.VectorDB;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.VectorDB;

namespace QAChat.View.VectorDB
{
    /// <summary>
    /// RagManagementWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListVectorDBWindow : Window {
        public ListVectorDBWindow() {
            InitializeComponent();
        }
        public static void OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum mode, ContentFolderViewModel folderViewModel, Action<VectorDBItem> callBackup) {
            ListVectorDBWindow listVectorDBWindow = new();
            ListVectorDBWindowViewModel listVectorDBWindowViewModel = new(mode, folderViewModel, callBackup);
            listVectorDBWindow.DataContext = listVectorDBWindowViewModel;
            listVectorDBWindow.ShowDialog();
        }
    }
}
