using System.Windows;
using QAChat.Resource;
using ClipboardApp.View.Main;
using ClipboardApp.Settings;

namespace ClipboardApp
{
    /// <summary>
    /// StartupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StartupWindow : Window {
        public StartupWindow() {
            InitializeComponent();
            // 言語設定
            // 文字列リソースの言語設定
            CommonStringResources.Lang = ClipboardAppConfig.Instance.ActualLang;

            // DataContextにViewModelを設定
            MainWindowViewModel mainWindowViewModel = new();
            // MainWindowを表示
            MainWindow mainWindow = new() {
                DataContext = mainWindowViewModel
            };
            mainWindowViewModel.Init();

            mainWindow.Show();

            // このウィンドウを閉じる
            this.Close();
        }
    }
}
