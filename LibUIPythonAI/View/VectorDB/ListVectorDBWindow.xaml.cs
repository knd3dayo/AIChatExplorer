using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.VectorDB;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.VectorDB;

namespace LibUIPythonAI.View.VectorDB
{
    /// <summary>
    /// RagManagementWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListVectorDBWindow : Window {
        public ListVectorDBWindow() {
            InitializeComponent();
        }
        public static void OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum mode, ObservableCollection<ContentFolderViewModel> rootFolderViewModels, Action<VectorDBProperty> callBackup) {
            ListVectorDBWindow listVectorDBWindow = new();
            ListVectorDBWindowViewModel listVectorDBWindowViewModel = new(mode, rootFolderViewModels, callBackup);
            listVectorDBWindow.DataContext = listVectorDBWindowViewModel;
            listVectorDBWindow.ShowDialog();
        }
    }
}
