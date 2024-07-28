using System.Windows;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.TagView
{
    /// <summary>
    /// TagWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TagWindow : Window
    {
        public TagWindow()
        {
            InitializeComponent();
        }
        public static void OpenTagWindow(ClipboardItemViewModel? viewModel, Action action) {
            TagWindow tagWindow = new();
            TagWindowViewModel tagWindowViewModel = (TagWindowViewModel)tagWindow.DataContext;
            tagWindowViewModel.Initialize(viewModel, action);
            tagWindow.ShowDialog();
        }
    }
}
