using System.ComponentModel;
using System.Windows;
using ClipboardApp.PythonIF;



namespace ClipboardApp {

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();

        }


        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);

            // StatusTextのスレッドを停止
            MainWindowViewModel.StatusText.Dispose();
        }
    }


}