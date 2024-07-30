using System.Drawing;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibGit2Sharp;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using QAChat.Model;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public enum ClipboardContentTypes {
        Text,
        Files,
        Image,
        Unknown
    }
    public class ClipboardItem {
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

            //-- 画像がある場合はコピー
            foreach (var imageObjectId in ImageObjectIds) {
                ClipboardItemImage? image = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(imageObjectId);
                ClipboardItemImage newImage = ClipboardItemImage.Create(newItem, image?.Image ?? throw new ThisApplicationException("画像が取得できません"));
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

        public void MergeItems(List<ClipboardItem> items, bool mergeWithHeader, Action<ActionMessage>? action) {
            if (ContentType != ClipboardContentTypes.Text) {
                action?.Invoke(ActionMessage.Error("Text以外のアイテムへのマージはできません"));
                return;
            }
            string mergeText = "\n";
            // 現在の時刻をYYYY/MM/DD HH:MM:SS形式で取得
            mergeText += "---\n";

            foreach (var item in items) {

                // Itemの種別がText以外が含まれている場合はマージしない
                if (item.ContentType != ClipboardContentTypes.Text) {
                    action?.Invoke(ActionMessage.Error("Text以外のアイテムが含まれているアイテムはマージできません"));
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
        public string ContentSummary {
            get {
                if (Content == null) {
                    return "";
                }
                if (Content.Length > 50) {
                    return Content[..50] + "...";
                } else {
                    return Content;
                }
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
                header1 += "[更新日時]" + UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 作成日時文字列を追加
                header1 += "[作成日時]" + CreatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 貼り付け元のアプリケーション名を追加
                header1 += "[ソースアプリ名]" + SourceApplicationName + "\n";
                // 貼り付け元のアプリケーションのタイトルを追加
                header1 += "[ソースタイトル]" + SourceApplicationTitle + "\n";
                // Tags
                header1 += "[タグ]" + TagsString() + "\n";
                // ピン留め中かどうか
                if (IsPinned) {
                    header1 += "[ピン留めしてます]\n";
                }

                if (ContentType == ClipboardContentTypes.Text) {
                    return header1 + "[種類]Text";
                } else if (ContentType == ClipboardContentTypes.Files) {
                    return header1 + "[種類]File";
                } else if (ContentType == ClipboardContentTypes.Image) {
                    return header1 + "[種類]Image";
                } else {
                    return header1 + "[種類]Unknown";
                }
            }
        }

        // Collectionに対応するClipboardFolderを取得
        public ClipboardFolder GetFolder(Type? objectType = null) {
            ClipboardFolder? folder = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(FolderObjectId);
            return folder ?? throw new Exception("フォルダが取得できません");
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
        public static ClipboardItem? FromJson(string json, Action<ActionMessage> action) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            ClipboardItem? item = System.Text.Json.JsonSerializer.Deserialize<ClipboardItem>(json, options);
            if (item == null) {
                action(ActionMessage.Error("JSON文字列をClipboardItemに変換できませんでした"));
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
                    SaveToOSFolder();
                });

                // Embeddingを更新
                Task.Run(() => {
                    UpdateEmbedding();
                });
            }
        }

        // OS上のファイルに保存する
        private void SaveToOSFolder() {
            LogWrapper.Info("OS上のファイルに保存します");
            // SyncClipboardItemAndOSFolder == trueの場合はOSのフォルダにも保存
            if (ClipboardAppConfig.SyncClipboardItemAndOSFolder) {
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
                    try {

                        using (var repo = new Repository(ClipboardAppConfig.SyncFolderName)) {
                            Commands.Stage(repo, syncFilePath);
                            Signature author = new("ClipboardApp", "ClipboardApp", DateTimeOffset.Now);
                            Signature committer = author;
                            repo.Commit("Auto commit", author, committer);
                            LogWrapper.Info($"Gitにコミットしました:{syncFilePath} {ClipboardAppConfig.SyncFolderName}");
                        }
                    } catch (RepositoryNotFoundException e) {
                        LogWrapper.Info($"リポジトリが見つかりませんでした:{ClipboardAppConfig.SyncFolderName} {e.Message}");
                    } catch (EmptyCommitException e) {
                        LogWrapper.Info($"コミットが空です:{syncFilePath} {e.Message}");
                    }
                }

            }
            LogWrapper.Info("OS上のファイルに保存しました");
        }

        // Embeddingを更新する
        private void UpdateEmbedding() {
            if (ClipboardAppConfig.AutoEmbedding) {
                LogWrapper.Info("Embeddingを保存します");
                // IPythonFunctions.ClipboardInfoを作成
                IPythonFunctions.ContentInfo clipboardInfo = new IPythonFunctions.ContentInfo(IPythonFunctions.VectorDBUpdateMode.update, this.Id.ToString(), this.Content);

                // VectorDBItemを取得
                VectorDBItem folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(GetFolder());
                // Embeddingを保存
                folderVectorDBItem.UpdateIndex(clipboardInfo);
                LogWrapper.Info("Embeddingを保存しました");
            }
        }

        // 自分自身をDBから削除する
        public void Delete() {
            // AutoEmbedding == Trueの場合はEmbeddingを削除
            Task.Run(() => {
                LogWrapper.Info("Embeddingを削除します");
                if (ClipboardAppConfig.AutoEmbedding) {
                    // IPythonFunctions.ClipboardInfoを作成
                    IPythonFunctions.ContentInfo clipboardInfo = new IPythonFunctions.ContentInfo(IPythonFunctions.VectorDBUpdateMode.delete, this.Id.ToString(), this.Content);

                    // VectorDBItemを取得
                    VectorDBItem folderVectorDBItem = ClipboardAppVectorDBItem.GetFolderVectorDBItem(GetFolder());

                    // Embeddingを削除
                    folderVectorDBItem.DeleteIndex(clipboardInfo);
                }
                LogWrapper.Info("Embeddingを削除しました");
            });
            Task.Run(() => {
                LogWrapper.Info("OS上のファイルを削除します");
                // SyncClipboardItemAndOSFolder == trueの場合はOSのフォルダからも削除
                if (ClipboardAppConfig.SyncClipboardItemAndOSFolder) {
                    // 保存先フォルダを取得
                    string folderPath = ClipboardAppConfig.SyncFolderName;
                    // syncFolder/フォルダ名を取得
                    folderPath = Path.Combine(folderPath, FolderPath);

                    // ClipboardFolderのFolderPath + Id + .txtをファイル名として削除
                    string syncFilePath = Path.Combine(folderPath, Id + ".txt");
                    // ファイルが存在する場合は削除
                    if (File.Exists(syncFilePath)) {
                        File.Delete(syncFilePath);
                    }
                    // 自動コミットが有効の場合はGitにコミット
                    if (ClipboardAppConfig.AutoCommit) {
                        try {

                            using (var repo = new Repository(ClipboardAppConfig.SyncFolderName)) {
                                Commands.Stage(repo, syncFilePath);
                                Signature author = new("ClipboardApp", "ClipboardApp", DateTimeOffset.Now);
                                Signature committer = author;
                                repo.Commit("Auto commit", author, committer);
                                LogWrapper.Info($"Gitにコミットしました:{syncFilePath} {ClipboardAppConfig.SyncFolderName}");
                            }

                        } catch (RepositoryNotFoundException e) {
                            LogWrapper.Info($"リポジトリが見つかりませんでした:{ClipboardAppConfig.SyncFolderName} {e.Message}");
                        } catch (EmptyCommitException e) {
                            LogWrapper.Info($"コミットが空です:{syncFilePath} {e.Message}");
                        }
                    }

                }
                LogWrapper.Info("OS上のファイルを削除しました");

            });
            // イメージが存在する場合は削除
            foreach (var imageObjectId in ImageObjectIds) {
                ClipboardItemImage? image = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(imageObjectId);
                image?.Delete();
            }
            // ファイルが存在する場合は削除
            foreach (var fileObjectId in FileObjectIds) {
                ClipboardItemFile? file = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(fileObjectId);
                file?.Delete();
            }

            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteItem(this);
        }

        public static void CreateAutoTitle(ClipboardItem item) {
            // TextとImageの場合
            if (item.ContentType == ClipboardContentTypes.Text || item.ContentType == ClipboardContentTypes.Image) {
                item.Description = $"{item.SourceApplicationTitle}";
            }
            // Fileの場合
            else if (item.ContentType == ClipboardContentTypes.Files) {
                item.Description = $"{item.SourceApplicationTitle}";
                // Contentのサイズが50文字以上の場合は先頭20文字 + ... + 最後の30文字をDescriptionに設定
                if (item.Content.Length > 20) {
                    item.Description += " ファイル：" + item.Content[..20] + "..." + item.Content[^30..];
                } else {
                    item.Description += " ファイル：" + item.Content;
                }
            }
        }
        // OpenAIを使用してタイトルを生成する
        public static void CreateAutoTitleWithOpenAI(ClipboardItem item) {

            // ChatCommandExecuteを実行
            string prompt = "この文章のタイトルを生成してください。\n";
            ChatRequest chatController = new(ClipboardAppConfig.CreateOpenAIProperties());
            chatController.ChatMode = OpenAIExecutionModeEnum.Normal;
            chatController.PromptTemplateText = prompt;
            // Item.ContentからContentTextを取得.文字数が4096文字を超える場合は4096文字までに制限
            chatController.ContentText = item.Content.Length > 4096 ? item.Content[..4096] : item.Content;
            ChatResult? result = chatController.ExecuteChat();
            if (result != null) {
                item.Description += result.Response;
            }
        }
        // OpenAIを使用してイメージからテキスト抽出する。
        public static void ExtractImageWithOpenAI(ClipboardItem item) {
            // ClipboardItemImagesがない場合は処理しない
            if (item.ClipboardItemImages.Count == 0) {
                return;
            }
            // ChatCommandExecuteを実行
            string prompt = "この画像のテキストを抽出してください。\n";
            ChatRequest chatController = new(ClipboardAppConfig.CreateOpenAIProperties()) {
                ChatMode = OpenAIExecutionModeEnum.Normal,
                PromptTemplateText = prompt,
            };
            // ChatRequestにImageURLsを設定
            chatController.ImageURLs = item.ClipboardItemImages.Select(image => ChatRequest.CreateImageURL(image.ImageBase64)).ToList();
            ChatResult? result = chatController.ExecuteChat();
            if (result != null) {
                item.Content += result.Response;
            }
        }

        // 自動でタグを付与するコマンド
        public static void CreateAutoTags(ClipboardItem item) {
            // PythonでItem.ContentからEntityを抽出
            string spacyModel = WpfAppCommon.Properties.Settings.Default.SpacyModel;
            HashSet<string> entities = PythonExecutor.PythonFunctions.ExtractEntity(spacyModel, item.Content);
            foreach (var entity in entities) {

                // タグを追加
                item.Tags.Add(entity);
            }

        }
        // 自動でコンテキスト情報を付与するコマンド
        public static void CreateAutoBackgroundInfo(ClipboardItem item) {
            // LangchainChatを実行
            string prompt = "汎用ベクトルDBの情報を参考にして、この文章の背景情報を生成してください。\n";
            ChatRequest chatController = new(ClipboardAppConfig.CreateOpenAIProperties());
            chatController.ChatMode = OpenAIExecutionModeEnum.RAG;
            chatController.PromptTemplateText = prompt;
            chatController.ContentText = item.Content;

            // ベクトルDBの設定
            VectorDBItem vectorDBItem = ClipboardAppVectorDBItem.SystemCommonVectorDB;
            vectorDBItem.CollectionName = item.FolderObjectId.ToString();

            chatController.VectorDBItems = [vectorDBItem];

            ChatResult? result = chatController.ExecuteChat();
            if (result != null) {
                item.BackgroundInfo = result.Response;
            }
        }

        // 自動処理でテキストを抽出」を実行するコマンド
        public static ClipboardItem ExtractTextCommandExecute(ClipboardItem clipboardItem) {

            foreach (var fileObjectId in clipboardItem.FileObjectIds) {
                ClipboardItemFile? clipboardItemFile = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemFile(fileObjectId);

                if (clipboardItemFile == null) {
                    throw new ThisApplicationException("ファイルが取得できません");
                }
                if (clipboardItemFile.FilePath == null) {
                    throw new ThisApplicationException("ファイルパスが取得できません");
                }
                string path = clipboardItemFile.FilePath;
                if (string.IsNullOrEmpty(path)) {
                    throw new ThisApplicationException("ファイルパスが取得できません");
                }
                try {
                    string text = PythonExecutor.PythonFunctions.ExtractText(path);
                    clipboardItem.Content += text + "\n";

                } catch (UnsupportedFileTypeException) {
                    LogWrapper.Info("サポートされていないファイル形式です");
                    return clipboardItem;
                }
                LogWrapper.Info($"{path}のテキストを抽出しました");

            }
            return clipboardItem;

        }

        // 自動処理でデータをマスキング」を実行するコマンド
        public ClipboardItem MaskDataCommandExecute() {

            if (this.ContentType != ClipboardContentTypes.Text) {
                LogWrapper.Info("テキスト以外のコンテンツはマスキングできません");
                return this;
            }
            string spacyModel = WpfAppCommon.Properties.Settings.Default.SpacyModel;
            string result = PythonExecutor.PythonFunctions.GetMaskedString(spacyModel, this.Content);
            this.Content = result;

            LogWrapper.Info("データをマスキングしました");
            return this;
        }

        public static string CovertMaskedDataToOriginalData(MaskedData? maskedData, string maskedText) {
            if (maskedData == null) {
                return maskedText;
            }
            // マスキングデータをもとに戻す
            string result = maskedText;
            foreach (var entity in maskedData.Entities) {
                // ステータスバーにメッセージを表示
                LogWrapper.Info($"マスキングデータをもとに戻します: {entity.Before} -> {entity.After}\n");
                result = result.Replace(entity.After, entity.Before);
            }
            return result;
        }

        // 画像からイメージを抽出するコマンド
        public static ClipboardItem ExtractTextFromImageCommandExecute(ClipboardItem clipboardItem) {
            if (clipboardItem.ContentType != ClipboardContentTypes.Image) {
                throw new ThisApplicationException("画像以外のコンテンツはテキストを抽出できません");
            }
            foreach (var imageObjectId in clipboardItem.ImageObjectIds) {
                ClipboardItemImage? imageItem = ClipboardAppFactory.Instance.GetClipboardDBController().GetItemImage(imageObjectId);
                if (imageItem == null) {
                    throw new ThisApplicationException("画像が取得できません");
                }
                Image? image = imageItem.Image;
                if (image == null) {
                    throw new ThisApplicationException("画像が取得できません");
                }
                string text = PythonExecutor.PythonFunctions.ExtractTextFromImage(image, ClipboardAppConfig.TesseractExePath);
                clipboardItem.Content += text + "\n";
            }

            return clipboardItem;
        }

    }
}
