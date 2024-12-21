using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetOffice;
using Outlook = NetOffice.OutlookApi;
using NetOffice.OutlookApi.Enums;
using NetOffice.OutlookApi;
using ClipboardApp.Factory;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.PythonIF;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;
using System.IO;

namespace ClipboardApp.Model.Folder {
    public  class OutlookFolder : ClipboardFolder {

        private static Outlook.Application? outlookApplication = null;

        private static Outlook.MAPIFolder? inboxFolder = null;

        public static Outlook.MAPIFolder InboxFolder {
            get {
                if (inboxFolder == null) {
                    outlookApplication = new Outlook.Application();
                    Outlook._NameSpace outlookNamespace = outlookApplication.GetNamespace("MAPI");
                    inboxFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
                }
                return inboxFolder;
            }
        }
        /*

        // ProcessClipboardItem
        public override void ProcessClipboardItem(ClipboardChangedEventArgs e, Action<ClipboardItem> _afterClipboardChanged) {
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
            SyncItems();

            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemCollection<ClipboardItem>();
            // FileSystemFolderPathフォルダ内のファイルを取得
            List<ClipboardItem> items = [.. collection.FindAll().Where(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt)];

            return items.Cast<T>().ToList();
        }
        public void SyncItems() {
            // FileSystemFolderPathフォルダ内のファイルを取得. FileSystemFolderPathが存在しない場合は処理しない
            if (!Directory.Exists(FileSystemFolderPath)) {
                return;
            }
            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemCollection<ClipboardItem>();
            // FileSystemFolderPathフォルダ内のファイルを取得
            List<ClipboardItem> items = [.. collection.FindAll().Where(x => x.CollectionId == Id).OrderByDescending(x => x.UpdatedAt)];

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
                    ClipboardItem item = new(Id) {
                        FilePath = localFileSystemFilePath,
                        ContentType = PythonAILib.Model.File.ContentTypes.ContentItemTypes.Files,
                        Description = Path.GetFileName(localFileSystemFilePath)
                    };
                    item.Save();
                }
            }
        }

        // 子フォルダ
        public override List<FileSystemFolder> GetChildren<FileSystemFolder>() {
            // ローカルファイルシステムとClipboardFolderのフォルダを同期
            SyncFolders();
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<FileSystemFolder>();
            var folders = collection.FindAll().Where(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            return folders.Cast<FileSystemFolder>().ToList();

        }

        public virtual void SyncFolders() {

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
                        child.Save<FileSystemFolder, ClipboardItem>();
                    } else {
                        // localFileSystemFolder からフォルダ名を取得
                        string folderName = Path.GetFileName(localFileSystemFolder);
                        FileSystemFolder child = CreateChild(folderName);
                        child.Save<FileSystemFolder, ClipboardItem>();
                    }
                }
            }
            // 自分自身を保存
            this.Save<FileSystemFolder, ClipboardItem>();
        }

        */
    }
}
