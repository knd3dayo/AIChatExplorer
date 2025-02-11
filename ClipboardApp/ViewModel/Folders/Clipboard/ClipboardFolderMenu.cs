using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace ClipboardApp.ViewModel.Folders.Clipboard {
    public class ClipboardFolderMenu : ClipboardAppViewModelBase {

        public ClipboardFolderViewModel ClipboardFolderViewModel { get; private set; }

        public ClipboardFolderMenu(ClipboardFolderViewModel clipboardFolderViewModel) {
            ClipboardFolderViewModel = clipboardFolderViewModel;
        }

        // -- virtual
        public virtual ObservableCollection<MenuItem> MenuItems {
            get {
                #region 全フォルダ共通
                // MenuItemのリストを作成
                ObservableCollection<MenuItem> menuItems = [];
                // 新規作成
                MenuItem createMenuItem = new() {
                    Header = StringResources.Create,
                    Command = ClipboardFolderViewModel.CreateFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(createMenuItem);

                // 編集
                MenuItem editMenuItem = new() {
                    Header = StringResources.Edit,
                    Command = ClipboardFolderViewModel.EditFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                menuItems.Add(editMenuItem);

                // 削除
                MenuItem deleteMenuItem = new();
                deleteMenuItem.Header = StringResources.Delete;
                deleteMenuItem.Command = ClipboardFolderViewModel.DeleteFolderCommand;
                deleteMenuItem.IsEnabled = ClipboardFolderViewModel.IsDeleteVisible;
                deleteMenuItem.CommandParameter = ClipboardFolderViewModel;
                menuItems.Add(deleteMenuItem);

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
