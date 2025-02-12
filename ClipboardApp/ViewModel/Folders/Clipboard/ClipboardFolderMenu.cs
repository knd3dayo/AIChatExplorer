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
                menuItems.Add(CreateMenuItem);

                // 編集
                menuItems.Add(EditMenuItem);

                // 削除
                menuItems.Add(DeleteMenuItem);

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

        // 新規作成
        public MenuItem CreateMenuItem {
            get {
                MenuItem createMenuItem = new() {
                    Header = StringResources.Create,
                    Command = ClipboardFolderViewModel.CreateFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                return createMenuItem;
            }
        }
        // 編集
        public MenuItem EditMenuItem {
            get {
                MenuItem editMenuItem = new() {
                    Header = StringResources.Edit,
                    Command = ClipboardFolderViewModel.EditFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                return editMenuItem;
            }
        }
        // 削除
        public MenuItem DeleteMenuItem {
            get {
                MenuItem deleteMenuItem = new();
                deleteMenuItem.Header = StringResources.Delete;
                deleteMenuItem.Command = ClipboardFolderViewModel.DeleteFolderCommand;
                deleteMenuItem.IsEnabled = ClipboardFolderViewModel.IsDeleteVisible;
                deleteMenuItem.CommandParameter = ClipboardFolderViewModel;
                return deleteMenuItem;
            }
        }

        //テキストの抽出
        public MenuItem ExtractTextMenuItem {
            get {
                MenuItem extractTextMenuItem = new() {
                    Header = StringResources.ExtractText,
                    Command = ClipboardFolderViewModel.ExtractTextCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                return extractTextMenuItem;
            }
        }
        // ベクトルのリフレッシュ
        public MenuItem RefreshMenuItem {
            get {
                MenuItem refreshMenuItem = new() {
                    Header = StringResources.RefreshVectorDB,
                    Command = ClipboardFolderViewModel.RefreshVectorDBCollectionCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                return refreshMenuItem;
            }
        }

        // エクスポート/インポート
        public MenuItem ExportImportMenuItem {
            get {
                MenuItem exportImportMenuItem = new() {
                    Header = StringResources.ExportImport,
                    Command = LibUIPythonAI.ViewModel.Folder.ContentFolderViewModel.ExportImportFolderCommand,
                    CommandParameter = ClipboardFolderViewModel
                };
                return exportImportMenuItem;
            }
        }

    }
}
