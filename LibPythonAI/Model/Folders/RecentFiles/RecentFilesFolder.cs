using System.IO;
using AIChatExplorer.Model.Folders.FileSystem;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Folders;
using LibPythonAI.Utils.FileUtils;

namespace AIChatExplorer.Model.Folders.Browser {
    public class RecentFilesFolder : FileSystemFolder {

        // コンストラクタ
        public RecentFilesFolder() : base() {
            IsAutoProcessEnabled = false;
            FolderTypeString = FolderManager.RECENT_FILES_ROOT_FOLDER_NAME_EN;
        }

        protected RecentFilesFolder(RecentFilesFolder parent, string folderName) : base(parent, folderName) {
            // ルートフォルダの場合は FileSystemFolderPath = "" とする。それ以外は、親フォルダのFileSystemFolderPath + FolderName
            if (IsRootFolder) {
                FileSystemFolderPath = "";
            } else {
                string parentFileSystemFolderPath = parent.FileSystemFolderPath ?? "";
                FileSystemFolderPath = Path.Combine(parentFileSystemFolderPath, folderName);
            }
            FolderTypeString = FolderManager.RECENT_FILES_ROOT_FOLDER_NAME_EN;

        }

        public override RecentFilesFolder CreateChild(string folderName) {
            RecentFilesFolder child = new(this, folderName);
            return child;
        }


        // 子フォルダ
        public override async Task<List<T>> GetChildrenAsync<T>(bool isSync = true) {
            await Task.CompletedTask;
            return []; ;
        }

        public override async Task SyncItemsAsync() {
            // GetItemsAsync(true)を実行すると無限ループになるため、GetItemsAsync(false)を使用
            var items = await base.GetItemsAsync<ContentItem>(false);

            // Items内のSourcePathとContentItemのDictionary
            Dictionary<string, ContentItem> itemFilePathDict = [];
            foreach (var item in items) {
                itemFilePathDict[item.SourcePath] = item;
            }
            // 最近使用したファイルのリストを取得
            HashSet<string> fileSystemFilePathSet = ExplorerUtil.GetRecentFilePaths().ToHashSet();

            // ファイルシステム上のファイルパス一覧に、items内にないファイルがある場合は削除
            var deleteFilePaths = itemFilePathDict.Keys.Except(fileSystemFilePathSet).ToList();
            foreach (var deleteFilePath in deleteFilePaths) {
                ContentItem contentItem = itemFilePathDict[deleteFilePath];
                await contentItem.DeleteAsync();
            }

            // Items内のファイルパス一覧に、fileSystemFilePathsにないファイルがある場合は追加
            var addFilePaths = fileSystemFilePathSet.Except(itemFilePathDict.Keys).ToList();
            var addTasks = addFilePaths.Select(async localFileSystemFilePath => {
                if (!File.Exists(localFileSystemFilePath)) {
                    return;
                }
                ContentItem contentItem = new(this.Entity)
                {
                    Description = Path.GetFileName(localFileSystemFilePath),
                    SourcePath = localFileSystemFilePath,
                    SourceType = ContentSourceType.File,
                    ContentType = ContentItemTypes.ContentItemTypeEnum.Files,
                    UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath),
                    CreatedAt = File.GetCreationTime(localFileSystemFilePath),
                };
                await contentItem.SaveAsync();
            });
            await Task.WhenAll(addTasks);

            // itemFilePathDictの中から、fileSystemFilePathsにあるItemのみを取得
            var updateFilePaths = fileSystemFilePathSet.Intersect(itemFilePathDict.Keys).ToList();
            var updateTasks = updateFilePaths.Select(async localFileSystemFilePath => {
                ContentItem contentItem = itemFilePathDict[localFileSystemFilePath];
                if (contentItem.UpdatedAt.Ticks < File.GetLastWriteTime(localFileSystemFilePath).Ticks)
                {
                    contentItem.Content = "";
                    contentItem.UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath);
                    await contentItem.SaveAsync();
                }
            });
            await Task.WhenAll(updateTasks);
        }
    }
}
