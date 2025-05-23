using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Folders.Application;
using LibUIPythonAI.Resource;

namespace AIChatExplorer.ViewModel.Folders.Browser {
    public class EdgeBrowseHistoryFolderMenu(ApplicationFolderViewModel clipboardFolderViewModel) : ApplicationFolderMenu(clipboardFolderViewModel) {

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
                    Command = ApplicationFolderViewModel.Commands.DownloadWebPageCommand,
                };
                return downloadWebPageCommandMenuItem;
            }
        }

    }
}
