using PythonAILib.Model;

namespace WpfAppCommon.Model.QAChat
{
    public abstract class RAGSourceItemBase {

        public string SourceURL { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";

        public string LastIndexCommitHash { get; set; } = "";

        public abstract void Save();
        public abstract void Delete();

        public abstract string SeekSourceURL(string workingDirectory);

        public abstract VectorDBItemBase? VectorDBItem { get; set; }

        public abstract CommitInfo GetCommit(string hash);

        public abstract bool CheckWorkingDirectory();

        public abstract List<CommitInfo> GetCommitList();

        public abstract List<QAChat.FileStatus> GetFileStatusList();

        public abstract List<QAChat.FileStatus> GetAfterIndexedCommitFileStatusList();

        public abstract List<QAChat.FileStatus> GetFileStatusList(string startHash);

        public abstract string GetHeadCommitHash();

        public abstract void SetLastIndexCommitHash();

        public abstract UpdateIndexResult UpdateIndex(QAChat.FileStatus fileStatus, UpdateIndexResult result);

        }
}
