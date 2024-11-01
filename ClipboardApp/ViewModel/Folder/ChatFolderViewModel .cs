using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.View.ClipboardItemFolderView;
using QAChat.Control;
using ClipboardApp.ViewModel.ClipboardItemView;

namespace ClipboardApp.ViewModel.Folder {
    public class ChatFolderViewModel(ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(clipboardItemFolder) {

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            var chatFolderViewModel = new ChatFolderViewModel(childFolder);
            // チャットフォルダの親フォルダにこのフォルダを追加
            chatFolderViewModel.ParentFolderViewModel = this;
            return chatFolderViewModel;
        }

        public override ObservableCollection<MenuItem> MenuItems {
            get {
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                // 新規作成
                MenuItem createMenuItem = new() {
                    Header = StringResources.Create,
                    Command = CreateFolderCommand,
                    CommandParameter = this
                };
                menuItems.Add(createMenuItem);

                // 編集
                MenuItem editMenuItem = new() {
                    Header = StringResources.Edit,
                    Command = EditFolderCommand,
                    IsEnabled = IsEditVisible,
                    CommandParameter = this
                };
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
            ClipboardItemViewModel clipboardItemViewModel = new(this, clipboardItem);
            OpenItemCommandExecute(clipboardItemViewModel);
        }
        public override void OpenItemCommandExecute(ClipboardItemViewModel itemViewModel) {
            QAChatStartupProps props = new(itemViewModel.ClipboardItem);
            QAChat.View.QAChatMain.QAChatMainWindow.OpenOpenAIChatWindow(props);
        }

        public override void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成する
            // 自身が画像チェックの場合は、画像チェックを作成
            ClipboardFolder childFolder = ClipboardItemFolder.CreateChild("");
            childFolder.FolderType = ClipboardFolder.FolderTypeEnum.ImageCheck;
            ImageCheckFolderViewModel childFolderViewModel = new(childFolder);
            // TODO チャット履歴作成画面を開くようにする。フォルダ名とRAGソースのリストを選択可能にする。
            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }
        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public override void EditFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            FolderEditWindow.OpenFolderEditWindow(folderViewModel, afterUpdate);
        }

    }
}

