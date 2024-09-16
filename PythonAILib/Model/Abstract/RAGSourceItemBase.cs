using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;

namespace PythonAILib.Model.Abstract
{
    public abstract class RAGSourceItemBase
    {

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

        public abstract List<FileStatus> GetFileStatusList();

        public abstract List<FileStatus> GetAfterIndexedCommitFileStatusList();

        public abstract List<FileStatus> GetFileStatusList(string startHash);

        public abstract string GetHeadCommitHash();

        public abstract void SetLastIndexCommitHash();

        public abstract UpdateIndexResult UpdateIndex(FileStatus fileStatus, UpdateIndexResult result);

    }
}
