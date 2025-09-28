using System.Windows;

namespace AIChatExplorer.AppStartup {
    /// <summary>
    /// StartupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StartupWindow : Window {
        public StartupWindow() {
            InitializeComponent();
            this.Loaded += StartupWindow_Loaded;
        }

        private async void StartupWindow_Loaded(object sender, RoutedEventArgs e) {
            await StartupWindowViewModel.StartupAsync();
            this.Close();
        }
    }
}
