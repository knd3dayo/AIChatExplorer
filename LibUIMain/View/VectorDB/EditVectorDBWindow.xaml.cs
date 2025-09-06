using System.Windows;
using LibUIMain.ViewModel.VectorDB;

namespace LibUIMain.View.VectorDB
{
    /// <summary>
    /// EditItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditVectorDBWindow : Window {
        public EditVectorDBWindow() {
            InitializeComponent();
        }

        public static void OpenEditVectorDBWindow(VectorDBItemViewModel itemViewModel, Action<VectorDBItemViewModel> callback) {
            EditVectorDBWindow editVectorDBWindow = new() {
                DataContext = new EditVectorDBWindowViewModel(itemViewModel, callback)
            };
            editVectorDBWindow.ShowDialog();
        }
    }
}
