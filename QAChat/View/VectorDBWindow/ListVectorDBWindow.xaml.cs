using System.Windows;
using PythonAILib.Model.Abstract;
using QAChat.ViewModel.VectorDBWindow;

namespace QAChat.View.VectorDBWindow {
    /// <summary>
    /// RagManagementWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListVectorDBWindow : Window {
        public ListVectorDBWindow() {
            InitializeComponent();
        }
        public static void OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum mode, Action<VectorDBItemBase> callBackup) {
            ListVectorDBWindow listVectorDBWindow = new();
            ListVectorDBWindowViewModel listVectorDBWindowViewModel = new(mode, callBackup);
            listVectorDBWindow.DataContext = listVectorDBWindowViewModel;
            listVectorDBWindow.ShowDialog();
        }
    }
}
