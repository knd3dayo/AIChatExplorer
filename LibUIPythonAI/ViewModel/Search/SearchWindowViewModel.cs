using System.Windows;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.Utils;
using LibPythonAI.Utils.Common;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Resource;

namespace LibUIPythonAI.ViewModel.Search {
    public class SearchWindowViewModel : CommonViewModelBase {
        public SearchWindowViewModel(
            SearchRule searchConditionRule,
            ContentFolderWrapper searchFolder, 
            Action afterUpdate
            ) {
            _searchConditionRule = searchConditionRule;
            _isSearchFolder = true;

            _afterUpdate = afterUpdate;
            _searchFolder = searchFolder;

            Name = SearchFolder?.FolderName ?? "";

            SearchFolderPath = SearchFolder?.ContentFolderPath;
            TargetFolderPath = searchConditionRule.TargetFolder?.ContentFolderPath;
            IsGlobalSearch = searchConditionRule.IsGlobalSearch;

            OnPropertyChanged(nameof(NameVisibility));
        }
        private bool _isSearchFolder;

        private Action? _afterUpdate;


        private SearchRule _searchConditionRule;
        public SearchRule SearchConditionRule {
            get {
                return _searchConditionRule;
            }
            set {
                _searchConditionRule = value;
                OnPropertyChanged(nameof(SearchConditionRule));
            }
        }

        private ContentFolderWrapper _searchFolder;
        public ContentFolderWrapper SearchFolder {
            get {
                return _searchFolder;
            }
            set {
                _searchFolder = value;
                OnPropertyChanged(nameof(SearchFolder));
            }
        }
        
        public Visibility NameVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(_isSearchFolder == true);


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

        // 全フォルダを検索するかどうか
        private bool _isGlobalSearch;
        public bool IsGlobalSearch {
            get {
                return _isGlobalSearch;
            }
            set {
                _isGlobalSearch = value;
                OnPropertyChanged(nameof(IsGlobalSearch));
                OnPropertyChanged(nameof(IsSearchSpecifiedFolder));
            }
        }
        // 特定のフォルダのみ検索するかどうか。 _isSearchFolderがTrueの場合は、Falseに設定する
        public bool IsSearchSpecifiedFolder {
            get {
                return !_isGlobalSearch;
            }
            set {
                _isGlobalSearch = !value;
                OnPropertyChanged(nameof(IsGlobalSearch));
                OnPropertyChanged(nameof(IsSearchSpecifiedFolder));
            }
        }


        public string Name {
            get {
                return SearchConditionRule?.Name ?? "";
            }
            set {
                if (SearchConditionRule == null) {
                    LogWrapper.Error(CommonStringResources.Instance.SearchConditionRuleIsNull);
                    return;
                }
                SearchConditionRule.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------

        public SimpleDelegateCommand<object> ClearCommand => new((parameter) => {
            SearchConditionRule?.SearchCondition?.Clear();
        });


        public SimpleDelegateCommand<Window> ApplyCommand => new((window) => {
            if (SearchConditionRule == null) {
                LogWrapper.Error(CommonStringResources.Instance.NoSearchConditions);
                return;
            }
            if (_isSearchFolder) {
                // SearchConditionRuleのSearchFolderにSearchFolderViewModelのApplicationItemFolderを設定
                SearchConditionRule.SearchFolder = SearchFolder;
                // _isSearchFolderがTrueの場合は、フォルダ名を更新
                SearchFolder.FolderName = SearchConditionRule.Name;
                // IsGlobalSearch
                SearchConditionRule.IsGlobalSearch = IsGlobalSearch;
            }

            // 検索条件をLiteDBに保存
            SearchConditionRule.Save();

            // 検索条件を適用後に実行する処理
            _afterUpdate?.Invoke();

            // Close the window
            window.Close();
        });

        // OpenApplicationFolderWindowCommand
        public SimpleDelegateCommand<object> OpenApplicationFolderWindowCommand => new((parameter) => {
            FolderSelectWindow.OpenFolderSelectWindow(FolderViewModelManagerBase.FolderViewModels, (folderViewModel, finished) => {
                if (finished) {
                    SearchConditionRule.TargetFolder = folderViewModel.Folder;
                    TargetFolderPath = folderViewModel.FolderPath;
                    OnPropertyChanged(nameof(TargetFolderPath));
                }
            });
        });
    }
}
