using System.IO;
using LibPythonAI.Data;
using LibPythonAI.Model.File;
using LibPythonAI.PythonIF.Request;
using PythonAILib.Common;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resources;
using PythonAILib.Utils.Git;

namespace LibPythonAI.Model.VectorDB {

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

        public List<LibPythonAI.Model.File.FileStatus> GetFileStatusList() {
            return GetFileStatusList(null, null);
        }

        // 指定した範囲のコミットで処理されたファイルのリストを取得
        public List<LibPythonAI.Model.File.FileStatus> GetFileStatusList(string? startHash, string? endHash) {
            return GitUtil.GetFileStatusList(WorkingDirectory, startHash, endHash);
        }

        // 指定したコミットの次のコミットで処理されたファイルのリストを取得

        public List<LibPythonAI.Model.File.FileStatus> GetAfterIndexedCommitFileStatusList() {
            return GitUtil.GetFileStatusList(WorkingDirectory, LastIndexCommitHash, null);
        }

        // 指定したコミット以後に処理されたファイルのリストを取得

        public List<LibPythonAI.Model.File.FileStatus> GetFileStatusList(string startHash) {
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

        public async Task<UpdateIndexResult> UpdateIndex(File.FileStatus fileStatus, UpdateIndexResult result, string description, int reliability) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            if (VectorDBItem == null) {
                throw new Exception(PythonAILibStringResources.Instance.NoVectorDBSet);
            }

            try {
                // GitFileInfoの作成
                string source_path = Path.Combine(WorkingDirectory, fileStatus.Path);
                if (fileStatus.Status == FileStatusEnum.Added || fileStatus.Status == FileStatusEnum.Modified) {
                    string content = await PythonExecutor.PythonAIFunctions.ExtractFileToTextAsync(source_path);
                    if (string.IsNullOrEmpty(content)) {
                        result.Result = UpdateIndexResult.UpdateIndexResultEnum.Failed_Other;
                        return result;
                    }
                    VectorEmbedding vectorDBEntry = new() {
                        SourcePath = source_path
                    };
                    vectorDBEntry.SetMetadata(
                        description, content, VectorSourceType.Git, source_path, SourceURL, fileStatus.Path, "");

                    VectorSearchProperty vectorSearchProperty = new() {
                        TopK = 4,
                        VectorDBItemName = VectorDBItem.Name,
                        Model = PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties().OpenAIEmbeddingModel,
                    };

                    ChatRequestContext chatRequestContext = new() {
                        OpenAIProperties = openAIProperties,
                    };

                    EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(VectorDBItem.Name, openAIProperties.OpenAIEmbeddingModel, vectorDBEntry);
                    await PythonExecutor.PythonAIFunctions.UpdateEmbeddingsAsync(chatRequestContext, embeddingRequestContext);

                } else if (fileStatus.Status == FileStatusEnum.Deleted) {
                    VectorSearchProperty vectorSearchProperty = new() {
                        TopK = 4,
                        VectorDBItemName = VectorDBItem.Name,
                        Model = PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties().OpenAIEmbeddingModel,
                    };
                    VectorEmbedding vectorDBEntry = new() {
                        SourcePath = source_path
                    };

                    ChatRequestContext chatRequestContext = new() {
                        OpenAIProperties = openAIProperties,
                    };

                    EmbeddingRequest embeddingRequestContext = new EmbeddingRequest(VectorDBItem.Name, openAIProperties.OpenAIEmbeddingModel, vectorDBEntry);
                    await PythonExecutor.PythonAIFunctions.DeleteEmbeddingsAsync(chatRequestContext, embeddingRequestContext);
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
