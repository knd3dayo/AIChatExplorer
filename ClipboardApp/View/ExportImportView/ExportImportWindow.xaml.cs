using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.ExportImportView {
    /// <summary>
    /// ExportImportWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ExportImportWindow : Window {
        public ExportImportWindow() {
            InitializeComponent();
        }

        public static void OpenExportImportFolderWindow(ClipboardFolderViewModel clibpboardFolderViewModel) {

            ExportImportWindow exportImportWindow = new() {
                DataContext = new ExportImportWindowViewModel(clibpboardFolderViewModel)
            };
            exportImportWindow.ShowDialog();
        }
    }
}
