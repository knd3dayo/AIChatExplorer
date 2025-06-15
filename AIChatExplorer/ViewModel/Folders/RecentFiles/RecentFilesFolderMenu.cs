using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Folders.Application;
using AIChatExplorer.ViewModel.Folders.FileSystem;

namespace AIChatExplorer.ViewModel.Folders.Browser {
    public class RecentFilesFolderMenu(ApplicationFolderViewModel clipboardFolderViewModel) : FileSystemFolderMenu(clipboardFolderViewModel) {

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems {
            get {
                #region 全フォルダ共通
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];

                // 編集
                menuItems.Add(EditMenuItem);

                // テキストを抽出
                menuItems.Add(ExtractTextMenuItem);
                // ベクトルのリフレッシュ
                menuItems.Add(RefreshMenuItem);

                // エクスポート/インポート
                menuItems.Add(ExportImportMenuItem);

                return menuItems;

                #endregion
            }
        }
    }
}
