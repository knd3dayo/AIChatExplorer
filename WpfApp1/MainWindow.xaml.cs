using System.ComponentModel;
using System.Windows;



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
        private void Close_Click(object sneder, RoutedEventArgs e)
        {
            Console.WriteLine("メニュー操作：閉じる");
            System.Windows.Application.Current.Shutdown();
        }
        private void Search_Click(object sneder, RoutedEventArgs e)
        {
            Console.WriteLine("メニュー操作：検索");
            SearchWindow  searchWindow = new SearchWindow();
            ((SearchWindowViewModel)searchWindow.DataContext).ClipboardItemFolder
                = MainWindowViewModel.Instance?.SelectedFolder;

            searchWindow.ShowDialog();
        }
        private void Setting_Click(object sneder, RoutedEventArgs e)
        {
            Console.WriteLine("メニュー操作：設定");
            SettingWindow settingWindow = new SettingWindow();
            settingWindow.ShowDialog();
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