using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibGit2Sharp;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using WpfAppCommon.Model.ClipboardApp;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public enum ClipboardContentTypes {
        Text,
        Files,
        Image,
        Unknown
    }
    public partial class ClipboardItem {
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

        // 生成日時
        public DateTime CreatedAt { get; set; }

        // 更新日時
        public DateTime UpdatedAt { get; set; }

        // クリップボードの内容
        public string Content { get; set; } = "";

        // 背景情報
        public string BackgroundInfo { get; set; } = "";

        // サマリー
        public string Summary { get; set; } = "";

        //　画像イメージのObjectId
        public List<LiteDB.ObjectId> ImageObjectIds { get; set; } = [];

        // ファイルのObjectId
        public List<LiteDB.ObjectId> FileObjectIds { get; set; } = [];

        // 画像ファイルチェッカー
        public ScreenShotCheckItem ScreenShotCheckItem { get; set; } = new();

        // 画像イメージ
        // LiteDBの別コレクションで保存されているオブジェクト。LiteDBからはLoad**メソッドで取得する。Saveメソッドで保存する
        private List<ClipboardItemImage> _clipboardItemImages = [];
        public List<ClipboardItemImage> ClipboardItemImages {
            get {
                if (_clipboardItemImages.Count() == 0) {
                    LoadImages();
                }
                return _clipboardItemImages;
            }
        }
        private void LoadImages() {
            foreach (var imageObjectId in ImageObjectIds) {
                ClipboardItemImage? image = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(imageObjectId);
                if (image != null) {
                    _clipboardItemImages.Add(image);
                }
            }
        }
        private void SaveImages() {
            //Imageを保存
            ImageObjectIds = [];
            foreach (var image in _clipboardItemImages) {
                image.Save();
                // ClipboardItemImageをSaveした後にIdが設定される。そのあとでImageObjectIdsに追加
                ImageObjectIds.Add(image.Id);
            }
        }

        // ファイル
        // LiteDBの別コレクションで保存されているオブジェクト。LiteDBからはLoad**メソッドで取得する。Saveメソッドで保存する
        private List<ClipboardItemFile> _clipboardItemFiles = [];
        public List<ClipboardItemFile> ClipboardItemFiles {
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
            foreach (var file in _clipboardItemFiles) {
                file.Save();
                // ClipboardItemFileをSaveした後にIdが設定される。そのあとでFileObjectIdsに追加
                FileObjectIds.Add(file.Id);
            }
        }

        // OpenAIチャットのChatItemコレクション
        // LiteDBの同一コレクションで保存されているオブジェクト。ClipboardItemオブジェクト生成時にロード、Save時に保存される。
        public List<ChatItem> ChatItems { get; set; } = [];


        // クリップボードの内容の種類
        public ClipboardContentTypes ContentType { get; set; }

        //Tags
        public HashSet<string> Tags { get; set; } = [];

        //説明
        public string Description { get; set; } = "";

        //　貼り付け元のアプリケーション名
        public string SourceApplicationName { get; set; } = "";
        //　貼り付け元のアプリケーションのタイトル
        public string SourceApplicationTitle { get; set; } = "";
        //　貼り付け元のアプリケーションのID
        public int? SourceApplicationID { get; set; }
        //　貼り付け元のアプリケーションのパス
        public string? SourceApplicationPath { get; set; }
        // ピン留め
        public bool IsPinned { get; set; }


        // -------------------------------------------------------------------
        // インスタンスメソッド
        // -------------------------------------------------------------------

        public ClipboardItem Copy() {
            ClipboardItem newItem = new(this.FolderObjectId);
            CopyTo(newItem);
            return newItem;

        }
        public void CopyTo(ClipboardItem newItem) {
            newItem.UpdatedAt = UpdatedAt;
            newItem.Content = Content;
            newItem.ContentType = ContentType;
            newItem.SourceApplicationName = SourceApplicationName;
            newItem.SourceApplicationTitle = SourceApplicationTitle;
            newItem.SourceApplicationID = SourceApplicationID;
            newItem.SourceApplicationPath = SourceApplicationPath;
            newItem.Tags = new HashSet<string>(Tags);
            newItem.Description = Description;
            // 背景情報
            newItem.BackgroundInfo = BackgroundInfo;
            // サマリー
            newItem.Summary = Summary;

            //-- 画像がある場合はコピー
            foreach (var imageObjectId in ImageObjectIds) {
                ClipboardItemImage? image = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(imageObjectId);
                ClipboardItemImage newImage = ClipboardItemImage.Create(newItem, image?.Image ?? throw new Exception(CommonStringResources.Instance.CannotGetImage));
                newImage.ImageBase64 = image.ImageBase64;
                newItem.ImageObjectIds.Add(newImage.Id);
            }
            //-- ファイルがある場合はコピー
            foreach (var FileObjectId in FileObjectIds) {
                ClipboardItemFile? file = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(FileObjectId);
                ClipboardItemFile newFile = ClipboardItemFile.Create(newItem, file?.FilePath ?? string.Empty);
                newItem.FileObjectIds.Add(newFile.Id);
            }
            //-- ChatItemsをコピー
            newItem.ChatItems = new List<ChatItem>(ChatItems);

        }

        public void MergeItems(List<ClipboardItem> items, bool mergeWithHeader) {
            if (ContentType != ClipboardContentTypes.Text) {
                LogWrapper.Error(CommonStringResources.Instance.CannotMergeToNonTextItems);
                return;
            }
            string mergeText = "\n";
            // 現在の時刻をYYYY/MM/DD HH:MM:SS形式で取得
            mergeText += "---\n";

            foreach (var item in items) {

                // Itemの種別がText以外が含まれている場合はマージしない
                if (item.ContentType != ClipboardContentTypes.Text) {
                    LogWrapper.Error(CommonStringResources.Instance.CannotMergeItemsContainingNonTextItems);
                    return;
                }
            }
            foreach (var item in items) {
                if (mergeWithHeader) {
                    // 説明がある場合は追加
                    if (Description != "") {
                        mergeText += item.Description + "\n";
                    }
                    // mergeTextにHeaderを追加
                    mergeText += item.HeaderText + "\n";
                }
                // Contentを追加
                mergeText += item.Content + "\n";

            }
            // mergeTextをContentに追加
            Content += mergeText;

            // Tagsのマージ。重複を除外して追加
            Tags.UnionWith(items.SelectMany(item => item.Tags));
        }

        // タグ表示用の文字列
        public string TagsString() {
            return string.Join(",", Tags);
        }

        public string UpdatedAtString {
            get {
                return UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        public string ContentTypeString {
            get {
                if (ContentType == ClipboardContentTypes.Text) {
                    return "Text";
                } else if (ContentType == ClipboardContentTypes.Files) {
                    return "File";
                } else if (ContentType == ClipboardContentTypes.Image) {
                    return "Image";
                } else {
                    return "Unknown";
                }

            }
        }

        public string? HeaderText {
            get {

                string header1 = "";
                // 更新日時文字列を追加
                header1 += $"[{CommonStringResources.Instance.UpdateDate}]" + UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 作成日時文字列を追加
                header1 += $"[{CommonStringResources.Instance.CreationDateTime}]" + CreatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 貼り付け元のアプリケーション名を追加
                header1 += $"[{CommonStringResources.Instance.SourceAppName}]" + SourceApplicationName + "\n";
                // 貼り付け元のアプリケーションのタイトルを追加
                header1 += $"[{CommonStringResources.Instance.SourceTitle}]" + SourceApplicationTitle + "\n";
                // Tags
                header1 += $"[{CommonStringResources.Instance.Tag}]" + TagsString() + "\n";
                // ピン留め中かどうか
                if (IsPinned) {
                    header1 += $"[{CommonStringResources.Instance.Pinned}]\n";
                }

                if (ContentType == ClipboardContentTypes.Text) {
                    return header1 + $"[{CommonStringResources.Instance.Type}]Text";
                } else if (ContentType == ClipboardContentTypes.Files) {
                    return header1 + $"[{CommonStringResources.Instance.Type}]File";
                } else if (ContentType == ClipboardContentTypes.Image) {
                    return header1 + $"[{CommonStringResources.Instance.Type}]Image";
                } else {
                    return header1 + $"[{CommonStringResources.Instance.Type}]Unknown";
                }
            }
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
            LoadImages();
            LoadFiles();
        }

        // 自分自身をDBに保存する
        public void Save(bool contentIsModified = true) {

            if (contentIsModified) {
                // ★TODO DBControllerに処理を移動する。
                // 画像を保存
                SaveImages();
                // ファイルを保存
                SaveFiles();
            }
            // 保存済みのアイテムを取得
            ClipboardItem? savedItem = ClipboardAppFactory.Instance.GetClipboardDBController().GetItem(Id);

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
            string folderPath = Path.Combine(syncFolder, FolderPath);
            // フォルダが存在しない場合は作成
            if (Directory.Exists(folderPath) == false) {
                Directory.CreateDirectory(folderPath);
            }

            // folderPath + Id + .txtをファイル名として保存
            string syncFilePath = Path.Combine(folderPath, Id + ".txt");
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

        // Embeddingを更新する
        public void UpdateEmbedding() {
            LogWrapper.Info(CommonStringResources.Instance.SaveEmbedding);
            // IPythonAIFunctions.ClipboardInfoを作成
            string content = this.Content;
            // 背景情報を含める場合
            if (ClipboardAppConfig.IncludeBackgroundInfoInEmbedding) {
                content += $"\n---{CommonStringResources.Instance.BackgroundInformation}--\n{BackgroundInfo}";
            }

            ContentInfo clipboardInfo = new(VectorDBUpdateMode.update, this.Id.ToString(), content);

            // VectorDBItemを取得
            VectorDBItemBase folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(GetFolder());
            // Embeddingを保存
            folderVectorDBItem.UpdateIndex(clipboardInfo);
            LogWrapper.Info(CommonStringResources.Instance.SavedEmbedding);
        }

        // 自分自身をDBから削除する
        public void Delete() {
            // IPythonAIFunctions.ClipboardInfoを作成
            ContentInfo clipboardInfo = new(VectorDBUpdateMode.delete, this.Id.ToString(), this.Content);

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
                LogWrapper.Info(CommonStringResources.Instance.DeleteEmbedding);

                // VectorDBItemを取得
                VectorDBItemBase folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(GetFolder());
                // Embeddingを削除
                folderVectorDBItem.DeleteIndex(clipboardInfo);
                LogWrapper.Info(CommonStringResources.Instance.DeletedEmbedding);

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
            // ★TODO イメージ削除とファイル削除を並列で非同期で行う。
            // イメージが存在する場合は削除
            LogWrapper.Info(CommonStringResources.Instance.DeleteEmbedding);
            foreach (var imageObjectId in imageObjectIds) {
                ClipboardItemImage? image = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(imageObjectId);
                if (image == null) {
                    continue;
                }
                image.Delete();

            }
            LogWrapper.Info(CommonStringResources.Instance.DeletedEmbedding);

            // ファイルが存在する場合は削除
            foreach (var fileObjectId in FileObjectIds) {
                ClipboardItemFile? file = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(fileObjectId);
                file?.Delete();
            }

            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItem(this);
        }
        // OpenAIを使用してタイトルを生成する
        public static void CreateAutoTitleWithOpenAI(ClipboardItem item) {
            // Contentがない場合は処理しない
            if (string.IsNullOrEmpty(item.Content)) {
                return;
            }
            // Item.ContentからContentTextを取得.文字数が4096文字を超える場合は4096文字までに制限
            string contentText = item.Content.Length > 4096 ? item.Content[..4096] : item.Content;
            // ChatRequest.CreateTitleを実行
            string result = ChatRequest.CreateTitle(ClipboardAppConfig.CreateOpenAIProperties(), contentText);

            if (string.IsNullOrEmpty(result) == false) {
                item.Description = result;
            }
        }
        // OpenAIを使用してイメージからテキスト抽出する。
        public static void ExtractImageWithOpenAI(ClipboardItem item) {
            // ClipboardItemImagesがない場合は処理しない
            if (item.ClipboardItemImages.Count == 0) {
                return;
            }
            foreach (var image in item.ClipboardItemImages) {
                // ImageBase64がない場合は処理しない
                if (string.IsNullOrEmpty(image.ImageBase64)) {
                    continue;
                }
                string result = ChatRequest.ExtractTextFromImage(ClipboardAppConfig.CreateOpenAIProperties(), [image.ImageBase64]);
                if (string.IsNullOrEmpty(result) == false) {
                    item.Content += "\n" + result;

                    // EmbeddingWhenExtractingTextFromImageがTrueの場合はEmbeddingを更新
                    if (ClipboardAppConfig.EmbeddingWhenExtractingTextFromImage) {
                        image.UpdateEmbedding();
                    }

                }
            }
        }

        // ベクトル検索を実行する
        public static List<VectorSearchResult> VectorSearchCommandExecute(ClipboardItem item) {
            // VectorDBItemを取得
            VectorDBItemBase vectorDBItem = ClipboardAppVectorDBItem.SystemCommonVectorDB;
            vectorDBItem.CollectionName = item.FolderObjectId.ToString();
            string contentText = item.Content;
            // IncludeBackgroundInfoInEmbeddingの場合はBackgroundInfoを含める
            if (ClipboardAppConfig.IncludeBackgroundInfoInEmbedding) {
                contentText += $"\n---{CommonStringResources.Instance.BackgroundInformation}--\n{item.BackgroundInfo}";
            }
            // VectorSearchRequestを作成
            VectorSearchRequest request = new() {
                Query = contentText,
                SearchKWArgs = new Dictionary<string, object> {
                    ["k"] = 10
                }
            };
            // ベクトル検索を実行
            List<VectorSearchResult> results = PythonExecutor.PythonAIFunctions.VectorSearch(ClipboardAppConfig.CreateOpenAIProperties(), vectorDBItem, request);
            return results;
        }


        // 自動でコンテキスト情報を付与するコマンド
        public static void CreateAutoBackgroundInfo(ClipboardItem item) {
            string contentText = item.Content;
            // ベクトルDBの設定
            VectorDBItemBase vectorDBItem = ClipboardAppVectorDBItem.SystemCommonVectorDB;
            vectorDBItem.CollectionName = item.FolderObjectId.ToString();

            string result = ChatRequest.CreateBackgroundInfo(ClipboardAppConfig.CreateOpenAIProperties(), [vectorDBItem], contentText);
            if (string.IsNullOrEmpty(result) == false) {
                item.BackgroundInfo = result;
            }
            // 背景情報に日本語解析追加が有効になっている場合
            if (ClipboardAppConfig.AnalyzeJapaneseSentence) {
                result = ChatRequest.AnalyzeJapaneseSentence(ClipboardAppConfig.CreateOpenAIProperties(), [vectorDBItem], contentText);
                if (string.IsNullOrEmpty(result) == false) {
                    item.BackgroundInfo += "\n" + result;
                }
            }
        }

        // 自動でサマリーを付与するコマンド
        public static void CreateAutoSummary(ClipboardItem item) {
            string contentText = item.Content;
            string result = ChatRequest.CreateSummary(ClipboardAppConfig.CreateOpenAIProperties(), contentText);
            if (string.IsNullOrEmpty(result) == false) {
                item.Summary = result;
            }
        }

        // 自動処理でテキストを抽出」を実行するコマンド
        public static ClipboardItem ExtractTextCommandExecute(ClipboardItem clipboardItem) {

            foreach (var fileObjectId in clipboardItem.FileObjectIds) {
                ClipboardItemFile? clipboardItemFile = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(fileObjectId);

                if (clipboardItemFile == null || clipboardItemFile.FilePath == null) {
                    throw new Exception(CommonStringResources.Instance.FileDoesNotExist);
                }
                string path = clipboardItemFile.FilePath;
                if (string.IsNullOrEmpty(path)) {
                    throw new Exception(CommonStringResources.Instance.FileDoesNotExist);
                }
                try {
                    string text = PythonExecutor.PythonAIFunctions.ExtractText(path);
                    clipboardItem.Content += text + "\n";

                } catch (UnsupportedFileTypeException) {
                    LogWrapper.Info(CommonStringResources.Instance.UnsupportedFileType);
                    return clipboardItem;
                }
                LogWrapper.Info($"{CommonStringResources.Instance.ExtractedText}:{path}");
            }
            return clipboardItem;
        }
    }
}
