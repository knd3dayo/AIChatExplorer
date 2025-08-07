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
        public override async Task<List<T>> GetChildrenAsync<T>(bool isSync) {
            if (isSync) {
                // SyncFoldersAsync
                await SyncFoldersAsync();
            }
            return await base.GetChildrenAsync<T>(isSync);
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
            // GetChildrenAsync()を実行すると無限ループになるため、GetChildrenAsync(false)を使用
            var folders = await GetChildrenAsync<FileSystemFolder>(false);
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

        public override async Task SyncFoldersAsync() {

            // Folders内のFileSystemFolderPathとIDのDictionary
            Dictionary<string, ContentFolderWrapper> folderPathIdDict = await GetFolderPathIdDict();
            // ファイルシステム上のフォルダのフルパス一覧のHashSet
            HashSet<string> fileSystemFolderPaths = GetFileSystemFolderPaths();

            // folders内に、fileSystemFolderPaths以外のFolderPathがある場合は削除
            var deleteFolderPaths = folderPathIdDict.Keys.Except(fileSystemFolderPaths);
            List<ContentFolderWrapper> deleteFolders = [];
            foreach (var deleteFolderPath in deleteFolderPaths) {
                ContentFolderWrapper contentFolder = folderPathIdDict[deleteFolderPath];
                deleteFolders.Add(contentFolder);
            }
            if (deleteFolders.Count > 0) {
                try {
                    await ContentFolderWrapper.DeleteFoldersAsync(deleteFolders);
                } catch (Exception ex) {
                    LogWrapper.Error($"DeleteFoldersAsync failed: {ex.Message}");
                }
            }

            // folders内に、FileSystemFolderPathがない場合は追加
            var fileSystemFolderPathsSet = new HashSet<string>(fileSystemFolderPaths);
            var addFolderPaths = fileSystemFolderPathsSet.Except(folderPathIdDict.Keys);


            List<ContentFolderWrapper> addChildren = [];
            foreach (var localFileSystemFolder in addFolderPaths) {
                try {
                    string folderName = IsRootFolder ? localFileSystemFolder : Path.GetFileName(localFileSystemFolder);
                    var child = CreateChild(folderName);
                    addChildren.Add(child);
                } catch (Exception ex) {
                    LogWrapper.Error($"Add failed: {localFileSystemFolder} {ex.Message}");
                }
            }
            if (addChildren.Count > 0) {
                await ContentFolderWrapper.SaveFoldersAsync(addChildren);
            }

            // 自分自身を保存
            await SaveAsync();
        }


        public override async Task SyncItemsAsync() {
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

            // GetItemsAsync(true)を実行すると無限ループになるため、GetItemsAsync(false)を使用
            var items = await base.GetItemsAsync<ContentItemWrapper>(false);

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
            // 一括削除
            if (deleteItems.Count > 0) {
                await ContentItemWrapper.DeleteItemsAsync(deleteItems);
            }


            // 追加対象格納用のリスト
            List<ContentItemWrapper> addItems = [];
            // Items内のファイルパス一覧に、fileSystemFilePathsにないファイルがある場合は追加
            var addFilePaths = fileSystemFilePathSet.Except(itemFilePathIdDict.Keys);

            foreach (var localFileSystemFilePath in addFilePaths) {
                ContentItemWrapper contentItem = new(this.Entity) {
                    Description = Path.GetFileName(localFileSystemFilePath),
                    SourcePath = localFileSystemFilePath,
                    SourceType = ContentSourceType.File,
                    ContentType = LibPythonAI.Model.Content.ContentItemTypes.ContentItemTypeEnum.Files,
                    UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath),
                    CreatedAt = File.GetCreationTime(localFileSystemFilePath),
                };
                addItems.Add(contentItem);
            }
            // 一括保存
            if (addItems.Count > 0) {
                await ContentItemWrapper.SaveItemsAsync(addItems);
            }

            // itemFilePathIdDictの中から、fileSystemFilePathsにあるItemのみを取得
            var updateFilePaths = fileSystemFilePathSet.Intersect(itemFilePathIdDict.Keys);

            // ItemのUpdatedAtよりもファイルの最終更新日時が新しい場合は更新（非同期一括処理）
            List<ContentItemWrapper> updateItems = [];
            foreach (var localFileSystemFilePath in updateFilePaths) {
                var contentItem = itemFilePathIdDict[localFileSystemFilePath];
                if (contentItem.UpdatedAt.Ticks < File.GetLastWriteTime(localFileSystemFilePath).Ticks) {
                    contentItem.Content = "";
                    contentItem.UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath);
                    updateItems.Add(contentItem);
                }
            }
            if (updateItems.Count > 0) {
                await ContentItemWrapper.SaveItemsAsync(updateItems);
            }

        }

    }
}
