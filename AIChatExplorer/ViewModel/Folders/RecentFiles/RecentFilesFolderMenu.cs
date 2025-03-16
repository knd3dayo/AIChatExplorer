using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Folders.Clipboard;
using AIChatExplorer.ViewModel.Folders.FileSystem;
using AIChatExplorer.ViewModel.Main;
using LibGit2Sharp;
using LibUIPythonAI.Resource;
using PythonAILibUI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Folders.Browser {
    public class RecentFilesFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) : FileSystemFolderMenu(clipboardFolderViewModel) {

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
