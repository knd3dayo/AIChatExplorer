using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Resource;
using WpfAppCommon.Utils;

namespace MergeChat.ViewModel {
    public class MergeTargetTreeViewControlViewModel : ObservableObject {

        public Action<bool> UpdateIndeterminateAction { get; set; } = (isIndeterminate) => { };

        public Action<MergeChatFolderViewModel> SelectedFolderChangedAction { get; set; } = (selectedFolder) => { };

        // 選択中のフォルダ
        private MergeChatFolderViewModel? _selectedFolder;
        public MergeChatFolderViewModel? SelectedFolder {
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


        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            MergeChatFolderViewModel clipboardItemFolderViewModel = (MergeChatFolderViewModel)treeView.SelectedItem;
            SelectedFolder = clipboardItemFolderViewModel;
            if (SelectedFolder != null) {
                // Load
                SelectedFolder.LoadFolderCommand.Execute();
                SelectedFolderChangedAction(SelectedFolder);
            }
        });

    }
}
