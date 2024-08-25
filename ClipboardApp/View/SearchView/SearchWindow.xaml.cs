using System.Windows;
using ClipboardApp.ViewModel;
using WpfAppCommon.Model.ClipboardApp;

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
            ClipboardFolderViewModel? searchFolderViewModel, bool isSearchFolder, Action afterUpdate) {
            SearchWindow searchWindow = new();
            SearchWindowViewModel searchWindowViewModel = (SearchWindowViewModel)searchWindow.DataContext;
            searchWindowViewModel.Initialize(searchConditionRule, searchFolderViewModel, isSearchFolder, afterUpdate);
            searchWindow.ShowDialog();
        }

    }
}
