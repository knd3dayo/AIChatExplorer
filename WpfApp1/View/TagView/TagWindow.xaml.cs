using System.Windows;

namespace WpfApp1.View.TagView
{
    /// <summary>
    /// TagWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TagWindow : Window
    {
        public static TagWindow? Current;

        public TagWindow()
        {
            InitializeComponent();
            Current = this;
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
