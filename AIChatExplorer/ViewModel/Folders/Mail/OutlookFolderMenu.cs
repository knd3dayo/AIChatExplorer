using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Folders.Application;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using AIChatExplorer.Model.Folders.Outlook;
using LibUIPythonAI.ViewModel.Common;

namespace AIChatExplorer.ViewModel.Folders.Mail {
    public class OutlookFolderMenu(ApplicationFolderViewModel applicationFolderViewModel) : ApplicationFolderMenu(applicationFolderViewModel) {

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems {
            get {
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
            }
        }
        // 同期
        public MenuItem SyncMenuItem {
            get {
                MenuItem syncMenuItem = new() {
                    Header = CommonStringResources.Instance.Sync,
                    Command = SyncItemCommand,
                    CommandParameter = ApplicationFolderViewModel
                };
                return syncMenuItem;
            }
        }
        public static SimpleDelegateCommand<OutlookFolderViewModel> SyncItemCommand => new(async (folderViewModel) => {
            try {
                OutlookFolder folder = (OutlookFolder)folderViewModel.Folder;
                CommonViewModelProperties.Instance.UpdateIndeterminate(true);
                await Task.Run(() => {
                    folder.SyncItems();
                });
            } finally {
                CommonViewModelProperties.Instance.UpdateIndeterminate(false);
            }
            folderViewModel.LoadItems();

        });

    }
}
