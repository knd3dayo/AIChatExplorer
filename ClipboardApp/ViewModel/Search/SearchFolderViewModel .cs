using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.View.SearchView;
using ClipboardApp.ViewModel.Folder;

namespace ClipboardApp.ViewModel.Search {
    public class SearchFolderViewModel(ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(clipboardItemFolder) {

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            var searchFolderViewModel = new SearchFolderViewModel(childFolder);
            // 検索フォルダの親フォルダにこのフォルダを追加
            searchFolderViewModel.ParentFolderViewModel = this;
            return searchFolderViewModel;
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


        public override void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成
            ClipboardFolder clipboardFolder = ClipboardItemFolder.CreateChild("新規フォルダ");

            // 検索フォルダの親フォルダにこのフォルダを追加

            SearchFolderViewModel searchFolderViewModel = new(clipboardFolder);
            SearchRule? searchConditionRule = new() {
                Type = SearchRule.SearchType.SearchFolder,
                SearchFolder = clipboardFolder
            };

            SearchWindow.OpenSearchWindow(searchConditionRule, clipboardFolder, true, () => {
                // 保存と再読み込み
                searchFolderViewModel.SaveFolderCommand.Execute(null);
                // 親フォルダを保存
                folderViewModel.SaveFolderCommand.Execute(null);
                folderViewModel.LoadFolderCommand.Execute(null);

            });

        }

        public override void EditFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {

            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(ClipboardItemFolder);
            searchConditionRule ??= new() {
                Type = SearchRule.SearchType.SearchFolder,
                SearchFolder = ClipboardItemFolder
            };
            SearchWindow.OpenSearchWindow(searchConditionRule, this.ClipboardItemFolder, true, afterUpdate);

        }

        public override void PasteClipboardItemCommandExecute(MainWindowViewModel.CutFlagEnum CutFlag, IEnumerable<object> items, ClipboardFolderViewModel toFolder) {
            // 検索フォルダには貼り付け不可

        }
        public override void MergeItemCommandExecute(ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems) {
            // 検索フォルダにはマージ不可
        }


        public override void CreateItemCommandExecute() {
            // 検査フォルダにアイテム追加不可
        }
    }
}

