using System.Windows;
using QAChat.ViewModel.Folder;

namespace QAChat.View.Folder {
    /// <summary>
    /// ExportImportWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ExportImportWindow : Window {
        public ExportImportWindow() {
            InitializeComponent();
        }

        public static void OpenExportImportFolderWindow(ContentFolderViewModel clibpboardFolderViewModel, Action afterUpdate) {

            ExportImportWindow exportImportWindow = new() {
                DataContext = new ExportImportWindowViewModel(clibpboardFolderViewModel, afterUpdate)
            };
            exportImportWindow.ShowDialog();
        }
    }
}
