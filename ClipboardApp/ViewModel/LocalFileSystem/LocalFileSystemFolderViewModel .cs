using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.ViewModel.ClipboardItemView;
using QAChat.Control;

namespace ClipboardApp.ViewModel.LocalFileSystem {
    public class LocalFileSystemFolderViewModel(ClipboardFolder clipboardItemFolder) : ClipboardFolderViewModel(clipboardItemFolder) {

        // 子フォルダのClipboardFolderViewModelを作成するメソッド
        public override ClipboardFolderViewModel CreateChildFolderViewModel(ClipboardFolder childFolder) {
            // ローカルファイルのファイル作成は行わない
            throw new NotImplementedException();
        }

        // アイテム作成コマンドの実装. 画像チェックの場合は、画像チェックー画面を開く
        public override void CreateItemCommandExecute() {
            // ローカルファイルのファイル作成は行わない

        }

        public override void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // ローカルファイルのフォルダ作成は行わない

        }
        public override void EditFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {
            // ローカルファイルのフォルダ編集は行わない
        }

    }
}

