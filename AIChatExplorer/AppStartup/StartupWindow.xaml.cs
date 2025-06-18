using System.Windows;

namespace AIChatExplorer.AppStartup {
    /// <summary>
    /// StartupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StartupWindow : Window {
        public StartupWindow() {
            InitializeComponent();

            _ = Startup();
        }

        private async Task Startup() {
            await StartupWindowViewModel.Startup();
            this.Close();
        }
    }
}
