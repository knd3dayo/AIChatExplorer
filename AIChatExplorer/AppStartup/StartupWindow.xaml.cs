using System.Windows;

namespace AIChatExplorer.AppStartup {
    /// <summary>
    /// StartupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StartupWindow : Window {
        public StartupWindow() {
            InitializeComponent();

            StartupWindowViewModel.Startup();

            this.Close();
        }
    }
}
