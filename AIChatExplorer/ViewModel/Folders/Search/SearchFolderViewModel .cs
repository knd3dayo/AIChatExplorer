using AIChatExplorer.Model.Folders.ClipboardHistory;
using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.ViewModel.Folders.Application;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Search;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Search;
using LibUIPythonAI.ViewModel.Common;
using LibUIPythonAI.ViewModel.Folder;

namespace AIChatExplorer.ViewModel.Folders.Search {
    public class SearchFolderViewModel(SearchFolder applicationItemFolder, CommonViewModelCommandExecutes commands) : ApplicationFolderViewModel(applicationItemFolder, commands) {

        // 子フォルダのApplicationFolderViewModelを作成するメソッド
        public override ApplicationFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            if (childFolder is not SearchFolder) {
                throw new ArgumentException("Child folder must be a SearchFolder.");
            }
            var searchFolderViewModel = new SearchFolderViewModel((SearchFolder)childFolder, commands) {
                // 検索フォルダの親フォルダにこのフォルダを追加
                ParentFolderViewModel = this
            };
            return searchFolderViewModel;
        }

        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 子フォルダを作成
            SearchFolder clipboardFolder = (SearchFolder)Folder.CreateChild(now);

            // 検索フォルダの親フォルダにこのフォルダを追加

            SearchFolderViewModel searchFolderViewModel = new(clipboardFolder, commands);
            SearchRule? searchConditionRule = new() {
                SearchFolderId = clipboardFolder.Id
            };

            SearchWindow.OpenSearchWindow(searchConditionRule, clipboardFolder, () => {
                // 保存と再読み込み
                searchFolderViewModel.FolderCommands.SaveFolderCommand.Execute(null);
                // 親フォルダを保存
                folderViewModel.FolderCommands.SaveFolderCommand.Execute(null);
                folderViewModel.FolderCommands.LoadFolderCommand.Execute(null);

            });

        }
        // LoadLLMConfigListAsync
        public override async Task LoadItemsAsync() {
            await Task.Run(async () => {
                if (Folder is not SearchFolder searchFolder) {
                    throw new InvalidOperationException("Folder is not a SearchFolder.");
                }
                var items = await searchFolder.GetItems();
                List<ContentItemWrapper> _items = items.OrderByDescending(x => x.UpdatedAt).ToList();
                MainUITask.Run(() => {
                    Items.Clear();
                    foreach (ContentItemWrapper item in _items) {
                        Items.Add(CreateItemViewModel(item));
                    }
                    OnPropertyChanged(nameof(Items));
                });
            });
        }

        // LoadChildren
        public override async Task LoadChildren(int nestLevel) {
            await LoadChildren<SearchFolderViewModel, SearchFolder>(nestLevel);
        }
        public override void EditFolderCommandExecute(Action afterUpdate) {
            if (Folder is not SearchFolder searchFolder) {
                return;
            }
            Task.Run(async () => {
                // SearchRuleを取得
                SearchRule? searchConditionRule = await SearchRule.GetItemBySearchFolder(searchFolder);
                searchConditionRule ??= new() {
                    SearchFolderId = searchFolder.Id
                };
                MainUITask.Run(() => {
                    // 検索ウィンドウを開く
                    SearchWindow.OpenSearchWindow(searchConditionRule, searchFolder, afterUpdate);
                });
            });

        }

        public override async Task PasteApplicationItemCommandExecute(ClipboardController.CutFlagEnum CutFlag, IEnumerable<object> items, ApplicationFolderViewModel toFolder) {
            // 検索フォルダには貼り付け不可
            await Task.CompletedTask;

        }

        public override void CreateItemCommandExecute() {
            // 検査フォルダにアイテム追加不可
        }
    }
}

