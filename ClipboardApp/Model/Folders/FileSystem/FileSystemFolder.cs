using System.IO;
using ClipboardApp.Model.Folders.Browser;
using ClipboardApp.Model.Folders.Clipboard;
using ClipboardApp.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folders.FileSystem {
    public class FileSystemFolder : ClipboardFolder {

        // コンストラクタ
        public FileSystemFolder(ContentFolderEntity folder) : base(folder) {
            IsAutoProcessEnabled = false;
            FolderTypeString = FolderManager.FILESYSTEM_ROOT_FOLDER_NAME_EN;
        }

        protected FileSystemFolder(FileSystemFolder parent, string folderName) : base(parent, folderName) {
            // ルートフォルダの場合は FileSystemFolderPath = "" とする。それ以外は、親フォルダのFileSystemFolderPath + FolderName
            if (IsRootFolder) {
                FileSystemFolderPath = "";
            } else {
                string parentFileSystemFolderPath = parent.FileSystemFolderPath ?? "";
                FileSystemFolderPath = Path.Combine(parentFileSystemFolderPath, folderName);
            }
            FolderTypeString = FolderManager.FILESYSTEM_ROOT_FOLDER_NAME_EN;

        }

        public override FileSystemFolder CreateChild(string folderName) {
            FileSystemFolder child = new(this , folderName);
            return child;
        }

        public override FileSystemFolder? GetParent() {
            using PythonAILibDBContext db = new();
            var parentFolder = db.ContentFolders.FirstOrDefault(x => x.Id == Entity.ParentId);
            if (parentFolder == null) {
                return null;
            }
            return new FileSystemFolder(parentFolder);
        }

        public override List<ContentItemWrapper> GetItems() {
            // SyncItems
            SyncItems();
            using PythonAILibDBContext context = new();
            var items = context.ContentItems.Where(x => x.FolderId == Entity.Id);
            List<ContentItemWrapper> result = [];
            foreach (var item in items) {
                result.Add(new FileSystemItem(item));
            }
            return result;
        }

        // 子フォルダ
        public override List<ContentFolderWrapper> GetChildren() {
            // SyncFolders
            SyncFolders();
            using PythonAILibDBContext context = new();
            var items = context.ContentFolders.Where(x => x.ParentId == this.Entity.Id).ToList();
            List<ContentFolderWrapper> result = [];
            foreach (var child in items) {
                result.Add(new FileSystemFolder(child));
            }
            return result;
        }

        // ProcessClipboardItem
        public override void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ContentItemWrapper> _afterClipboardChanged) {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }

        // ファイルシステム上のフォルダのフルパス一覧のHashSetを取得する。
        protected virtual HashSet<string> GetFileSystemFolderPaths() {
            HashSet<string> fileSystemFolderPaths = [];
            // ルートフォルダの場合は、Environment.GetLogicalDrives()を取得
            if (IsRootFolder) {
                string[] drives = Environment.GetLogicalDrives();
                foreach (var drive in drives) {
                    fileSystemFolderPaths.Add(drive);
                }
            } else {
                // ルートフォルダ以外は自分自身のFolderPath配下のフォルダを取得
                try {
                    fileSystemFolderPaths = new HashSet<string>(Directory.GetDirectories(FileSystemFolderPath));
                } catch (UnauthorizedAccessException e) {
                    LogWrapper.Info($"Access Denied:{FileSystemFolderPath} {e.Message}");
                }
            }
            return fileSystemFolderPaths;
        }

        // Folders内のFileSystemFolderPathとContentFolderのDictionary
        protected virtual Dictionary<string, ContentFolderWrapper> GetFolderPathIdDict() {
            // コレクション
            using PythonAILibDBContext context = new();
            var folders = context.ContentFolders.Where(x => x.ParentId == Entity.Id).Select(x => new FileSystemFolder(x)).ToList();

            Dictionary<string, ContentFolderWrapper> folderPathIdDict = [];
            foreach (var folder in folders) {
                // folder.FileSystemFolderPathが存在する場合
                if (!string.IsNullOrEmpty(folder.FileSystemFolderPath)) {
                    folderPathIdDict[folder.FileSystemFolderPath] = folder;
                }
            }
            return folderPathIdDict;
        }

        public virtual void SyncFolders() {

            // Folders内のFileSystemFolderPathとIDのDictionary
            Dictionary<string, ContentFolderWrapper> folderPathIdDict = GetFolderPathIdDict();
            // ファイルシステム上のフォルダのフルパス一覧のHashSet
            HashSet<string> fileSystemFolderPaths = GetFileSystemFolderPaths();

            // folders内に、fileSystemFolderPaths以外のFolderPathがある場合は削除
            // Exceptで差集合を取得
            var deleteFolderPaths = folderPathIdDict.Keys.Except(fileSystemFolderPaths);
            foreach (var deleteFolderPath in deleteFolderPaths) {
                ContentFolderWrapper contentFolder = folderPathIdDict[deleteFolderPath];
                contentFolder.Delete();
            }
            // folders内に、FileSystemFolderPathがない場合は追加
            // Exceptで差集合を取得
            var fileSystemFolderPathsSet = new HashSet<string>(fileSystemFolderPaths);
            var addFolderPaths = fileSystemFolderPathsSet.Except(folderPathIdDict.Keys);

            // Parallel処理
            Parallel.ForEach(addFolderPaths, localFileSystemFolder => {

                if (IsRootFolder) {
                    var child = CreateChild(localFileSystemFolder);
                    child.Save();
                } else {
                    // localFileSystemFolder からフォルダ名を取得
                    string folderName = Path.GetFileName(localFileSystemFolder);
                    var child = CreateChild(folderName);
                    child.Save();
                }
            }
            );
            // 自分自身を保存
            Save();
        }


        public void SyncItems() {
            // FileSystemFolderPathフォルダ内のファイルを取得. FileSystemFolderPathが存在しない場合は処理しない
            if (!Directory.Exists(FileSystemFolderPath)) {
                return;
            }
            // ファイルシステム上のファイルパス一覧
            HashSet<string> fileSystemFilePathSet = [];
            try {
                fileSystemFilePathSet = Directory.GetFiles(FileSystemFolderPath).ToHashSet();
            } catch (UnauthorizedAccessException e) {
                LogWrapper.Info($"Access Denied:{FileSystemFolderPath} {e.Message}");
            } catch (IOException e) {
                LogWrapper.Info($"IOException:{FileSystemFolderPath} {e.Message}");
            }

            // コレクション
            using PythonAILibDBContext context = new();
            var items = context.ContentItems.Where(x => x.FolderId == Entity.Id).Select(x => new FileSystemItem(x)).ToList();

            // Items内のFilePathとContentItemのDictionary
            Dictionary<string, ContentItemWrapper> itemFilePathIdDict = [];
            foreach (var item in items) {
                itemFilePathIdDict[item.SourcePath] = item;
            }

            // ファイルシステム上のファイルパス一覧に、items内にないファイルがある場合は削除
            // Exceptで差集合を取得
            var deleteFilePaths = itemFilePathIdDict.Keys.Except(fileSystemFilePathSet);
            foreach (var deleteFilePath in deleteFilePaths) {
                ContentItemWrapper contentItem = itemFilePathIdDict[deleteFilePath];
                contentItem.Delete();
            }
            // Items内のファイルパス一覧に、fileSystemFilePathsにないファイルがある場合は追加
            // Exceptで差集合を取得
            var addFilePaths = fileSystemFilePathSet.Except(itemFilePathIdDict.Keys);

            // itemsのアイテムに、filePathがFileSystemFilePathsにない場合はアイテムを追加
            ParallelOptions parallelOptions = new() {
                MaxDegreeOfParallelism = 8
            };

            Parallel.ForEach(addFilePaths, parallelOptions, localFileSystemFilePath => {

                ContentItemWrapper contentItem = new(this.Entity) {
                    Description = Path.GetFileName(localFileSystemFilePath),
                    SourcePath = localFileSystemFilePath,
                    SourceType = ContentSourceType.File,
                    ContentType = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files,
                    UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath),
                    CreatedAt = File.GetCreationTime(localFileSystemFilePath),

                };
                contentItem.Save(false, false);
                // 自動処理ルールを適用
                // Task<ContentItem> task = AutoProcessRuleController.ApplyGlobalAutoAction(item);
                // ContentItem result = task.Result;
                // result.Save();
            });

            // itemFilePathIdDictの中から、fileSystemFilePathsにあるItemのみを取得
            var updateFilePaths = fileSystemFilePathSet.Intersect(itemFilePathIdDict.Keys);

            // ItemのUpdatedAtよりもファイルの最終更新日時が新しい場合は更新
            Dictionary<string, ContentItemWrapper> oldItemsDict = [];
            Parallel.ForEach(updateFilePaths, parallelOptions, localFileSystemFilePath => {
                ContentItemWrapper contentItem = itemFilePathIdDict[localFileSystemFilePath];
                if (contentItem.UpdatedAt.Ticks < File.GetLastWriteTime(localFileSystemFilePath).Ticks) {
                    contentItem.Content = "";
                    contentItem.UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath);
                    contentItem.Save(false, false);
                }
            });

        }

    }
}
