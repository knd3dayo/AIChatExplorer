using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.ClipboardItemFolderView;

namespace WpfApp1.View.SearchView {
    public class SearchWindowViewModel : ObservableObject {

        private SearchCondition? searchCondition;
        public SearchCondition? SearchCondition {
            get {
                return searchCondition;
            }
            set {
                searchCondition = value;
                OnPropertyChanged("SearchCondition");
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
        // 検索フォルダ用の検索かどうか
        public bool IsSearchFolder {
            get {
                if ( SearchFolderViewModel == null) { 
                    return false;
                }
                return SearchFolderViewModel.ClipboardItemFolder.IsSearchFolder;
            }
        }

        public bool IsIncludeSubFolder {
            get {
                if (SearchCondition == null) {
                    return false;
                }
                return SearchCondition.IncludeSubFolder;
            }
            set {
                if (SearchCondition == null) {
                    return;
                }
                SearchCondition.IncludeSubFolder = value;
                OnPropertyChanged("IsIncludeSubFolder");
            }
        }
        // 検索フォルダの場合は表示する、それ以外は非表示
        public Visibility SearchFolderVisibility {

            get {
                if (IsSearchFolder) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        private Action? _afterUpdate;

        public void Initialize(SearchCondition searchCondition, ClipboardItemFolderViewModel? searchFolderViewModel, Action afterUpdate) {
            this.SearchCondition = searchCondition;
            _afterUpdate = afterUpdate;
            SearchFolderViewModel = searchFolderViewModel;

        }

        public void Initialize(SearchCondition searchCondition, Action afterUpdate) {
            Initialize(searchCondition, null, afterUpdate);
        }

        //--------------------------------------------------------------------------------
        // コマンド
        //--------------------------------------------------------------------------------

        public SimpleDelegateCommand ClearCommand => new SimpleDelegateCommand(ClearCommandExecute);
        private void ClearCommandExecute(object parameter) {
            SearchCondition?.Clear();
        }

        public SimpleDelegateCommand ApplyCommand => new SimpleDelegateCommand(ApplyCommandExecute);

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

        // OpenSelectSearchFolderWindowCommand
        // 検索フォルダを選択する
        public SimpleDelegateCommand OpenSelectSearchFolderWindowCommand => new SimpleDelegateCommand(OpenSelectSearchFolderWindowCommandExecute);
        public void OpenSelectSearchFolderWindowCommandExecute(object parameter) {
            // フォルダが選択されたら、SearchFolderに設定
            void FolderSelectedAction(ClipboardItemFolderViewModel folderViewModel) {
                SearchFolderViewModel = folderViewModel;
            }

            FolderSelectWindow FolderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;
            ClipboardItemFolderViewModel? rootFolderViewModel = new ClipboardItemFolderViewModel(ClipboardItemFolder.SearchRootFolder);
            FolderSelectWindowViewModel.Initialize(rootFolderViewModel, FolderSelectedAction);
            FolderSelectWindow.ShowDialog();
        }


        // OpenSelectTargetFolderWindowCommand
        public SimpleDelegateCommand OpenSelectTargetFolderWindowCommand => new SimpleDelegateCommand(OpenSelectTargetFolderWindowCommandExecute);
        public void OpenSelectTargetFolderWindowCommandExecute(object parameter) {
            if (!IsSearchFolder) {
                Tools.Error("検索フォルダ以外では選択できません");
                return;
            }
            // フォルダが選択されたら、TargetFolderに設定
            void FolderSelectedAction(ClipboardItemFolderViewModel folderViewModel) {
                SearchCondition?.TargetFolderHashSet.Add(folderViewModel.ClipboardItemFolder.AbsoluteCollectionName);
            }

            FolderSelectWindow FolderSelectWindow = new FolderSelectWindow();
            FolderSelectWindowViewModel FolderSelectWindowViewModel = (FolderSelectWindowViewModel)FolderSelectWindow.DataContext;
            ClipboardItemFolderViewModel? rootFolderViewModel = new ClipboardItemFolderViewModel(ClipboardItemFolder.RootFolder);
            FolderSelectWindowViewModel.Initialize(rootFolderViewModel, FolderSelectedAction);
            FolderSelectWindow.ShowDialog();
        }

    }
}
