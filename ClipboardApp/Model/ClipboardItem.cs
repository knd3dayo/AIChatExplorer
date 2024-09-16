using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using ClipboardApp.Factory;
using LibGit2Sharp;
using PythonAILib.Model;
using PythonAILib.Model.Abstract;
using PythonAILib.Model.Chat;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.Model {
    public partial class ClipboardItem : ContentItemBase {
        // コンストラクタ
        public ClipboardItem(LiteDB.ObjectId folderObjectId) {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            FolderObjectId = folderObjectId;
        }

        // プロパティ
        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.NewObjectId();

        // ClipboardFolderのObjectId
        public LiteDB.ObjectId FolderObjectId { get; set; } = LiteDB.ObjectId.Empty;

        public string FolderPath {
            get {
                // FolderObjectIdからClipboardFolderを取得
                ClipboardFolder? folder = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(FolderObjectId);
                if (folder == null) {
                    return "";
                }
                return folder.FolderPath;
            }
        }

        //　画像イメージのObjectId
        public List<LiteDB.ObjectId> ImageObjectIds { get; set; } = [];

        // ファイルのObjectId
        public List<LiteDB.ObjectId> FileObjectIds { get; set; } = [];


        // ファイル
        // LiteDBの別コレクションで保存されているオブジェクト。LiteDBからはLoad**メソッドで取得する。Saveメソッドで保存する
        private List<ContentAttachedItemBase> _clipboardItemFiles = [];
        public override List<ContentAttachedItemBase> ClipboardItemFiles {
            get {
                if (_clipboardItemFiles.Count() == 0) {
                    LoadFiles();
                }
                return _clipboardItemFiles;
            }
        }
        private void LoadFiles() {
            foreach (var fileObjectId in FileObjectIds) {
                ClipboardItemFile? file = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(fileObjectId);
                if (file != null) {
                    _clipboardItemFiles.Add(file);
                }
            }
        }

        private void SaveFiles() {
            //Fileを保存
            FileObjectIds = [];
            foreach (ClipboardItemFile file in _clipboardItemFiles) {
                file.Save();
                // ClipboardItemFileをSaveした後にIdが設定される。そのあとでFileObjectIdsに追加
                FileObjectIds.Add(file.Id);
            }
        }

        // -------------------------------------------------------------------
        // インスタンスメソッド
        // -------------------------------------------------------------------

        public override ClipboardItem Copy() {
            ClipboardItem newItem = new(this.FolderObjectId);
            CopyTo(newItem);
            return newItem;

        }
        public override void CopyTo(ContentItemBase newItem) {
            if (newItem is not ClipboardItem) {
                return;
            }
            ClipboardItem clipboardItem = (ClipboardItem)newItem;
            clipboardItem.UpdatedAt = UpdatedAt;
            clipboardItem.Content = Content;
            clipboardItem.ContentType = ContentType;
            clipboardItem.SourceApplicationName = SourceApplicationName;
            clipboardItem.SourceApplicationTitle = SourceApplicationTitle;
            clipboardItem.SourceApplicationID = SourceApplicationID;
            clipboardItem.SourceApplicationPath = SourceApplicationPath;
            clipboardItem.Tags = new HashSet<string>(Tags);
            clipboardItem.Description = Description;
            // 背景情報
            clipboardItem.BackgroundInfo = BackgroundInfo;
            // サマリー
            clipboardItem.Summary = Summary;

            //-- ファイルがある場合はコピー
            foreach (var FileObjectId in FileObjectIds) {
                ClipboardItemFile? file = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(FileObjectId);
                ClipboardItemFile newFile = ClipboardItemFile.Create(clipboardItem, file?.FilePath ?? string.Empty);
                clipboardItem.FileObjectIds.Add(newFile.Id);
            }
            //-- ChatItemsをコピー
            newItem.ChatItems = new List<ChatIHistorytem>(ChatItems);

        }

        // ベクトルDBを返す。
        public override VectorDBItemBase GetVectorDBItem() {
            return GetFolder().GetVectorDBItem();
        }

        public void MergeItems(List<ClipboardItem> items) {
            // itemsが空の場合は何もしない
            if (items.Count == 0) {
                return;
            }

            string mergeText = "\n";
            mergeText += "---\n";

            foreach (var item in items) {
                // itemが自分自身の場合はスキップ
                if (item.Id == Id) {
                    continue;
                }
                // Contentを追加
                mergeText += item.Content + "\n";
                // itemのAttachmentsを追加
                foreach (var fileObjectId in item.FileObjectIds) {
                    ClipboardItemFile? file = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(fileObjectId);
                    if (file != null) {
                        if (file.IsImage() && file.Base64String != null) {
                            // 画像の場合は新しいファイルを作成
                            ClipboardItemFile imageFile = ClipboardItemFile.CreateFromBase64(this, file.Base64String);
                            _clipboardItemFiles.Add(imageFile);
                        } else {
                            ClipboardItemFile newFile = ClipboardItemFile.Create(this, file.FilePath);
                            _clipboardItemFiles.Add(newFile);
                        }
                    }
                }
            }
            // mergeTextをContentに追加
            Content += mergeText;

            // Tagsのマージ。重複を除外して追加
            Tags.UnionWith(items.SelectMany(item => item.Tags));

            // マージしたアイテムを削除
            foreach (var item in items) {
                // itemが自分自身の場合はスキップ
                if (item.Id == Id) {
                    continue;
                }
                item.Delete();
            }
            // 保存
            Save();

        }

        // Collectionに対応するClipboardFolderを取得
        public ClipboardFolder GetFolder(Type? objectType = null) {
            ClipboardFolder? folder = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(FolderObjectId);
            return folder ?? throw new Exception(CommonStringResources.Instance.CannotGetFolder);
        }

        //--------------------------------------------------------------------------------
        // staticメソッド
        //--------------------------------------------------------------------------------

        // ClipboardItemをJSON文字列に変換する
        public static string ToJson(ClipboardItem item) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(item, options);
        }


        // JSON文字列をClipboardItemに変換する
        public static ClipboardItem? FromJson(string json) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            ClipboardItem? item = System.Text.Json.JsonSerializer.Deserialize<ClipboardItem>(json, options);
            if (item == null) {
                LogWrapper.Error(CommonStringResources.Instance.FailedToParseJSONString);
                return null;
            }
            return item;

        }
        // ClipboardItemのListからContentsを取得して文字列として返す
        public static string GetContentsString(List<ClipboardItem> items) {
            StringBuilder sb = new();
            foreach (var item in items) {
                sb.AppendLine(item.Content);
            }
            return sb.ToString();
        }

        // 別コレクションのオブジェクトをLoadする
        public void Load() {
            LoadFiles();
        }

        // 自分自身をDBに保存する
        public override void Save(bool contentIsModified = true) {

            if (contentIsModified) {
                // ★TODO DBControllerに処理を移動する。
                // ファイルを保存
                SaveFiles();
            }
            // 保存済みのアイテムを取得
            ClipboardItem? savedItem = (ClipboardItem?)ClipboardAppFactory.Instance.GetClipboardDBController().GetItem(this);

            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertItem(this, contentIsModified);

            if (contentIsModified == false) {
                // ピン留め変更のみの場合を想定。
                return;
            }


            // SaveContentがNullの場合、またはContentが変更されている場合はOS上のファイル更新とEmbedding更新を行う
            if (savedItem == null || savedItem.Content != Content) {
                // OS上のファイルに保存
                Task.Run(() => {
                    // SyncClipboardItemAndOSFolder == trueの場合はOSのフォルダにも保存
                    if (ClipboardAppConfig.SyncClipboardItemAndOSFolder) {
                        SaveToOSFolder();
                    }
                });

                // Embeddingを更新
                Task.Run(() => {
                    if (ClipboardAppConfig.AutoEmbedding) {
                        UpdateEmbedding();
                    }
                });
            }
        }

        // OS上のファイルに保存する
        private void SaveToOSFolder() {
            LogWrapper.Info(CommonStringResources.Instance.SaveToFileOnOS);
            // 保存先フォルダを取得
            string syncFolder = ClipboardAppConfig.SyncFolderName;
            // フォルダが存在しない場合は作成
            if (Directory.Exists(syncFolder) == false) {
                Directory.CreateDirectory(syncFolder);
            }
            // syncFolder/フォルダ名を作成
            string folderPath = System.IO.Path.Combine(syncFolder, FolderPath);
            // フォルダが存在しない場合は作成
            if (Directory.Exists(folderPath) == false) {
                Directory.CreateDirectory(folderPath);
            }

            // folderPath + Id + .txtをファイル名として保存
            string syncFilePath = System.IO.Path.Combine(folderPath, Id + ".txt");
            // 保存
            File.WriteAllText(syncFilePath, this.Content);

            // 自動コミットが有効の場合はGitにコミット
            if (ClipboardAppConfig.AutoCommit) {
                GitCommit(syncFilePath);
            }

            LogWrapper.Info(CommonStringResources.Instance.SavedToFileOnOS);
        }

        public void GitCommit(string syncFilePath) {
            try {

                using (var repo = new Repository(ClipboardAppConfig.SyncFolderName)) {
                    Commands.Stage(repo, syncFilePath);
                    Signature author = new("ClipboardApp", "ClipboardApp", DateTimeOffset.Now);
                    Signature committer = author;
                    repo.Commit("Auto commit", author, committer);
                    LogWrapper.Info($"{CommonStringResources.Instance.CommittedToGit}:{syncFilePath} {ClipboardAppConfig.SyncFolderName}");
                }
            } catch (RepositoryNotFoundException e) {
                LogWrapper.Info($"{CommonStringResources.Instance.RepositoryNotFound}:{ClipboardAppConfig.SyncFolderName} {e.Message}");
            } catch (EmptyCommitException e) {
                LogWrapper.Info($"{CommonStringResources.Instance.CommitIsEmpty}:{syncFilePath} {e.Message}");
            }
        }

        // 自分自身をDBから削除する
        public override void Delete() {
            // 保存先フォルダを取得
            string folderPath = ClipboardAppConfig.SyncFolderName;
            // syncFolder/フォルダ名を取得
            folderPath = Path.Combine(folderPath, FolderPath);
            // ClipboardFolderのFolderPath + Id + .txtをファイル名として削除
            string syncFilePath = Path.Combine(folderPath, Id + ".txt");

            // ImageObjectIdsのコピーを作成
            List<LiteDB.ObjectId> imageObjectIds = new(ImageObjectIds);

            // AutoEmbedding == Trueの場合はEmbeddingを削除
            Task.Run(() => {
                UpdateEmbedding(VectorDBUpdateMode.delete);
            });
            Task.Run(() => {
                LogWrapper.Info(CommonStringResources.Instance.DeleteFileOnOS);
                // SyncClipboardItemAndOSFolder == trueの場合はOSのフォルダからも削除
                if (ClipboardAppConfig.SyncClipboardItemAndOSFolder) {
                    // ファイルが存在する場合は削除
                    if (File.Exists(syncFilePath)) {
                        File.Delete(syncFilePath);
                    }
                    // 自動コミットが有効の場合はGitにコミット
                    if (ClipboardAppConfig.AutoCommit) {
                        GitCommit(syncFilePath);
                    }
                }
                LogWrapper.Info(CommonStringResources.Instance.DeletedFileOnOS);

            });
            LogWrapper.Info(CommonStringResources.Instance.DeletedEmbedding);

            // ファイルが存在する場合は削除
            foreach (var fileObjectId in FileObjectIds) {
                ClipboardItemFile? file = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(fileObjectId);
                file?.Delete();
            }
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItem(this);
        }

        public override void UpdateEmbedding(VectorDBUpdateMode mode) {
            if (mode == VectorDBUpdateMode.delete) {
                LogWrapper.Info(CommonStringResources.Instance.DeleteEmbedding);
                // VectorDBItemを取得
                VectorDBItemBase folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(GetFolder());
                // IPythonAIFunctions.ClipboardInfoを作成
                ContentInfo clipboardInfo = new(VectorDBUpdateMode.delete, this.Id.ToString(), this.Content);
                // Embeddingを削除
                folderVectorDBItem.DeleteIndex(clipboardInfo);
                LogWrapper.Info(CommonStringResources.Instance.DeletedEmbedding);
                return;
            }
            if (mode == VectorDBUpdateMode.update) {
                LogWrapper.Info(CommonStringResources.Instance.SaveEmbedding);
                // IPythonAIFunctions.ClipboardInfoを作成
                string content = this.Content;
                // 背景情報を含める場合
                if (ClipboardAppConfig.IncludeBackgroundInfoInEmbedding) {
                    content += $"\n---{PythonAILibStringResources.Instance.BackgroundInformation}--\n{BackgroundInfo}";
                }

                ContentInfo clipboardInfo = new(VectorDBUpdateMode.update, this.Id.ToString(), content);

                // VectorDBItemを取得
                VectorDBItemBase folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(GetFolder());
                // Embeddingを保存
                folderVectorDBItem.UpdateIndex(clipboardInfo);
                LogWrapper.Info(CommonStringResources.Instance.SavedEmbedding);
            }
        }

        // Embeddingを更新する
        public void UpdateEmbedding() {
            UpdateEmbedding(VectorDBUpdateMode.update);
        }

        // ベクトル検索を実行する
        public List<VectorSearchResult> VectorSearchCommandExecute(bool IncludeBackgroundInfo) {
            // VectorDBItemを取得
            VectorDBItemBase vectorDBItem = ClipboardAppVectorDBItem.SystemCommonVectorDB;
            return VectorSearchCommandExecute(ClipboardAppConfig.CreateOpenAIProperties(), vectorDBItem, IncludeBackgroundInfo);
        }


        // OpenAIを使用してタイトルを生成する
        public void CreateAutoTitleWithOpenAI() {
            CreateAutoTitleWithOpenAI(PromptItem.GetSystemPromptItemByName(PromptItem.SystemDefinedPromptNames.TitleGeneration), ClipboardAppConfig.CreateOpenAIProperties());
        }

        // 自動でコンテキスト情報を付与するコマンド
        public void CreateAutoBackgroundInfo() {
            string contentText = Content;
            // contentTextがない場合は処理しない
            if (string.IsNullOrEmpty(contentText)) {
                return;
            }
            var task1 = Task.Run(() => {
                // 標準背景情報を生成
                string? normalBackgroundInfo = GetNormalBackgroundInfo();
                return normalBackgroundInfo;
            });

            var task2 = Task.Run(() => {
                // 背景情報に日本語解析追加が有効になっている場合
                if (ClipboardAppConfig.AnalyzeJapaneseSentence) {
                    string? analyzedJapaneseSentence = GetAnalyzedJapaneseSentence();
                    return analyzedJapaneseSentence;
                }
                return null;
            });
            var task3 = Task.Run(() => {
                // 背景情報に自動QA生成が有効になっている場合
                if (ClipboardAppConfig.AutoGenerateQA) {
                    string? generatedQA = GetGeneratedQA();
                    return generatedQA;
                }
                return null;
            });

            // すべてのタスクが完了するまで待機
            Task.WaitAll(task1, task2, task3);
            // 背景情報を更新 taskの結果がNullでない場合は追加
            if (task1.Result != null) {
                BackgroundInfo += task1.Result;
            }
            if (task2.Result != null) {
                BackgroundInfo += task2.Result;
            }
            if (task3.Result != null) {
                BackgroundInfo += task3.Result;
            }
        }

        public string? GetNormalBackgroundInfo() {
            string result;
            string contentText = Content;
            // contentTextがない場合は処理しない
            if (string.IsNullOrEmpty(contentText)) {
                return null;
            }
            // ベクトルDBの設定
            VectorDBItemBase vectorDBItem = ClipboardAppVectorDBItem.SystemCommonVectorDB;
            vectorDBItem.CollectionName = FolderObjectId.ToString();

            // システム定義のPromptItemを取得
            PromptItem promptItem = PromptItem.GetSystemPromptItemByName(PromptItem.SystemDefinedPromptNames.BackgroundInformationGeneration);
            result = ChatUtil.CreateBackgroundInfo(ClipboardAppConfig.CreateOpenAIProperties(), [vectorDBItem], contentText, promptItem.Prompt);

            return result;

        }

        public string? GetAnalyzedJapaneseSentence() {
            string result;
            string contentText = Content;
            // contentTextがない場合は処理しない
            if (string.IsNullOrEmpty(contentText)) {
                return null;
            }
            // ベクトルDBの設定
            VectorDBItemBase vectorDBItem = ClipboardAppVectorDBItem.SystemCommonVectorDB;
            vectorDBItem.CollectionName = FolderObjectId.ToString();

            result = ChatUtil.AnalyzeJapaneseSentence(ClipboardAppConfig.CreateOpenAIProperties(), [vectorDBItem], contentText);
            if (string.IsNullOrEmpty(result) == false) {
                BackgroundInfo += "\n" + result;
            }
            return result;
        }

        public string? GetGeneratedQA() {
            string result;
            string contentText = Content;
            // contentTextがない場合は処理しない
            if (string.IsNullOrEmpty(contentText)) {
                return null;
            }
            // ベクトルDBの設定
            VectorDBItemBase vectorDBItem = ClipboardAppVectorDBItem.SystemCommonVectorDB;
            vectorDBItem.CollectionName = FolderObjectId.ToString();

            result = ChatUtil.GenerateQA(ClipboardAppConfig.CreateOpenAIProperties(), [vectorDBItem], contentText);
            if (string.IsNullOrEmpty(result) == false) {
                BackgroundInfo += "\n" + result;
            }
            return result;
        }

        // 自動でサマリーを付与するコマンド
        public void CreateSummary() {
            Task.Run(() => {
                // システム定義のPromptItemを取得
                PromptItem promptItem = PromptItem.GetSystemPromptItemByName(PromptItem.SystemDefinedPromptNames.SummaryGeneration);
                CreateSummary(promptItem, ClipboardAppConfig.CreateOpenAIProperties());
            });
        }
        // 課題リストを作成する
        public void CreateIssues() {
            Task.Run(() => {
                // システム定義のPromptItemを取得
                PromptItem promptItem = PromptItem.GetSystemPromptItemByName(PromptItem.SystemDefinedPromptNames.IssuesGeneration);
                CreateIssues(ClipboardAppConfig.CreateOpenAIProperties(), [ClipboardAppVectorDBItem.SystemCommonVectorDB], promptItem);
            });
        }
    }
}
