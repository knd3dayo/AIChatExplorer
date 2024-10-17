using System.Windows;

namespace ClipboardApp {
    /// <summary>
    /// StartupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StartupWindow : Window {
        public StartupWindow() {
            InitializeComponent();

            // MainWindowを表示
            MainWindow mainWindow = new MainWindow();
            // DataContextにViewModelを設定
            mainWindow.DataContext = new MainWindowViewModel();
            mainWindow.Show();

            // このウィンドウを閉じる
            this.Close();
        }
    }
}
