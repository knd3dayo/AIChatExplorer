using System.Windows;
using System.Windows.Controls;
using LibUIPythonAI.Resource;

namespace ClipboardApp.Settings {
    /// <summary>
    /// TestResultUserControl.xaml の相互作用ロジック
    /// </summary>
    public partial class TestResultUserControl : UserControl {
        public TestResultUserControl() {
            InitializeComponent();
        }
        public static void OpenTestResultWindow(string resultString) {
            TestResultUserControl testResultWindow = new();
            testResultWindow.DataContext = new TestResultUserControlViewModel(resultString);

            Window window = new() {
                Title = CommonStringResources.Instance.SettingCheckResultWindowTitle,
                Content = testResultWindow
            };
            window.ShowDialog();
        }
    }
}
