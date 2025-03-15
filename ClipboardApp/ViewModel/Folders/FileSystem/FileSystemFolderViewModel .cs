using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model.Folders.FileSystem;
using ClipboardApp.Model.Folders.ShortCut;
using ClipboardApp.Model.Item;
using ClipboardApp.Model.Main;
using ClipboardApp.ViewModel.Folders.Clipboard;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibUIPythonAI.Utils;
using PythonAILibUI.ViewModel.Item;

namespace ClipboardApp.ViewModel.Folders.FileSystem {
    public class FileSystemFolderViewModel(FileSystemFolder clipboardItemFolder, ContentItemViewModelCommands commands) : ClipboardFolderViewModel(clipboardItemFolder, commands) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 1;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                FileSystemFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
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

        // LoadItems
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
            ContentFolderEntity contentFolder = new() {
                FolderTypeString = FolderManager.SHORTCUT_ROOT_FOLDER_NAME_EN,
                Description = folderViewModel.FolderName,
                FolderName = folderViewModel.FolderName,
                ParentId = shortCutRootFolder.Id,
            };
            ShortCutFolder subFolder = new(contentFolder) {
                FileSystemFolderPath = fileSystemFolder.FileSystemFolderPath,
            };
            subFolder.Save();

        });

    }
}

