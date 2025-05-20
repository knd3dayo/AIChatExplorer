using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.Search;
using AIChatExplorer.Model.Main;
using AIChatExplorer.ViewModel.Folders.Search;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;

namespace AIChatExplorer.ViewModel.Main {
    public class MainPanelTreeViewControlViewModel : CommonViewModelBase {

        private AppViewModelCommands Commands { get; set; }

        // constructor
        public MainPanelTreeViewControlViewModel(AppViewModelCommands commands) {
            Commands = commands;
        }
        public Action<bool> UpdateIndeterminateAction { get; set; } = (isIndeterminate) => { };

        public Action<ContentFolderViewModel> SelectedFolderChangedAction { get; set; } = (selectedFolder) => { };

        // Null許容型
        [AllowNull]
        public FolderViewModelManager RootFolderViewModelContainer { get; set; }

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


        // Ctrl + C or X  が押された時のClipboardItemFolder
        private ContentFolderViewModel? _copiedFolder;
        public ContentFolderViewModel? CopiedFolder {
            get {
                return _copiedFolder;
            }
            set {
                _copiedFolder = value;
                OnPropertyChanged(nameof(CopiedFolder));
            }
        }


        #region フォルダツリーのInputBinding用のコマンド
        // Ctrl + R が押された時の処理
        public SimpleDelegateCommand<object> ReloadCommand => new((parameter) => {
            AppViewModelCommands.ReloadFolderCommandExecute(this.SelectedFolder,
                () => {
                    UpdateIndeterminateAction(true);
                },
                () => {
                    UpdateIndeterminateAction(false);
                }
                );
        });

        // クリップボードアイテムを作成する。
        // Ctrl + N が押された時の処理
        // メニューの「アイテム作成」をクリックしたときの処理
        public SimpleDelegateCommand<object> CreateItemCommand => new((parameter) => {
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            this.SelectedFolder.CreateItemCommandExecute();
        });

        // Ctrl + V が押された時の処理
        public SimpleDelegateCommand<object> PasteCommand => new((parameter) => {
            Commands.PasteFromClipboardCommandExecute();
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutFolderCommand => new((parameter) => {
            AppViewModelCommands.CutFolderCommandExecute(this);
        });


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
        #endregion

        // SelectedTreeViewItemChangeCommandExecute
        public void SelectedTreeViewItemChangeCommandExecute(ContentFolderViewModel folder) {
            TreeView? treeView = ThisUserControl?.FindName("FolderTreeView") as TreeView;
            if (treeView == null) {
                LogWrapper.Error("FolderTreeView is null.");
                return;
            }
            ItemsControl itemsControl = treeView;

            List<ContentFolderViewModel> items = [];
            // folderからRootFolderまでのフォルダを取得 
            ContentFolderViewModel? currentFolder = folder;
            while (currentFolder != null) {
                items.Add(currentFolder);
                currentFolder = currentFolder.ParentFolderViewModel;
            }
            // itemsの順番を逆にして、RootFolderからFolderまでのフォルダをExpandする
            for (int i = items.Count - 1; i >= 0; i--) {

                for (int j = 0; j < itemsControl.Items.Count; j++) {
                    TreeViewItem? childItem = (TreeViewItem?)itemsControl.ItemContainerGenerator.ContainerFromIndex(j);
                    if (childItem == null) {
                        continue;
                    }
                    if (childItem.Header is ContentFolderViewModel folderViewModel) {
                        if (folderViewModel.Folder.Id == items[i].Folder.Id) {
                            if (childItem.Items.Count == 0) {
                                childItem.IsExpanded = true;
                                break;
                            }
                            childItem.IsExpanded = true;
                            itemsControl.UpdateLayout();
                            itemsControl = childItem;
                            break;
                        }
                    }
                }
            }
        }

        // CreateFolderCommand
        public SimpleDelegateCommand<object> CreateFolderCommand => new((parameter) => {
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            // フォルダを作成する
            SelectedFolder.CreateFolderCommand.Execute();
        });

    }
}
