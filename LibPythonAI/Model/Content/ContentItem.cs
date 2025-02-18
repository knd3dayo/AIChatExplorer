using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Image;
using PythonAILib.Model.Prompt;

namespace PythonAILib.Model.Content {
    public class ContentItem {

        // 日時のダミー初期値。2000/1/1 0:0:0
        public static readonly DateTime InitialDateTime = new(2000, 1, 1, 0, 0, 0);

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

        // ObjectPath
        public string ObjectPath {
            get {
                // Folderを取得
                var folder = GetFolder<ContentFolder>();
                // folderのFolderPath + Path.Separator + Idを返す
                return $"{folder.FolderPath}/{Id}";
            }
        }
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

        // ファイルの最終更新日時
        public long LastModified { get; set; } = 0;


        // 拡張プロパティ
        public Dictionary<string, object> ExtendedProperties { get; set; } = new();

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
        public ContentItem Copy() {
            ContentItem clipboardItem = new(this.CollectionId);
            CopyTo(clipboardItem);
            return clipboardItem;
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

        public virtual void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            libManager.DataFactory.GetItemCollection<ContentItem>().Delete(Id);
        }

        // Collectionに対応するClipboardFolderを取得
        public T GetFolder<T>() where T : ContentFolder {
            T folder = PythonAILibManager.Instance.DataFactory.GetFolderCollection<T>().FindById(CollectionId);
            return folder;
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
