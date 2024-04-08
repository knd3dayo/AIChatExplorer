using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.ClipboardItemFolderView;

namespace WpfApp1.View.SearchView {
    public class SearchWindowViewModel : ObservableObject {
        private SearchCondition searchCondition = new SearchCondition();

        public SearchWindowViewModel(SearchCondition searchCondition) {
            this.SearchCondition = searchCondition;
        }

        public SearchCondition SearchCondition {
            get {
                return searchCondition;
            }
            set {
                searchCondition = value;
                OnPropertyChanged("SearchCondition");
            }
        }
        // フォルダ選択用DockPanelを有効にするかどうか
        private bool isTargetFolderEnabled = false;
        public bool IsTargetFolderEnabled {
            get {
                return isTargetFolderEnabled;
            }
            set {
                isTargetFolderEnabled = value;
                OnPropertyChanged("IsTargetFolderEnabled");
            }
        }

        public bool IsIncludeSubFolder {
            get {
                return SearchCondition.IncludeSubFolder;
            }
            set {
                SearchCondition.IncludeSubFolder = value;
                OnPropertyChanged("IsIncludeSubFolder");
            }
        }

        public SimpleDelegateCommand ClearCommand => new SimpleDelegateCommand(ClearCommandExecute);

        private Action? _afterUpdate;

        // 設定対象の検索フォルダ
        public ClipboardItemFolderViewModel? ApplyTargetFolderViewModel { get; set; }
        public void Initialize(SearchCondition searchCondition, ClipboardItemFolderViewModel? applyTargetFolderViewModel, Action afterUpdate) {
            this.SearchCondition = searchCondition;
            _afterUpdate = afterUpdate;
            ApplyTargetFolderViewModel = applyTargetFolderViewModel;
            IsTargetFolderEnabled = ApplyTargetFolderViewModel != null;

        }
        public void Initialize(SearchCondition searchCondition, Action afterUpdate) {
            Initialize(searchCondition, null, afterUpdate);
        }

        private void ClearCommandExecute(object parameter) {
            SearchCondition.Clear();
        }


        public SimpleDelegateCommand applyCommand => new SimpleDelegateCommand(ApplyCommandExecute);

        private void ApplyCommandExecute(object parameter) {
            if (SearchCondition == null) {
                Tools.Error("検索条件がNullです");
                return;
            }
            // 検索条件をLiteDBに保存
            SearchCondition.Upsert();

            // 検索条件を適用後に実行する処理
            _afterUpdate?.Invoke();

            // Close the window
            SearchWindow.Current?.Close();
        }
        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand OpenSelectTargetFolderWindowCommand => new SimpleDelegateCommand(OpenSelectTargetFolderWindowCommandExecute);
        public void OpenSelectTargetFolderWindowCommandExecute(object parameter) {
            // フォルダが選択されたら、TargetFolderに設定
            void FolderSelectedAction(ClipboardItemFolderViewModel folderViewModel) {
                SearchCondition.TargetFolderHashSet.Add(folderViewModel.ClipboardItemFolder.AbsoluteCollectionName);
            }
            FolderSelectWindow FolderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;
            FolderSelectWindowViewModel.Initialize(FolderSelectedAction);
            FolderSelectWindow.ShowDialog();
        }

    }
}
