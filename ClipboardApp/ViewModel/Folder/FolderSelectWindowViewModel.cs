using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows;
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
        }

        // フォルダツリーのルート
        public ObservableCollection<ClipboardFolderViewModel> RootFolders { get; set; } = [];

        // フォルダ選択時のAction
        public Action<ClipboardFolderViewModel>? FolderSelectedAction { get; set; }

        // 選択されたフォルダ
        public ClipboardFolderViewModel? SelectedFolder { get; set; } 


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
            FolderSelectedAction?.Invoke(SelectedFolder);
            // Windowを閉じる
            folderSelectWindow.Close();

        });

        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new ((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ClipboardFolderViewModel clipboardItemFolderViewModel = (ClipboardFolderViewModel)treeView.SelectedItem;

            SelectedFolder = clipboardItemFolderViewModel;
            SelectedFolderAbsoluteCollectionName = clipboardItemFolderViewModel.FolderPath;
            SelectedFolder.LoadFolderCommand.Execute(null);

        });
    }
}
