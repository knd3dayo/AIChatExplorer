using System.IO;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Utils.Common;

namespace ClipboardApp.Model.Folder {
    public class ShortCutFolder : FileSystemFolder {


        public override void SyncFolders() {

            if ( string.IsNullOrEmpty(FileSystemFolderPath)) {
                return;
            }

            // DBからParentIDが自分のIDのものを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<FileSystemFolder>();
            var folders = collection.FindAll().Where(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            // ファイルシステム上のフォルダのフルパス一覧
            List<string> fileSystemFolderPaths = [];
            // ルートフォルダの場合、配下のフォルダはショートカット。何もしない。
            if (IsRootFolder) {
                return;
            }
            // ルートフォルダ以外は自分自身のFolderPath配下のフォルダを取得
            try {
                fileSystemFolderPaths = Directory.GetDirectories(FileSystemFolderPath).ToList();
            } catch (UnauthorizedAccessException e) {
                LogWrapper.Info($"Access Denied:{FileSystemFolderPath} {e.Message}");
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
                    child.Save<FileSystemFolder, ClipboardItem>();
                }
            }
            // 自分自身を保存
            this.Save<FileSystemFolder, ClipboardItem>();
        }

    }
}
