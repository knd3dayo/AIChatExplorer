using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Model.Folders.Search;
using ClipboardApp.Model.Main;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Folders.Search;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;

namespace ClipboardApp.ViewModel.Main {
    public class MainPanelTreeViewControlViewModel : AppViewModelBase {

        private AppItemViewModelCommands Commands { get; set; }

         // constructor
         public MainPanelTreeViewControlViewModel(AppItemViewModelCommands commands) {
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
            Commands.ReloadFolderCommandExecute(this.SelectedFolder,
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
            Commands.CutFolderCommandExecute(this);
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
                return;
            }
            List<ContentFolderViewModel> items = [];
            // folderからRootFolderまでのフォルダを取得 
            ContentFolderViewModel? currentFolder = folder;
            while (currentFolder != null) {
                items.Add(currentFolder);
                currentFolder = currentFolder.ParentFolderViewModel;
            }
            ItemsControl itemsControl = treeView;
            // itemsの順番を逆にして、RootFolderからFolderまでのフォルダをExpandする
            for (int i = items.Count - 1; i >= 0; i--) {

                for (int j = 0; j < itemsControl.Items.Count; j++) {
                    TreeViewItem? childItem = (TreeViewItem?)itemsControl.ItemContainerGenerator.ContainerFromIndex(j);
                    if (childItem == null) {
                        continue;
                    }
                    if (childItem.Header is ContentFolderViewModel folderViewModel) {
                        if (folderViewModel.Folder.Entity.Id == items[i].Folder.Entity.Id) {
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

        public void CreateSearchFolder() {
            // 現在の日付 時刻の文字列を取得
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            SearchFolder folder = FolderManager.SearchRootFolder.CreateChild(now);

            // 検索フォルダの親フォルダにこのフォルダを追加

            SearchFolderViewModel searchFolderViewModel = new(folder, Commands);

            Commands.OpenSearchWindowCommand(searchFolderViewModel, () => {
                // 保存と再読み込み
                searchFolderViewModel.ParentFolderViewModel = RootFolderViewModelContainer.SearchRootFolderViewModel;
                searchFolderViewModel.SaveFolderCommand.Execute(null);
                // 親フォルダを保存
                RootFolderViewModelContainer.SearchRootFolderViewModel.SaveFolderCommand.Execute(null);
                // Load
                RootFolderViewModelContainer.SearchRootFolderViewModel.LoadFolderExecute(
                () => {
                    Commands.UpdateIndeterminate(true);
                },
                () => {
                    MainUITask.Run(() => {
                        Commands.UpdateIndeterminate(false);

                        SelectedTreeViewItemChangeCommandExecute(searchFolderViewModel);
                        // SelectedFolder に　SearchFolderViewModelを設定
                        SelectedFolder = searchFolderViewModel;
                    });
                });

            });
        }

    }
}
