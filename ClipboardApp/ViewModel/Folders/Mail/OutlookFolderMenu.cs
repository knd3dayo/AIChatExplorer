using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;
using LibUIPythonAI.Resource;

namespace ClipboardApp.ViewModel.Folders.Mail {
    public class OutlookFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) : ClipboardFolderMenu(clipboardFolderViewModel) {

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems {
            get {
                #region 全フォルダ共通
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];

                // 編集
                menuItems.Add(EditMenuItem);

                // 同期
                menuItems.Add(SyncMenuItem);

                // ベクトルのリフレッシュ
                menuItems.Add(RefreshMenuItem);
                // エクスポート/インポート
                menuItems.Add(ExportImportMenuItem);

                return menuItems;

                #endregion
            }
        }
        // 同期
        public MenuItem SyncMenuItem {
            get {
                MenuItem syncMenuItem = new() {
                    Header = CommonStringResources.Instance.Sync,
                    Command = OutlookFolderViewModel.SyncItemCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                return syncMenuItem;
            }
        }
    }
}
