using System.IO;
using ClipboardApp.Utils;
using LiteDB;
using Microsoft.VisualBasic.Logging;
using PythonAILib.Common;
using PythonAILib.Utils.Common;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.Model.Folder {
    public class FileSystemFolder : ClipboardFolder {

        public string FileSystemFolderPath { get; set; } = "";
        // コンストラクタ
        public FileSystemFolder() { }
        protected FileSystemFolder(FileSystemFolder parent, string folderName) : base(parent, folderName) {
            FolderType = FolderTypeEnum.FileSystem;
            // ルートフォルダの場合は FileSystemFolderPath = "" とする。それ以外は、親フォルダのFileSystemFolderPath + FolderName
            if (parent.IsRootFolder) {
                FileSystemFolderPath = "";
            } else {
                FileSystemFolderPath = Path.Combine(parent.FileSystemFolderPath, folderName);
            }
        }


        // ProcessClipboardItem
        public override void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ClipboardItem> _afterClipboardChanged) {
            // ローカルファイルのフォルダは処理しない
            throw new NotImplementedException();
        }

        public override FileSystemFolder CreateChild(string folderName) {
            FileSystemFolder child = new(this, folderName);
            return child;
        }

        public void SyncFolders()   {
            if (FileSystemFolderPath == null) {
                this.Delete();
                return;
            }
            // DBからParentIDが自分のIDのものを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<FileSystemFolder>();
            var folders = collection.FindAll().Where(x => x.ParentId == Id).OrderBy(x => x.FolderName);
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
                }catch(UnauthorizedAccessException e) {
                    LogWrapper.Info($"Access Denied:{FileSystemFolderPath}");
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
                    // localFileSystemFolder からフォルダ名を取得
                    string folderName = Path.GetFileName(localFileSystemFolder);
                    FileSystemFolder child = CreateChild(folderName);
                    child.Save();
                }
            }
            // 自分自身を保存
            this.Save();
        }
        // 子フォルダ
        public override List<T> GetChildren<T>() {
            // ローカルファイルシステムとClipboardFolderのフォルダを同期
            SyncFolders();
            var currentFolder = this;

            // DBからParentIDが自分のIDのものを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>();
            var folders = collection.FindAll().Where(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders.Cast<T>().ToList();

        }
        // アイテム LiteDBには保存しない。
        [BsonIgnore]
        public virtual List<ClipboardItem> Items {
            get {
                return ClipboardFolderUtil.GetNormalFolderItems(this);
            }
        }


    }
}
