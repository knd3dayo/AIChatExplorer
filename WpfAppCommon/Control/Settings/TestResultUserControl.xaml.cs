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
using WpfAppCommon.Model;

namespace WpfAppCommon.Control.Settings {
    /// <summary>
    /// TestResultUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class TestResultUserControl : UserControl {
        public TestResultUserControl() {
            InitializeComponent();
        }
        public static void OpenTestResultWindow(string resultString) {
            TestResultUserControl testResultWindow = new();
            TestResultUserControlViewModel testResultWindowViewModel = (TestResultUserControlViewModel)testResultWindow.DataContext;
            testResultWindowViewModel.LogText = resultString;
            Window window = new() {
                Title = StringResources.Instance.SettingCheckResultWindowTitle,
                Content = testResultWindow
            };
            window.ShowDialog();
        }
    }
}
