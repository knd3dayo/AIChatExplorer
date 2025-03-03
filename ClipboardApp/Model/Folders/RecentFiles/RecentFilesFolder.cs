using System.IO;
using ClipboardApp.Model.Folders.FileSystem;
using ClipboardApp.Model.Folders.Outlook;
using ClipboardApp.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.FileUtils;
using PythonAILib.Common;

namespace ClipboardApp.Model.Folders.Browser {
    public class RecentFilesFolder : FileSystemFolder {

        // コンストラクタ
        public RecentFilesFolder(ContentFolderEntity folder) : base(folder) {
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
            ContentFolderEntity childFolder = new() {
                ParentId = Entity.Id,
                FolderName = folderName,
            };
            RecentFilesFolder child = new(childFolder);
            return child;
        }

        public override RecentFilesFolder? GetParent() {
            using PythonAILibDBContext db = new();
            var parentFolder = db.ContentFolders.FirstOrDefault(x => x.Id == Entity.ParentId);
            if (parentFolder == null) {
                return null;
            }
            return new RecentFilesFolder(parentFolder);
        }

        public override List<ContentItemWrapper> GetItems() {
            // SyncItems
            SyncItems();
            using PythonAILibDBContext context = new();
            var items = context.ContentItems.Where(x => x.FolderId == this.Entity.Id).Select(x => new ContentItemWrapper(x)).ToList();
            return items;
        }

        // 子フォルダ
        public override List<ContentFolderWrapper> GetChildren() {
            return []; ;
        }

        public void SyncItems() {
            // コレクション
            using PythonAILibDBContext context = new();
            var items = context.ContentItems.Where(x => x.FolderId == this.Entity.Id).Select(x => new RecentFilesItem(x)).ToList();

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

                // ファイルが存在しない場合はスキップ
                if (!File.Exists(localFileSystemFilePath)) {
                    return;
                }

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
