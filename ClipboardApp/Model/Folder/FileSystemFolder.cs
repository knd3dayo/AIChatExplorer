using System.IO;
using ClipboardApp.Factory;
using ClipboardApp.Model.Item;
using ClipboardApp.Settings;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.PythonIF;
using PythonAILib.Utils.Common;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folder {
    public class FileSystemFolder : ClipboardFolder {

        public override void Save() {
            Save<FileSystemFolder, FileSystemItem>();
        }
        // 削除
        public override void Delete() {
            DeleteFolder<FileSystemFolder, FileSystemItem>(this);
        }
        // 親フォルダ
        public override FileSystemFolder? GetParent() {
            return GetParent<FileSystemFolder>();
        }


        private string _fileSystemFolderPath = "";
        public string FileSystemFolderPath {
            get {
                return _fileSystemFolderPath ?? "";
            }
            set {
                if (_fileSystemFolderPath == null) {
                    value = "";
                }
                _fileSystemFolderPath = value;
            }
        }
        public static List<string> TargetMimeTypes { get; set; } = [
            "text/",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        ];

        // コンストラクタ
        public FileSystemFolder() { }
        protected FileSystemFolder(FileSystemFolder parent, string folderName) : base(parent, folderName) {
            FolderType = FolderTypeEnum.FileSystem;
            // ルートフォルダの場合は FileSystemFolderPath = "" とする。それ以外は、親フォルダのFileSystemFolderPath + FolderName
            if (IsRootFolder) {
                FileSystemFolderPath = "";
            } else {
                string parentFileSystemFolderPath = parent.FileSystemFolderPath ?? "";
                FileSystemFolderPath = Path.Combine(parentFileSystemFolderPath, folderName);
            }
        }

        // ProcessClipboardItem
        public override void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ContentItem> _afterClipboardChanged) {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }

        public override FileSystemFolder CreateChild(string folderName) {
            FileSystemFolder child = new(this, folderName);
            return child;
        }


        [BsonIgnore]
        public override List<T> GetItems<T>() {
            // ローカルファイルシステムとClipboardFolderのファイルを同期
            // SyncItems();

            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemCollection<T>();
            // FileSystemFolderPathフォルダ内のファイルを取得
            List<T> items = [.. collection.Find(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt)];

            return items;
        }
        public void SyncItems() {
            // FileSystemFolderPathフォルダ内のファイルを取得. FileSystemFolderPathが存在しない場合は処理しない
            if (!Directory.Exists(FileSystemFolderPath)) {
                return;
            }
            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemCollection<FileSystemItem>();
            // FileSystemFolderPathフォルダ内のファイルを取得
            List<FileSystemItem> items = [.. collection.Find(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt)];

            // ファイルシステム上のファイル一覧
            List<string> fileSystemFilePaths = [];
            try {
                fileSystemFilePaths = Directory.GetFiles(FileSystemFolderPath).ToList();
            } catch (UnauthorizedAccessException e) {
                LogWrapper.Info($"Access Denied:{FileSystemFolderPath} {e.Message}");
            }
            // items内に、fileSystemFilePaths以外のFilePathがある場合は削除
            foreach (var item in items) {
                if (!fileSystemFilePaths.Any(x => x == item.FilePath)) {
                    collection.Delete(item.Id);
                }
            }
            // itemsのアイテムに、filePathがFileSystemFilePathsにない場合はアイテムを追加
            foreach (var localFileSystemFilePath in fileSystemFilePaths) {
                // GetMimeTypeを実行して、ファイルのContentTypeを取得
                string contentType = PythonExecutor.PythonAIFunctions.GetMimeType(localFileSystemFilePath);
                // TargetMimeTypesに含まれるContentTypeの場合のみ処理
                if (!TargetMimeTypes.Any(x => contentType.StartsWith(x))) {
                    continue;
                }
                if (!items.Any(x => x.FilePath == localFileSystemFilePath)) {
                    FileSystemItem item = new() {
                        FilePath = localFileSystemFilePath,
                        SourcePath = localFileSystemFilePath,
                        CollectionId = Id,
                        ContentType = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files,
                        Description = Path.GetFileName(localFileSystemFilePath)
                    };
                    // 自動処理ルールを適用
                    Task<ContentItem> task = AutoProcessRuleController.ApplyGlobalAutoAction(item);
                    ContentItem result = task.Result;
                    result.Save();

                }
            }
        }

        // 子フォルダ
        public override List<FileSystemFolder> GetChildren<FileSystemFolder>() {
            // ローカルファイルシステムとClipboardFolderのフォルダを同期
            SyncFolders();
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<FileSystemFolder>();
            var folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders.Cast<FileSystemFolder>().ToList();

        }

        public virtual void SyncFolders() {

            // DBからParentIDが自分のIDのものを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<FileSystemFolder>();
            var folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            // ファイルシステム上のフォルダのフルパス一覧
            List<string> fileSystemFolderPaths = [];
            // ルートフォルダの場合は、Environment.GetLogicalDrives()を取得
            if (IsRootFolder) {
                string[] drives = Environment.GetLogicalDrives();
                foreach (var drive in drives) {
                    fileSystemFolderPaths.Add(drive);
                }
            } else {
                // ルートフォルダ以外は自分自身のFolderPath配下のフォルダを取得
                try {
                    fileSystemFolderPaths = Directory.GetDirectories(FileSystemFolderPath).ToList();
                } catch (UnauthorizedAccessException e) {
                    LogWrapper.Info($"Access Denied:{FileSystemFolderPath} {e.Message}");
                }
            }
            // folders内に、fileSystemFolderPaths以外のFolderPathがある場合は削除
            foreach (var folder in folders) {
                if (!fileSystemFolderPaths.Any(x => x == folder.FileSystemFolderPath)) {
                    collection.Delete(folder.Id);
                }
            }
            // foldersのアイテムに、folderPath=ドライブ名:\のアイテムがない場合はアイテムを追加
            foreach (var localFileSystemFolder in fileSystemFolderPaths) {
                if (!folders.Any(x => x.FileSystemFolderPath == localFileSystemFolder)) {
                    if (IsRootFolder) {
                        FileSystemFolder child = CreateChild(localFileSystemFolder);
                        child.Save();
                    } else {
                        // localFileSystemFolder からフォルダ名を取得
                        string folderName = Path.GetFileName(localFileSystemFolder);
                        FileSystemFolder child = CreateChild(folderName);
                        child.Save();
                    }
                }
            }
            // 自分自身を保存
            this.Save();
        }

    }
}
