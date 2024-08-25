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
using QAChat.ViewModel;

namespace QAChat.View.RAGWindow
{
    /// <summary>
    /// RagManagementWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectCommitWindow : Window {
        public SelectCommitWindow() {
            InitializeComponent();
        }

        public static void OpenSelectCommitWindow(RAGSourceItemViewModel itemViewModel, Action<string> callback) {
            SelectCommitWindow selectCommitWindow = new();
            SelectCommitWindowViewModel selectCommitWindowViewModel = new (itemViewModel, callback);
            selectCommitWindow.DataContext = selectCommitWindowViewModel;
            selectCommitWindow.ShowDialog();
        }
    }
}
