using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibUIPythonAI.Resource;
using LibUIPythonAI.View.Folder;
using WpfAppCommon.Utils;

namespace LibUIPythonAI.ViewModel.Folder {

    public class FolderSelectWindowViewModel : ChatViewModelBase {

        public FolderSelectWindowViewModel(ObservableCollection<ContentFolderViewModel> rootFolderViewModelList, Action<ContentFolderViewModel, bool> _FolderSelectedAction) {

            FolderSelectedAction = _FolderSelectedAction;
            foreach (var rootFolderViewModel in rootFolderViewModelList) {
                RootFolders.Add(rootFolderViewModel);
            }
        }

        // フォルダツリーのルート
        public ObservableCollection<ContentFolderViewModel> RootFolders { get; set; } = [];

        // フォルダ選択時のAction
        public Action<ContentFolderViewModel, bool>? FolderSelectedAction { get; set; }

        // 選択されたフォルダ
        public ContentFolderViewModel? SelectedFolder { get; set; }


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
        public SimpleDelegateCommand<FolderSelectWindow> SelectFolderCommand => new((folderSelectWindow) => {

            if (SelectedFolder == null) {
                LogWrapper.Warn(CommonStringResources.Instance.SelectedFolderNotFound);
                return;
            }
            FolderSelectedAction?.Invoke(SelectedFolder, true);
            // Windowを閉じる
            folderSelectWindow.Close();

        });

        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ContentFolderViewModel clipboardItemFolderViewModel = (ContentFolderViewModel)treeView.SelectedItem;

            SelectedFolder = clipboardItemFolderViewModel;
            SelectedFolderAbsoluteCollectionName = clipboardItemFolderViewModel.FolderPath;
            FolderSelectedAction?.Invoke(SelectedFolder, false);

            SelectedFolder.LoadFolderCommand.Execute(null);

        });
    }
}
