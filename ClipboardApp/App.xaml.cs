using System.Windows;
using System.Windows.Threading;
using Python.Runtime;

namespace ClipboardApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);

            DispatcherUnhandledException += OnUnhandledException;
        }

        private void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e) {
            Exception ex = e.Exception;
            string message = $"エラーが発生しました\nメッセージ：{ex.Message}\nスタックトレース:\n{ex.StackTrace}";
            System.Windows.MessageBox.Show(message);
        }
    }

}
