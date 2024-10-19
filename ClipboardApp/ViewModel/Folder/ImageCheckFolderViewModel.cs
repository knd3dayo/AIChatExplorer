using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.View.ClipboardItemFolderView;
using QAChat.View.ImageChat;

namespace ClipboardApp.ViewModel.Folder {
    public class ImageCheckFolderViewModel(MainWindowViewModel mainWindowViewModel, ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(mainWindowViewModel, clipboardItemFolder) {

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            return new ImageCheckFolderViewModel(MainWindowViewModel, childFolder);
        }

        public override ObservableCollection<MenuItem> MenuItems {
            get {
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                // 新規作成
                MenuItem createMenuItem = new();
                createMenuItem.Header = StringResources.Create;
                createMenuItem.Command = CreateFolderCommand;
                createMenuItem.CommandParameter = this;
                menuItems.Add(createMenuItem);

                // 編集
                MenuItem editMenuItem = new();
                editMenuItem.Header = StringResources.Edit;
                editMenuItem.Command = EditFolderCommand;
                editMenuItem.IsEnabled = IsEditVisible;
                editMenuItem.CommandParameter = this;
                menuItems.Add(editMenuItem);

                // 削除
                MenuItem deleteMenuItem = new();
                deleteMenuItem.Header = StringResources.Delete;
                deleteMenuItem.Command = DeleteFolderCommand;
                deleteMenuItem.IsEnabled = IsDeleteVisible;
                deleteMenuItem.CommandParameter = this;
                menuItems.Add(deleteMenuItem);

                // アイテムのバックアップ/リストア
                MenuItem backupRestoreMenuItem = new();
                backupRestoreMenuItem.Header = StringResources.BackupRestore;

                // バックアップ
                MenuItem backupMenuItem = new() {
                    Header = StringResources.BackupItem,
                    Command = BackupItemsFromFolderCommand,
                    CommandParameter = this
                };
                backupRestoreMenuItem.Items.Add(backupMenuItem);

                menuItems.Add(backupRestoreMenuItem);

                return menuItems;

            }
        }

        // アイテム作成コマンドの実装. 画像チェックの場合は、画像チェックー画面を開く
        public override void CreateItemCommandExecute() {
            ClipboardItem clipboardItem = new(ClipboardItemFolder.Id);
            ImageChatMainWindow.OpenMainWindow(clipboardItem, () => {
                LoadFolderCommand.Execute();
            });
        }
        public override void OpenItemCommandExecute(ClipboardItemViewModel itemViewModel) {
            // 画像チェック画面を開く
            ImageChatMainWindow.OpenMainWindow(itemViewModel.ClipboardItem, () => {
                LoadFolderCommand.Execute();
            });
        }

        public override void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            // 自身が画像チェックの場合は、画像チェックを作成
            ClipboardFolder childFolder = ClipboardItemFolder.CreateChild("");
            childFolder.FolderType = ClipboardFolder.FolderTypeEnum.ImageCheck;
            ImageCheckFolderViewModel childFolderViewModel = new(MainWindowViewModel, childFolder);

            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }

    }
}

