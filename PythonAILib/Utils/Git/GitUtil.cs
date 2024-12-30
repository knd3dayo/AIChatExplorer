using LibGit2Sharp;
using PythonAILib.Model.File;
using PythonAILib.Resource;

namespace PythonAILib.Utils.Git {
    public class GitUtil {

        public static string GetRemoteURL(string workingDirectory) {
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

        public static CommitInfo GetCommit(string workingDirectory, string hash) {
            CommitInfo commitInfo = new();
            // リポジトリの取得
            using (var repository = new Repository(workingDirectory)) {
                // リポジトリのコミットを取得
                var commit = repository.Lookup<Commit>(hash);
                commitInfo.Hash = commit.Sha;
                commitInfo.Message = commit.Message;
                commitInfo.Date = commit.Author.When;
            }
            return commitInfo;
        }

        public static List<CommitInfo> GetCommitList(string workingDirectory) {
            List<CommitInfo> commitList = [];

            // リポジトリの取得
            using (var repository = new Repository(workingDirectory)) {
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

        // 指定した範囲のコミットで処理されたファイルのリストを取得
        public static List<PythonAILib.Model.File.FileStatus> GetFileStatusList(string workingDirectory, string? startHash, string? endHash) {
            List< PythonAILib.Model.File.FileStatus > fileStatusList = [];
            // リポジトリの取得
            using (var repository = new Repository(workingDirectory)) {
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
                    PythonAILib.Model.File.FileStatus fileStatus = new() {
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
        public static List<PythonAILib.Model.File.FileStatus> GetAfterIndexedCommitFileStatusList(string workingDirectory, string? startHash) {
            return GetFileStatusList(workingDirectory, startHash, null);
        }

        // 指定したコミット以後に処理されたファイルのリストを取得
        public static List<PythonAILib.Model.File.FileStatus> GetFileStatusList(string workingDirectory, string startHash) {
            return GetFileStatusList(workingDirectory, startHash, "HEAD");

        }
        // HEADのコミットハッシュを取得
        public static string GetHeadCommitHash(string workingDirectory) {
            using var repository = new Repository(workingDirectory);
            return repository.Head.Tip.Sha;
        }

        // 最後にインデックス化したコミットの情報
        public static string GetCommitInfoDisplayString(string workingDirectory, string commitHash) {
            if (string.IsNullOrEmpty(commitHash)) {
                return "";
            }
            CommitInfo commitInfo = GetCommit(workingDirectory, commitHash);
            return commitInfo.GetDisplayString();
        }

    }
}
