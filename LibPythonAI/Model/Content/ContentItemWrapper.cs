using System.IO;
using System.Windows.Media.Imaging;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Image;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.Resources;

namespace PythonAILib.Model.Content {
    public class ContentItemWrapper {

        public ContentItemWrapper(ContentItem contentItem) {
            ContentItemInstance = contentItem;
        }
        public ContentItemWrapper(LiteDB.ObjectId folderObjectId) {
            ContentItemInstance = new(folderObjectId);
        }

        protected ContentItem ContentItemInstance { get; set; }


        public ObjectId Id {
            get {
                return ContentItemInstance.Id;
            }
            set {
                ContentItemInstance.Id = value;
            }
        }
        [BsonIgnore]
        // Folder
        public ContentFolderWrapper Folder {
            get {
                return GetFolder();
            }
        }

        // ClipboardFolderのObjectId
        public ObjectId CollectionId {
            get {
                return ContentItemInstance.CollectionId;
            }
            set {
                ContentItemInstance.CollectionId = value;
            }
        }
        // 生成日時
        public DateTime CreatedAt {
            get {
                return ContentItemInstance.CreatedAt;
            }
            set {
                ContentItemInstance.CreatedAt = value;
            }
        }
        // 更新日時
        public DateTime UpdatedAt {
            get {
                return ContentItemInstance.UpdatedAt;
            }
            set {
                ContentItemInstance.UpdatedAt = value;
            }
        }
        // ベクトル化日時
        public DateTime VectorizedAt {
            get {
                return ContentItemInstance.VectorizedAt;
            }
            set {
                ContentItemInstance.VectorizedAt = value;
            }
        }
        // クリップボードの内容
        public string Content {
            get {
                return ContentItemInstance.Content;
            }
            set {
                ContentItemInstance.Content = value;
            }
        }
        //説明
        public string Description {
            get {
                return ContentItemInstance.Description;
            }
            set {
                ContentItemInstance.Description = value;
            }
        }

        // クリップボードの内容の種類
        public ContentTypes.ContentItemTypes ContentType {
            get {
                return ContentItemInstance.ContentType;
            }
            set {
                ContentItemInstance.ContentType = value;
            }
        }

        // OpenAIチャットのChatItemコレクション
        // LiteDBの同一コレクションで保存されているオブジェクト。ClipboardItemオブジェクト生成時にロード、Save時に保存される。
        public List<ChatMessage> ChatItems {
            get {
                return ContentItemInstance.ChatItems;
            }
            set {
                ContentItemInstance.ChatItems = value;
            }
        }

        // プロンプトテンプレートに基づくチャットの結果
        public PromptChatResult PromptChatResult {
            get {
                return ContentItemInstance.PromptChatResult;
            }
            set {
                ContentItemInstance.PromptChatResult = value;
            }
        }

        //Tags
        public HashSet<string> Tags {
            get {
                return ContentItemInstance.Tags;
            }
            set {
                ContentItemInstance.Tags = value;
            }
        }

        // 画像ファイルチェッカー
        public ScreenShotCheckItem ScreenShotCheckItem {
            get {
                return ContentItemInstance.ScreenShotCheckItem;
            }
            set {
                ContentItemInstance.ScreenShotCheckItem = value;
            }
        }

        //　貼り付け元のアプリケーション名
        public string SourceApplicationName {
            get {
                return ContentItemInstance.SourceApplicationName;
            }
            set {
                ContentItemInstance.SourceApplicationName = value;
            }
        }
        //　貼り付け元のアプリケーションのタイトル
        public string SourceApplicationTitle {
            get {
                return ContentItemInstance.SourceApplicationTitle;
            }
            set {
                ContentItemInstance.SourceApplicationTitle = value;
            }
        }
        //　貼り付け元のアプリケーションのID
        public int? SourceApplicationID {
            get {
                return ContentItemInstance.SourceApplicationID;
            }
            set {
                ContentItemInstance.SourceApplicationID = value;
            }
        }
        //　貼り付け元のアプリケーションのパス
        public string? SourceApplicationPath {
            get {
                return ContentItemInstance.SourceApplicationPath;
            }
            set {
                ContentItemInstance.SourceApplicationPath = value;
            }
        }
        // ピン留め
        public bool IsPinned {
            get {
                return ContentItemInstance.IsPinned;
            }
            set {
                ContentItemInstance.IsPinned = value;
            }
        }

        // 文書の信頼度(0-100)
        public int DocumentReliability {
            get {
                return ContentItemInstance.DocumentReliability;
            }
            set {
                ContentItemInstance.DocumentReliability = value;
            }
        }
        // 文書の信頼度の判定理由
        public string DocumentReliabilityReason {
            get {
                return ContentItemInstance.DocumentReliabilityReason;
            }
            set {
                ContentItemInstance.DocumentReliabilityReason = value;
            }
        }

        // ReferenceVectorDBItemsがフォルダのReferenceVectorDBItemsと同期済みかどうか
        public bool IsReferenceVectorDBItemsSynced {
            get {
                return ContentItemInstance.IsReferenceVectorDBItemsSynced;
            }
            set {
                ContentItemInstance.IsReferenceVectorDBItemsSynced = value;
            }
        }
        // SourceType
        public string SourceType {
            get {
                return ContentItemInstance.SourceType;
            }
            set {
                ContentItemInstance.SourceType = value;
            }
        }

        // SourcePath
        public string SourcePath {
            get {
                return ContentItemInstance.SourcePath;
            }
            set {
                ContentItemInstance.SourcePath = value;
            }
        }
        // LiteDBに保存するためのBase64文字列. 元ファイルまたは画像データをBase64エンコードした文字列
        public string Base64Image {
            get {
                if (ContentItemInstance.ContentType == ContentTypes.ContentItemTypes.Files) {
                    (bool isImage, ContentTypes.ImageType imageType) = ContentTypes.IsImageFile(SourcePath);
                    if (isImage) {
                        byte[] imageBytes = System.IO.File.ReadAllBytes(SourcePath);
                        return Convert.ToBase64String(imageBytes);
                    }
                }
                if (string.IsNullOrEmpty(ContentItemInstance.CachedBase64String) == false) {
                    return ContentItemInstance.CachedBase64String;
                }
                return "";
            }
            set {
                ContentItemInstance.CachedBase64String = value;
            }
        }
        // ファイルパス

        // ファイルの最終更新日時
        public long LastModified {
            get {
                return ContentItemInstance.LastModified;
            }
            set {
                ContentItemInstance.LastModified = value;
            }
        }


        // タグ表示用の文字列
        public string TagsString() {
            return string.Join(",", ContentItemInstance.Tags);
        }


        // 別フォルダに移動
        public virtual void MoveToFolder(ContentFolderWrapper folder){
            ContentItemInstance.CollectionId = folder.Id;
            Save();
        }

        // 別フォルダにコピー
        public virtual void CopyToFolder(ContentFolderWrapper folder) {
            ContentItem newItem = ContentItemInstance.Copy();
            newItem.CollectionId = folder.Id;
            newItem.Save();
        }

        public virtual ContentItemWrapper Copy() {
            return new ContentItemWrapper(ContentItemInstance.Copy());
        }


        // 背景情報
        [BsonIgnore]
        public string BackgroundInfo {
            get {
                return ContentItemInstance.PromptChatResult.GetTextContent(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
            }
            set {
                ContentItemInstance.PromptChatResult.SetTextContent(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(), value);
            }
        }

        // サマリー
        [BsonIgnore]
        public string Summary {
            get {
                return ContentItemInstance.PromptChatResult.GetTextContent(SystemDefinedPromptNames.SummaryGeneration.ToString());
            }
            set {
                ContentItemInstance.PromptChatResult.SetTextContent(SystemDefinedPromptNames.SummaryGeneration.ToString(), value);
            }
        }
        // 文章の信頼度
        [BsonIgnore]
        public string InformationReliability {
            get {
                return ContentItemInstance.PromptChatResult.GetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString());
            }
            set {
                ContentItemInstance.PromptChatResult.SetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString(), value);
            }
        }
        [BsonIgnore]
        public string HeaderText {
            get {
                string Description = ContentItemInstance.Description;
                string SourceApplicationName = ContentItemInstance.SourceApplicationName;
                string SourceApplicationTitle = ContentItemInstance.SourceApplicationTitle;
                string SourcePath = ContentItemInstance.SourcePath;
                ContentTypes.ContentItemTypes ContentType = ContentItemInstance.ContentType;
                int DocumentReliability = ContentItemInstance.DocumentReliability;
                LiteDB.ObjectId CollectionId = ContentItemInstance.CollectionId;

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
                foreach (var item in ContentItemInstance.ChatItems) {
                    chatHistoryText += $"--- {item.Role} ---\n";
                    chatHistoryText += item.ContentWithSources + "\n\n";
                }
                return chatHistoryText;
            }
        }


        [BsonIgnore]
        public string UpdatedAtString {
            get {
                return ContentItemInstance.UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        [BsonIgnore]
        public string CreatedAtString {
            get {
                return ContentItemInstance.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        [BsonIgnore]
        // ベクトル化日時の文字列
        public string VectorizedAtString {
            get {
                if (ContentItemInstance.VectorizedAt <= ContentItem.InitialDateTime) {
                    return "";
                }
                return ContentItemInstance.VectorizedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        [BsonIgnore]
        public string ContentTypeString {
            get {
                ContentTypes.ContentItemTypes ContentType = ContentItemInstance.ContentType;
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
                string FilePath = ContentItemInstance.SourcePath;
                return Path.GetDirectoryName(FilePath) ?? "";
            }
        }
        // ファイル名
        [BsonIgnore]
        public string FileName {
            get {
                string FilePath = ContentItemInstance.SourcePath;
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
        public virtual ContentFolderWrapper GetFolder() {
            return new ContentFolderWrapper(ContentItemInstance.GetFolder<ContentFolder>());
        }


        public bool IsImage() {

            if (string.IsNullOrEmpty(Base64Image)) {
                return false;
            }
            if ( ContentTypes.GetImageTypeFromBase64(Base64Image).Item1) {
                return true;
            } else {
                Base64Image = "";
            }
            if (ContentItemInstance.ContentType == ContentTypes.ContentItemTypes.Files) {
               ( bool isImage, ContentTypes.ImageType imageType) = ContentTypes.IsImageFile(SourcePath);
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
            ContentItemInstance.Delete();
        }

        public static void DeleteItems(List<ContentItemWrapper> items) {
            ContentItemCommands.DeleteEmbeddings(items);
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetItemCollection<ContentItem>();
            foreach (ContentItemWrapper contentItem in items) {
                collection.Delete(contentItem.Id);
            }
        }

        public virtual void Save(bool updateLastModifiedTime = true, bool applyAutoProcess = false) {
            ContentItemInstance.Save(updateLastModifiedTime, applyAutoProcess);
        }


    }
}
