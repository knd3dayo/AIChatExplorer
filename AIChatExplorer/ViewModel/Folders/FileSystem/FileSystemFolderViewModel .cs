using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.ViewModel.Folders.Application;
using LibMain.Model.Content;
using LibUIMain.ViewModel.Common;

namespace AIChatExplorer.ViewModel.Folders.FileSystem {
    public class FileSystemFolderViewModel(FileSystemFolder applicationItemFolder, CommonViewModelCommandExecutes Commands) : ApplicationFolderViewModel(applicationItemFolder, Commands) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 1;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                FileSystemFolderMenu applicationItemMenu = new(this);
                return applicationItemMenu.MenuItems;
            }
        }

        // 子フォルダのApplicationFolderViewModelを作成するメソッド
        public override FileSystemFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            if (childFolder is not FileSystemFolder) {
                throw new System.Exception("childFolder is not FileSystemFolder");
            }
            var childFolderViewModel = new FileSystemFolderViewModel((FileSystemFolder)childFolder, Commands) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // LoadLLMConfigListAsync
        public override async Task LoadItemsAsync() {
            await LoadItemsAsync<FileSystemItem>();
        }

        // LoadChildren
        public override async Task LoadChildren(int nestLevel) {
            await LoadChildren<FileSystemFolderViewModel, FileSystemFolder>(nestLevel);
        }

    }
}

