using System.IO;
using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.Model.Folders.Browser;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folders;
using LibPythonAI.Utils.Common;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace AIChatExplorer.Model.Folders.FileSystem {
    public class FileSystemFolder : ApplicationFolder {

        // コンストラクタ
        public FileSystemFolder() : base() {
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

        // 子フォルダ
        public override async Task<List<T>> GetChildren<T>(bool isSync = true) {
            if (isSync) {
                // SyncFolders
                await SyncFolders();
            }
            return await base.GetChildren<T>();
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
                } catch (IOException e) {
                    LogWrapper.Info($"IOException:{FileSystemFolderPath} {e.Message}");
                }
            }
            return fileSystemFolderPaths;
        }

        // Folders内のFileSystemFolderPathとContentFolderのDictionary
        protected virtual async Task<Dictionary<string, ContentFolderWrapper>> GetFolderPathIdDict() {
            // GetChildren()を実行すると無限ループになるため、GetChildren(false)を使用
            var folders = await GetChildren<FileSystemFolder>(false);
            folders = folders.Select(x => new FileSystemFolder() { Entity = x.Entity}).ToList();

            Dictionary<string, ContentFolderWrapper> folderPathIdDict = [];
            foreach (var folder in folders) {
                // folder.FileSystemFolderPathが存在する場合
                if (!string.IsNullOrEmpty(folder.FileSystemFolderPath)) {
                    folderPathIdDict[folder.FileSystemFolderPath] = folder;
                }
            }
            return folderPathIdDict;
        }

        public virtual async Task SyncFolders() {

            // Folders内のFileSystemFolderPathとIDのDictionary
            Dictionary<string, ContentFolderWrapper> folderPathIdDict = await GetFolderPathIdDict();
            // ファイルシステム上のフォルダのフルパス一覧のHashSet
            HashSet<string> fileSystemFolderPaths = GetFileSystemFolderPaths();

            // folders内に、fileSystemFolderPaths以外のFolderPathがある場合は削除
            // Exceptで差集合を取得
            var deleteFolderPaths = folderPathIdDict.Keys.Except(fileSystemFolderPaths);
            foreach (var deleteFolderPath in deleteFolderPaths) {
                ContentFolderWrapper contentFolder = folderPathIdDict[deleteFolderPath];
                await contentFolder.Delete();
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


        public override async Task SyncItems() {
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

            // GetItems(true)を実行すると無限ループになるため、GetItems(false)を使用
            var items = await base.GetItems<ContentItemWrapper>(false);

            // Items内のFilePathとContentItemのDictionary
            Dictionary<string, ContentItemWrapper> itemFilePathIdDict = [];
            foreach (var item in items) {
                itemFilePathIdDict[item.SourcePath] = item;
            }

            // 削除対象格納用のリスト
            List<ContentItemWrapper> deleteItems = [];
            // ファイルシステム上のファイルパス一覧に、items内にないファイルがある場合は削除
            // Exceptで差集合を取得
            var deleteFilePaths = itemFilePathIdDict.Keys.Except(fileSystemFilePathSet);
            foreach (var deleteFilePath in deleteFilePaths) {
                ContentItemWrapper contentItem = itemFilePathIdDict[deleteFilePath];
                deleteItems.Add(contentItem);
            }
            // 削除対象のアイテムを削除
            foreach (var deleteItem in deleteItems) {
                // アイテムの削除
                await deleteItem.Delete();
            }

            // 追加対象格納用のリスト
            List<ContentItemWrapper> addItems = [];
            // Items内のファイルパス一覧に、fileSystemFilePathsにないファイルがある場合は追加
            // Exceptで差集合を取得
            var addFilePaths = fileSystemFilePathSet.Except(itemFilePathIdDict.Keys);

            // itemsのアイテムに、filePathがFileSystemFilePathsにない場合はアイテムを追加
            ParallelOptions parallelOptions = new() {
                MaxDegreeOfParallelism = 4
            };

            Parallel.ForEach(addFilePaths, parallelOptions, async localFileSystemFilePath => {

                ContentItemWrapper contentItem = new(this.Entity) {
                    Description = Path.GetFileName(localFileSystemFilePath),
                    SourcePath = localFileSystemFilePath,
                    SourceType = ContentSourceType.File,
                    ContentType = LibPythonAI.Model.Content.ContentItemTypes.ContentItemTypeEnum.Files,
                    UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath),
                    CreatedAt = File.GetCreationTime(localFileSystemFilePath),

                };
                addItems.Add(contentItem);
                await contentItem.Save();

            });

            // itemFilePathIdDictの中から、fileSystemFilePathsにあるItemのみを取得
            var updateFilePaths = fileSystemFilePathSet.Intersect(itemFilePathIdDict.Keys);

            // ItemのUpdatedAtよりもファイルの最終更新日時が新しい場合は更新
            Dictionary<string, ContentItemWrapper> oldItemsDict = [];
            Parallel.ForEach(updateFilePaths, parallelOptions, async localFileSystemFilePath => {
                ContentItemWrapper contentItem = itemFilePathIdDict[localFileSystemFilePath];
                if (contentItem.UpdatedAt.Ticks < File.GetLastWriteTime(localFileSystemFilePath).Ticks) {
                    contentItem.Content = "";
                    contentItem.UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath);
                    await contentItem.Save();
                }
            });

        }

    }
}
