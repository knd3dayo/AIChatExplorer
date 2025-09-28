using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Folders.Application;
using AIChatExplorer.ViewModel.Folders.Search;

namespace AIChatExplorer.ViewModel.Folders.Chat {
    public class SearchFolderMenu : ApplicationFolderMenu {

        public SearchFolderViewModel FolderViewModel { get; private set; }

        public SearchFolderMenu(SearchFolderViewModel folderViewModel) : base(folderViewModel) {
            FolderViewModel = folderViewModel;
        }

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems {
            get {
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];

                // 編集
                menuItems.Add(EditMenuItem);

                //テキストの抽出
                menuItems.Add(ExtractTextMenuItem);

                // ベクトルのリフレッシュ
                menuItems.Add(RefreshMenuItem);

                // エクスポート/インポート
                menuItems.Add(ExportImportMenuItem);

                return menuItems;
            }
        }

    }
}
