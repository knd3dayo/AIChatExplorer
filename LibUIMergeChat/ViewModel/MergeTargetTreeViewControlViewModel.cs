using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIMergeChat.ViewModel {
    public class MergeTargetTreeViewControlViewModel : ObservableObject {

        public MergeTargetTreeViewControlViewModel(Action<ContentFolderViewModel> selectFolderAction, Action<bool> updateIndeterminateAction) {
            SelectFolderAction = selectFolderAction;
            UpdateIndeterminateAction = updateIndeterminateAction;
        }

        public Action<bool> UpdateIndeterminateAction { get; set; }

        public Action<ContentFolderViewModel> SelectFolderAction { get; set; } = (folder) => { };


        // 選択中のフォルダ
        private ContentFolderViewModel? _selectedFolder;
        public ContentFolderViewModel? SelectedFolder {
            get {

                return _selectedFolder;
            }
            set {
                if (value == null) {
                    _selectedFolder = null;
                } else {
                    _selectedFolder = value;
                    // Load
                    _selectedFolder.LoadFolderExecute(beforeAction: () => { }, afterAction: () => {
                        SelectFolderAction(_selectedFolder);
                    });
                }
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }
        public ObservableCollection<ContentFolderViewModel> FolderViewModels { get; set; } = RootFolderViewModelContainer.FolderViewModels;


        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ContentFolderViewModel clipboardItemFolderViewModel = (ContentFolderViewModel)treeView.SelectedItem;
            SelectedFolder = clipboardItemFolderViewModel;
        });

    }
}
