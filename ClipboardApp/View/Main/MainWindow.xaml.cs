using System.ComponentModel;
using System.Windows;
using ClipboardApp.ViewModel.Main;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Utils;
using PythonAILib.PythonIF;



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

            App.Current.Shutdown();
        }
    }


}