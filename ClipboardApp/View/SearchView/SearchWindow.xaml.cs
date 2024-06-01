using System.Windows;
using ClipboardApp.View.ClipboardItemFolderView;
using WpfAppCommon.Model;

namespace ClipboardApp.View.SearchView
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchWindow : Window
    {
        public SearchWindow()
        {
            InitializeComponent();
        }

        public static void OpenSearchWindow(SearchRule searchConditionRule,
            ClipboardFolderViewModel? searchFolderViewModel,
            Action afterUpdate) {
            SearchWindow searchWindow = new();
            SearchWindowViewModel searchWindowViewModel = (SearchWindowViewModel)searchWindow.DataContext;
            searchWindowViewModel.Initialize(searchConditionRule, searchFolderViewModel, afterUpdate);
            searchWindow.ShowDialog();
        }

    }
}
