using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.View.SearchView;
using ClipboardApp.Model;
using ClipboardApp.Model.Search;
using ClipboardApp.Model.Folder;

namespace ClipboardApp.ViewModel
{
    public class SearchFolderViewModel(MainWindowViewModel mainWindowViewModel, ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(mainWindowViewModel, clipboardItemFolder) {
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

        // LoadChildren
        public override void LoadChildren() {
            Children.Clear();
            foreach (var child in ClipboardItemFolder.Children) {
                if (child == null) {
                    continue;
                }
                Children.Add(new SearchFolderViewModel(MainWindowViewModel, child));
            }

        }
        // LoadItems
        public override void LoadItems() {
            Items.Clear();
            foreach (ClipboardItem item in ClipboardItemFolder.Items) {
                Items.Add(new ClipboardItemViewModel(this, item));
            }
        }


        public override void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成
            ClipboardFolder clipboardFolder = ClipboardItemFolder.CreateChild("新規フォルダ");

            // 検索フォルダの親フォルダにこのフォルダを追加

            SearchFolderViewModel searchFolderViewModel = new(MainWindowViewModel, clipboardFolder);
            SearchRule? searchConditionRule = new() {
                Type = SearchRule.SearchType.SearchFolder,
                SearchFolder = clipboardFolder
            };

            SearchWindow.OpenSearchWindow(searchConditionRule, searchFolderViewModel, true, () => {
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
            SearchWindow.OpenSearchWindow(searchConditionRule, this, true, afterUpdate);

        }

        public override void PasteClipboardItemCommandExecute(bool CutFlag, IEnumerable<ClipboardItemViewModel> items, ClipboardFolderViewModel fromFolder, ClipboardFolderViewModel toFolder) {
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

