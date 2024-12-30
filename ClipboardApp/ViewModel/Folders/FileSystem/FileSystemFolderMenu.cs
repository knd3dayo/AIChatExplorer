using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;
using PythonAILib.Model.Prompt;
using PythonAILib.Resource;

namespace ClipboardApp.ViewModel.Folders.FileSystem
{
    public class FileSystemFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) : ClipboardFolderMenu(clipboardFolderViewModel)
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

                // 同期
                MenuItem createSyncMenuItem = new()
                {
                    Header = StringResources.Sync,
                    Command = FileSystemFolderViewModel.SyncItemCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(createSyncMenuItem);

                // ショートカット登録
                MenuItem createShortCutMenuItem = new()
                {
                    Header = StringResources.CreateShortCut,
                    Command = FileSystemFolderViewModel.CreateShortCutCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(createShortCutMenuItem);


                // エクスポート/インポート
                MenuItem exportImportMenuItem = new()
                {
                    Header = StringResources.ExportImport,
                    Command = QAChat.ViewModel.Folder.ContentFolderViewModel.ExportImportFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(exportImportMenuItem);


                // アイテムのバックアップ/リストア
                MenuItem backupRestoreMenuItem = new()
                {
                    Header = StringResources.BackupRestore
                };

                // バックアップ
                MenuItem backupMenuItem = new()
                {
                    Header = StringResources.BackupItem,
                    Command = ClipboardFolderViewModel.BackupItemsFromFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                backupRestoreMenuItem.Items.Add(backupMenuItem);


                // リストア
                MenuItem restoreMenuItem = new()
                {
                    Header = StringResources.RestoreItem,
                    Command = ClipboardFolderViewModel.RestoreItemsToFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                backupRestoreMenuItem.Items.Add(restoreMenuItem);

                menuItems.Add(backupRestoreMenuItem);

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
