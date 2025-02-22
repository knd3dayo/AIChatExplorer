using ClipboardApp.Model.Folders.Search;
using ClipboardApp.Model.Main;
using ClipboardApp.ViewModel.Folders.Clipboard;
using LibUIPythonAI.View.Search;
using LibUIPythonAI.ViewModel.Folder;
using PythonAILib.Model.Content;
using PythonAILib.Model.Search;
using PythonAILibUI.ViewModel.Item;

namespace ClipboardApp.ViewModel.Folders.Search {
    public class SearchFolderViewModel(ContentFolderWrapper clipboardItemFolder, ContentItemViewModelCommands commands) : ClipboardFolderViewModel(clipboardItemFolder, commands) {

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            var searchFolderViewModel = new SearchFolderViewModel(childFolder, Commands);
            // 検索フォルダの親フォルダにこのフォルダを追加
            searchFolderViewModel.ParentFolderViewModel = this;
            return searchFolderViewModel;
        }

        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            // 子フォルダを作成
            SearchFolder clipboardFolder = (SearchFolder)Folder.CreateChild("New Folder");

            // 検索フォルダの親フォルダにこのフォルダを追加

            SearchFolderViewModel searchFolderViewModel = new(clipboardFolder, Commands);
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

        public override void EditFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate) {
            if (Folder is not SearchFolder searchFolder) {
                return;
            }

            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(Folder);
            searchConditionRule ??= new() {
                Type = SearchRule.SearchType.SearchFolder,
                SearchFolder = Folder
            };
            SearchWindow.OpenSearchWindow(searchConditionRule, searchFolder, true, afterUpdate);

        }

        public override void PasteClipboardItemCommandExecute(ClipboardController.CutFlagEnum CutFlag, IEnumerable<object> items, ClipboardFolderViewModel toFolder) {
            // 検索フォルダには貼り付け不可

        }

        public override void CreateItemCommandExecute() {
            // 検査フォルダにアイテム追加不可
        }
    }
}

