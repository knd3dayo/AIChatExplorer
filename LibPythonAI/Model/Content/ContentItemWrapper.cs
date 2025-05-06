using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows.Media.Imaging;
using LibPythonAI.Data;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using PythonAILib.Resources;

namespace LibPythonAI.Model.Content {
    public class ContentItemWrapper {
        public ContentItemWrapper() {}

        public ContentItemWrapper(ContentFolderEntity folder) {
            FolderId = folder.Id;
        }

        public ContentItemEntity Entity { get; protected set; }　= new ContentItemEntity();

        // ID
        public string Id { get => Entity.Id; }

        // FolderId
        public string? FolderId {
            get {
                return Entity.FolderId;
            }
            set {
                Entity.FolderId = value;
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

        public bool ContentModified { get; set; } = false;
        // クリップボードの内容
        public string Content {
            get {
                return Entity.Content;
            }
            set {
                Entity.Content = value;
                ContentModified = true;
            }
        }
        public bool DescriptionModified { get; set; } = false;
        //説明
        public string Description {
            get {
                return Entity.Description;
            }
            set {
                Entity.Description = value;
                DescriptionModified = true;
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
        }

        // プロンプトテンプレートに基づくチャットの結果
        public PromptChatResult PromptChatResult {
            get {
                return Entity.PromptChatResult;
            }
        }

        //Tags
        public HashSet<string> Tags {
            get {
                return Entity.Tags;
            }
            set {
                Entity.Tags = value;
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

        // LiteDBに保存するためのBase64文字列. 元ファイルまたは画像データをBase64エンコードした文字列
        public string Base64Image {
            get {
                if (Entity.ContentType == ContentTypes.ContentItemTypes.Files) {
                    // IOExceptionが発生する可能性があるため、try-catchで囲む
                    try {
                        (bool isImage, ContentTypes.ImageType imageType) = ContentTypes.IsImageFile(SourcePath);
                        if (isImage) {
                            byte[] imageBytes = File.ReadAllBytes(SourcePath);
                            return Convert.ToBase64String(imageBytes);
                        }
                    } catch (IOException e) {
                        LogWrapper.Info($"access error: {e.Message}");
                        return "";
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

        public string UpdatedAtString {
            get {
                return Entity.UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }

        public string CreatedAtString {
            get {
                return Entity.CreatedAt.ToString("yyyy/MM/dd HH:mm:ss");
            }
        }


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


        public string DisplayName {
            get {
                if (string.IsNullOrEmpty(FileName)) {
                    return "No Name";
                }
                return FileName;
            }
        }


        // フォルダ名
        public string FolderName  => Path.GetDirectoryName(SourcePath) ?? "";

        // ファイル名
        public string FileName => Path.GetFileName(SourcePath) ?? "";

        // フォルダ名 + \n + ファイル名
        public string FolderAndFileName => FolderName + Path.PathSeparator + FileName;

        // 画像イメージ
        public BitmapImage? BitmapImage {
            get {
                if (!IsImage()) {
                    return null;
                }
                byte[] imageBytes = Convert.FromBase64String(Base64Image);
                return ContentTypes.GetBitmapImage(imageBytes);
            }
        }
        public System.Drawing.Image? Image {
            get {
                if (!IsImage()) {
                    return null;
                }
                return ContentTypes.GetImageFromBase64(Base64Image);
            }
        }


        // ベクトル化日時の文字列
        public string VectorizedAtString {
            get {
                Entity.ExtendedProperties.TryGetValue("VectorizedAtString", out object? value);
                if (value is string strValue) {
                    return strValue;
                }
                return string.Empty;
            }
            set {
                Entity.ExtendedProperties["VectorizedAtString"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }
        // フォルダに設定されたVerctorDBPropertyを使うかどうか
        public bool UseFolderVectorDBProperty {
            get {
                Entity.ExtendedProperties.TryGetValue("UseFolderVectorDBProperty", out object? value);
                if (value is bool boolValue) {
                    return boolValue;
                }
                return true;
            }
            set {
                Entity.ExtendedProperties["UseFolderVectorDBProperty"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }
        // このアイテムに紐付けらされたVectorDBProperty
        // UseFolderVectorDBPropertyがtrueの場合は、フォルダに設定されたVectorDBPropertyを使用する
        public List<VectorDBProperty> VectorDBProperties {
            get {
                if (UseFolderVectorDBProperty) {
                    return GetFolder().GetVectorSearchProperties();
                }

                Entity.ExtendedProperties.TryGetValue("VectorDBProperties", out object? value);
                if (value is string strValue) {
                    return VectorDBProperty.FromListJson(strValue) ?? [];
                }
                return [];
            }
            set {
                Entity.ExtendedProperties["VectorDBProperties"] = VectorDBProperty.ToListJson(value);
                Entity.SaveExtendedPropertiesJson();
            }
        }

        //　貼り付け元のアプリケーション名
        public string? SourceApplicationName {
            get {
                Entity.ExtendedProperties.TryGetValue("SourceApplicationName", out object? value);
                if (value is string strValue) {
                    return strValue;
                }
                return string.Empty;
            }
            set {
                Entity.ExtendedProperties["SourceApplicationName"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }
        //　貼り付け元のアプリケーションのタイトル
        public string SourceApplicationTitle {
            get {
                Entity.ExtendedProperties.TryGetValue("SourceApplicationTitle", out object? value);
                if (value is string strValue) {
                    return strValue;
                }
                return string.Empty;
            }
            set {
                Entity.ExtendedProperties["SourceApplicationTitle"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }
        //　貼り付け元のアプリケーションのID
        public int SourceApplicationID {
            get {
                Entity.ExtendedProperties.TryGetValue("SourceApplicationID", out object? value);
                if (value is Decimal intValue) {
                    return Decimal.ToInt32(intValue);
                }
                if (value is int intValue2) {
                    return intValue2;
                }
                return 0;
            }
            set {
                Entity.ExtendedProperties["SourceApplicationID"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }
        //　貼り付け元のアプリケーションのパス
        public string SourceApplicationPath {
            get {
                Entity.ExtendedProperties.TryGetValue("SourceApplicationPath", out object? value);
                if (value is string strValue) {
                    return strValue;
                }
                return string.Empty;
            }
            set {
                Entity.ExtendedProperties["SourceApplicationPath"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }

        // 文書の信頼度(0-100)
        public int DocumentReliability {
            get {
                Entity.ExtendedProperties.TryGetValue("DocumentReliability", out object? value);
                if (value is Decimal intValue) {
                    return Decimal.ToInt32(intValue);
                }
                if (value is int intValue2) {
                    return intValue2;
                }
                return 0;
            }
            set {
                Entity.ExtendedProperties["DocumentReliability"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }
        // 文書の信頼度の判定理由
        public string DocumentReliabilityReason {
            get {
                Entity.ExtendedProperties.TryGetValue("DocumentReliabilityReason", out object? value);
                if (value is string strValue) {
                    return strValue;
                }
                return string.Empty;
            }
            set {
                Entity.ExtendedProperties["DocumentReliabilityReason"] = value;
                Entity.SaveExtendedPropertiesJson();
            }
        }

        // SourcePath
        public string SourcePath {
            get {
                Entity.ExtendedProperties.TryGetValue("SourcePath", out object? value);
                if (value is string strValue) {
                    return strValue;
                }
                return string.Empty;
            }
            set {
                Entity.ExtendedProperties["SourcePath"] = value;
                Entity.SaveExtendedPropertiesJson();
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

        public long LastModified {
            get {
                Entity.ExtendedProperties.TryGetValue("LastModified", out object? value);
                return value != null ? (long)value : 0;
            }
            set {
                Entity.ExtendedProperties["LastModified"] = value;
            }
        }


        // Folder
        public virtual ContentFolderWrapper GetFolder() {
            var folder = ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(Entity.FolderId);
            if (folder == null) {
                throw new Exception("Folder not found");
            }
            return folder;

        }

        // タグ表示用の文字列
        public string TagsString() {
            return string.Join(",", Entity.Tags);
        }


        // 別フォルダに移動
        public virtual void MoveTo(ContentFolderWrapper folder) {
            Entity.FolderId = folder.Id;
            Save();
        }

        // 別フォルダにコピー
        public virtual void CopyToFolder(ContentFolderWrapper folder) {
            ContentItemWrapper newItem = Copy();
            newItem.Entity.FolderId = folder.Id;
            newItem.Save();
        }

        public virtual ContentItemWrapper Copy() {
            return new ContentItemWrapper() { Entity = Entity.Copy() };
        }


        // 背景情報
        public string BackgroundInfo {
            get {
                return Entity.PromptChatResult.GetTextContent(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
            }
            set {
                Entity.PromptChatResult.SetTextContent(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(), value);
            }
        }

        // サマリー
        public string Summary {
            get {
                return Entity.PromptChatResult.GetTextContent(SystemDefinedPromptNames.SummaryGeneration.ToString());
            }
            set {
                Entity.PromptChatResult.SetTextContent(SystemDefinedPromptNames.SummaryGeneration.ToString(), value);
            }
        }
        // 文章の信頼度
        public string InformationReliability {
            get {
                return Entity.PromptChatResult.GetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString());
            }
            set {
                Entity.PromptChatResult.SetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString(), value);
            }
        }
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
                var Folder = GetFolder();
                if (Folder != null && !string.IsNullOrEmpty(Folder.Description)) {
                    header1 += $"[{PythonAILibStringResources.Instance.DocumentCategorySummary}]" + Folder.Description + "\n";
                }

                // Tags
                header1 += $"[{PythonAILibStringResources.Instance.Tag}]" + TagsString() + "\n";
                return header1;
            }
        }

        // Load
        public virtual void Load(Action beforeAction, Action afterAction) { }

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


        public virtual VectorDBProperty GetMainVectorSearchProperty() {
            return GetFolder().GetMainVectorSearchProperty();
        }


        public virtual void Save() {
            if (ContentModified || DescriptionModified) {
                // 更新日時を設定
                UpdatedAt = DateTime.Now;
                // ベクトルを更新
                Task.Run(() => {
                    VectorDBProperty.UpdateEmbeddings(GetFolder().GetMainVectorSearchProperty());
                });
            }

            ContentItemEntity.SaveItems([Entity]);
            ContentModified = false;
            DescriptionModified = false;

        }

        public void Delete() {
            ContentItemCommands.DeleteEmbeddings([this]);
            ContentItemEntity.DeleteItems([Entity]);
        }

        // FolderIdが一致するContentItemWrapperを取得
        public static List<T> GetItems<T>(ContentFolderWrapper folder) where T: ContentItemWrapper {
            using PythonAILibDBContext db = new();
            var items = db.ContentItems.Where(x => x.FolderId == folder.Id);
            List<T> result = [];
            foreach (var item in items) {
                if (Activator.CreateInstance(typeof(T)) is T newItem) {
                    newItem.Entity = item;
                    result.Add(newItem);
                }
            }
            return result;
        }


        public static void DeleteItems(List<ContentItemWrapper> items) {
            ContentItemCommands.DeleteEmbeddings(items);
            ContentItemEntity.DeleteItems(items.Select(item => item.Entity).ToList());
        }

        public static void SaveItems(List<ContentItemWrapper> items, bool updateLastModifiedTime = true, bool applyAutoProcess = false) {
            if (updateLastModifiedTime) {
                // 更新日時を設定
                items.ForEach(item => item.UpdatedAt = DateTime.Now);
            }
            if (applyAutoProcess) {
                // 自動処理を適用
            }
            ContentItemEntity.SaveItems(items.Select(item => item.Entity).ToList());
        }

        // ClipboardItemをJSON文字列に変換する
        public static string ToJson(ContentItemWrapper item) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            return JsonSerializer.Serialize(item, options);
        }


        // JSON文字列をClipboardItemに変換する
        public static ContentItemWrapper? FromJson(string json) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            ContentItemWrapper? item = JsonSerializer.Deserialize<ContentItemWrapper>(json, options);
            return item;

        }


        public static List<ContentItemWrapper> SearchAll(SearchCondition searchCondition) {
            if (searchCondition.IsEmpty()) {
                return [];
            }
            using PythonAILibDBContext db = new();
            var items = db.ContentItems.Select(x => new ContentItemWrapper() { Entity = x });
            return Search(searchCondition, items).ToList();
        }

        public static List<ContentItemWrapper> Search(SearchCondition searchCondition, ContentFolderWrapper targetFolder, bool isIncludeSubFolders) {
            if (searchCondition.IsEmpty()) {
                return [];
            }

            List<string> folderIds = [];
            folderIds.Add(targetFolder.Entity.Id);
            if (isIncludeSubFolders) {
                folderIds.AddRange(targetFolder.Entity.GetChildrenAll().Select(x => x.Id));
            }
            using PythonAILibDBContext db = new();
            var items = db.ContentItems.Where(x => x.FolderId != null && folderIds.Contains(x.FolderId)).Select(x => new ContentItemWrapper() { Entity = x });

            return Search(searchCondition, items).ToList();


        }

        private static IEnumerable<ContentItemWrapper> Search(SearchCondition searchCondition, IEnumerable<ContentItemWrapper> items) {
            IEnumerable<ContentItemWrapper> results = items;

            // SearchConditionの内容に従ってフィルタリング
            if (string.IsNullOrEmpty(searchCondition.Description) == false) {
                if (searchCondition.ExcludeDescription) {
                    results = results.Where(x => x.Description != null && x.Description.Contains(searchCondition.Description) == false);
                } else {
                    results = results.Where(x => x.Description != null && x.Description.Contains(searchCondition.Description));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Content) == false) {
                if (searchCondition.ExcludeContent) {
                    results = results.Where(x => x.Content != null && x.Content.Contains(searchCondition.Content) == false);
                } else {
                    results = results.Where(x => x.Content != null && x.Content.Contains(searchCondition.Content));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.Tags) == false) {
                if (searchCondition.ExcludeTags) {
                    results = results.Where(x => x.Tags != null && x.Tags.Contains(searchCondition.Tags) == false);
                } else {
                    results = results.Where(x => x.Tags != null && x.Tags.Contains(searchCondition.Tags));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationName) == false) {
                if (searchCondition.ExcludeSourceApplicationName) {
                    results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(searchCondition.SourceApplicationName) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationName != null && x.SourceApplicationName.Contains(searchCondition.SourceApplicationName));
                }
            }
            if (string.IsNullOrEmpty(searchCondition.SourceApplicationTitle) == false) {
                if (searchCondition.ExcludeSourceApplicationTitle) {
                    results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle) == false);
                } else {
                    results = results.Where(x => x.SourceApplicationTitle != null && x.SourceApplicationTitle.Contains(searchCondition.SourceApplicationTitle));
                }
            }
            if (searchCondition.EnableStartTime) {
                results = results.Where(x => x.CreatedAt > searchCondition.StartTime);
            }
            if (searchCondition.EnableEndTime) {
                results = results.Where(x => x.CreatedAt < searchCondition.EndTime);
            }
            results = results.OrderByDescending(x => x.UpdatedAt);

            return results;
        }


    }
}
