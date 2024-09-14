using ClipboardApp.Factory;
using LibGit2Sharp;
using LiteDB;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using WpfAppCommon.Model;

namespace ClipboardApp.Model {
    /// <summary>
    /// RAGのソースとなるドキュメントを格納したリポジトリを管理するためのクラス
    /// </summary>
    public class RAGSourceItem : RAGSourceItemBase {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;

        // ベクトルを格納するためのVectorDBItemのId
        private LiteDB.ObjectId VectorDBItemId { get; set; } = LiteDB.ObjectId.Empty;

        // VectorDBItemの取得
        public override VectorDBItemBase? VectorDBItem {
            get {
                return ClipboardAppVectorDBItem.GetItemById(VectorDBItemId);
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
        public override void Save() {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // UpsertItemメソッドを呼び出して保存
            dbController.UpsertRAGSourceItem(this);

        }

        // Delete
        public override void Delete() {
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
            IEnumerable<RAGSourceItemBase> items = dbController.GetRAGSourceItems();
            return items.Cast<RAGSourceItem>();
        }

        public override string SeekSourceURL(string path) {
            try {
                // pathが存在するか確認
                if (!System.IO.Directory.Exists(path)) {
                    return "";
                }
                Repository repo = new(path);
                // リモートリポジトリのURLを取得
                ConfigurationEntry<string> remoteURL = repo.Config.Get<string>("remote.origin.url") ?? throw new Exception(CommonStringResources.Instance.NoRemoteRepositorySet);
                // リモートリポジトリのURLをSourceURLに設定
                return remoteURL.Value;

            } catch (RepositoryNotFoundException) {
                return "";
            }
        }


        // Git作業ディレクトリの確認を行う。
        public override bool CheckWorkingDirectory() {
            string path = WorkingDirectory;
            if (string.IsNullOrEmpty(path)) {
                throw new Exception(CommonStringResources.Instance.NoWorkingDirectorySpecified);
            }
            if (!System.IO.Directory.Exists(path)) {
                throw new Exception(CommonStringResources.Instance.SpecifiedDirectoryDoesNotExist);
            }
            try {
                Repository repo = new(path);
                // リモートリポジトリのURLを取得
                ConfigurationEntry<string> remoteURL = repo.Config.Get<string>("remote.origin.url") ?? throw new Exception(CommonStringResources.Instance.NoRemoteRepositorySet);
                // リモートリポジトリのURLをSourceURLに設定
                SourceURL = remoteURL.Value;

            } catch (RepositoryNotFoundException) {
                throw new Exception(CommonStringResources.Instance.SpecifiedDirectoryIsNotAGitRepository);
            }
            return true;

        }

        public override List<CommitInfo> GetCommitList() {
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
        public override CommitInfo GetCommit(string hash) {
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
        public override List<PythonAILib.Model.FileStatus> GetFileStatusList() {
            return GetFileStatusList(null, null);
        }
        // 指定したコミットの次のコミットで処理されたファイルのリストを取得
        public override List<PythonAILib.Model.FileStatus> GetAfterIndexedCommitFileStatusList() {
            string? startHash = LastIndexCommitHash;
            return GetFileStatusList(startHash, null);
        }
        // 指定したコミット以後に処理されたファイルのリストを取得
        public override List<PythonAILib.Model.FileStatus> GetFileStatusList(string startHash) {
            return GetFileStatusList(startHash, "HEAD");
        }
        // HEADのコミットハッシュを取得
        public override string GetHeadCommitHash() {
            using var repository = new Repository(WorkingDirectory);
            return repository.Head.Tip.Sha;
        }
        // LastIndexCommitHashをHEADのコミットハッシュに設定
        public override void SetLastIndexCommitHash() {
            LastIndexCommitHash = GetHeadCommitHash();
        }


        // 指定した範囲のコミットで処理されたファイルのリストを取得
        private List<PythonAILib.Model.FileStatus> GetFileStatusList(string? startHash, string? endHash) {
            List<PythonAILib.Model.FileStatus> fileStatusList = [];
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
                    PythonAILib.Model.FileStatus fileStatus = new() {
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
        public override UpdateIndexResult UpdateIndex(PythonAILib.Model.FileStatus fileStatus, UpdateIndexResult result) {

            if (VectorDBItem == null) {
                throw new Exception(CommonStringResources.Instance.NoVectorDBSet);
            }
            int token = 0;
            try {
                // GitFileInfoの作成
                VectorDBUpdateMode mode = VectorDBUpdateMode.update;
                if (fileStatus.Status == FileStatusEnum.Added || fileStatus.Status == FileStatusEnum.Modified) {
                    mode = VectorDBUpdateMode.update;
                } else if (fileStatus.Status == FileStatusEnum.Deleted) {
                    mode = VectorDBUpdateMode.delete;
                }
                GitFileInfo gitFileInfo = new GitFileInfo(mode, fileStatus.Path, WorkingDirectory, SourceURL);
                PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(ClipboardAppConfig.CreateOpenAIProperties(), gitFileInfo, VectorDBItem);
            } catch (UnsupportedFileTypeException e) {
                // ファイルタイプが未対応の場合
                result.Result = UpdateIndexResult.UpdateIndexResultEnum.Failed_InvalidFileType;
                result.Message = e.Message;
            }

            result.TokenCount = token;
            result.Result = UpdateIndexResult.UpdateIndexResultEnum.Success;

            return result;

        }

    }
}
