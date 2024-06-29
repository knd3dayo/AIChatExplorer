using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.SearchView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemFolderView {
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
        public override ObservableCollection<MenuItem> ItemContextMenuItems {
            get {
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                if (MainWindowViewModel.ActiveInstance == null) {
                    return menuItems;
                }
                // 開く
                MenuItem createMenuItem = new();
                createMenuItem.Header = "開く";
                createMenuItem.Command = MainWindowViewModel.ActiveInstance.OpenSelectedItemCommand;
                createMenuItem.CommandParameter = this;
                createMenuItem.InputGestureText = "Ctrl+O";
                menuItems.Add(createMenuItem);

                // ファイルとして開く
                MenuItem openContentAsFileMenuItem = new();
                openContentAsFileMenuItem.Header = "ファイルとして開く";
                openContentAsFileMenuItem.Command = MainWindowViewModel.ActiveInstance.OpenContentAsFileCommand;
                openContentAsFileMenuItem.CommandParameter = this;
                openContentAsFileMenuItem.InputGestureText = "Ctrl+Shit+O";

                // ピン留め
                MenuItem pinnedStateChangeMenuItem = new();
                pinnedStateChangeMenuItem.Header = "ピン留め";
                pinnedStateChangeMenuItem.Command = MainWindowViewModel.ActiveInstance.ChangePinCommand;
                pinnedStateChangeMenuItem.CommandParameter = this;
                menuItems.Add(pinnedStateChangeMenuItem);

                // コピー
                MenuItem copyMenuItem = new();
                copyMenuItem.Header = "コピー";
                copyMenuItem.Command = MainWindowViewModel.ActiveInstance.CopyItemCommand;
                copyMenuItem.CommandParameter = this;
                copyMenuItem.InputGestureText = "Ctrl+C";
                menuItems.Add(copyMenuItem);

                // 削除
                MenuItem deleteMnuItem = new();
                deleteMnuItem.Header = "削除";
                deleteMnuItem.Command = MainWindowViewModel.ActiveInstance.DeleteSelectedItemCommand;
                deleteMnuItem.CommandParameter = this;
                deleteMnuItem.InputGestureText = "Delete";
                menuItems.Add(deleteMnuItem);

                return menuItems;

            }
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

        // アイテム作成コマンドの実装. 画像チェックフォルダの場合は、画像チェックー画面を開く
        public override void CreateItemCommandExecute() {
            ClipboardItem clipboardItem = new(this.ClipboardItemFolder.Id);
            ImageChat.MainWindow.OpenMainWindow(clipboardItem, false);
        }
        public override void OpenItemCommandExecute(ClipboardItemViewModel itemViewModel) {
            // 画像チェック画面を開く
            ImageChat.MainWindow.OpenMainWindow(itemViewModel.ClipboardItem, false);
        }

        public override void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            // 自身が画像チェックフォルダの場合は、画像チェックフォルダを作成
            ClipboardFolder childFolder = ClipboardItemFolder.CreateChild("");
            childFolder.FolderType = ClipboardFolder.FolderTypeEnum.ImageCheck;
            ImageCheckFolderViewModel childFolderViewModel = new(MainWindowViewModel, childFolder);

            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }

        public override void PasteClipboardItemCommandExecute(bool CutFlag, IEnumerable<ClipboardItemViewModel> items, ClipboardFolderViewModel fromFolder, ClipboardFolderViewModel toFolder) {
            // 検索フォルダには貼り付け不可

        }
        public override void MergeItemCommandExecute(ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems, bool mergeWithHeader) {
            // 検索フォルダにはマージ不可
        }

    }
}

