using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Content;

namespace ClipboardApp.ViewModel.FileSystem {
    public class ShortCutFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) : ClipboardFolderMenu(clipboardFolderViewModel) {

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems {
            get {
                #region 全フォルダ共通
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];

                // 編集
                MenuItem editMenuItem = new() {
                    Header = StringResources.Edit,
                    Command = ClipboardFolderViewModel.EditFolderCommand,
                    IsEnabled = ClipboardFolderViewModel.IsEditVisible,
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


                // エクスポート/インポート
                MenuItem exportImportMenuItem = new() {
                    Header = StringResources.ExportImport,
                    Command = ClipboardFolderViewModel.ExportImportFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(exportImportMenuItem);


                // アイテムのバックアップ/リストア
                MenuItem backupRestoreMenuItem = new() {
                    Header = StringResources.BackupRestore
                };

                // バックアップ
                MenuItem backupMenuItem = new() {
                    Header = StringResources.BackupItem,
                    Command = ClipboardFolderViewModel.BackupItemsFromFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                backupRestoreMenuItem.Items.Add(backupMenuItem);


                // リストア
                MenuItem restoreMenuItem = new() {
                    Header = StringResources.RestoreItem,
                    Command = ClipboardFolderViewModel.RestoreItemsToFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                backupRestoreMenuItem.Items.Add(restoreMenuItem);

                menuItems.Add(backupRestoreMenuItem);

                // ベクトルのリフレッシュ
                MenuItem refreshMenuItem = new() {
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
