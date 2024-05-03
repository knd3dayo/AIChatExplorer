using System.Windows;
using ClipboardApp.View.ClipboardItemFolderView;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.SearchView {
    public class SearchWindowViewModel : MyWindowViewModel {
        public MainWindowViewModel? MainWindowViewModel { get; private set; }

        private SearchRule? _searchConditionRule;
        public SearchRule? SearchConditionRule {
            get {
                return _searchConditionRule;
            }
            set {
                _searchConditionRule = value;
                OnPropertyChanged(nameof(SearchConditionRule));
            }
        }

        private ClipboardFolderViewModel? searchFolderViewModel;
        public ClipboardFolderViewModel? SearchFolderViewModel {
            get {
                return searchFolderViewModel;
            }
            set {
                searchFolderViewModel = value;
                OnPropertyChanged(nameof(SearchFolderViewModel));
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
                OnPropertyChanged(nameof(SearchFolderPath));
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
                OnPropertyChanged(nameof(TargetFolderPath));
            }
        }

        private Action? _afterUpdate;

        public void Initialize(
            SearchRule searchConditionRule,
            ClipboardFolderViewModel? searchFolderViewModel,
            Action afterUpdate
            ) {
            this.SearchConditionRule = searchConditionRule;

            _afterUpdate = afterUpdate;
            SearchFolderViewModel = searchFolderViewModel;
            SearchFolderPath = searchFolderViewModel?.AbsoluteCollectionName;
            TargetFolderPath = searchConditionRule.TargetFolder?.AbsoluteCollectionName;

            OnPropertyChanged(nameof(SearchTypeText));
            OnPropertyChanged(nameof(SearchFolderVisibility));

        }

        public void Initialize(SearchRule searchConditionRule, Action afterUpdate) {
            Initialize(searchConditionRule, null, afterUpdate);
        }
        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------

        public SimpleDelegateCommand ClearCommand => new (ClearCommandExecute);
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
            if (MainWindowViewModel == null) {
                Tools.Error("MainWindowViewModelがNullです");
                return;
            }

            // フォルダが選択されたら、SearchFolderに設定
            void FolderSelectedAction(ClipboardFolderViewModel folderViewModel) {
                SearchFolderViewModel = folderViewModel;
                folderViewModel.SetSearchFolder(SearchConditionRule);
                SearchFolderPath = folderViewModel.AbsoluteCollectionName;
                OnPropertyChanged(nameof(SearchFolderPath));

            }

            FolderSelectWindow FolderSelectWindow = new ();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;
            ClipboardFolderViewModel? rootFolderViewModel = new (MainWindowViewModel, ClipboardFolder.SearchRootFolder);
            FolderSelectWindowViewModel.Initialize(rootFolderViewModel, FolderSelectedAction);
            FolderSelectWindow.ShowDialog();

        });

        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand OpenSelectTargetFolderWindowCommand => new((parameter) => {
            if (MainWindowViewModel == null) {
                Tools.Error("MainWindowViewModelがNullです");
                return;
            }
            if (SearchConditionRule == null) {
                Tools.Error("検索条件がNullです");
                return;
            }
            if (SearchConditionRule.Type != SearchRule.SearchType.SearchFolder) {
                Tools.Error("検索フォルダ以外では選択できません");
                return;
            }
            // フォルダが選択されたら、TargetFolderに設定
            void FolderSelectedAction(ClipboardFolderViewModel folderViewModel) {
                folderViewModel.SetSearchTargetFolder(SearchConditionRule);
                TargetFolderPath = folderViewModel.AbsoluteCollectionName;
                OnPropertyChanged(nameof(TargetFolderPath));
            }

            FolderSelectWindow FolderSelectWindow = new ();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;
            ClipboardFolderViewModel? rootFolderViewModel = new (MainWindowViewModel, ClipboardFolder.RootFolder);
            FolderSelectWindowViewModel.Initialize(rootFolderViewModel, FolderSelectedAction);
            FolderSelectWindow.ShowDialog();

        });

    }
}
