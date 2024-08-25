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
using PythonAILib.Model;
using QAChat.ViewModel;
using WpfAppCommon.Model;

namespace QAChat.View.VectorDBWindow
{
    /// <summary>
    /// RagManagementWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListVectorDBWindow : Window {
        public ListVectorDBWindow() {
            InitializeComponent();
        }
        public static void OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum mode, Action<VectorDBItemBase> callBackup) {
            ListVectorDBWindow listVectorDBWindow = new();
            ListVectorDBWindowViewModel listVectorDBWindowViewModel = (ListVectorDBWindowViewModel)listVectorDBWindow.DataContext;
            listVectorDBWindowViewModel.Initialize(mode, callBackup);
            listVectorDBWindow.ShowDialog();
        }
    }
}
