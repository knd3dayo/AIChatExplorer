using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemFolderView;
using WpfAppCommon.Model;

namespace ClipboardApp.ViewModel {
    public class ImageCheckFolderViewModel(MainWindowViewModel mainWindowViewModel, ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(mainWindowViewModel, clipboardItemFolder) {
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

                // インポート    
                MenuItem importMenuItem = new();
                importMenuItem.Header = StringResources.Import;
                importMenuItem.Command = ImportItemsToFolderCommand;
                importMenuItem.CommandParameter = this;
                menuItems.Add(importMenuItem);

                // エクスポート
                MenuItem exportMenuItem = new();
                exportMenuItem.Header = StringResources.Export;
                exportMenuItem.Command = ExportItemsFromFolderCommand;
                exportMenuItem.CommandParameter = this;
                menuItems.Add(exportMenuItem);

                return menuItems;

            }
        }
        // Itemのコンテキストメニュー
        public override ObservableCollection<MenuItem> CreateItemContextMenuItems(ClipboardItemViewModel itemViewModel) {
            // MenuItemのリストを作成
            ObservableCollection<MenuItem> menuItems = [];
            if (MainWindowViewModel.ActiveInstance == null) {
                return menuItems;
            }
            // 開く
            MenuItem createMenuItem = new() {
                Header = "開く",
                Command = OpenItemCommand,
                CommandParameter = itemViewModel,
                InputGestureText = "Ctrl+O"
            };
            menuItems.Add(createMenuItem);

            // ピン留め
            MenuItem pinnedStateChangeMenuItem = new() {
                Header = "ピン留め",
                Command = itemViewModel.ChangePinCommand,
                CommandParameter = itemViewModel
            };
            menuItems.Add(pinnedStateChangeMenuItem);

            // コピー
            MenuItem copyMenuItem = new() {
                Header = "コピー",
                Command = MainWindowViewModel.ActiveInstance.CopyItemCommand,
                CommandParameter = this,
                InputGestureText = "Ctrl+C"
            };
            menuItems.Add(copyMenuItem);

            // 削除
            MenuItem deleteMnuItem = new() {
                Header = "削除",
                Command = itemViewModel.DeleteItemCommand,
                CommandParameter = itemViewModel,
                InputGestureText = "Delete"
            };
            menuItems.Add(deleteMnuItem);

            return menuItems;

        }

        // LoadChildren
        public override void LoadChildren() {
            Children.Clear();
            foreach (var child in ClipboardItemFolder.Children) {
                if (child == null) {
                    continue;
                }
                Children.Add(new ImageCheckFolderViewModel(MainWindowViewModel, child));
            }

        }
        // LoadItems
        public override void LoadItems() {
            Items.Clear();
            foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                Items.Add(new ClipboardItemViewModel(this, item));
            }
        }

        // アイテム作成コマンドの実装. 画像チェックの場合は、画像チェックー画面を開く
        public override void CreateItemCommandExecute() {
            ClipboardItem clipboardItem = new(ClipboardItemFolder.Id);
            ImageChat.MainWindow.OpenMainWindow(clipboardItem, false, () => {
                LoadFolderCommand.Execute();
            });
        }
        public override void OpenItemCommandExecute(ClipboardItemViewModel itemViewModel) {
            // 画像チェック画面を開く
            ImageChat.MainWindow.OpenMainWindow(itemViewModel.ClipboardItem, false, () => {
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

