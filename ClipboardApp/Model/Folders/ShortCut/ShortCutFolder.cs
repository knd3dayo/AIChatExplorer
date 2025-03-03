using System.IO;
using ClipboardApp.Model.Folders.FileSystem;
using ClipboardApp.Model.Folders.Search;
using ClipboardApp.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;

namespace ClipboardApp.Model.Folders.ShortCut {
    public class ShortCutFolder : FileSystemFolder {

        // コンストラクタ
        public ShortCutFolder(ContentFolderEntity folder) : base(folder) {
            FolderTypeString = FolderManager.SEARCH_ROOT_FOLDER_NAME_EN;
        }

        public ShortCutFolder(FileSystemFolder parent, string folderName) : base(parent, folderName) {
            FolderTypeString = FolderManager.SEARCH_ROOT_FOLDER_NAME_EN;
        }

        public override ShortCutFolder CreateChild(string folderName) {
            ContentFolderEntity childFolder = new() {
                ParentId = Entity.Id,
                FolderName = folderName,
            };
            ShortCutFolder child = new(childFolder);
            return child;
        }

        public override ShortCutFolder? GetParent() {
            using PythonAILibDBContext db = new();
            var parentFolder = db.ContentFolders.FirstOrDefault(x => x.Id == Entity.ParentId);
            if (parentFolder == null) {
                return null;
            }
            return new ShortCutFolder(parentFolder);
        }

        public override List<ContentItemWrapper> GetItems() {
            SyncItems();
            using PythonAILibDBContext context = new();
            var items = context.ContentItems.Where(x => x.FolderId == this.Entity.Id).ToList();
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
            using PythonAILibDBContext context = new();
            var items = context.ContentFolders.Where(x => x.ParentId == this.Entity.Id).ToList();
            List<ContentFolderWrapper> result = [];
            foreach (var child in items) {
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
