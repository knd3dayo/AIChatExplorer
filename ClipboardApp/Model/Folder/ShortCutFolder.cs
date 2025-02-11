using System.IO;
using ClipboardApp.Model.Item;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Utils.Common;

namespace ClipboardApp.Model.Folder {
    public class ShortCutFolder : FileSystemFolder {

        public override void Save() {
            Save<ShortCutFolder, FileSystemItem>();
        }
        // 削除
        public override void Delete() {
            DeleteFolder<ShortCutFolder, FileSystemItem>(this);
        }
        // 親フォルダ
        public override ShortCutFolder? GetParent() {
            return GetParent<ShortCutFolder>();
        }


        public override void SyncFolders() {

            if (string.IsNullOrEmpty(FileSystemFolderPath)) {
                return;
            }

            // DBからParentIDが自分のIDのものを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetFolderCollection<FileSystemFolder>();
            var folders = collection.Find(x => x.ParentId == Id).OrderBy(x => x.FolderName);
            // FoldersのFileSystemFolderPathとIdのDictionary
            Dictionary<string, LiteDB.ObjectId> folderPathIdDict = folders.ToDictionary(x => x.FileSystemFolderPath, x => x.Id);

            // ファイルシステム上のフォルダのフルパス一覧のHashSet
            HashSet<string> fileSystemFolderPaths = new HashSet<string>();

            // ルートフォルダの場合、配下のフォルダはショートカット。何もしない。
            if (IsRootFolder) {
                return;
            }
            // ルートフォルダ以外は自分自身のFolderPath配下のフォルダを取得
            try {
                fileSystemFolderPaths = new HashSet<string>(Directory.GetDirectories(FileSystemFolderPath));
            } catch (UnauthorizedAccessException e) {
                LogWrapper.Info($"Access Denied:{FileSystemFolderPath} {e.Message}");
            }
            // folders内に、fileSystemFolderPaths以外のFolderPathがある場合は削除
            // Exceptで差集合を取得
            HashSet<string> deleteFolderPaths = folderPathIdDict.Keys.Except(fileSystemFolderPaths).ToHashSet();

            foreach (var deleteFolderPath in deleteFolderPaths) {
                ObjectId deleteId = folderPathIdDict[deleteFolderPath];
                collection.Delete(deleteId);
            }


            // foldersのアイテムに、folderPath=ドライブ名:\のアイテムがない場合はアイテムを追加
            // Exceptで差集合を取得
            HashSet<string> addFolderPaths = fileSystemFolderPaths.Except(folderPathIdDict.Keys).ToHashSet();

            // Parallel処理
            Parallel.ForEach(addFolderPaths, localFileSystemFolder => {
                if (!folders.Any(x => x.FileSystemFolderPath == localFileSystemFolder)) {
                    // localFileSystemFolder からフォルダ名を取得
                    string folderName = Path.GetFileName(localFileSystemFolder);
                    FileSystemFolder child = CreateChild(folderName);
                    child.Save();
                }
            });
            // 自分自身を保存
            this.Save<FileSystemFolder, FileSystemItem>();
        }

    }
}
