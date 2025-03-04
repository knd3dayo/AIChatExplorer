using System.IO;
using LibPythonAI.Data;
using LibPythonAI.Model.VectorDB;
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

        public string SourceURL { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";

        public string LastIndexCommitHash { get; set; } = "";
        // Get
        public static IEnumerable<RAGSourceItem> GetItems() {
            using PythonAILibDBContext db = new();
            var items = db.RAGSourceItems;

            foreach (var item in items) {
                yield return new RAGSourceItem(item);
            }
        }

        public void Save() {
            using PythonAILibDBContext db = new();
            var item = db.RAGSourceItems.Find(Entity.Id);
            if (item == null) {
                db.RAGSourceItems.Add(Entity);
            } else {
                db.RAGSourceItems.Entry(item).CurrentValues.SetValues(Entity);
            }
            db.SaveChanges();


        }
        public void Delete() {
            using PythonAILibDBContext db = new();
            var item = db.RAGSourceItems.Find(Entity.Id);
            if (item != null) {
                db.Remove(item);
            }

        }

        public string GetRemoteURL() {
            return GitUtil.GetRemoteURL(WorkingDirectory);
        }

        public VectorDBItem? VectorDBItem {
            get {
                var item =  Entity.VectorDBItem;
                if (item == null) {
                    return null;
                }
                return new VectorDBItem(item);
            }
            set {
                if (value == null) {
                    Entity.VectorDBItem = null;
                } else {
                    Entity.VectorDBItem = value.Entity;
                }
            }
        }

        public IEnumerable<VectorDBItem>? VectorDBItems {
            get {
                using PythonAILibDBContext db = new();
                var items = db.VectorDBItems.Where(item => !item.IsSystem && item.Name != VectorDBItem.SystemCommonVectorDBName);
                foreach (var item in items) {
                    yield return new VectorDBItem(item);
                }
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
                    VectorMetadata vectorDBEntry = new(source_path);
                    vectorDBEntry.UpdateSourceInfo(
                        description, content, VectorSourceType.Git, source_path, SourceURL, fileStatus.Path, "");

                    VectorDBPropertyEntity searchPropertyEntity = new() {
                        TopK = 4,
                        VectorDBItemId = VectorDBItem.Entity.Id,
                    };
                    VectorDBProperty vectorDBProperty = new(searchPropertyEntity);
                    vectorDBProperty.VectorMetadataList = [vectorDBEntry];
                    ChatRequestContext chatRequestContext = new() {
                        OpenAIProperties = openAIProperties,
                        VectorDBProperties = [vectorDBProperty]
                    };

                    PythonExecutor.PythonAIFunctions.UpdateEmbeddings(chatRequestContext);

                } else if (fileStatus.Status == FileStatusEnum.Deleted) {
                    VectorDBPropertyEntity searchPropertyEntity = new() {
                        TopK = 4,
                        VectorDBItemId = VectorDBItem.Entity.Id,
                    }; VectorDBProperty vectorDBProperty = new(searchPropertyEntity);
                    VectorMetadata vectorDBEntry = new(source_path);
                    vectorDBProperty.VectorMetadataList = [vectorDBEntry];
                    ChatRequestContext chatRequestContext = new() {
                        OpenAIProperties = openAIProperties,
                        VectorDBProperties = [vectorDBProperty]
                    };

                    PythonExecutor.PythonAIFunctions.DeleteEmbeddings(chatRequestContext);
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
