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
            SearchFolderPath = searchFolderViewModel?.FolderPath;
            TargetFolderPath = searchConditionRule.TargetFolder?.FolderPath;

            OnPropertyChanged(nameof(SearchTypeText));
            OnPropertyChanged(nameof(SearchFolderVisibility));

        }

        public void Initialize(SearchRule searchConditionRule, Action afterUpdate) {
            Initialize(searchConditionRule, null, afterUpdate);
        }
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


            ClipboardFolderViewModel? rootFolderViewModel = new (MainWindowViewModel.ActiveInstance, ClipboardFolder.SearchRootFolder);
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

            ClipboardFolderViewModel? rootFolderViewModel = new (MainWindowViewModel.ActiveInstance, ClipboardFolder.RootFolder);
            FolderSelectWindow.OpenFolderSelectWindow(rootFolderViewModel, (folderViewModel) => {
                SearchConditionRule.TargetFolder = folderViewModel.ClipboardItemFolder;
                TargetFolderPath = folderViewModel.FolderPath;
                OnPropertyChanged(nameof(TargetFolderPath));
            });

        });

    }
}
