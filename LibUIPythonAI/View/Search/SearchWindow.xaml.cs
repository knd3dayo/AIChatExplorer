using System.Windows;
using LibUIPythonAI.ViewModel.Search;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.Content;

namespace LibUIPythonAI.View.Search {
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
