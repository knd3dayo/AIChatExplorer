using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemFolderView {

    public class FolderSelectWindowViewModel : MyWindowViewModel {
        private static FolderSelectWindowViewModel? Instance;
        // フォルダツリーのルート
        public ObservableCollection<ClipboardFolderViewModel> RootFolders { get; set; } = [];

        // フォルダ選択時のAction
        public Action<ClipboardFolderViewModel>? FolderSelectedAction { get; set; }

        // 選択されたフォルダ
        private ClipboardFolderViewModel? selectedFolder;
        public ClipboardFolderViewModel? SelectedFolder {
            get {
                return selectedFolder;
            }
            set {
                selectedFolder = value;
                selectedFolder?.LoadChildren();
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }

        private string _selectedFolderAbsoluteCollectionName = "";
        public string SelectedFolderAbsoluteCollectionName {
            get {
                return _selectedFolderAbsoluteCollectionName;
            }
            set {
                _selectedFolderAbsoluteCollectionName = value;
                OnPropertyChanged(nameof(SelectedFolderAbsoluteCollectionName));
            }
        }

        public void Initialize(ClipboardFolderViewModel rootFolderViewModel, Action<ClipboardFolderViewModel> _FolderSelectedAction) {

            FolderSelectedAction = _FolderSelectedAction;
            if (rootFolderViewModel == null) {
                return;
            }
            RootFolders.Add(rootFolderViewModel);
            Instance = this;
        }
        public static SimpleDelegateCommand SelectFolderCommand => new((parameter) => {
            if (Instance == null) {
                Tools.Warn("エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない");
                return;
            }
            if (Instance.SelectedFolder == null) {
                Tools.Warn("エラーが発生しました。選択中のフォルダがない");
                return;
            }
            Instance.FolderSelectedAction?.Invoke(Instance.SelectedFolder);
            // Windowを閉じる
            if (parameter is FolderSelectWindow folderSelectWindow) {
                folderSelectWindow.Close();
            }

        });

        public SimpleDelegateCommand CancelCommand => new((parameter) => {
            // Windowを閉じる
            if (parameter is FolderSelectWindow folderSelectWindow) {
                folderSelectWindow.Close();
            }

        });

        public static void FolderSelectWindowSelectFolderCommandExecute(object parameter) {
            if (Instance == null) {
                Tools.Warn("エラーが発生しました。FolderSelectWindowViewModelのインスタンスがない");
                return;
            }
            if (parameter is not ClipboardFolderViewModel folder) {
                Tools.Warn("エラーが発生しました。選択中のフォルダがない");
                return;
            }
            Instance.SelectedFolder = folder;
            Instance.SelectedFolderAbsoluteCollectionName = folder.AbsoluteCollectionName;

        }
    }
}
