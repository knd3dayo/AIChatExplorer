using System.IO;
using AIChatExplorer.Model.Folders.Browser;
using AIChatExplorer.Model.Folders.Clipboard;
using AIChatExplorer.Model.Main;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace AIChatExplorer.Model.Folders.FileSystem {
    public class FileSystemFolder : ClipboardFolder {

        // コンストラクタ
        public FileSystemFolder(ContentFolderEntity folder) : base(folder) {
            IsAutoProcessEnabled = false;
            FolderTypeString = AIChatExplorerFolderManager.FILESYSTEM_ROOT_FOLDER_NAME_EN;
        }

        protected FileSystemFolder(FileSystemFolder parent, string folderName) : base(parent, folderName) {
            // ルートフォルダの場合は FileSystemFolderPath = "" とする。それ以外は、親フォルダのFileSystemFolderPath + FolderName
            if (IsRootFolder) {
                FileSystemFolderPath = "";
            } else {
                string parentFileSystemFolderPath = parent.FileSystemFolderPath ?? "";
                FileSystemFolderPath = Path.Combine(parentFileSystemFolderPath, folderName);
            }
            FolderTypeString = AIChatExplorerFolderManager.FILESYSTEM_ROOT_FOLDER_NAME_EN;

        }

        public override FileSystemFolder CreateChild(string folderName) {
            FileSystemFolder child = new(this , folderName);
            return child;
        }

        public override List<T> GetItems<T>() {
            // SyncItems
            SyncItems();
            return base.GetItems<T>();
        }

        // 子フォルダ
        public override List<T> GetChildren<T>() {
            // SyncFolders
            SyncFolders();
            return base.GetChildren<T>();
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
        protected virtual Dictionary<string, ContentFolderWrapper> GetFolderPathIdDict() {
            // GetChildrenを実行すると無限ループになるため、Entity.GetChildren()を使用
            var folders = Entity.GetChildren().Select(x => new FileSystemFolder(x)).ToList();

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


        public virtual void SyncItems() {
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

            // GetItemsを実行すると無限ループになるため、Entity.GetContentItems()を使用
            var items = Entity.GetContentItems().Select(x => new FileSystemItem(x)).ToList();

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
            if (deleteItems.Count > 0) {
                ContentItemWrapper.DeleteItems(deleteItems);
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

            Parallel.ForEach(addFilePaths, parallelOptions, localFileSystemFilePath => {

                ContentItemWrapper contentItem = new(this.Entity) {
                    Description = Path.GetFileName(localFileSystemFilePath),
                    SourcePath = localFileSystemFilePath,
                    SourceType = ContentSourceType.File,
                    ContentType = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files,
                    UpdatedAt = File.GetLastWriteTime(localFileSystemFilePath),
                    CreatedAt = File.GetCreationTime(localFileSystemFilePath),

                };
                addItems.Add(contentItem);
                contentItem.Save();
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
                    contentItem.Save();
                }
            });

        }
        // ProcessClipboardItem
        public override void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ContentItemWrapper> _afterClipboardChanged) {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }

    }
}
