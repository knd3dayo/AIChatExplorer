using System.Collections.ObjectModel;
using System.Windows.Controls;
using ClipboardApp.ViewModel.Folders.Clipboard;
using LibUIPythonAI.Resource;

namespace ClipboardApp.ViewModel.Folders.FileSystem {
    public class FileSystemFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) : ClipboardFolderMenu(clipboardFolderViewModel) {

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

        // CreateShortCut
        public MenuItem CreateShortCutMenuItem {
            get {
                MenuItem createShortCutMenuItem = new() {
                    Header = CommonStringResources.Instance.CreateShortCut,
                    Command = FileSystemFolderViewModel.CreateShortCutCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                return createShortCutMenuItem;
            }
        }
    }
}
