using System.ComponentModel;
using System.Windows;
using WpfApp1.Model;
using WpfApp1.View.SearchView;



namespace WpfApp1
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }


        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            // StatusTextのスレッドを停止
            MainWindowViewModel.StatusText.Dispose();
            // TODO Pythonのスレッドを停止
            PythonExecutor.Dispose();
        }
    }


}