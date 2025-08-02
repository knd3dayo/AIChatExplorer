using System.Collections.ObjectModel;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Folders.Application;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using AIChatExplorer.Model.Folders.ShortCut;
using AIChatExplorer.Model.Folders.FileSystem;
using LibPythonAI.Model.Folders;

namespace AIChatExplorer.ViewModel.Folders.FileSystem {
    public class FileSystemFolderMenu(ApplicationFolderViewModel applicationFolderViewModel) : ApplicationFolderMenu(applicationFolderViewModel) {

        // -- virtual
        public override ObservableCollection<MenuItem> MenuItems {
            get {
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
            }

        }

        // CreateShortCut
        public MenuItem CreateShortCutMenuItem {
            get {
                MenuItem createShortCutMenuItem = new() {
                    Header = CommonStringResources.Instance.CreateShortCut,
                    Command = CreateShortCutCommand,
                    CommandParameter = ApplicationFolderViewModel
                };
                return createShortCutMenuItem;
            }
        }


        // ショートカット登録コマンド
        public static SimpleDelegateCommand<FileSystemFolderViewModel> CreateShortCutCommand => new((folderViewModel) => {

            FileSystemFolder fileSystemFolder = (FileSystemFolder)folderViewModel.Folder;
            // ショートカット登録
            // ShortCutRootFolderを取得
            FileSystemFolder shortCutRootFolder = FolderManager.ShortcutRootFolder;
            // ショートカットフォルダを作成
            ShortCutFolder contentFolder = new() {
                FolderTypeString = FolderManager.SHORTCUT_ROOT_FOLDER_NAME_EN,
                Description = folderViewModel.FolderName,
                FolderName = folderViewModel.FolderName,
                Parent = shortCutRootFolder,
                FileSystemFolderPath = fileSystemFolder.FileSystemFolderPath,
            };
            contentFolder.Save();

        });

    }
}
