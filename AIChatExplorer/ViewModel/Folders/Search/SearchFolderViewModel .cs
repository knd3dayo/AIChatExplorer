using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.Model.Main;
using AIChatExplorer.ViewModel.Folders.Application;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Search;
using LibUIPythonAI.View.Search;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Folders.Search {
    public class SearchFolderViewModel(ContentFolderWrapper applicationItemFolder, ContentItemViewModelCommands commands) : ApplicationFolderViewModel(applicationItemFolder, commands) {

        // 子フォルダのApplicationFolderViewModelを作成するメソッド
        public override ApplicationFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            var searchFolderViewModel = new SearchFolderViewModel(childFolder, Commands) {
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

            SearchFolderViewModel searchFolderViewModel = new(clipboardFolder, Commands);
            SearchRule? searchConditionRule = new(new LibPythonAI.Data.SearchRuleEntity()) {
                SearchFolder = clipboardFolder
            };

            SearchWindow.OpenSearchWindow(searchConditionRule, clipboardFolder, () => {
                // 保存と再読み込み
                searchFolderViewModel.SaveFolderCommand.Execute(null);
                // 親フォルダを保存
                folderViewModel.SaveFolderCommand.Execute(null);
                folderViewModel.LoadFolderCommand.Execute(null);

            });

        }
        // LoadLLMConfigListAsync
        public override void LoadItems() {
            LoadItems<ContentItemWrapper>();
        }

        // LoadChildren
        public override void LoadChildren(int nestLevel) {
            LoadChildren<SearchFolderViewModel, SearchFolder>(nestLevel);
        }
        public override void EditFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            if (Folder is not SearchFolder searchFolder) {
                return;
            }

            SearchRule? searchConditionRule = SearchRule.GetItemBySearchFolder(Folder);
            searchConditionRule ??= new(new LibPythonAI.Data.SearchRuleEntity()) {
                SearchFolder = Folder
            };
            SearchWindow.OpenSearchWindow(searchConditionRule, searchFolder, afterUpdate);

        }

        public override void PasteApplicationItemCommandExecute(ClipboardController.CutFlagEnum CutFlag, IEnumerable<object> items, ApplicationFolderViewModel toFolder) {
            // 検索フォルダには貼り付け不可

        }

        public override void CreateItemCommandExecute() {
            // 検査フォルダにアイテム追加不可
        }
    }
}

