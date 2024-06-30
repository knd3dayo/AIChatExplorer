using System.Windows;
using ClipboardApp.View.ClipboardItemFolderView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.SearchView {
    public partial class SearchWindowViewModel  {

        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------

        public SimpleDelegateCommand<object> ClearCommand => new((parameter) => {
            SearchConditionRule?.SearchCondition?.Clear();
        });

        public SimpleDelegateCommand<Window> CancelCommand => new((window) => {

            window.Close();
        });

        public SimpleDelegateCommand<Window> ApplyCommand => new((window) => {
            if (SearchConditionRule == null) {
                LogWrapper.Error("検索条件がNullです");
                return;
            }
            if (_isSearchFolder && SearchFolderViewModel != null) {
                // SearchConditionRuleのSearchFolderにSearchFolderViewModelのClipboardItemFolderを設定
                SearchConditionRule.SearchFolder = SearchFolderViewModel.ClipboardItemFolder;
                // _isSearchFolderがTrueの場合は、フォルダ名を更新
                SearchFolderViewModel.FolderName = SearchConditionRule.Name;
            }
            // 検索条件をLiteDBに保存
            SearchConditionRule.Save();


            // 検索条件を適用後に実行する処理
            _afterUpdate?.Invoke();

            // Close the window
            window.Close();
        });

        // OpenSelectSearchFolderWindowCommand
        // 検索フォルダを選択する
        public SimpleDelegateCommand<object> OpenSelectSearchFolderWindowCommand => new((parameter) => {
            if (SearchConditionRule == null) {
                LogWrapper.Error("検索条件がNullです");
                return;
            }
            if (MainWindowViewModel.ActiveInstance == null) {
                LogWrapper.Error("MainWindowViewModelがNullです");
                return;
            }


            ClipboardFolderViewModel? rootFolderViewModel = new(MainWindowViewModel.ActiveInstance, ClipboardFolder.SearchRootFolder);
            FolderSelectWindow.OpenFolderSelectWindow(rootFolderViewModel, (folderViewModel) => {
                SearchFolderViewModel = folderViewModel;
                SearchConditionRule.SearchFolder = folderViewModel.ClipboardItemFolder;
                SearchFolderPath = folderViewModel.FolderPath;
                OnPropertyChanged(nameof(SearchFolderPath));
            });

        });

        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand<object> OpenSelectTargetFolderWindowCommand => new((parameter) => {
            if (MainWindowViewModel.ActiveInstance == null) {
                LogWrapper.Error("MainWindowViewModelがNullです");
                return;
            }
            if (SearchConditionRule == null) {
                LogWrapper.Error("検索条件がNullです");
                return;
            }

            ClipboardFolderViewModel? rootFolderViewModel = new(MainWindowViewModel.ActiveInstance, ClipboardFolder.RootFolder);
            FolderSelectWindow.OpenFolderSelectWindow(rootFolderViewModel, (folderViewModel) => {
                SearchConditionRule.TargetFolder = folderViewModel.ClipboardItemFolder;
                TargetFolderPath = folderViewModel.FolderPath;
                OnPropertyChanged(nameof(TargetFolderPath));
            });

        });

    }
}
