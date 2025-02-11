using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;

namespace ClipboardApp.ViewModel.Folders.FileSystem {
    public class FileSystemFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) : ClipboardFolderMenu(clipboardFolderViewModel) {

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems {
            get {
                #region 全フォルダ共通
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];

                // 編集
                MenuItem editMenuItem = new() {
                    Header = StringResources.Edit,
                    Command = ClipboardFolderViewModel.EditFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(editMenuItem);

                // ショートカット登録
                MenuItem createShortCutMenuItem = new() {
                    Header = StringResources.CreateShortCut,
                    Command = FileSystemFolderViewModel.CreateShortCutCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(createShortCutMenuItem);

                //テキストの抽出
                MenuItem extractTextMenuItem = new() {
                    Header = StringResources.ExtractText,
                    Command = ClipboardFolderViewModel.ExtractTextCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(extractTextMenuItem);

                // ベクトルのリフレッシュ
                MenuItem refreshMenuItem = new() {
                    Header = StringResources.RefreshVectorDB,
                    Command = ClipboardFolderViewModel.RefreshVectorDBCollectionCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(refreshMenuItem);

                // エクスポート/インポート
                MenuItem exportImportMenuItem = new() {
                    Header = StringResources.ExportImport,
                    Command = LibUIPythonAI.ViewModel.Folder.ContentFolderViewModel.ExportImportFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(exportImportMenuItem);

                return menuItems;

                #endregion
            }
        }
    }
}
