using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.Model.Folders.ShortCut;
using AIChatExplorer.Model.Item;
using AIChatExplorer.Model.Main;
using AIChatExplorer.ViewModel.Folders.Application;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Folders.FileSystem {
    public class FileSystemFolderViewModel(FileSystemFolder applicationItemFolder, ContentItemViewModelCommands commands) : ApplicationFolderViewModel(applicationItemFolder, commands) {
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
        public override void LoadItems() {
            LoadItems<FileSystemItem>();
        }

        // LoadChildren
        public override void LoadChildren(int nestLevel) {
            LoadChildren<FileSystemFolderViewModel, FileSystemFolder>(nestLevel);
        }

        // ショートカット登録コマンド
        public static SimpleDelegateCommand<FileSystemFolderViewModel> CreateShortCutCommand => new((folderViewModel) => {

            FileSystemFolder fileSystemFolder = (FileSystemFolder)folderViewModel.Folder;
            // ショートカット登録
            // ShortCutRootFolderを取得
            FileSystemFolder shortCutRootFolder = FolderManager.ShortcutRootFolder;
            // ショートカットフォルダを作成
            ShortCutFolder contentFolder = new() {
                FolderTypeString = FolderManager.SHORTCUT_ROOT_FOLDER_NAME_EN,
                Description = folderViewModel.FolderName,
                FolderName = folderViewModel.FolderName,
                Parent = shortCutRootFolder,
                FileSystemFolderPath = fileSystemFolder.FileSystemFolderPath,
            };
            contentFolder.Save();

        });

    }
}

