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
using LibUIPythonAI.ViewModel.Folder;
using PythonAILib.Model.Folder;
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



        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        public override async void LoadChildren(int nestLevel) {

            // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
            List<ContentFolderViewModel> _children = [];

            await Task.Run(() => {
                foreach (var child in Folder.GetChildren()) {
                    if (child == null) {
                        continue;
                    }
                    FileSystemFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                    // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                    if (nestLevel > 0) {
                        childViewModel.LoadChildren(nestLevel - 1);
                    }
                    _children.Add(childViewModel);
                }
            });
            Children = new ObservableCollection<ContentFolderViewModel>(_children);
            OnPropertyChanged(nameof(Children));
        }
        // LoadItems
        protected override void LoadItems() {
            List<ContentItemWrapper> _items = Folder.GetItems();
            MainUITask.Run(() => {
                Items.Clear();
                foreach (var item in _items) {

                    Items.Add(CreateItemViewModel(item));
                }
            });
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
                Parent = shortCutRootFolder.Entity,
            };
            ShortCutFolder subFolder = new(contentFolder) {
                FileSystemFolderPath = fileSystemFolder.FileSystemFolderPath,
            };
            subFolder.Save();
        });

    }
}

