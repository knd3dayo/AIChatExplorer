using LibGit2Sharp;
using LiteDB;
using WpfAppCommon.Factory;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {

    public enum FileStatusEnum {
        Untracked,
        Modified,
        Added,
        Deleted,
        Renamed,
        Copied,
        UpdatedButUnmerged,
        Unmodified,
        Ignored,
        Conflicted,
        Unknown
    }

    // Index更新処理結果を格納するクラス
    public class UpdateIndexResult {
        public enum UpdateIndexResultEnum {
            Success,
            Failed_FileNotFound,
            Failed_InvalidFileType,
            Failed_Other
        }
        public UpdateIndexResultEnum Result { get; set; } = UpdateIndexResultEnum.Failed_Other;

        public string Message { get; set; } = "";

        public int TokenCount { get; set; } = 0;
    }
    public class FileStatus {
        public string Path { get; set; } = "";
        public FileStatusEnum Status { get; set; } = FileStatusEnum.Unknown;
    }
    public class CommitInfo {
        // コミットのハッシュ
        public string Hash { get; set; } = "";
        // コミットのメッセージ
        public string Message { get; set; } = "";
        // コミットの日時
        public DateTimeOffset Date { get; set; } = DateTime.Now;
    }
    /// <summary>
    /// RAGのソースとなるドキュメントを格納したリポジトリを管理するためのクラス
    /// </summary>
    public class RAGSourceItem {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;
        public string SourceURL { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";

        public string LastIndexCommitHash { get; set; } = "";

        // ベクトルを格納するためのVectorDBItemのId
        private LiteDB.ObjectId VectorDBItemId { get; set; } = LiteDB.ObjectId.Empty;

        // VectorDBItemの取得
        public VectorDBItem? VectorDBItem {
            get {
                return VectorDBItem.GetItemById(VectorDBItemId);
            }
            set {
                if (value == null) {
                    VectorDBItemId = LiteDB.ObjectId.Empty;
                } else {
                    VectorDBItemId = value.Id;
                }
            }
        }

        // --- RAGSourceItem
        // Save
        public void Save() {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // UpsertItemメソッドを呼び出して保存
            dbController.UpsertRAGSourceItem(this);

        }

        // Delete
        public void Delete() {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // DeleteItemメソッドを呼び出して削除
            dbController.DeleteRAGSourceItem(this);
        }
        // Get
        public static IEnumerable<RAGSourceItem> GetItems() {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // GetItemsメソッドを呼び出して取得
            return dbController.GetRAGSourceItems();
        }

        public string SeekSourceURL(string path) {
            try {
                // pathが存在するか確認
                if (!System.IO.Directory.Exists(path)) {
                    return "";
                }
                LibGit2Sharp.Repository repo = new(path);
                // リモートリポジトリのURLを取得
                LibGit2Sharp.ConfigurationEntry<string> remoteURL = repo.Config.Get<string>("remote.origin.url") ?? throw new Exception("リモートリポジトリが設定されていません");
                // リモートリポジトリのURLをSourceURLに設定
                return remoteURL.Value;

            } catch (LibGit2Sharp.RepositoryNotFoundException) {
                return "";
            }
        }


        // Git作業ディレクトリの確認を行う。
        public bool CheckWorkingDirectory() {
            string path = WorkingDirectory;
            if (string.IsNullOrEmpty(path)) {
                throw new Exception("作業ディレクトリが指定されていません");
            }
            if (!System.IO.Directory.Exists(path)) {
                throw new Exception("指定されたディレクトリが存在しません");
            }
            try {
                LibGit2Sharp.Repository repo = new(path);
                // リモートリポジトリのURLを取得
                LibGit2Sharp.ConfigurationEntry<string> remoteURL = repo.Config.Get<string>("remote.origin.url") ?? throw new Exception("リモートリポジトリが設定されていません");
                // リモートリポジトリのURLをSourceURLに設定
                SourceURL = remoteURL.Value;

            } catch (LibGit2Sharp.RepositoryNotFoundException) {
                throw new Exception("指定されたディレクトリはGitリポジトリではありません");
            }
            return true;

        }

        public List<CommitInfo> GetCommitList() {
            List<CommitInfo> commitList = [];

            // リポジトリの取得
            using (var repository = new Repository(WorkingDirectory)) {
                // リポジトリのコミットを取得
                var commitsToRewrite = repository.Commits.QueryBy(new CommitFilter { IncludeReachableFrom = repository.Refs.ToList() })
                            .Distinct<Commit>(EqualityComparer<GitObject>.Default)
                            .ToList();
                foreach (var commit in commitsToRewrite) {
                    CommitInfo commitInfo = new() {
                        Hash = commit.Sha,
                        Message = commit.Message,
                        Date = commit.Author.When
                    };
                    commitList.Add(commitInfo);
                }
            }
            return commitList;
        }

        // 指定したコミットハッシュのコミットを取得
        public CommitInfo GetCommit(string hash) {
            CommitInfo commitInfo = new();
            // リポジトリの取得
            using (var repository = new Repository(WorkingDirectory)) {
                // リポジトリのコミットを取得
                var commit = repository.Lookup<Commit>(hash);
                commitInfo.Hash = commit.Sha;
                commitInfo.Message = commit.Message;
                commitInfo.Date = commit.Author.When;
            }
            return commitInfo;
        }
        // 最初のコミットから最後のコミットで処理されたファイルのリストを取得
        public List<FileStatus> GetFileStatusList() {
            return GetFileStatusList(null, null);
        }
        // 指定したコミットの次のコミットで処理されたファイルのリストを取得
        public List<FileStatus> GetAfterIndexedCommitFileStatusList() {
            string? startHash = LastIndexCommitHash;
            return GetFileStatusList(startHash, null);
        }
        // 指定したコミット以後に処理されたファイルのリストを取得
        public List<FileStatus> GetFileStatusList(string startHash) {
            return GetFileStatusList(startHash, "HEAD");
        }
        // HEADのコミットハッシュを取得
        public string GetHeadCommitHash() {
            using var repository = new Repository(WorkingDirectory);
            return repository.Head.Tip.Sha;
        }
        // LastIndexCommitHashをHEADのコミットハッシュに設定
        public void SetLastIndexCommitHash() {
            LastIndexCommitHash = GetHeadCommitHash();
        }


        // 指定した範囲のコミットで処理されたファイルのリストを取得
        private List<FileStatus> GetFileStatusList(string? startHash, string? endHash) {
            List<FileStatus> fileStatusList = [];
            // リポジトリの取得
            using (var repository = new Repository(WorkingDirectory)) {
                // 現在のブランチのコミット一覧を取得
                var commitsToRewrite = repository.Commits.QueryBy(new CommitFilter { FirstParentOnly = true, IncludeReachableFrom = repository.Head.FriendlyName })
                            .Distinct<Commit>(EqualityComparer<GitObject>.Default)
                            .ToList();


                // 開始コミットが指定されていない場合は最初のコミットのハッシュを取得
                if (string.IsNullOrEmpty(startHash)) {
                    startHash = commitsToRewrite.Last().Sha;
                }
                // 終了コミットが指定されていない場合は最後のコミットのハッシュを取得
                if (string.IsNullOrEmpty(endHash)) {
                    endHash = commitsToRewrite.First().Sha;
                }
                // startHashTmp startHashがHEADの場合はHEADのコミットハッシュを取得
                string startHashTmp = startHash == "HEAD" ? repository.Head.Tip.Sha : startHash;
                // endHashTmp endHashがHEADの場合はHEADのコミットハッシュを取得
                string endHashTmp = endHash == "HEAD" ? repository.Head.Tip.Sha : endHash;
                // 開始コミットと終了コミットが同じ場合は処理を終了
                if (startHashTmp == endHashTmp) {
                    return fileStatusList;
                }

                // Treeの取得
                var startTree = repository.Lookup<Commit>(startHash).Tree;
                var endTree = repository.Lookup<Commit>(endHash).Tree;
                // コミットの差分を取得
                var changes = repository.Diff.Compare<TreeChanges>(startTree, endTree);
                foreach (var change in changes) {
                    FileStatus fileStatus = new() {
                        Path = change.Path,
                        Status = change.Status switch {
                            ChangeKind.Added => FileStatusEnum.Added,
                            ChangeKind.Copied => FileStatusEnum.Copied,
                            ChangeKind.Deleted => FileStatusEnum.Deleted,
                            ChangeKind.Modified => FileStatusEnum.Modified,
                            ChangeKind.Renamed => FileStatusEnum.Renamed,
                            ChangeKind.TypeChanged => FileStatusEnum.UpdatedButUnmerged,
                            ChangeKind.Unmodified => FileStatusEnum.Unmodified,
                            ChangeKind.Untracked => FileStatusEnum.Untracked,
                            _ => FileStatusEnum.Unknown
                        }
                    };
                    fileStatusList.Add(fileStatus);
                }
            }
            return fileStatusList;
        }

        // 更新処理
        public UpdateIndexResult UpdateIndex(FileStatus fileStatus, UpdateIndexResult result) {

            if (VectorDBItem == null) {
                throw new Exception("ベクトルDBが設定されていません");
            }
            int token = 0;
            try {
                PythonExecutor.PythonFunctions.UpdateVectorDBIndex(fileStatus, WorkingDirectory, SourceURL, VectorDBItem);
            } catch (UnsupportedFileTypeException e) {
                // ファイルタイプが未対応の場合
                result.Result = UpdateIndexResult.UpdateIndexResultEnum.Failed_InvalidFileType;
                result.Message = e.Message;
            } catch ( Exception e) {
                // その他のエラー
                result.Result = UpdateIndexResult.UpdateIndexResultEnum.Failed_Other;
                result.Message = e.Message;
            }
            result.TokenCount = token;
            result.Result = UpdateIndexResult.UpdateIndexResultEnum.Success;

            return result;

        }

    }
}
