using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model.Folder;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Folders.FileSystem;
using LibUIPythonAI.ViewModel.Folder;

namespace ClipboardApp.ViewModel.Folders.ShortCut {
    public class ShortCutFolderViewModel(FileSystemFolder clipboardItemFolder) : FileSystemFolderViewModel(clipboardItemFolder) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 0;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                ShortCutFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ShortCutFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            if (childFolder is not FileSystemFolder) {
                throw new Exception("childFolder is not FileSystemFolder");
            }
            var childFolderViewModel = new ShortCutFolderViewModel((FileSystemFolder)childFolder) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        protected override async void LoadChildren(int nestLevel = 0) {
            // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
            List<ClipboardFolderViewModel> _children = [];

            await Task.Run(() => {
                // RootFolderの場合は、ShortCutFolderを取得
                if (Folder.IsRootFolder) {
                    foreach (var child in Folder.GetChildren<FileSystemFolder>()) {
                        if (child == null) {
                            continue;
                        }
                        ShortCutFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                        _children.Add(childViewModel);
                    }
                    return;
                }
                // RootFolder以外の場合は、FileSystemFolderを取得 
                foreach (var child in Folder.GetChildren<FileSystemFolder>()) {
                    if (child == null) {
                        continue;
                    }
                    ShortCutFolderViewModel childViewModel = CreateChildFolderViewModel(child);
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

