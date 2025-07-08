using System.IO;
using AIChatExplorer.Model.Folders.FileSystem;
using AIChatExplorer.Model.Folders.Search;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;

namespace AIChatExplorer.Model.Folders.ShortCut {
    public class ShortCutFolder : FileSystemFolder {

        // コンストラクタ
        public ShortCutFolder() : base() {
            FolderTypeString = FolderManager.SHORTCUT_ROOT_FOLDER_NAME_EN;
        }

        public ShortCutFolder(FileSystemFolder parent, string folderName) : base(parent, folderName) {
            FolderTypeString = FolderManager.SHORTCUT_ROOT_FOLDER_NAME_EN;
        }

        public override ShortCutFolder CreateChild(string folderName) {
            ShortCutFolder child = new(this, folderName);
            return child;
        }

        public override void SyncFolders() {
            // SyncFoldersを実行
            if (IsRootFolder) {
                return;
            }
            // SyncFoldersを実行
            base.SyncFolders();
        }

        // ファイルシステム上のフォルダのフルパス一覧のHashSetを取得する。
        protected override HashSet<string> GetFileSystemFolderPaths() {
            HashSet<string> fileSystemFolderPaths = [];
            if (IsRootFolder) {
                return fileSystemFolderPaths;
            }
            try {
                fileSystemFolderPaths = [.. Directory.GetDirectories(FileSystemFolderPath)];
            } catch (UnauthorizedAccessException e) {
                LogWrapper.Info($"Access Denied:{FileSystemFolderPath} {e.Message}");
            } catch(IOException e) {
                LogWrapper.Info($"IOException:{FileSystemFolderPath} {e.Message}");
            }

            return fileSystemFolderPaths;
        }

        // Folders内のFileSystemFolderPathとContentFolderのDictionary
        protected override Dictionary<string, ContentFolderWrapper> GetFolderPathIdDict() {
            // GetChildrenを実行すると無限ループになるため、Entity.GetChildren()を使用
            var folders = Entity.GetChildren().Select(x => new ShortCutFolder() { Entity =x}).ToList();

            Dictionary<string, ContentFolderWrapper> folderPathIdDict = [];
            foreach (var folder in folders) {
                // folder.FileSystemFolderPathが存在する場合
                if (!string.IsNullOrEmpty(folder.FileSystemFolderPath)) {
                    folderPathIdDict[folder.FileSystemFolderPath] = folder;
                }
            }
            return folderPathIdDict;
        }


    }
}
