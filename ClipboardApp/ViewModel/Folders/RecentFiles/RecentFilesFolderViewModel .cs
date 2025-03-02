using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model.Folders.Browser;
using ClipboardApp.Model.Folders.FileSystem;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Folders.FileSystem;
using ClipboardApp.ViewModel.Folders.ShortCut;
using LibPythonAI.Model.Content;
using LibUIPythonAI.ViewModel.Folder;
using PythonAILibUI.ViewModel.Item;

namespace ClipboardApp.ViewModel.Folders.Browser {
    public class RecentFilesFolderViewModel(FileSystemFolder clipboardItemFolder, ContentItemViewModelCommands commands) : FileSystemFolderViewModel(clipboardItemFolder, commands) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 1;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                ShortCutFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override RecentFilesFolderViewModel CreateChildFolderViewModel(ContentFolderWrapper childFolder) {
            if (childFolder is not RecentFilesFolder) {
                throw new Exception("childFolder is not RecentFilesFolder");
            }
            var childFolderViewModel = new RecentFilesFolderViewModel((RecentFilesFolder)childFolder, Commands) {
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
            List<ClipboardFolderViewModel> _children = [];

            await Task.Run(() => {
                // RootFolderの場合は、ShortCutFolderを取得
                if (Folder.IsRootFolder) {
                    foreach (var child in Folder.GetChildren()) {
                        if (child == null) {
                            continue;
                        }
                        RecentFilesFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                        _children.Add(childViewModel);
                    }
                    return;
                }
                // RootFolder以外の場合は、FileSystemFolderを取得 
                foreach (var child in Folder.GetChildren()) {
                    if (child == null) {
                        continue;
                    }
                    RecentFilesFolderViewModel childViewModel = CreateChildFolderViewModel(child);
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
    }
}

