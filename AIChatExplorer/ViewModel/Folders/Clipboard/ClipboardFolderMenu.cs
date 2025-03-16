using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Main;
using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Resource;

namespace AIChatExplorer.ViewModel.Folders.Clipboard {
    public class ClipboardFolderMenu : ObservableObject {

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
                    Header = CommonStringResources.Instance.Create,
                    Command = ClipboardFolderViewModel.CreateFolderCommand,
                };
                return createMenuItem;
            }
        }
        // 編集
        public MenuItem EditMenuItem {
            get {
                MenuItem editMenuItem = new() {
                    Header = CommonStringResources.Instance.Edit,
                    Command = ClipboardFolderViewModel.EditFolderCommand,
                };
                return editMenuItem;
            }
        }
        // 削除
        public MenuItem DeleteMenuItem {
            get {
                MenuItem deleteMenuItem = new();
                deleteMenuItem.Header = CommonStringResources.Instance.Delete;
                deleteMenuItem.Command = ClipboardFolderViewModel.DeleteFolderCommand;
                deleteMenuItem.IsEnabled = ClipboardFolderViewModel.IsDeleteVisible;
                return deleteMenuItem;
            }
        }

        //テキストの抽出
        public MenuItem ExtractTextMenuItem {
            get {
                MenuItem extractTextMenuItem = new() {
                    Header = CommonStringResources.Instance.ExtractText,
                    Command = ClipboardFolderViewModel.ExtractTextCommand,
                };
                return extractTextMenuItem;
            }
        }
        // ベクトルのリフレッシュ
        public MenuItem RefreshMenuItem {
            get {
                MenuItem refreshMenuItem = new() {
                    Header = CommonStringResources.Instance.RefreshVectorDB,
                    Command = ClipboardFolderViewModel.RefreshVectorDBCollectionCommand,
                };
                return refreshMenuItem;
            }
        }

        // エクスポート/インポート
        public MenuItem ExportImportMenuItem {
            get {
                MenuItem exportImportMenuItem = new() {
                    Header = CommonStringResources.Instance.ExportImport,
                    Command = ClipboardFolderViewModel.ExportImportFolderCommand,
                };
                return exportImportMenuItem;
            }
        }

    }
}
