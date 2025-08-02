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
        public override async Task<List<T>> GetChildren<T>() {
            await Task.CompletedTask;
            return []; ;
        }

        public override async Task SyncItems() {
            // GetItems(true)を実行すると無限ループになるため、GetItems(false)を使用
            var items = await base.GetItems<ContentItemWrapper>(false);

            // Items内のSourcePathとContentItemのDictionary
            Dictionary<string, ContentItemWrapper> itemPathDict = [];
            foreach (var item in items) {
                itemPathDict[item.SourcePath] = item;
            }
            // 最近使用したファイルのリストを取得
            // ファイルシステム上のファイルパス一覧
            HashSet<string> fileSystemFilePathSet = ExplorerUtil.GetRecentFilePaths().ToHashSet();

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
                await contentItem.Delete();
            }
            // Items内のファイルパス一覧に、fileSystemFilePathsにないファイルがある場合は追加
            // Exceptで差集合を取得
            var addFilePaths = fileSystemFilePathSet.Except(itemFilePathIdDict.Keys);

            // itemsのアイテムに、filePathがFileSystemFilePathsにない場合はアイテムを追加
            ParallelOptions parallelOptions = new() {
                MaxDegreeOfParallelism = 4
            };

            Parallel.ForEach(addFilePaths, parallelOptions, async localFileSystemFilePath => {

                // ファイルが存在しない場合はスキップ
                if (!File.Exists(localFileSystemFilePath)) {
                    return;
                }

                ContentItemWrapper contentItem = new(this.Entity) {
                    Description = Path.GetFileName(localFileSystemFilePath),
                    SourcePath = localFileSystemFilePath,
                    SourceType = ContentSourceType.File,
                    ContentType = ContentItemTypes.ContentItemTypeEnum.Files,
                    UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath),
                    CreatedAt = File.GetCreationTime(localFileSystemFilePath),

                };
                await contentItem.Save();
;
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
