using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;
using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Main {
    public class MainPanelTreeViewControlViewModel : ObservableObject {

        public Action<bool> UpdateIndeterminateAction { get; set; } = (isIndeterminate) => { };

        public Action<ClipboardFolderViewModel> SelectedFolderChangedAction { get; set; } = (selectedFolder) => { };

        // Null許容型
        [AllowNull]
        public AppRootFolderViewModelContainer RootFolderViewModelContainer { get; set; }

        // 選択中のフォルダ
        private ClipboardFolderViewModel? _selectedFolder;
        public ClipboardFolderViewModel? SelectedFolder {
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
        private ClipboardFolderViewModel? _copiedFolder;
        public ClipboardFolderViewModel? CopiedFolder {
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
            AppItemViewModelCommands commands = new();
            commands.ReloadFolderCommandExecute(this.SelectedFolder,
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
            AppItemViewModelCommands commands = new();
            commands.PasteFromClipboardCommandExecute();
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutFolderCommand => new((parameter) => {
            AppItemViewModelCommands commands = new();
            commands.CutFolderCommandExecute(this);
        });


        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ClipboardFolderViewModel clipboardItemFolderViewModel = (ClipboardFolderViewModel)treeView.SelectedItem;
            SelectedFolder = clipboardItemFolderViewModel;
            if (SelectedFolder != null) {
                // Load
                SelectedFolder.LoadFolderCommand.Execute();
                SelectedFolderChangedAction(SelectedFolder);
            }
        });
        #endregion


    }
}
