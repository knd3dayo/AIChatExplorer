using LibGit2Sharp;
using LiteDB;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public class ClipboardItemFile {

        // コンストラクタ
        public ClipboardItemFile() {
        }
        public static ClipboardItemFile Create(ClipboardItem clipboardItem, string filePath) {
            ClipboardItemFile itemFile = new ClipboardItemFile();
            itemFile.ClipboardFolderPath = clipboardItem.FolderPath;
            itemFile.FilePath = filePath;
            return itemFile;
        }

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;

        // クリップボードアイテムのフォルダパス
        public string ClipboardFolderPath { get; set; } = "";

        // ファイルパス
        public string FilePath { get; set; } = "";
        // フォルダ名
        public string FolderName {
            get {
                return System.IO.Path.GetDirectoryName(FilePath) ?? "";
            }
        }
        // ファイル名
        public string FileName {
            get {
                return System.IO.Path.GetFileName(FilePath) ?? "";
            }
        }
        // フォルダ名 + \n + ファイル名
        public string FolderAndFileName {
            get {
                return FolderName + "\n" + FileName;
            }
        }

        // 削除
        public void Delete() {
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItemFile(this);
            // クリップボードアイテムとファイルを同期する
            if (ClipboardAppConfig.SyncClipboardItemAndOSFolder) {
                // SyncFolderName/フォルダ名/ファイル名を削除する
                string syncFolderName = ClipboardAppConfig.SyncFolderName;

                string syncFolder = System.IO.Path.Combine(syncFolderName, ClipboardFolderPath);
                string syncFilePath = System.IO.Path.Combine(syncFolder, FileName);
                if (System.IO.File.Exists(syncFilePath)) {
                    System.IO.File.Delete(syncFilePath);
                }
                // 自動コミットが有効の場合はGitにコミット
                if (ClipboardAppConfig.AutoCommit) {
                    try {
                        using (var repo = new Repository(ClipboardAppConfig.SyncFolderName)) {
                            Commands.Stage(repo, syncFilePath);
                            Signature author = new("ClipboardApp", "ClipboardApp", DateTimeOffset.Now);
                            Signature committer = author;
                            repo.Commit("Auto commit", author, committer);
                            //Tools.Info
                            Tools.Info($"Gitにコミットしました:{syncFilePath} {ClipboardAppConfig.SyncFolderName}");
                        }
                    } catch (RepositoryNotFoundException e) {
                        Tools.Info($"リポジトリが見つかりませんでした:{ClipboardAppConfig.SyncFolderName} {e.Message}");
                    } catch (EmptyCommitException e) {
                        Tools.Info($"コミットが空です:{syncFilePath} {e.Message}");
                    }
                }

            }
        }
        // 保存
        public void Save() {
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertItemFile(this);
            // クリップボードアイテムとファイルを同期する
            if (ClipboardAppConfig.SyncClipboardItemAndOSFolder) {
                if (FilePath == null) {
                    throw new Exception("FilePath is null");
                }
                // SyncFolderName/フォルダ名/ファイル名にファイルを保存する
                string syncFolderName = ClipboardAppConfig.SyncFolderName;
                string syncFolder = System.IO.Path.Combine(syncFolderName, ClipboardFolderPath);
                string syncFilePath = System.IO.Path.Combine(syncFolder, FileName);
                if (!System.IO.Directory.Exists(syncFolder)) {
                    System.IO.Directory.CreateDirectory(syncFolder);
                }
                if (System.IO.File.Exists(FilePath)) {
                    System.IO.File.Copy(FilePath, syncFilePath, true);
                }
                // 自動コミットが有効の場合はGitにコミット
                if (ClipboardAppConfig.AutoCommit) {
                    try {
                        using (var repo = new Repository(ClipboardAppConfig.SyncFolderName)) {
                            Commands.Stage(repo, syncFilePath);
                            Signature author = new("ClipboardApp", "ClipboardApp", DateTimeOffset.Now);
                            Signature committer = author;
                            repo.Commit("Auto commit", author, committer);
                            Tools.Info($"Gitにコミットしました:{syncFilePath} {ClipboardAppConfig.SyncFolderName}");
                        }
                    } catch (RepositoryNotFoundException e) {
                        Tools.Info($"リポジトリが見つかりませんでした:{ClipboardAppConfig.SyncFolderName} {e.Message}");
                    } catch (EmptyCommitException e) {
                        Tools.Info($"コミットが空です:{syncFilePath} {e.Message}");
                    }
                }
            }
        }
        // 取得
        public static ClipboardItemFile? GetItem(LiteDB.ObjectId objectId) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(objectId);
        }
    }

}
