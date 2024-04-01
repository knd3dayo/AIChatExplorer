using System.Windows;

namespace WpfApp1
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchWindow : Window
    {
        public static SearchWindow? Current;

        public SearchWindow()
        {
            InitializeComponent();
            Current = this;
        }

        public void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
