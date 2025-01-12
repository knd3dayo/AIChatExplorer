using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Folders.FileSystem;

namespace ClipboardApp.ViewModel.Folders.ShortCut
{
    public class ShortCutFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) : ClipboardFolderMenu(clipboardFolderViewModel)
    {

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems
        {
            get
            {
                #region 全フォルダ共通
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];

                // 編集
                MenuItem editMenuItem = new()
                {
                    Header = StringResources.Edit,
                    Command = ClipboardFolderViewModel.EditFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(editMenuItem);

                // 削除
                MenuItem deleteMenuItem = new();
                deleteMenuItem.Header = StringResources.Delete;
                deleteMenuItem.Command = ClipboardFolderViewModel.DeleteFolderCommand;
                deleteMenuItem.IsEnabled = ClipboardFolderViewModel.IsDeleteVisible;
                deleteMenuItem.CommandParameter = ClipboardFolderViewModel;
                menuItems.Add(deleteMenuItem);

                // 同期
                MenuItem createSyncMenuItem = new()
                {
                    Header = StringResources.Sync,
                    Command = FileSystemFolderViewModel.SyncItemCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(createSyncMenuItem);

                // エクスポート/インポート
                MenuItem exportImportMenuItem = new()
                {
                    Header = StringResources.ExportImport,
                    Command = QAChat.ViewModel.Folder.ContentFolderViewModel.ExportImportFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(exportImportMenuItem);

                // ベクトルのリフレッシュ
                MenuItem refreshMenuItem = new()
                {
                    Header = StringResources.RefreshVectorDB,
                    Command = ClipboardFolderViewModel.RefreshVectorDBCollectionCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(refreshMenuItem);

                return menuItems;

                #endregion
            }
        }
    }
}
