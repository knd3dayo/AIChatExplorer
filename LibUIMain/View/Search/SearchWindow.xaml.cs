using System.Windows;
using LibUIMain.ViewModel.Search;
using LibMain.Model.Search;
using LibMain.Model.Content;

namespace LibUIMain.View.Search {
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class SearchWindow : Window {
        public SearchWindow() {
            InitializeComponent();
        }

        public static void OpenSearchWindow(SearchRule searchConditionRule,
            ContentFolderWrapper searchFolder,  Action afterUpdate) {
            SearchWindow searchWindow = new() {
                DataContext = new SearchWindowViewModel(searchConditionRule, searchFolder, afterUpdate)
            };
            searchWindow.ShowDialog();
        }

    }
}
