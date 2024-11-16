using System.ComponentModel;
using System.Windows;
using QAChat.Utils;



namespace ClipboardApp.View.Main {

    /// <summary>
    /// Interaction logic for Main.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

        }


        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
        }
        // Closedイベント
        protected override void OnClosed(EventArgs e) {
            base.OnClosed(e);
            // AutoGenStudioプロセスを終了
            AutoGenProcessController.StopAutoGenStudio();

            App.Current.Shutdown();
        }
    }


}