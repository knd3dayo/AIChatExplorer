using LibGit2Sharp;
using PythonAILib.Model.File;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using System.IO;

namespace PythonAILib.Model.VectorDB
{
    public class RAGSourceItem {

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;

        // ベクトルを格納するためのVectorDBItemのId
        protected LiteDB.ObjectId VectorDBItemId { get; set; } = LiteDB.ObjectId.Empty;


        public string SourceURL { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";

        public string LastIndexCommitHash { get; set; } = "";
        // Get
        public static IEnumerable<RAGSourceItem> GetItems() {
            PythonAILibManager libManager = PythonAILibManager.Instance ;
            var collection = libManager.DataFactory.GetRAGSourceCollection<RAGSourceItem>();
            return collection.FindAll();
        }

        public void Save() {

            PythonAILibManager libManager = PythonAILibManager.Instance ;
            var collection = libManager.DataFactory.GetRAGSourceCollection<RAGSourceItem>();
            collection.Upsert(this);

        }
        public void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetRAGSourceCollection<RAGSourceItem>();
            collection.Delete(Id);

        }

        public string SeekSourceURL(string workingDirectory) {
            try {
                // pathが存在するか確認
                if (!System.IO.Directory.Exists(workingDirectory)) {
                    return "";
                }
                Repository repo = new(workingDirectory);
                // リモートリポジトリのURLを取得
                ConfigurationEntry<string> remoteURL = repo.Config.Get<string>("remote.origin.url") ?? throw new Exception(PythonAILibStringResources.Instance.NoRemoteRepositorySet);
                // リモートリポジトリのURLをSourceURLに設定
                return remoteURL.Value;

            } catch (RepositoryNotFoundException) {
                return "";
            }

        }

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

        public bool CheckWorkingDirectory() {
            string path = WorkingDirectory;
            if (string.IsNullOrEmpty(path)) {
                throw new Exception(PythonAILibStringResources.Instance.NoWorkingDirectorySpecified);
            }
            if (!System.IO.Directory.Exists(path)) {
                throw new Exception(PythonAILibStringResources.Instance.SpecifiedDirectoryDoesNotExist);
            }
            try {
                Repository repo = new(path);
                // リモートリポジトリのURLを取得
                ConfigurationEntry<string> remoteURL = repo.Config.Get<string>("remote.origin.url") ?? throw new Exception(PythonAILibStringResources.Instance.NoRemoteRepositorySet);
                // リモートリポジトリのURLをSourceURLに設定
                SourceURL = remoteURL.Value;

            } catch (RepositoryNotFoundException) {
                throw new Exception(PythonAILibStringResources.Instance.SpecifiedDirectoryIsNotAGitRepository);
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


        // 最初のコミットから最後のコミットで処理されたファイルのリストを取得

        public List<File.FileStatus> GetFileStatusList() {

            return GetFileStatusList(null, null);

        }

        // 指定した範囲のコミットで処理されたファイルのリストを取得
        public List<File.FileStatus> GetFileStatusList(string? startHash, string? endHash) {
            List<File.FileStatus> fileStatusList = [];
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
                    File.FileStatus fileStatus = new() {
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

        // 指定したコミットの次のコミットで処理されたファイルのリストを取得

        public List<File.FileStatus> GetAfterIndexedCommitFileStatusList() {
            string? startHash = LastIndexCommitHash;
            return GetFileStatusList(startHash, null);

        }

        // 指定したコミット以後に処理されたファイルのリストを取得

        public List<File.FileStatus> GetFileStatusList(string startHash) {
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

        public UpdateIndexResult UpdateIndex(File.FileStatus fileStatus, UpdateIndexResult result, string description, int reliability) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            if (VectorDBItem == null) {
                throw new Exception(PythonAILibStringResources.Instance.NoVectorDBSet);
            }
            ChatRequestContext chatRequestContext = new() {
                OpenAIProperties = openAIProperties,
                VectorDBItems = [VectorDBItem]
            };

            try {
                // GitFileInfoの作成
                string source_path = Path.Combine(WorkingDirectory, fileStatus.Path);
                VectorDBEntry vectorDBEntry = new(source_path);
                if (fileStatus.Status == FileStatusEnum.Added || fileStatus.Status == FileStatusEnum.Modified) {
                    string content = PythonExecutor.PythonAIFunctions.ExtractFileToText(fileStatus.Path);
                    if (string.IsNullOrEmpty(content)) {
                        throw new Exception($"Invalid FileType");
                    }
                    vectorDBEntry.UpdateSourceInfo(
                        description, content , VectorSourceType.Git, source_path, SourceURL, fileStatus.Path, "");
                    PythonExecutor.PythonAIFunctions.UpdateVectorDBIndex(chatRequestContext, vectorDBEntry);

                } else if (fileStatus.Status == FileStatusEnum.Deleted) {
                    PythonExecutor.PythonAIFunctions.DeleteVectorDBIndex(chatRequestContext, vectorDBEntry);
                }
            } catch (UnsupportedFileTypeException e) {
                // ファイルタイプが未対応の場合
                result.Result = UpdateIndexResult.UpdateIndexResultEnum.Failed_InvalidFileType;
                result.Message = e.Message;
            }

            result.Result = UpdateIndexResult.UpdateIndexResultEnum.Success;

            return result;


        }

    }
}
