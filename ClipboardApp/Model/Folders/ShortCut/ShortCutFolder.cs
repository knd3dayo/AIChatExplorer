using System.IO;
using System.Linq;
using ClipboardApp.Model.Folders.FileSystem;
using ClipboardApp.Model.Item;
using ClipboardApp.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Utils.Common;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;

namespace ClipboardApp.Model.Folders.ShortCut {
    public class ShortCutFolder : FileSystemFolder {

        // コンストラクタ
        public ShortCutFolder(ContentFolderEntity folder) : base(folder) {
            FolderTypeString = FolderManager.SEARCH_ROOT_FOLDER_NAME_EN;
        }

        public ShortCutFolder(FileSystemFolder parent, string folderName) : base(parent, folderName) {
            FolderTypeString = FolderManager.SEARCH_ROOT_FOLDER_NAME_EN;
        }

        public override ShortCutFolder? GetParent() {
            var parentFolder = Entity.Parent;
            if (parentFolder == null) {
                return null;
            }
            return new ShortCutFolder(parentFolder);
        }

        public override List<ContentItemWrapper> GetItems() {
            SyncItems();
            var items = Entity.Children;
            List<ContentItemWrapper> result = [];
            foreach (var item in items) {
                result.Add(new ShortCutItem(item));
            }
            return result;
        }

        // 子フォルダ
        public override List<ContentFolderWrapper> GetChildren() {
            // RootFolder以外の場合はSyncFoldersを実行
            if (!IsRootFolder) {
                SyncFolders();
            }
            var children = Entity.Children;
            List<ContentFolderWrapper> result = [];
            foreach (var child in children) {
                result.Add(new ShortCutFolder(child));
            }
            return result;
        }
        // ファイルシステム上のフォルダのフルパス一覧のHashSetを取得する。
        protected override HashSet<string> GetFileSystemFolderPaths() {
            HashSet<string> fileSystemFolderPaths = [];
            if (IsRootFolder) {
                return fileSystemFolderPaths;
            }
            try {
                fileSystemFolderPaths = new HashSet<string>(Directory.GetDirectories(FileSystemFolderPath));
            } catch (UnauthorizedAccessException e) {
                LogWrapper.Info($"Access Denied:{FileSystemFolderPath} {e.Message}");
            }
            return fileSystemFolderPaths;
        }

        // Folders内のFileSystemFolderPathとContentFolderのDictionary
        protected override Dictionary<string, ContentFolderWrapper> GetFolderPathIdDict() {
            // コレクション
            using PythonAILibDBContext context = new();
            var folders = context.ContentFolders.Where(x => x.ParentId == Entity.Id).Select(x => new ShortCutFolder(x)).ToList();

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
