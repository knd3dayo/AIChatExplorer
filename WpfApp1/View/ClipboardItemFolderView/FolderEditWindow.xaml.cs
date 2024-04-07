using System.Windows;

namespace WpfApp1.View.ClipboardItemFolderView
{
    /// <summary>
    /// FolderEditWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class FolderEditWindow : Window
    {
        public static FolderEditWindow? Current;
        public FolderEditWindow()
        {
            InitializeComponent();
            Current = this;
        }
    }
}
