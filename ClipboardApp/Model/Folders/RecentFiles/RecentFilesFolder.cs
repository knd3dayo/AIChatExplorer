using System.IO;
using ClipboardApp.Model.Folders.FileSystem;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.FileUtils;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.PythonIF;

namespace ClipboardApp.Model.Folders.Browser {
    public class RecentFilesFolder : FileSystemFolder {

        // コンストラクタ
        public RecentFilesFolder(ContentFolder folder) : base(folder) {
            IsAutoProcessEnabled = false;
            FolderType = FolderTypeEnum.EdgeBrowseHistory;
        }

        protected RecentFilesFolder(RecentFilesFolder parent, string folderName) : base(parent, folderName) {
            // ルートフォルダの場合は FileSystemFolderPath = "" とする。それ以外は、親フォルダのFileSystemFolderPath + FolderName
            if (IsRootFolder) {
                FileSystemFolderPath = "";
            } else {
                string parentFileSystemFolderPath = parent.FileSystemFolderPath ?? "";
                FileSystemFolderPath = Path.Combine(parentFileSystemFolderPath, folderName);
            }
            FolderType = FolderTypeEnum.EdgeBrowseHistory;

        }

        public override RecentFilesFolder CreateChild(string folderName) {
            RecentFilesFolder child = new(this, folderName);
            return child;
        }

        public override RecentFilesFolder? GetParent() {
            var parentFolder = ContentFolderInstance.GetParent();
            if (parentFolder == null) {
                return null;
            }
            return new RecentFilesFolder(parentFolder);
        }

        public override List<ContentItemWrapper> GetItems() {
            // SyncItems
            SyncItems();
            var items = ContentFolderInstance.GetItems<ContentItem>();
            List<ContentItemWrapper> result = [];
            foreach (var item in items) {
                result.Add(new EdgeBrowseHistoryItem(item));
            }
            return result;
        }

        // 子フォルダ
        public override List<ContentFolderWrapper> GetChildren() {
            return []; ;
        }

        public void SyncItems() {
            // コレクション
            var collection = PythonAILibManager.Instance.DataFactory.GetItemCollection<ContentItem>();
            var items = collection.Find(x => x.CollectionId == Id);

            // Items内のSourcePathとContentItemのDictionary
            Dictionary<string, ContentItem> itemPathdDict = [];
            foreach (var item in items) {
                itemPathdDict[item.SourcePath] = item;
            }
            // 最近使用したファイルのリストを取得
            // ファイルシステム上のファイルパス一覧
            HashSet<string> fileSystemFilePathSet = ExplorerUtil.GetRecentFilePaths().ToHashSet();

            // Items内のFilePathとContentItemのDictionary
            Dictionary<string, ContentItem> itemFilePathIdDict = [];
            foreach (var item in items) {
                itemFilePathIdDict[item.SourcePath] = item;
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
            ParallelOptions parallelOptions = new() {
                MaxDegreeOfParallelism = 8
            };

            Parallel.ForEach(addFilePaths, parallelOptions, localFileSystemFilePath => {

                // ファイルが存在しない場合はスキップ
                if (!File.Exists(localFileSystemFilePath)) {
                    return;
                }

                ContentItem contentItem = new() {
                    CollectionId = Id,
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
