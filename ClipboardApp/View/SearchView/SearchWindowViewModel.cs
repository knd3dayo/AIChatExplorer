using System.Reflection.Metadata;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.Factory.Default;

namespace ClipboardApp.View.SearchView
{
    public class SearchWindowViewModel : ObservableObject {

        private SearchRule? _searchConditionRule;
        public SearchRule? SearchConditionRule {
            get {
                return _searchConditionRule;
            }
            set {
                _searchConditionRule = value;
                OnPropertyChanged("SearchConditionRule");
            }
        }

        private ClipboardItemFolderViewModel? searchFolderViewModel;
        public ClipboardItemFolderViewModel? SearchFolderViewModel {
            get {
                return searchFolderViewModel;
            }
            set {
                searchFolderViewModel = value;
                OnPropertyChanged("SearchFolderViewModel");
            }
        }
        // 検索フォルダの場合は表示する、それ以外は非表示
        public Visibility SearchFolderVisibility {

            get {
                if (SearchConditionRule?.Type == SearchRule.SearchType.SearchFolder) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        // 検索タイプ 標準 or 検索フォルダ
        public string SearchTypeText {
            get {
                if (SearchConditionRule?.Type == SearchRule.SearchType.SearchFolder) {
                    return "検索フォルダ";
                }   
                return "標準";
            }
        }

        // 検索フォルダのパス
        private string? _searchFolderPath;
        public string? SearchFolderPath {
            get {
                return _searchFolderPath;
            }
            set {
                _searchFolderPath = value;
                OnPropertyChanged("SearchFolderPath");
            }
        }
        // 対象フォルダのパス
        private string? _targetFolderPath;
        public string? TargetFolderPath {
            get {
                return _targetFolderPath;
            }
            set {
                _targetFolderPath = value;
                OnPropertyChanged("TargetFolderPath");
            }
        }
        
        private Action? _afterUpdate;

        public void Initialize(SearchRule searchConditionRule, ClipboardItemFolderViewModel? searchFolderViewModel, Action afterUpdate) {
            this.SearchConditionRule = searchConditionRule;

            _afterUpdate = afterUpdate;
            SearchFolderViewModel = searchFolderViewModel;
            SearchFolderPath = searchFolderViewModel?.ClipboardItemFolder.AbsoluteCollectionName;
            TargetFolderPath = searchConditionRule.TargetFolder?.AbsoluteCollectionName;

            OnPropertyChanged("SearchTypeText");
            OnPropertyChanged("SearchFolderVisibility");

        }

        public void Initialize(SearchRule searchConditionRule, Action afterUpdate) {
            Initialize(searchConditionRule, null, afterUpdate);
        }
        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------

        public SimpleDelegateCommand ClearCommand => new SimpleDelegateCommand(ClearCommandExecute);
        private void ClearCommandExecute(object parameter) {
            SearchConditionRule?.SearchCondition?.Clear();
        }

        public SimpleDelegateCommand CancelCommand => new((parameter) => {
            if (parameter is Window window) {
                window.Close();
            }
        });

        public SimpleDelegateCommand ApplyCommand => new((parameter) => {
            if (SearchConditionRule == null) {
                Tools.Error("検索条件がNullです");
                return;
            }
            // 検索条件をLiteDBに保存
            SearchConditionRule.Save();

            // 検索条件を適用後に実行する処理
            _afterUpdate?.Invoke();

            // Close the window
            if (parameter is Window window) {
                window.Close();
            }
        });

        // OpenSelectSearchFolderWindowCommand
        // 検索フォルダを選択する
        public SimpleDelegateCommand OpenSelectSearchFolderWindowCommand => new((parameter) => {
            if (SearchConditionRule == null) {
                Tools.Error("検索条件がNullです");
                return;
            }
            // フォルダが選択されたら、SearchFolderに設定
            void FolderSelectedAction(ClipboardItemFolderViewModel folderViewModel) {
                SearchFolderViewModel = folderViewModel;
                SearchConditionRule.SearchFolder = folderViewModel.ClipboardItemFolder;
                SearchFolderPath = folderViewModel.ClipboardItemFolder.AbsoluteCollectionName;
                OnPropertyChanged("SearchFolderPath");

            }

            FolderSelectWindow FolderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;
            ClipboardItemFolderViewModel? rootFolderViewModel = new ClipboardItemFolderViewModel(ClipboardItemFolder.SearchRootFolder);
            FolderSelectWindowViewModel.Initialize(rootFolderViewModel, FolderSelectedAction);
            FolderSelectWindow.ShowDialog();

        });

        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand OpenSelectTargetFolderWindowCommand => new((parameter) => {
            if (SearchConditionRule == null) {
                Tools.Error("検索条件がNullです");
                return;
            }
            if (SearchConditionRule.Type != SearchRule.SearchType.SearchFolder) {
                Tools.Error("検索フォルダ以外では選択できません");
                return;
            }
            // フォルダが選択されたら、TargetFolderに設定
            void FolderSelectedAction(ClipboardItemFolderViewModel folderViewModel) {
                SearchConditionRule.TargetFolder = folderViewModel.ClipboardItemFolder;
                TargetFolderPath = folderViewModel.ClipboardItemFolder.AbsoluteCollectionName;
                OnPropertyChanged("TargetFolderPath");
            }

            FolderSelectWindow FolderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;
            ClipboardItemFolderViewModel? rootFolderViewModel = new ClipboardItemFolderViewModel(ClipboardItemFolder.RootFolder);
            FolderSelectWindowViewModel.Initialize(rootFolderViewModel, FolderSelectedAction);
            FolderSelectWindow.ShowDialog();

        });

    }
}
