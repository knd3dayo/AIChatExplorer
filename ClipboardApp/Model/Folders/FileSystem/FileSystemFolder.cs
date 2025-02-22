using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using ClipboardApp.Model.Folders.Clipboard;
using ClipboardApp.Model.Item;
using ClipboardApp.Model.Main;
using LibPythonAI.Utils.Common;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.PythonIF;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folders.FileSystem {
    public class FileSystemFolder : ClipboardFolder {


        public static List<string> TargetMimeTypes { get; set; } = [
            "text/",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        ];

        // コンストラクタ
        public FileSystemFolder(ContentFolder folder) : base(folder) {
            IsAutoProcessEnabled = false;
            FolderType = FolderTypeEnum.FileSystem;
        }

        protected FileSystemFolder(FileSystemFolder parent, string folderName) : base(parent, folderName) {
            // ルートフォルダの場合は FileSystemFolderPath = "" とする。それ以外は、親フォルダのFileSystemFolderPath + FolderName
            if (IsRootFolder) {
                FileSystemFolderPath = "";
            } else {
                string parentFileSystemFolderPath = parent.FileSystemFolderPath ?? "";
                FileSystemFolderPath = Path.Combine(parentFileSystemFolderPath, folderName);
            }
            FolderType = FolderTypeEnum.FileSystem;

        }

        public override FileSystemFolder CreateChild(string folderName) {
            FileSystemFolder child = new(this, folderName);
            return child;
        }

        public override FileSystemFolder? GetParent() {
            var parentFolder = ContentFolderInstance.GetParent();
            if (parentFolder == null) {
                return null;
            }
            return new FileSystemFolder(parentFolder);
        }

        public override List<ContentItemWrapper> GetItems() {
            // SyncItems
            SyncItems();
            var items = ContentFolderInstance.GetItems<ContentItem>();
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
            var children = ContentFolderInstance.GetChildren<ContentFolder>();
            List<ContentFolderWrapper> result = [];
            foreach (var child in children) {
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
        protected virtual Dictionary<string, ContentFolder> GetFolderPathIdDict() {
            // コレクション
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<ContentFolder>();
            var folders = collection.Find(x => x.ParentId == Id).Select(x => new FileSystemFolder(x)).ToList();

            Dictionary<string, ContentFolder> folderPathIdDict = [];
            foreach (var folder in folders) {
                // folder.FileSystemFolderPathが存在する場合
                if (!string.IsNullOrEmpty(folder.FileSystemFolderPath)) {
                    folderPathIdDict[folder.FileSystemFolderPath] = folder.ContentFolderInstance;
                }
            }
            return folderPathIdDict;
        }

        public virtual void SyncFolders() {

            // Folders内のFileSystemFolderPathとIDのDictionary
            Dictionary<string, ContentFolder> folderPathIdDict = GetFolderPathIdDict();
            // ファイルシステム上のフォルダのフルパス一覧のHashSet
            HashSet<string> fileSystemFolderPaths = GetFileSystemFolderPaths();

            // folders内に、fileSystemFolderPaths以外のFolderPathがある場合は削除
            // Exceptで差集合を取得
            var deleteFolderPaths = folderPathIdDict.Keys.Except(fileSystemFolderPaths);
            foreach (var deleteFolderPath in deleteFolderPaths) {
                ContentFolder contentFolder = folderPathIdDict[deleteFolderPath];
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
            }

            // コレクション
            var collection = PythonAILibManager.Instance.DataFactory.GetItemCollection<ContentItem>();
            var items = collection.Find(x => x.CollectionId == Id);

            // Items内のFilePathとContentItemのDictionary
            Dictionary<string, ContentItem> itemFilePathIdDict = [];
            foreach (var item in items) {
                itemFilePathIdDict[item.FilePath] = item;
            }

            // ファイルシステム上のファイルパス一覧に、items内にないファイルがある場合は削除
            // Exceptで差集合を取得
            var deleteFilePaths = itemFilePathIdDict.Keys.Except(fileSystemFilePathSet);
            foreach (var deleteFilePath in deleteFilePaths) {
                ContentItem contentItem = itemFilePathIdDict[deleteFilePath];
                contentItem.Delete();
            }
            // Items内のファイルパス一覧に、fileSystemFilePathsにないファイルがある場合は追加
            // Exceptで差集合を取得
            var addFilePaths = fileSystemFilePathSet.Except(itemFilePathIdDict.Keys);

            // itemsのアイテムに、filePathがFileSystemFilePathsにない場合はアイテムを追加
            var targetMimeTypesSet = new HashSet<string>(TargetMimeTypes);
            ParallelOptions parallelOptions = new() {
                MaxDegreeOfParallelism = 8
            };

            Parallel.ForEach(addFilePaths, parallelOptions, localFileSystemFilePath => {
                string contentType = PythonExecutor.PythonAIFunctions.GetMimeType(localFileSystemFilePath);

                ContentItem contentItem = new() {
                    CollectionId = Id,
                    Description = Path.GetFileName(localFileSystemFilePath),
                    SourcePath = localFileSystemFilePath,
                    FilePath = localFileSystemFilePath,
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
            Dictionary<string, ContentItem> oldItemsDict = [];
            Parallel.ForEach(updateFilePaths, parallelOptions, localFileSystemFilePath => {
                ContentItem contentItem = itemFilePathIdDict[localFileSystemFilePath];
                if (contentItem.UpdatedAt.Ticks < File.GetLastWriteTime(localFileSystemFilePath).Ticks) {
                    contentItem.Content = "";
                    contentItem.UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath);
                    contentItem.Save(false, false);
                }
            });

        }

    }
}
