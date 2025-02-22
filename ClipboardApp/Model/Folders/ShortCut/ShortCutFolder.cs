using System.IO;
using System.Linq;
using ClipboardApp.Model.Folders.FileSystem;
using ClipboardApp.Model.Item;
using LibPythonAI.Utils.Common;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;

namespace ClipboardApp.Model.Folders.ShortCut {
    public class ShortCutFolder : FileSystemFolder {

        // コンストラクタ
        public ShortCutFolder(ContentFolder folder) : base(folder) {
            FolderType = FolderTypeEnum.ShortCut;
        }

        public ShortCutFolder(FileSystemFolder parent, string folderName) : base(parent, folderName) {
            FolderType = FolderTypeEnum.ShortCut;
        }

        public override ShortCutFolder? GetParent() {
            var parentFolder = ContentFolderInstance.GetParent();
            if (parentFolder == null) {
                return null;
            }
            return new ShortCutFolder(parentFolder);
        }

        public override List<ContentItemWrapper> GetItems() {
            SyncItems();
            var items = ContentFolderInstance.GetItems<ContentItem>();
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
            var children = ContentFolderInstance.GetChildren<ContentFolder>();
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
        protected override Dictionary<string, ContentFolder> GetFolderPathIdDict() {
            // コレクション
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
            var folders = collection.Find(x => x.ParentId == Id).Select(x => new ShortCutFolder(x)).ToList();

            Dictionary<string, ContentFolder> folderPathIdDict = [];
            foreach (var folder in folders) {
                // folder.FileSystemFolderPathが存在する場合
                if (!string.IsNullOrEmpty(folder.FileSystemFolderPath)) {
                    folderPathIdDict[folder.FileSystemFolderPath] = folder.ContentFolderInstance;
                }
            }
            return folderPathIdDict;
        }


    }
}
