using System.Collections.ObjectModel;
using ClipboardApp.View.ClipboardItemFolderView;
using QAChat.Resource;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Folder {

    public class FolderSelectWindowViewModel : ClipboardAppViewModelBase {

        public FolderSelectWindowViewModel(ClipboardFolderViewModel rootFolderViewModel, Action<ClipboardFolderViewModel> _FolderSelectedAction) {

            FolderSelectedAction = _FolderSelectedAction;
            if (rootFolderViewModel == null) {
                return;
            }
            RootFolders.Add(rootFolderViewModel);
            Instance = this;
        }

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
        public static SimpleDelegateCommand<FolderSelectWindow> SelectFolderCommand => new((folderSelectWindow) => {
            if (Instance == null) {
                LogWrapper.Warn(CommonStringResources.Instance.FolderSelectWindowViewModelInstanceNotFound);
                return;
            }
            if (Instance.SelectedFolder == null) {
                LogWrapper.Warn(CommonStringResources.Instance.SelectedFolderNotFound);
                return;
            }
            Instance.FolderSelectedAction?.Invoke(Instance.SelectedFolder);
            // Windowを閉じる
            folderSelectWindow.Close();

        });

        public static void FolderSelectWindowSelectFolderCommandExecute(object parameter) {
            if (Instance == null) {
                LogWrapper.Warn(CommonStringResources.Instance.FolderSelectWindowViewModelInstanceNotFound);
                return;
            }
            if (parameter is not ClipboardFolderViewModel folder) {
                LogWrapper.Warn(CommonStringResources.Instance.SelectedFolderNotFound);
                return;
            }
            folder.LoadFolderCommand.Execute();
            Instance.SelectedFolder = folder;
            Instance.SelectedFolderAbsoluteCollectionName = folder.FolderPath;

        }
    }
}
