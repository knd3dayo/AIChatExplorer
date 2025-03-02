using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Folders.FileSystem;
using LibPythonAI.Model.Content;

namespace ClipboardApp.ViewModel.Folders.ShortCut {
    public class ShortCutFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) : FileSystemFolderMenu(clipboardFolderViewModel) {

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems {
            get {
                #region 全フォルダ共通
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];

                // 編集
                menuItems.Add(EditMenuItem);

                // ショートカット登録
                menuItems.Add(CreateShortCutMenuItem);

                // RootFolderの場合
                ContentFolderWrapper? parentFolder = ClipboardFolderViewModel.Folder.GetParent();
                if (parentFolder != null && parentFolder.IsRootFolder) {
                    // 削除
                    menuItems.Add(DeleteMenuItem);
                }
                //テキストの抽出
                menuItems.Add(ExtractTextMenuItem);

                // ベクトルのリフレッシュ
                menuItems.Add(RefreshMenuItem);

                // エクスポート/インポート
                menuItems.Add(ExportImportMenuItem);

                return menuItems;

                #endregion
            }
        }
    }
}
