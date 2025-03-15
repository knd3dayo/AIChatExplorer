using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Folders.FileSystem;
using ClipboardApp.ViewModel.Main;
using LibGit2Sharp;
using LibUIPythonAI.Resource;
using PythonAILibUI.ViewModel.Item;

namespace ClipboardApp.ViewModel.Folders.Browser {
    public class EdgeBrowseHistoryFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) : ClipboardFolderMenu(clipboardFolderViewModel) {

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems {
            get {
                #region 全フォルダ共通
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];

                // 編集
                menuItems.Add(EditMenuItem);

                // Webぺージをダウンロード
                menuItems.Add(DownloadWebPageCommandMenuItem);
                // ベクトルのリフレッシュ
                menuItems.Add(RefreshMenuItem);

                // エクスポート/インポート
                menuItems.Add(ExportImportMenuItem);

                return menuItems;

                #endregion
            }

        }

        // DownloadWebPageCommandMenuItem

        public MenuItem DownloadWebPageCommandMenuItem {
            get {
                MenuItem downloadWebPageCommandMenuItem = new() {
                    Header = CommonStringResources.Instance.DownloadWebPage,
                    Command = ClipboardFolderViewModel.Commands.DownloadWebPageCommand,
                };
                return downloadWebPageCommandMenuItem;
            }
        }

    }
}
