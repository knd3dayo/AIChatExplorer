using System.Windows;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.Utils;
using LibPythonAI.Utils.Common;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.Content;

namespace LibUIPythonAI.ViewModel.Search {
    public class SearchWindowViewModel : ChatViewModelBase {
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
        
        public Visibility NameVisibility => Tools.BoolToVisibility(_isSearchFolder == true);


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
        private bool _isSearchAllFolders;
        public bool IsSearchAllFolders {
            get {
                return _isSearchAllFolders;
            }
            set {
                _isSearchAllFolders = value;
                OnPropertyChanged(nameof(IsSearchAllFolders));
                OnPropertyChanged(nameof(IsSearchSpecifiedFolder));
            }
        }
        // 特定のフォルダのみ検索するかどうか。 _isSearchFolderがTrueの場合は、Falseに設定する
        public bool IsSearchSpecifiedFolder {
            get {
                return !_isSearchAllFolders;
            }
            set {
                _isSearchAllFolders = !value;
                OnPropertyChanged(nameof(IsSearchAllFolders));
                OnPropertyChanged(nameof(IsSearchSpecifiedFolder));
            }
        }


        public string Name {
            get {
                return SearchConditionRule?.Name ?? "";
            }
            set {
                if (SearchConditionRule == null) {
                    LogWrapper.Error(StringResources.SearchConditionRuleIsNull);
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
                LogWrapper.Error(StringResources.NoSearchConditions);
                return;
            }
            if (_isSearchFolder) {
                // SearchConditionRuleのSearchFolderにSearchFolderViewModelのClipboardItemFolderを設定
                SearchConditionRule.SearchFolder = SearchFolder;
                // _isSearchFolderがTrueの場合は、フォルダ名を更新
                SearchFolder.FolderName = SearchConditionRule.Name;
            }

            // 検索条件をLiteDBに保存
            SearchConditionRule.Save();

            // 検索条件を適用後に実行する処理
            _afterUpdate?.Invoke();

            // Close the window
            window.Close();
        });

        // OpenClipboardFolderWindowCommand
        public SimpleDelegateCommand<object> OpenClipboardFolderWindowCommand => new((parameter) => {
            FolderSelectWindow.OpenFolderSelectWindow(RootFolderViewModelContainer.FolderViewModels, (folderViewModel, finished) => {
                if (finished) {
                    SearchConditionRule.TargetFolder = folderViewModel.Folder;
                    TargetFolderPath = folderViewModel.FolderPath;
                    OnPropertyChanged(nameof(TargetFolderPath));
                }
            });
        });
    }
}
