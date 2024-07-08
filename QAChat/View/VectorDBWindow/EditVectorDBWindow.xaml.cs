using System.Windows;
using QAChat.ViewModel;

namespace QAChat.View.VectorDBWindow
{
    /// <summary>
    /// EditItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditVectorDBWindow : Window {
        public EditVectorDBWindow() {
            InitializeComponent();
        }

        public static void OpenEditVectorDBWindow(VectorDBItemViewModel itemViewModel, Action<VectorDBItemViewModel> callback) {
            EditVectorDBWindow editVectorDBWindow = new();
            EditVectorDBWindowViewModel editVectorDBWindowViewModel = (EditVectorDBWindowViewModel)editVectorDBWindow.DataContext;
            editVectorDBWindowViewModel.Initialize(itemViewModel, callback);
            editVectorDBWindow.ShowDialog();
        }
    }
}
