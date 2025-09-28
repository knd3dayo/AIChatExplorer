using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibMain.Utils.Common;
using LibUIMain.Resource;
using LibUIMain.Utils;
using LibUIMain.View.Folder;

namespace LibUIMain.ViewModel.Folder {

    public class FolderSelectWindowViewModel : CommonViewModelBase {

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

        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new(async (routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ContentFolderViewModel applicationItemFolderViewModel = (ContentFolderViewModel)treeView.SelectedItem;

            SelectedFolder = applicationItemFolderViewModel;
            SelectedFolderAbsoluteCollectionName = await applicationItemFolderViewModel.Folder.GetContentFolderPath();
            FolderSelectedAction?.Invoke(SelectedFolder, false);

            SelectedFolder.FolderCommands.LoadFolderCommand.Execute(null);

        });
    }
}
