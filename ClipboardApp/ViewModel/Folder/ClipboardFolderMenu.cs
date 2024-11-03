using System.Collections.ObjectModel;
using System.Windows.Controls;
using PythonAILib.Model.Prompt;
using PythonAILib.Resource;

namespace ClipboardApp.ViewModel.ClipboardItemView {
    public class ClipboardFolderMenu : ClipboardAppViewModelBase {

        public ClipboardFolderViewModel ClipboardFolderViewModel { get; private set; }

        public ClipboardFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) {
            ClipboardFolderViewModel = clipboardFolderViewModel;
        }

        // -- virtual
        public ObservableCollection<MenuItem> MenuItems {
            get {
                #region 全フォルダ共通
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                // 新規作成
                MenuItem createMenuItem = new() {
                    Header = StringResources.Create,
                    Command = ClipboardFolderViewModel.CreateFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(createMenuItem);

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
                deleteMenuItem.CommandParameter = this;
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

                return menuItems;

                #endregion
            }
        }
    }
}
