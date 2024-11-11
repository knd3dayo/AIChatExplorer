using ClipboardApp.Model;
using ClipboardApp.Model.Folder;

namespace ClipboardApp.ViewModel.FileSystem {
    public class FileSystemFolderViewModel(ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(clipboardItemFolder) {


        // LoadChildren
        // 子フォルダを読み込む。nestLevelはネストの深さを指定する。1以上の値を指定すると、子フォルダの子フォルダも読み込む
        // 0を指定すると、子フォルダの子フォルダは読み込まない
        public override void LoadChildren(int nestLevel = 1) {
            Children = [];

            // Childrenがクリアされていない場合
            if (Children.Count > 0) {
                throw new Exception("Children is not cleared");
            }
            foreach (var child in ClipboardItemFolder.GetChildren<ClipboardFolder>()) {
                if (child == null) {
                    continue;
                }
                ClipboardFolderViewModel childViewModel = CreateChildFolderViewModel(child);
                // ネストの深さが1以上の場合は、子フォルダの子フォルダも読み込む
                if (nestLevel > 0) {
                    childViewModel.LoadChildren(nestLevel - 1);
                }
                Children.Add(childViewModel);
            }
            OnPropertyChanged(nameof(Children));

        }
        // LoadItems
        public override void LoadItems() {
            Items.Clear();
            foreach (ClipboardItem item in ClipboardItemFolder.Items) {

                Items.Add(CreateItemViewModel(item));
            }
        }


    }
}

