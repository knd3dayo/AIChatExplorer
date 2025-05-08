using System.IO;
using LibPythonAI.Data;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF.Request;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.PythonIF;
using PythonAILib.Resources;
using PythonAILib.Utils.Git;

namespace PythonAILib.Model.VectorDB {
    public class RAGSourceItem {

        public RAGSourceItemEntity Entity { get; set; }

        public RAGSourceItem(RAGSourceItemEntity entity) {
            Entity = entity;
        }

        // Id
        public string Id {
            get => Entity.Id;
        }

        public string SourceURL {
            get => Entity.SourceURL;
            set => Entity.SourceURL = value;
        }
        public string WorkingDirectory {
            get => Entity.WorkingDirectory;
            set => Entity.WorkingDirectory = value;
        }

        public string LastIndexCommitHash {
            get => Entity.LastIndexCommitHash;
            set => Entity.LastIndexCommitHash = value;
        }

        public string GetRemoteURL() {
            return GitUtil.GetRemoteURL(WorkingDirectory);
        }

        public VectorDBItem? VectorDBItem {
            get {
                return VectorDBItem.GetItemById(Entity.VectorDBItemId);

            }
            set {
                Entity.VectorDBItemId = value?.Id;
            }
        }

        public string LastIndexedCommitInfoDisplayString {
            get {
                return GitUtil.GetCommitInfoDisplayString(WorkingDirectory, LastIndexCommitHash);
            }
        }

        public CommitInfo GetCommit(string hash) {
            return GitUtil.GetCommit(WorkingDirectory, hash);
        }
        // checkWorkingDirectory
        public bool CheckWorkingDirectory() {
            return string.IsNullOrEmpty(GetRemoteURL()) == false;
        }

        public List<CommitInfo> GetCommitList() {
            return GitUtil.GetCommitList(WorkingDirectory);
        }

        // 最初のコミットから最後のコミットで処理されたファイルのリストを取得

        public List<File.FileStatus> GetFileStatusList() {
            return GetFileStatusList(null, null);
        }

        // 指定した範囲のコミットで処理されたファイルのリストを取得
        public List<File.FileStatus> GetFileStatusList(string? startHash, string? endHash) {
            return GitUtil.GetFileStatusList(WorkingDirectory, startHash, endHash);
        }

        // 指定したコミットの次のコミットで処理されたファイルのリストを取得

        public List<File.FileStatus> GetAfterIndexedCommitFileStatusList() {
            return GitUtil.GetFileStatusList(WorkingDirectory, LastIndexCommitHash, null);
        }

        // 指定したコミット以後に処理されたファイルのリストを取得

        public List<File.FileStatus> GetFileStatusList(string startHash) {
            return GitUtil.GetFileStatusList(WorkingDirectory, startHash, "HEAD");
        }

        // HEADのコミットハッシュを取得
        public string GetHeadCommitHash() {
            return GitUtil.GetHeadCommitHash(WorkingDirectory);
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

            try {
                // GitFileInfoの作成
                string source_path = Path.Combine(WorkingDirectory, fileStatus.Path);
                if (fileStatus.Status == FileStatusEnum.Added || fileStatus.Status == FileStatusEnum.Modified) {
                    string content = PythonExecutor.PythonAIFunctions.ExtractFileToText(source_path);
                    if (string.IsNullOrEmpty(content)) {
                        result.Result = UpdateIndexResult.UpdateIndexResultEnum.Failed_Other;
                        return result;
                    }
                    VectorDBEmbedding vectorDBEntry = new() {
                        SourcePath = source_path
                    };
                    vectorDBEntry.UpdateSourceInfo(
                        description, content, VectorSourceType.Git, source_path, SourceURL, fileStatus.Path, "");

                    VectorDBProperty vectorDBProperty = new() {
                        TopK = 4,
                        VectorDBItemName = VectorDBItem.Name,
                    };

                    ChatRequestContext chatRequestContext = new() {
                        OpenAIProperties = openAIProperties,
                    };

                    EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(VectorDBItem.Name, openAIProperties.OpenAIEmbeddingModel, vectorDBEntry);
                    PythonExecutor.PythonAIFunctions.UpdateEmbeddings(chatRequestContext, embeddingRequestContext);

                } else if (fileStatus.Status == FileStatusEnum.Deleted) {
                    VectorDBProperty vectorDBProperty = new() {
                        TopK = 4,
                        VectorDBItemName = VectorDBItem.Name,
                    };
                    VectorDBEmbedding vectorDBEntry = new() {
                        SourcePath = source_path
                    };

                    ChatRequestContext chatRequestContext = new() {
                        OpenAIProperties = openAIProperties,
                        VectorDBProperties = [vectorDBProperty],

                    };

                    EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(VectorDBItem.Name, openAIProperties.OpenAIEmbeddingModel, vectorDBEntry);
                    PythonExecutor.PythonAIFunctions.DeleteEmbeddings(chatRequestContext, embeddingRequestContext);
                }
            } catch (UnsupportedFileTypeException e) {
                // ファイルタイプが未対応の場合
                result.Result = UpdateIndexResult.UpdateIndexResultEnum.Failed_InvalidFileType;
                result.Message = e.Message;
            }

            result.Result = UpdateIndexResult.UpdateIndexResultEnum.Success;

            return result;


        }

        // Get
        public static IEnumerable<RAGSourceItem> GetItems() {
            using PythonAILibDBContext db = new();
            var items = db.RAGSourceItems;

            foreach (var item in items) {
                yield return new RAGSourceItem(item);
            }
        }

        public void Save() {
            RAGSourceItemEntity.SaveItems([Entity]);
        }

        public void Delete() {
            RAGSourceItemEntity.DeleteItems([Entity]);
        }

    }
}
