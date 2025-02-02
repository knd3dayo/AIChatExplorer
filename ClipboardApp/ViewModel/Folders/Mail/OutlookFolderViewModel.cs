using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Item;
using ClipboardApp.ViewModel.Folders.Clipboard;
using LibUIPythonAI.ViewModel.Folder;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Folders.Mail {
    public class OutlookFolderViewModel(OutlookFolder clipboardItemFolder) : ClipboardFolderViewModel(clipboardItemFolder) {
        // LoadChildrenで再帰読み込みするデフォルトのネストの深さ
        public override int DefaultNextLevel { get; } = 0;

        // -- virtual
        public override ObservableCollection<MenuItem> FolderMenuItems {
            get {
                OutlookFolderMenu clipboardItemMenu = new(this);
                return clipboardItemMenu.MenuItems;
            }
        }

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override OutlookFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            if (childFolder is not OutlookFolder) {
                throw new Exception("childFolder is not OutlookFolder");
            }
            var childFolderViewModel = new OutlookFolderViewModel((OutlookFolder)childFolder) {
                // 親フォルダとして自分自身を設定
                ParentFolderViewModel = this
            };
            return childFolderViewModel;
        }


        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        protected override async void LoadChildren(int nestLevel = 0) {
            try {
                UpdateIndeterminate(true);
                // ChildrenはメインUIスレッドで更新するため、別のリストに追加してからChildrenに代入する
                List<ContentFolderViewModel> _children = [];

                await Task.Run(() => {
                    foreach (var child in Folder.GetChildren<OutlookFolder>()) {
                        if (child == null) {
                            continue;
                        }
                        OutlookFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                        // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                        if (nestLevel > 0) {
                            childViewModel.LoadChildren(nestLevel - 1);
                        }
                        _children.Add(childViewModel);
                    }
                });
                Children = new ObservableCollection<ContentFolderViewModel>(_children);
                OnPropertyChanged(nameof(Children));
            } finally {
                UpdateIndeterminate(false);
            }

        }
        // LoadItems
        protected override async void LoadItems() {
            Items.Clear();
            // ClipboardItemFolder.Itemsは別スレッドで実行
            List<OutlookItem> _items = [];
            try {
                UpdateIndeterminate(true);
                await Task.Run(() => {
                    _items = Folder.GetItems<OutlookItem>();
                });
                foreach (OutlookItem item in _items) {
                    Items.Add(CreateItemViewModel(item));
                }
            } finally {
                UpdateIndeterminate(false);
            }
        }

        public static SimpleDelegateCommand<OutlookFolderViewModel> SyncItemCommand => new(async (folderViewModel) => {
            try {
                OutlookFolder folder = (OutlookFolder)folderViewModel.Folder;
                folderViewModel.UpdateIndeterminate(true);
                await Task.Run(() => {
                    folder.SyncItems();
                });
            } finally {
                folderViewModel.UpdateIndeterminate(false);
            }
            folderViewModel.LoadItems();

        });

    }
}

