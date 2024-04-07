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
            // データベースのチェックポイント処理
            ClipboardDatabaseController.GetClipboardDatabase().Checkpoint();
            InitializeComponent();

        }

        private void Close_Click(object sneder, RoutedEventArgs e)
        {
            Console.WriteLine("メニュー操作：閉じる");
            Application.Current.Shutdown();
        }
        private void Search_Click(object sneder, RoutedEventArgs e)
        {
            Console.WriteLine("メニュー操作：検索");
            SearchWindow searchWindow = new SearchWindow();
            SearchWindowViewModel searchWindowViewModel = (SearchWindowViewModel)searchWindow.DataContext;
            searchWindowViewModel.Initialize(ClipboardItemFolder.GlobalSearchCondition, () =>
            {
                MainWindowViewModel.Instance?.SelectedFolder?.Load();
            });

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