using System.Windows;
using PythonAILib.Model.VectorDB;
using QAChat.ViewModel.VectorDBWindow;

namespace QAChat.View.VectorDB
{
    /// <summary>
    /// RagManagementWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListVectorDBWindow : Window {
        public ListVectorDBWindow() {
            InitializeComponent();
        }
        public static void OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum mode, Action<VectorDBItem> callBackup) {
            ListVectorDBWindow listVectorDBWindow = new();
            ListVectorDBWindowViewModel listVectorDBWindowViewModel = new(mode, callBackup);
            listVectorDBWindow.DataContext = listVectorDBWindowViewModel;
            listVectorDBWindow.ShowDialog();
        }
    }
}
