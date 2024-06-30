using System.Windows;
using ClipboardApp.View.ClipboardItemFolderView;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.SearchView {
    public partial class SearchWindowViewModel : MyWindowViewModel {
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

        public Visibility NameVisibility {
            get {
                return _isSearchFolder ? Visibility.Visible : Visibility.Collapsed;
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

        private bool _isSearchFolder;

        private Action? _afterUpdate;

        public string Name {
            get {
                return SearchConditionRule?.Name ?? "";
            }
            set {
                if (SearchConditionRule == null) {
                    LogWrapper.Error("SearchConditionRuleがNullです");
                    return;
                }
                SearchConditionRule.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public void Initialize(
            SearchRule searchConditionRule,
            ClipboardFolderViewModel? searchFolderViewModel, bool isSearchFolder,
            Action afterUpdate
            ) {
            this.SearchConditionRule = searchConditionRule;
            this._isSearchFolder = isSearchFolder;

            _afterUpdate = afterUpdate;
            SearchFolderViewModel = searchFolderViewModel;

            Name = searchFolderViewModel?.FolderName ?? "";

            SearchFolderPath = searchFolderViewModel?.FolderPath;
            TargetFolderPath = searchConditionRule.TargetFolder?.FolderPath;

            OnPropertyChanged(nameof(SearchTypeText));
            OnPropertyChanged(nameof(SearchFolderVisibility));
            OnPropertyChanged(nameof(NameVisibility));

        }
    }
}
