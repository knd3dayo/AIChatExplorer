using AIChatExplorer.Model.Folders.ClipboardHistory;
using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.ViewModel.Folders.Application;
using LibMain.Model.Content;
using LibMain.Model.Search;
using LibUIMain.Utils;
using LibUIMain.View.Search;
using LibUIMain.ViewModel.Common;
using LibUIMain.ViewModel.Folder;

namespace AIChatExplorer.ViewModel.Folders.Search {
    public class SearchFolderViewModel(SearchFolder applicationItemFolder, CommonViewModelCommandExecutes commands) : ApplicationFolderViewModel(applicationItemFolder, commands) {

        // 子フォルダの ApplicationFolderViewModel を作成するメソッド
        public override ApplicationFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder)
        {
            if (childFolder is not SearchFolder)
            {
                throw new ArgumentException($"CreateChildFolderViewModel: childFolder の型が SearchFolder ではありません。型: {childFolder?.GetType().Name}");
            }
            var searchFolderViewModel = new SearchFolderViewModel((SearchFolder)childFolder, commands)
            {
                // 検索フォルダの親フォルダにこのフォルダを追加
                ParentFolderViewModel = this
            };
            return searchFolderViewModel;
        }

        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate)
        {
            if (folderViewModel == null)
                throw new ArgumentNullException(nameof(folderViewModel), "CreateFolderCommandExecute: folderViewModel が null です。");
            if (folderViewModel.FolderCommands == null)
                throw new ArgumentNullException(nameof(folderViewModel.FolderCommands), "CreateFolderCommandExecute: FolderCommands が null です。");

            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 子フォルダを作成
            if (Folder == null)
                throw new InvalidOperationException("CreateFolderCommandExecute: Folder が null です。");

            if (Folder.CreateChild(now) is not SearchFolder clipboardFolder)
                throw new InvalidOperationException("CreateFolderCommandExecute: 子フォルダの作成に失敗しました。");

            SearchFolderViewModel searchFolderViewModel = new(clipboardFolder, commands);
            SearchRule searchConditionRule = new()
            {
                SearchFolderId = clipboardFolder.Id
            };

            // 検索ウィンドウを開く
            SearchWindow.OpenSearchWindow(searchConditionRule, clipboardFolder, () =>
            {
                // 保存と再読み込み
                searchFolderViewModel.FolderCommands?.SaveFolderCommand?.Execute(null);
                folderViewModel.FolderCommands.SaveFolderCommand.Execute(null);
                folderViewModel.FolderCommands.LoadFolderCommand.Execute(null);
                afterUpdate?.Invoke();
            });
        }
        // 検索フォルダ内のアイテムを読み込む
        public override async Task LoadItemsAsync()
        {
            if (Folder is not SearchFolder searchFolder)
            {
                throw new InvalidOperationException("LoadItemsAsync: Folder の型が SearchFolder ではありません。");
            }
            var items = await searchFolder.GetItems();
            List<ContentItem> sortedItems = items.OrderByDescending(x => x.UpdatedAt).ToList();
            MainUITask.Run(() =>
            {
                Items.Clear();
                foreach (ContentItem item in sortedItems)
                {
                    Items.Add(CreateItemViewModel(item));
                }
                OnPropertyChanged(nameof(Items));
            });
        }

        // 子フォルダの読み込み
        public override async Task LoadChildren(int nestLevel)
        {
            await LoadChildren<SearchFolderViewModel, SearchFolder>(nestLevel);
        }
        // 検索フォルダの編集コマンド
        public override void EditFolderCommandExecute(Action afterUpdate)
        {
            if (Folder is not SearchFolder searchFolder)
            {
                return;
            }
            _ = EditFolderAsync(searchFolder, afterUpdate);
        }

        private async Task EditFolderAsync(SearchFolder searchFolder, Action afterUpdate)
        {
            // SearchRuleを取得
            SearchRule? searchConditionRule = await SearchRule.GetItemBySearchFolder(searchFolder);
            searchConditionRule ??= new()
            {
                SearchFolderId = searchFolder.Id
            };
            MainUITask.Run(() =>
            {
                // 検索ウィンドウを開く
                SearchWindow.OpenSearchWindow(searchConditionRule, searchFolder, afterUpdate);
            });
        }

        // 検索フォルダには貼り付け不可
        public override async Task PasteApplicationItemCommandExecute(ClipboardController.CutFlagEnum CutFlag, IEnumerable<object> items, ApplicationFolderViewModel toFolder)
        {
            await Task.CompletedTask;
        }

        // 検索フォルダにはアイテム追加不可
        public override void CreateItemCommandExecute()
        {
            // 何もしない
        }
    }
}

