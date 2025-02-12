using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows.Media.Imaging;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Image;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resources;

namespace PythonAILib.Model.Content {
    public class ContentItem {

        // 日時のダミー初期値。2000/1/1 0:0:0
        private static readonly DateTime InitialDateTime = new(2000, 1, 1, 0, 0, 0);

        // コンストラクタ
        public ContentItem() {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public ContentItem(ObjectId folderObjectId) {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            CollectionId = folderObjectId;
        }

        public ObjectId Id { get; set; } = ObjectId.NewObjectId();
        // ClipboardFolderのObjectId
        public ObjectId CollectionId { get; set; } = ObjectId.Empty;
        // 生成日時
        public DateTime CreatedAt { get; set; }
        // 更新日時
        public DateTime UpdatedAt { get; set; }
        // ベクトル化日時
        public DateTime VectorizedAt { get; set; } = InitialDateTime;
        // クリップボードの内容
        public string Content { get; set; } = "";
        //説明
        public string Description { get; set; } = "";

        // クリップボードの内容の種類
        public ContentTypes.ContentItemTypes ContentType { get; set; }

        // OpenAIチャットのChatItemコレクション
        // LiteDBの同一コレクションで保存されているオブジェクト。ClipboardItemオブジェクト生成時にロード、Save時に保存される。
        public List<ChatMessage> ChatItems { get; set; } = [];

        // プロンプトテンプレートに基づくチャットの結果
        public PromptChatResult PromptChatResult { get; set; } = new();

        //Tags
        public HashSet<string> Tags { get; set; } = [];

        // 画像ファイルチェッカー
        public ScreenShotCheckItem ScreenShotCheckItem { get; set; } = new();

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

        // 文書の信頼度(0-100)
        public int DocumentReliability { get; set; } = 0;
        // 文書の信頼度の判定理由
        public string DocumentReliabilityReason { get; set; } = "";

        // ReferenceVectorDBItemsがフォルダのReferenceVectorDBItemsと同期済みかどうか
        public bool IsReferenceVectorDBItemsSynced { get; set; } = false;

        // SourcePath
        public string SourcePath { get; set; } = "";

        // 拡張プロパティ
        public Dictionary<string, object> ExtendedProperties { get; set; } = new();

        // 別フォルダに移動
        public virtual void MoveToFolder<T>(T folder) where T : ContentFolder {
            CollectionId = folder.Id;
            Save();
        }

        // 別フォルダにコピー
        public virtual void CopyToFolder(ContentFolder folder) {
            ContentItem newItem = (ContentItem)Copy();
            newItem.CollectionId = folder.Id;
            newItem.Save();
        }

        public virtual object Copy() {
            ContentItem clipboardItem = new(this.CollectionId);
            CopyTo(clipboardItem);
            return clipboardItem;
        }

        public virtual void CopyTo(ContentItem clipboardItem) {

            clipboardItem.UpdatedAt = UpdatedAt;
            clipboardItem.Content = Content;
            clipboardItem.ContentType = ContentType;
            clipboardItem.SourceApplicationName = SourceApplicationName;
            clipboardItem.SourceApplicationTitle = SourceApplicationTitle;
            clipboardItem.SourceApplicationID = SourceApplicationID;
            clipboardItem.SourceApplicationPath = SourceApplicationPath;
            clipboardItem.Tags = new HashSet<string>(Tags);
            clipboardItem.Description = Description;
            clipboardItem.PromptChatResult = PromptChatResult;
            //-- ChatItemsをコピー
            clipboardItem.ChatItems = new List<ChatMessage>(ChatItems);
            // SourcePath
            clipboardItem.SourcePath = SourcePath;
        }

        // タグ表示用の文字列
        public string TagsString() {
            return string.Join(",", Tags);
        }

        // 背景情報
        [BsonIgnore]
        public string BackgroundInfo {
            get {
                return PromptChatResult.GetTextContent(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
            }
            set {
                PromptChatResult.SetTextContent(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(), value);
            }
        }

        // サマリー
        [BsonIgnore]
        public string Summary {
            get {
                return PromptChatResult.GetTextContent(SystemDefinedPromptNames.SummaryGeneration.ToString());
            }
            set {
                PromptChatResult.SetTextContent(SystemDefinedPromptNames.SummaryGeneration.ToString(), value);
            }
        }
        // 文章の信頼度
        [BsonIgnore]
        public string InformationReliability {
            get {
                return PromptChatResult.GetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString());
            }
            set {
                PromptChatResult.SetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString(), value);
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
                PythonAILibManager libManager = PythonAILibManager.Instance;
                ContentFolder? folder = libManager.DataFactory.GetFolderCollection<ContentFolder>().FindById(CollectionId);
                if (folder != null && !string.IsNullOrEmpty(folder.Description)) {
                    header1 += $"[{PythonAILibStringResources.Instance.DocumentCategorySummary}]" + folder.Description + "\n";
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
                foreach (var item in ChatItems) {
                    chatHistoryText += $"--- {item.Role} ---\n";
                    chatHistoryText += item.ContentWithSources + "\n\n";
                }
                return chatHistoryText;
            }
        }

        // Collectionに対応するClipboardFolderを取得
        public T GetFolder<T>() where T : ContentFolder {
            T folder = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>().FindById(CollectionId);
            return folder;
        }


        public string UpdatedAtString {
            get {
                return UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }
        public string CreatedAtString {
            get {
                return CreatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        // ベクトル化日時の文字列
        public string VectorizedAtString {
            get {
                if (VectorizedAt <= InitialDateTime) {
                    return "";
                }
                return VectorizedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        public string ContentTypeString {
            get {
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
        // LiteDBに保存するためのBase64文字列. 元ファイルまたは画像データをBase64エンコードした文字列
        private string _cachedBase64String = "";
        public string CachedBase64String {
            get {
                return _cachedBase64String;
            }
            set {
                if (value == null) {
                    _cachedBase64String = string.Empty;
                } else {
                    _cachedBase64String = value;
                }
            }
        }
        // ファイルパス
        public string FilePath { get; set; } = "";
        // ファイルの最終更新日時
        public long LastModified { get; set; } = 0;

        [BsonIgnore]
        public string DisplayName {
            get {
                if (string.IsNullOrEmpty(FileName)) {
                    return "No Name";
                }
                return FileName;
            }
        }

        [BsonIgnore]
        public string Base64String {
            get {

                // FilePathがない場合はキャッシュを返す
                if (FilePath == null || System.IO.File.Exists(FilePath) == false) {
                    return CachedBase64String;
                }
                // FilePathがある場合はLastModifiedをチェックしてキャッシュを更新する
                if (LastModified < new System.IO.FileInfo(FilePath).LastWriteTime.Ticks) {
                    ContentItemCommands.UpdateCache(this);
                }
                return CachedBase64String;
            }
        }

        // フォルダ名
        [BsonIgnore]
        public string FolderName {
            get {
                return Path.GetDirectoryName(FilePath) ?? "";
            }
        }
        // ファイル名
        [BsonIgnore]
        public string FileName {
            get {
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
                byte[] imageBytes = Convert.FromBase64String(Base64String);
                return ContentTypes.GetBitmapImage(imageBytes);
            }
        }
        [BsonIgnore]
        public System.Drawing.Image? Image {
            get {
                if (!IsImage()) {
                    return null;
                }
                return ContentTypes.GetImageFromBase64(Base64String);
            }
        }

        public bool IsImage() {
            if (Base64String == null) {
                return false;
            }
            return ContentTypes.IsImageData(Convert.FromBase64String(Base64String));
        }
        #endregion


        public virtual VectorDBProperty GetMainVectorSearchProperty() {
            return GetFolder<ContentFolder>().GetMainVectorSearchProperty();
        }

        public virtual void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            ContentItemCommands.DeleteEmbeddings([this]);
            libManager.DataFactory.GetItemCollection<ContentItem>().Delete(Id);
        }

        public static void DeleteItems(List<ContentItem> items) {
            ContentItemCommands.DeleteEmbeddings(items);
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetItemCollection<ContentItem>();
            foreach (ContentItem contentItem in items) {
                collection.Delete(contentItem.Id);
            }
        }

        public virtual void Save(bool updateLastModifiedTime = true, bool applyAutoProcess = false) {
            PythonAILibManager libManager = PythonAILibManager.Instance;

            if (updateLastModifiedTime) {
                // 更新日時を設定
                UpdatedAt = DateTime.Now;
            }
            if (applyAutoProcess) {
                // 自動処理を適用

            }
            libManager.DataFactory.GetItemCollection<ContentItem>().Upsert(this);

        }

        // ClipboardItemをJSON文字列に変換する
        public static string ToJson<T>(T item) where T : ContentItem {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(item, options);
        }


        // JSON文字列をClipboardItemに変換する
        public static T? FromJson<T>(string json) where T : ContentItem {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            T? item = System.Text.Json.JsonSerializer.Deserialize<T>(json, options);
            return item;

        }

    }
}
