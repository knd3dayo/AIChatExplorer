using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows.Media.Imaging;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LiteDB;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resources;

namespace PythonAILib.Model.Content {
    public class ContentItemWrapper {

        public ContentItemWrapper(ContentItemEntity contentItem) {
            Entity = contentItem;
        }
        public ContentItemWrapper(ContentFolderEntity folder) {
            Entity = new() {
                Folder = folder
            };
        }

        public ContentItemEntity Entity { get; set; }

        [BsonIgnore]
        // Folder
        public ContentFolderWrapper? Folder {
            get {
                if (Entity.Folder == null) {
                    return null;
                }
                return new ContentFolderWrapper(Entity.Folder);
            }
            set {
                if (value == null) {
                    Entity.Folder = null;
                } else {
                    Entity.Folder = value.Entity;
                }
            }
        }

        // 生成日時
        public DateTime CreatedAt {
            get {
                return Entity.CreatedAt;
            }
            set {
                Entity.CreatedAt = value;
            }
        }
        // 更新日時
        public DateTime UpdatedAt {
            get {
                return Entity.UpdatedAt;
            }
            set {
                Entity.UpdatedAt = value;
            }
        }
        // ベクトル化日時
        public DateTime VectorizedAt {
            get {
                return Entity.VectorizedAt;
            }
            set {
                Entity.VectorizedAt = value;
            }
        }
        // クリップボードの内容
        public string Content {
            get {
                return Entity.Content;
            }
            set {
                Entity.Content = value;
            }
        }
        //説明
        public string Description {
            get {
                return Entity.Description;
            }
            set {
                Entity.Description = value;
            }
        }

        // クリップボードの内容の種類
        public ContentTypes.ContentItemTypes ContentType {
            get {
                return Entity.ContentType;
            }
            set {
                Entity.ContentType = value;
            }
        }

        // OpenAIチャットのChatItemコレクション
        // LiteDBの同一コレクションで保存されているオブジェクト。ClipboardItemオブジェクト生成時にロード、Save時に保存される。
        public List<ChatMessage> ChatItems {
            get {
                return Entity.ChatItems;
            }
            set {
                Entity.ChatItems = value;
            }
        }

        // プロンプトテンプレートに基づくチャットの結果
        public PromptChatResult PromptChatResult {
            get {
                return Entity.PromptChatResult;
            }
            set {
                Entity.PromptChatResult = value;
            }
        }

        //Tags
        public HashSet<string> Tags {
            get {
                return [.. Entity.Tags.Select(tag => tag.Tag)];
            }
            set {
                Entity.Tags = [.. value.Select(tag => new TagItemEntity() { Tag = tag })];
            }
        }

        //　貼り付け元のアプリケーション名
        public string SourceApplicationName {
            get {
                return Entity.SourceApplicationName;
            }
            set {
                Entity.SourceApplicationName = value;
            }
        }
        //　貼り付け元のアプリケーションのタイトル
        public string SourceApplicationTitle {
            get {
                return Entity.SourceApplicationTitle;
            }
            set {
                Entity.SourceApplicationTitle = value;
            }
        }
        //　貼り付け元のアプリケーションのID
        public int? SourceApplicationID {
            get {
                return Entity.SourceApplicationID;
            }
            set {
                Entity.SourceApplicationID = value;
            }
        }
        //　貼り付け元のアプリケーションのパス
        public string? SourceApplicationPath {
            get {
                return Entity.SourceApplicationPath;
            }
            set {
                Entity.SourceApplicationPath = value;
            }
        }
        // ピン留め
        public bool IsPinned {
            get {
                return Entity.IsPinned;
            }
            set {
                Entity.IsPinned = value;
            }
        }

        // 文書の信頼度(0-100)
        public int DocumentReliability {
            get {
                return Entity.DocumentReliability;
            }
            set {
                Entity.DocumentReliability = value;
            }
        }
        // 文書の信頼度の判定理由
        public string DocumentReliabilityReason {
            get {
                return Entity.DocumentReliabilityReason;
            }
            set {
                Entity.DocumentReliabilityReason = value;
            }
        }

        // ReferenceVectorDBItemsがフォルダのReferenceVectorDBItemsと同期済みかどうか
        public bool IsReferenceVectorDBItemsSynced {
            get {
                return Entity.IsReferenceVectorDBItemsSynced;
            }
            set {
                Entity.IsReferenceVectorDBItemsSynced = value;
            }
        }
        // SourcePath
        public string SourcePath {
            get {
                Entity.ExtendedProperties.TryGetValue("SourcePath", out object? value);
                return value?.ToString() ?? "";
            }
            set {
                Entity.ExtendedProperties["SourcePath"] = value;
            }
        }

        // SourceType 
        public string SourceType {
            get {
                Entity.ExtendedProperties.TryGetValue("SourceType", out object? value);
                string? sourceTypeString = value?.ToString();
                if (!string.IsNullOrEmpty(sourceTypeString)) {
                    return sourceTypeString;
                } else {
                    return ContentSourceType.Application;
                }
            }
            set {
                Entity.ExtendedProperties["SourceType"] = value.ToString();
            }
        }
        // ファイルの最終更新日時
        [BsonIgnore]
        public long LastModified {
            get {
                Entity.ExtendedProperties.TryGetValue("LastModified", out object? value);
                return value != null ? (long)value : 0;
            }
            set {
                Entity.ExtendedProperties["LastModified"] = value;
            }
        }

        // LiteDBに保存するためのBase64文字列. 元ファイルまたは画像データをBase64エンコードした文字列
        public string Base64Image {
            get {
                if (Entity.ContentType == ContentTypes.ContentItemTypes.Files) {
                    (bool isImage, ContentTypes.ImageType imageType) = ContentTypes.IsImageFile(SourcePath);
                    if (isImage) {
                        byte[] imageBytes = System.IO.File.ReadAllBytes(SourcePath);
                        return Convert.ToBase64String(imageBytes);
                    }
                }
                if (string.IsNullOrEmpty(Entity.CachedBase64String) == false) {
                    return Entity.CachedBase64String;
                }
                return "";
            }
            set {
                Entity.CachedBase64String = value;
            }
        }

        // タグ表示用の文字列
        public string TagsString() {
            return string.Join(",", Entity.Tags);
        }


        // 別フォルダに移動
        public virtual void MoveTo(ContentFolderWrapper folder) {
            Folder = folder;
            Save();
        }

        // 別フォルダにコピー
        public virtual void CopyToFolder(ContentFolderWrapper folder) {
            ContentItemWrapper newItem = Copy();
            newItem.Folder = folder;
            newItem.Save();
        }

        public virtual ContentItemWrapper Copy() {
            return new ContentItemWrapper(Entity.Copy());
        }


        // 背景情報
        [BsonIgnore]
        public string BackgroundInfo {
            get {
                return Entity.PromptChatResult.GetTextContent(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
            }
            set {
                Entity.PromptChatResult.SetTextContent(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(), value);
            }
        }

        // サマリー
        [BsonIgnore]
        public string Summary {
            get {
                return Entity.PromptChatResult.GetTextContent(SystemDefinedPromptNames.SummaryGeneration.ToString());
            }
            set {
                Entity.PromptChatResult.SetTextContent(SystemDefinedPromptNames.SummaryGeneration.ToString(), value);
            }
        }
        // 文章の信頼度
        [BsonIgnore]
        public string InformationReliability {
            get {
                return Entity.PromptChatResult.GetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString());
            }
            set {
                Entity.PromptChatResult.SetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString(), value);
            }
        }
        [BsonIgnore]
        public string HeaderText {
            get {

                string header1 = "";

                // タイトルを追加
                header1 += $"[{PythonAILibStringResources.Instance.Title}]" + Description + "\n";
                // 作成日時文字列を追加
                header1 += $"[{PythonAILibStringResources.Instance.CreationDateTime}]" + CreatedAtString + "\n";
                // 更新日時文字列を追加
                header1 += $"[{PythonAILibStringResources.Instance.UpdateDate}]" + UpdatedAtString + "\n";
                // ベクトル化日時文字列を追加
                header1 += $"[{PythonAILibStringResources.Instance.VectorizedDate}]" + VectorizedAtString + "\n";
                // 貼り付け元のアプリケーション名を追加
                header1 += $"[{PythonAILibStringResources.Instance.SourceAppName}]" + SourceApplicationName + "\n";
                // 貼り付け元のアプリケーションのタイトルを追加
                header1 += $"[{PythonAILibStringResources.Instance.SourceTitle}]" + SourceApplicationTitle + "\n";
                // SourcePathを追加
                header1 += $"[{PythonAILibStringResources.Instance.SourcePath}]" + SourcePath + "\n";

                if (ContentType == ContentTypes.ContentItemTypes.Text) {
                    header1 += $"[{PythonAILibStringResources.Instance.Type}]Text";
                } else if (ContentType == ContentTypes.ContentItemTypes.Files) {
                    header1 += $"[{PythonAILibStringResources.Instance.Type}]File";
                } else if (ContentType == ContentTypes.ContentItemTypes.Image) {
                    header1 += $"[{PythonAILibStringResources.Instance.Type}]Image";
                } else {
                    header1 += $"[{PythonAILibStringResources.Instance.Type}]Unknown";
                }
                // 文書の信頼度
                header1 += $"\n[{PythonAILibStringResources.Instance.DocumentReliability}]" + DocumentReliability + "%\n";
                // ★TODO フォルダーの説明を文章のカテゴリーの説明として追加
                if (Folder != null && !string.IsNullOrEmpty(Folder.Description)) {
                    header1 += $"[{PythonAILibStringResources.Instance.DocumentCategorySummary}]" + Folder.Description + "\n";
                }

                // Tags
                header1 += $"[{PythonAILibStringResources.Instance.Tag}]" + TagsString() + "\n";
                return header1;
            }
        }
        [BsonIgnore]
        public string ChatItemsText {
            get {
                // chatHistoryItemの内容をテキスト化
                string chatHistoryText = "";
                foreach (var item in Entity.ChatItems) {
                    chatHistoryText += $"--- {item.Role} ---\n";
                    chatHistoryText += item.ContentWithSources + "\n\n";
                }
                return chatHistoryText;
            }
        }


        [BsonIgnore]
        public string UpdatedAtString {
            get {
                return Entity.UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        [BsonIgnore]
        public string CreatedAtString {
            get {
                return Entity.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        [BsonIgnore]
        // ベクトル化日時の文字列
        public string VectorizedAtString {
            get {
                if (Entity.VectorizedAt <= ContentItem.InitialDateTime) {
                    return "";
                }
                return Entity.VectorizedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        [BsonIgnore]
        public string ContentTypeString {
            get {
                ContentTypes.ContentItemTypes ContentType = Entity.ContentType;
                if (ContentType == ContentTypes.ContentItemTypes.Text) {
                    return "Text";
                } else if (ContentType == ContentTypes.ContentItemTypes.Files) {
                    return "File";
                } else if (ContentType == ContentTypes.ContentItemTypes.Image) {
                    return "Image";
                } else {
                    return "Unknown";
                }
            }
        }


        #region ファイル/画像関連

        [BsonIgnore]
        public string DisplayName {
            get {
                if (string.IsNullOrEmpty(FileName)) {
                    return "No Name";
                }
                return FileName;
            }
        }

        // フォルダ名
        [BsonIgnore]
        public string FolderName {
            get {
                string FilePath = SourcePath;
                return Path.GetDirectoryName(FilePath) ?? "";
            }
        }
        // ファイル名
        [BsonIgnore]
        public string FileName {
            get {
                string FilePath = SourcePath;
                return Path.GetFileName(FilePath) ?? "";
            }
        }
        // フォルダ名 + \n + ファイル名
        [BsonIgnore]
        public string FolderAndFileName {
            get {
                return FolderName + Path.PathSeparator + FileName;
            }
        }

        // 画像イメージ
        [BsonIgnore]
        public BitmapImage? BitmapImage {
            get {
                if (!IsImage()) {
                    return null;
                }
                byte[] imageBytes = Convert.FromBase64String(Base64Image);
                return ContentTypes.GetBitmapImage(imageBytes);
            }
        }
        [BsonIgnore]
        public System.Drawing.Image? Image {
            get {
                if (!IsImage()) {
                    return null;
                }
                return ContentTypes.GetImageFromBase64(Base64Image);
            }
        }

        // Load
        public virtual void Load(Action beforeAction, Action afterAction) { }

        // Collectionに対応するClipboardFolderを取得
        public virtual ContentFolderWrapper? GetFolder() {
            if (Entity.Folder == null) {
                return null;
            }
            return new(Entity.Folder);
        }


        public bool IsImage() {

            if (string.IsNullOrEmpty(Base64Image)) {
                return false;
            }
            if (ContentTypes.GetImageTypeFromBase64(Base64Image).Item1) {
                return true;
            } else {
                Base64Image = "";
            }
            if (Entity.ContentType == ContentTypes.ContentItemTypes.Files) {
                (bool isImage, ContentTypes.ImageType imageType) = ContentTypes.IsImageFile(SourcePath);
                return isImage;
            }
            return false;
        }
        #endregion


        public virtual VectorDBProperty GetMainVectorSearchProperty() {
            return GetFolder().GetMainVectorSearchProperty();
        }

        public virtual void Delete() {
            ContentItemCommands.DeleteEmbeddings([this]);
            using PythonAILibDBContext db = new PythonAILibDBContext();
            db.ContentItems.Remove(Entity);
            db.SaveChanges();
        }

        public static void DeleteItems(List<ContentItemWrapper> items) {
            ContentItemCommands.DeleteEmbeddings(items);
            using PythonAILibDBContext db = new PythonAILibDBContext();
            db.ContentItems.RemoveRange(items.Select(item => item.Entity));
            db.SaveChanges();

        }

        public virtual void Save(bool updateLastModifiedTime = true, bool applyAutoProcess = false) {
            if (updateLastModifiedTime) {
                // 更新日時を設定
                UpdatedAt = DateTime.Now;
            }
            if (applyAutoProcess) {
                // 自動処理を適用

            }
            using PythonAILibDBContext db = new PythonAILibDBContext();
            db.ContentItems.Update(Entity);
            db.SaveChanges();
        }

        // ClipboardItemをJSON文字列に変換する
        public static string ToJson(ContentItemWrapper item) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(item, options);
        }


        // JSON文字列をClipboardItemに変換する
        public static ContentItemWrapper? FromJson(string json) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            ContentItemWrapper? item = System.Text.Json.JsonSerializer.Deserialize<ContentItemWrapper>(json, options);
            return item;

        }

    }
}
