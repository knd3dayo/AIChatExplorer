using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILibUI.ViewModel.Folder;
using QAChat.Resource;
using QAChat.ViewModel.Folder;
using WpfAppCommon.Utils;

namespace MergeChat.ViewModel {
    public class MergeTargetTreeViewControlViewModel : ObservableObject {

        public Action<bool> UpdateIndeterminateAction { get; set; } = (isIndeterminate) => { };

        public Action<ContentFolderViewModel> SelectedFolderChangedAction { get; set; } = (selectedFolder) => { };

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
            if (SelectedFolder != null) {
                // Load
                SelectedFolder.LoadFolderCommand.Execute();
                SelectedFolderChangedAction(SelectedFolder);
            }
        });

    }
}
