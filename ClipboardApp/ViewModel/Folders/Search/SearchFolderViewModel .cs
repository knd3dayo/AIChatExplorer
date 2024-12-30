using System.Collections.ObjectModel;
using ClipboardApp.Model.Folder;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Folders.Clipboard;
using PythonAILib.Model.Search;
using QAChat.View.Search;
using QAChat.ViewModel.Folder;

namespace ClipboardApp.ViewModel.Folders.Search
{
    public class SearchFolderViewModel(ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(clipboardItemFolder)
    {

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder)
        {
            var searchFolderViewModel = new SearchFolderViewModel(childFolder);
            // 検索フォルダの親フォルダにこのフォルダを追加
            searchFolderViewModel.ParentFolderViewModel = this;
            return searchFolderViewModel;
        }

        public override void CreateFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate)
        {
            // 子フォルダを作成
            ClipboardFolder clipboardFolder = (ClipboardFolder)Folder.CreateChild("新規フォルダ");

            // 検索フォルダの親フォルダにこのフォルダを追加

            SearchFolderViewModel searchFolderViewModel = new(clipboardFolder);
            SearchRule? searchConditionRule = new()
            {
                Type = SearchRule.SearchType.SearchFolder,
                SearchFolder = clipboardFolder
            };

            SearchWindow.OpenSearchWindow(searchConditionRule, clipboardFolder, true, () =>
            {
                // 保存と再読み込み
                searchFolderViewModel.SaveFolderCommand.Execute(null);
                // 親フォルダを保存
                folderViewModel.SaveFolderCommand.Execute(null);
                folderViewModel.LoadFolderCommand.Execute(null);

            });

        }

        public override void EditFolderCommandExecute(ContentFolderViewModel folderViewModel, Action afterUpdate)
        {
            if (Folder is not ClipboardFolder clipboardFolder)
            {
                return;
            }

            SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolder(Folder);
            searchConditionRule ??= new()
            {
                Type = SearchRule.SearchType.SearchFolder,
                SearchFolder = Folder
            };
            SearchWindow.OpenSearchWindow(searchConditionRule, clipboardFolder, true, afterUpdate);

        }

        public override void PasteClipboardItemCommandExecute(MainWindowViewModel.CutFlagEnum CutFlag, IEnumerable<object> items, ClipboardFolderViewModel toFolder)
        {
            // 検索フォルダには貼り付け不可

        }
        public override void MergeItemCommandExecute(ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems)
        {
            // 検索フォルダにはマージ不可
        }


        public override void CreateItemCommandExecute()
        {
            // 検査フォルダにアイテム追加不可
        }
    }
}

