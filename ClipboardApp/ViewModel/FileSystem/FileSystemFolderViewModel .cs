using System.Collections.ObjectModel;
using ClipboardApp.Model.Folder;

namespace ClipboardApp.ViewModel.FileSystem {
    public class FileSystemFolderViewModel(FileSystemFolder clipboardItemFolder) : ClipboardFolderViewModel(clipboardItemFolder) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 0;

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override FileSystemFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            if (childFolder is not FileSystemFolder) {
                throw new Exception("childFolder is not FileSystemFolder");
            }
            var childFolderViewModel = new FileSystemFolderViewModel((FileSystemFolder)childFolder) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }

        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        public override async void LoadChildren(int nestLevel = 0) {
            try {
                MainWindowViewModel.Instance.UpdateIndeterminate(true);
                // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
                List<ClipboardFolderViewModel> _children = [];

                await Task.Run(() => {
                    foreach (var child in ClipboardItemFolder.GetChildren<FileSystemFolder>()) {
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
                Children = new ObservableCollection<ClipboardFolderViewModel>(_children);
                OnPropertyChanged(nameof(Children));
            } finally {
                MainWindowViewModel.Instance.UpdateIndeterminate(false);
            }


        }
    }
}

