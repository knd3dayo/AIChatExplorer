using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;
using LibPythonAI.Data;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Model.Content {
    public class ContentItem {

        public static readonly string TEMPORARY_ITEM_ID = "TemporaryItemId";

        public ContentItem() {
        }

        public ContentItem(ContentFolderEntity folder) {
            FolderId = folder.Id;
        }


        public ContentItemEntity Entity { get; set; } = new ContentItemEntity();

        // ID
        public string Id { get => Entity.Id; set { Entity.Id = value; } }

        // FolderId
        public string? FolderId { get => Entity.FolderId; set { Entity.FolderId = value; } }

        // 生成日時
        public DateTime CreatedAt { get => Entity.CreatedAt; set { Entity.CreatedAt = value; } }
        // 更新日時
        public DateTime UpdatedAt { get => Entity.UpdatedAt; set { Entity.UpdatedAt = value; } }
        // ベクトル化日時
        public DateTime VectorizedAt { get => Entity.VectorizedAt; set { Entity.VectorizedAt = value; } }

        public bool ContentModified { get; set; } = false;
        // クリップボードの内容
        public string Content {
            get => Entity.Content;
            set {
                var oldValue = Entity.Content;
                Entity.Content = value;
                // 内容が変更された場合のみContentModifiedをtrueにする
                if (oldValue != value) {
                    ContentModified = true;
                } else {
                    ContentModified = false;
                }
            }
        }
        public bool DescriptionModified { get; set; } = false;

        //説明
        public string Description {
            get => Entity.Description;
            set {
                var oldValue = Entity.Description;
                Entity.Description = value;
                // 内容が変更された場合のみDescriptionModifiedをtrueにする
                if (oldValue != value) {
                    DescriptionModified = true;
                } else {
                    DescriptionModified = false;
                }
            }
        }

        // クリップボードの内容の種類
        public ContentItemTypes.ContentItemTypeEnum ContentType { get => Entity.ContentType; set { Entity.ContentType = value; } }


        // OpenAIチャットのChatItemコレクション
        // LiteDBの同一コレクションで保存されているオブジェクト。ApplicationItemオブジェクト生成時にロード、Save時に保存される。
        public List<ChatMessage> ChatItems { get => Entity.ChatItems; }

        // プロンプトテンプレートに基づくチャットの結果
        public PromptChatResult PromptChatResult { get => Entity.PromptChatResult; }

        //Tags
        public HashSet<string> Tags { get => Entity.Tags; set { Entity.Tags = value; } }

        // ピン留め
        public bool IsPinned {
            get => Entity.IsPinned;
            set {
                var oldValue = Entity.IsPinned;
                Entity.IsPinned = value;
                // ピン留め状態が変更された場合はSaveを呼び出す
                if (oldValue != value) {
                    Task.Run(async () => await SaveAsync());
                }
            }
        }

        // ChatMessagesJson
        public string ChatMessagesJson {
            get => Entity.ChatMessagesJson;
            set {
                Entity.ChatMessagesJson = value;
            }
        }
        // PromptChatResultJson
        public string PromptChatResultJson {
            get => Entity.PromptChatResultJson;
            set {
                Entity.PromptChatResultJson = value;
            }
        }
        // ExtendedPropertiesJson
        public string ExtendedPropertiesJson {
            get {
                return Entity.ExtendedPropertiesJson;
            }
            set {
                Entity.ExtendedPropertiesJson = value;
            }
        }
        public string CachedBase64String {
            get => Entity.CachedBase64String;
            set {
                Entity.CachedBase64String = value;
            }
        }

        // LiteDBに保存するためのBase64文字列. 元ファイルまたは画像データをBase64エンコードした文字列
        public string Base64Image {
            get {
                if (Entity.ContentType == ContentItemTypes.ContentItemTypeEnum.Files) {
                    // IOExceptionが発生する可能性があるため、try-catchで囲む
                    try {
                        (bool isImage, ContentItemTypes.ImageType imageType) = ContentItemTypes.IsImageFile(SourcePath);
                        if (isImage) {
                            byte[] imageBytes = System.IO.File.ReadAllBytes(SourcePath);
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
                ContentItemTypes.ContentItemTypeEnum ContentType = Entity.ContentType;
                if (ContentType == ContentItemTypes.ContentItemTypeEnum.Text) {
                    return "Text";
                } else if (ContentType == ContentItemTypes.ContentItemTypeEnum.Files) {
                    return "File";
                } else if (ContentType == ContentItemTypes.ContentItemTypeEnum.Image) {
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
        public string FolderName => Path.GetDirectoryName(SourcePath) ?? "";

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
                return ContentItemTypes.GetBitmapImage(imageBytes);
            }
        }
        public System.Drawing.Image? Image {
            get {
                if (!IsImage()) {
                    return null;
                }
                return ContentItemTypes.GetImageFromBase64(Base64Image);
            }
        }

        private void SetExtendedProperty<T>(string key, T value) {
            Entity.ExtendedProperties[key] = value;
        }
        private T? GetExtendedProperty<T>(string key) {
            if (Entity.ExtendedProperties.TryGetValue(key, out object? value)) {
                if (value is T typedValue) {
                    return typedValue;
                }
            }
            return default;
        }

        // ベクトル化日時の文字列
        public string VectorizedAtString {
            get => GetExtendedProperty<string>("VectorizedAtString") ?? string.Empty;
            set => SetExtendedProperty("VectorizedAtString", value);
        }
        // フォルダに設定されたVerctorDBPropertyを使うかどうか
        public bool UseFolderVectorSearchItem {
            get => GetExtendedProperty<bool>("UseFolderVectorSearchItem");
            set => SetExtendedProperty("UseFolderVectorSearchItem", value);
        }

        private ObservableCollection<VectorSearchItem>? _vectorDBProperties;
        public async Task<ObservableCollection<VectorSearchItem>> GetVectorDBPropertiesAsync() {
            if (_vectorDBProperties != null) {
                return _vectorDBProperties;
            }
            if (UseFolderVectorSearchItem) {
                var folder = await GetFolderAsync();
                var items = await folder.GetVectorSearchProperties();
                _vectorDBProperties = [.. items];
            }
            var value = Entity.ExtendedProperties.GetValueOrDefault("VectorDBProperties", null);
            if (value is string strValue) {
                _vectorDBProperties = [.. VectorSearchItem.FromListJson(strValue)];
            } else if (value is List<VectorSearchItem> list) {
                _vectorDBProperties = [.. list];
            } else {
                _vectorDBProperties = [];
            }

            // Addイベント発生時の処理
            _vectorDBProperties.CollectionChanged += (sender, e) => {
                if (e.NewItems != null) {
                    // Entityを更新
                    Entity.ExtendedProperties["VectorDBProperties"] = VectorSearchItem.ToListJson(_vectorDBProperties);
                }
            };
            // Removeイベント発生時の処理
            _vectorDBProperties.CollectionChanged += (sender, e) => {
                if (e.OldItems != null) {
                    // Entityを更新
                    Entity.ExtendedProperties["VectorDBProperties"] = VectorSearchItem.ToListJson(_vectorDBProperties);
                }
            };
            // Clearイベント発生時の処理
            _vectorDBProperties.CollectionChanged += (sender, e) => {
                // Entityを更新
                Entity.ExtendedProperties["VectorDBProperties"] = VectorSearchItem.ToListJson(_vectorDBProperties);
            };
            return _vectorDBProperties;
        }

        //　貼り付け元のアプリケーション名
        public string? SourceApplicationName {
            get => GetExtendedProperty<string>("SourceApplicationName") ?? string.Empty;
            set => SetExtendedProperty("SourceApplicationName", value ?? string.Empty);
        }
        //　貼り付け元のアプリケーションのタイトル
        public string SourceApplicationTitle {
            get => GetExtendedProperty<string>("SourceApplicationTitle") ?? string.Empty;
            set => SetExtendedProperty("SourceApplicationTitle", value ?? string.Empty);
        }
        //　貼り付け元のアプリケーションのID
        public int SourceApplicationID {
            get {
                var value = GetExtendedProperty<object>("SourceApplicationID");
                if (value is Decimal intValue) {
                    return Decimal.ToInt32(intValue);
                }
                if (value is int intValue2) {
                    return intValue2;
                }
                return 0;
            }
            set => SetExtendedProperty("SourceApplicationID", value);
        }
        //　貼り付け元のアプリケーションのパス
        public string SourceApplicationPath {
            get {
                var value = GetExtendedProperty<object>("SourceApplicationPath");
                if (value is string strValue) {
                    return strValue;
                }
                return string.Empty;
            }
            set => SetExtendedProperty("SourceApplicationPath", value);
        }

        // 文書の信頼度(0-100)
        public int DocumentReliability {
            get {
                var value = GetExtendedProperty<object>("DocumentReliability");
                if (value is Decimal intValue) {
                    return Decimal.ToInt32(intValue);
                }
                if (value is int intValue2) {
                    return intValue2;
                }
                return 0;
            }
            set {
                if (value < 0 || value > 100) {
                    throw new ArgumentOutOfRangeException(nameof(value), "DocumentReliability must be between 0 and 100.");
                }
                SetExtendedProperty("DocumentReliability", value);
            }
        }
        // 文書の信頼度の判定理由
        public string DocumentReliabilityReason {
            get => GetExtendedProperty<string>("DocumentReliabilityReason") ?? string.Empty;
            set => SetExtendedProperty("DocumentReliabilityReason", value ?? string.Empty);
        }

        // Path
        public string SourcePath {
            get => GetExtendedProperty<string>("SourcePath") ?? string.Empty;
            set => SetExtendedProperty("SourcePath", value ?? string.Empty);
        }

        // SourceType 
        public string SourceType {
            get => GetExtendedProperty<string>("SourceType") ?? ContentSourceType.Application.ToString();
            set => SetExtendedProperty("SourceType", value ?? ContentSourceType.Application);
        }
        // ファイルの最終更新日時

        public long LastModified {
            get {
                var value = GetExtendedProperty<object>("LastModified");
                if (value is long longValue) {
                    return longValue;
                }
                if (value is decimal decimalValue) {
                    return Convert.ToInt64(decimalValue);
                }
                return 0L;
            }
            set => SetExtendedProperty("LastModified", value);
        }

        // ChatSettings
        public ChatSettings ChatSettings {
            get => GetExtendedProperty<ChatSettings>("ChatSettings") ?? new ChatSettings();
            set => SetExtendedProperty("ChatSettings", value);
        }


        private void SaveChatSettingsToExtendedProperties() {
            if (ChatSettings == null) {
                Entity.ExtendedProperties.Remove("ChatSettingsJson");
            } else {
                Entity.ExtendedProperties["ChatSettingsJson"] = ChatSettings.ToJson();
            }
        }

        // Folder
        // public ContentFolderWrapper? Folder { get; protected set; }
        public  async Task<ContentFolderWrapper> GetFolderAsync() {
            var folder = await ContentFolderWrapper.GetFolderById<ContentFolderWrapper>(Entity.FolderId);
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
        public virtual async Task MoveToAsync(ContentFolderWrapper folder) {
            Entity.FolderId = folder.Id;
            await SaveAsync();
        }

        // 別フォルダにコピー
        public virtual async Task CopyToFolderAsync(ContentFolderWrapper folder) {
            ContentItem newItem = Copy();
            newItem.Entity.FolderId = folder.Id;
            await newItem.SaveAsync();
        }

        public virtual ContentItem Copy() {
            return new ContentItem() {
                Entity = Entity.Copy(),
                ChatSettings = ChatSettings
            };
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
        public async Task<string> GetHeaderTextAsync() {
            string header1 = "";
            header1 += $"[{PythonAILibStringResourcesJa.Instance.Title}]" + Description + "\n";
            header1 += $"[{PythonAILibStringResourcesJa.Instance.CreationDateTime}]" + CreatedAtString + "\n";
            header1 += $"[{PythonAILibStringResourcesJa.Instance.UpdateDate}]" + UpdatedAtString + "\n";
            header1 += $"[{PythonAILibStringResourcesJa.Instance.VectorizedDate}]" + VectorizedAtString + "\n";
            header1 += $"[{PythonAILibStringResourcesJa.Instance.SourceAppName}]" + SourceApplicationName + "\n";
            header1 += $"[{PythonAILibStringResourcesJa.Instance.SourceTitle}]" + SourceApplicationTitle + "\n";
            header1 += $"[{PythonAILibStringResourcesJa.Instance.SourcePath}]" + SourcePath + "\n";

            if (ContentType == ContentItemTypes.ContentItemTypeEnum.Text)
                header1 += $"[{PythonAILibStringResourcesJa.Instance.Type}]Text";
            else if (ContentType == ContentItemTypes.ContentItemTypeEnum.Files)
                header1 += $"[{PythonAILibStringResourcesJa.Instance.Type}]File";
            else if (ContentType == ContentItemTypes.ContentItemTypeEnum.Image)
                header1 += $"[{PythonAILibStringResourcesJa.Instance.Type}]Image";
            else
                header1 += $"[{PythonAILibStringResourcesJa.Instance.Type}]Unknown";

            header1 += $"\n[{PythonAILibStringResourcesJa.Instance.DocumentReliability}]" + DocumentReliability + "%\n";
            var folder = await GetFolderAsync();
            if (folder != null && !string.IsNullOrEmpty(folder.Description))
                header1 += $"[{PythonAILibStringResourcesJa.Instance.DocumentCategorySummary}]" + folder.Description + "\n";

            header1 += $"[{PythonAILibStringResourcesJa.Instance.Tag}]" + TagsString() + "\n";
            return header1;
        }

        public bool IsImage() {

            if (string.IsNullOrEmpty(Base64Image)) {
                return false;
            }
            if (ContentItemTypes.GetImageTypeFromBase64(Base64Image).Item1) {
                return true;
            } else {
                Base64Image = "";
            }
            if (Entity.ContentType == ContentItemTypes.ContentItemTypeEnum.Files) {
                (bool isImage, ContentItemTypes.ImageType imageType) = ContentItemTypes.IsImageFile(SourcePath);
                return isImage;
            }
            return false;
        }


        public virtual async Task<VectorSearchItem> GetMainVectorSearchItemAsync() {
            var folder = await GetFolderAsync();
            return await folder.GetMainVectorSearchItem();
        }

        public async Task SaveAsync() {
            // IdがTEMPORARY_ITEM_IDの場合は保存しない
            if (Id == TEMPORARY_ITEM_ID) {
                return;
            }
            if (ContentModified || DescriptionModified) {
                // 更新日時を設定
                UpdatedAt = DateTime.Now;
                // ベクトルを更新
                await UpdateEmbeddingAsync();
            }

            // ChatSettingsをExtendedPropertiesに保存
            SaveChatSettingsToExtendedProperties();
            ContentItemRequest request = new(Entity);
            await PythonExecutor.PythonAIFunctions.UpdateContentItemAsync([request]);
            ContentModified = false;
            DescriptionModified = false;

        }
        public static async Task SaveItemsAsync(List<ContentItem> items) {
            if (items.Count == 0) {
                return;
            }
            // ContentItemRequestのリストを作成
            List<ContentItemRequest> requests = items.Select(item => new ContentItemRequest(item.Entity)).ToList();
            // APIを使用して更新
            await PythonExecutor.PythonAIFunctions.UpdateContentItemAsync(requests);
        }

        public virtual async Task UpdateEmbeddingAsync() {
            // ベクトルを更新
            var item = await GetMainVectorSearchItemAsync();
            string? vectorDBItemName = item.VectorDBItemName;
            if (string.IsNullOrEmpty(vectorDBItemName)) {
                LogWrapper.Error(PythonAILibStringResourcesJa.Instance.NoVectorDBSet);
                return;
            }
            var folder = await GetFolderAsync();
            var contentFolderPath = await folder.GetContentFolderPath();
            VectorEmbeddingItem vectorDBEntry = new(Id.ToString(), contentFolderPath);
            await vectorDBEntry.SetMetadata(this);
            await VectorEmbeddingItem.UpdateEmbeddingsAsync(vectorDBItemName, vectorDBEntry);
        }


        public async Task DeleteAsync() {
            await ContentItemCommands.DeleteEmbeddingsAsync([this]);
            await PythonExecutor.PythonAIFunctions.DeleteContentItemsAsync([new ContentItemRequest(Entity)]);
        }

        public static async Task DeleteItemsAsync(List<ContentItem> items) {
            if (items.Count == 0) {
                return;
            }
            // ContentItemRequestのリストを作成
            List<ContentItemRequest> requests = items.Select(item => new ContentItemRequest(item.Entity)).ToList();
            // APIを使用して削除
            await PythonExecutor.PythonAIFunctions.DeleteContentItemsAsync(requests);
        }

        public static async Task<ContentItemEntity?> LoadEntityAsync(string id) {
            // APIを使用してアイテムを取得
            ContentItemEntity? item = await PythonExecutor.PythonAIFunctions.GetContentItemByIdAsync(id);
            return item;
        }
        public static async Task<List<ContentItemEntity>> LoadEntitiesAsync(ContentFolderWrapper folder) {
            // APIを使用してフォルダ内のアイテムを取得
            return await PythonExecutor.PythonAIFunctions.GetContentItemsByFolderAsync(folder.Id);
        }

        public static async Task<List<T>> GetItemsAsync<T>(ContentFolderWrapper folder) where T : ContentItem {
            List<ContentItemEntity> items = await LoadEntitiesAsync(folder);
            List<T> result = [];
            foreach (var item in items) {
                if (Activator.CreateInstance(typeof(T)) is T newItem) {
                    newItem.Entity = item;
                    result.Add(newItem);
                }
            }
            return result;
        }

        public static async Task<T?> GetItemAsync<T>(string id) where T : ContentItem, new() {
            ContentItemEntity? item = await LoadEntityAsync(id);
            if (item == null) {
                return null;
            }
            T result = new() {
                Entity = item
            };
            return result;
        }


        // ApplicationItemをJSON文字列に変換する
        public static string ToJson(ContentItem item) {
            return JsonSerializer.Serialize(item, JsonUtil.JsonSerializerOptions);
        }


        // JSON文字列をApplicationItemに変換する
        public static ContentItem? FromJson(string json) {
            ContentItem? item = JsonSerializer.Deserialize<ContentItem>(json, JsonUtil.JsonSerializerOptions);
            return item;

        }


        public static async Task<List<ContentItem>> SearchAll(SearchCondition searchCondition) {
            if (searchCondition.IsEmpty()) {
                return [];
            }
            // APIを使用して検索を実行
            List<ContentItemEntity> items = await PythonExecutor.PythonAIFunctions.SearchContentItems(searchCondition);

            List<ContentItem> result = [];
            foreach (var item in items) {
                ContentItem wrapper = new() {
                    Entity = item,
                };
                result.Add(wrapper);
            }
            return result;
        }

        public static async Task<List<ContentItem>> Search(SearchCondition searchCondition, ContentFolderWrapper targetFolder, bool isIncludeSubFolders) {
            if (searchCondition.IsEmpty()) {
                return [];
            }
            searchCondition.TargetFolderId = targetFolder.Id;
            searchCondition.IncludeSubFolders = isIncludeSubFolders;

            // APIを使用して検索を実行
            List<ContentItemEntity> items = await PythonExecutor.PythonAIFunctions.SearchContentItems(searchCondition);

            List<ContentItem> result = [];
            foreach (var item in items) {
                ContentItem wrapper = new() {
                    Entity = item,
                };
                result.Add(wrapper);
            }
            return result;
        }
    }
}
