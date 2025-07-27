using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows.Media.Imaging;
using LibPythonAI.Data;
using LibPythonAI.Model.Chat;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.Search;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Model.Content {
    public class ContentItemWrapper {

        public static readonly string TEMPORARY_ITEM_ID = "TemporaryItemId";
        public ContentItemWrapper() {
            Task.Run(() => {
                VectorDBProperties = UpdateVectorDBProperties();
            });
            ChatSettings = LoadChatSettingsFromExtendedProperties();
        }

        public ContentItemWrapper(ContentFolderEntity folder) {
            FolderId = folder.Id;
            Task.Run(() => {
                VectorDBProperties = UpdateVectorDBProperties();
            });
            ChatSettings = LoadChatSettingsFromExtendedProperties();
        }

        public ContentItemEntity Entity { get; protected set; } = new ContentItemEntity();

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
                Entity.Content = value;
                ContentModified = true;
            }
        }
        public bool DescriptionModified { get; set; } = false;

        //説明
        public string Description {
            get => Entity.Description;
            set {
                Entity.Description = value;
                DescriptionModified = true;
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
        public bool IsPinned { get => Entity.IsPinned; set { Entity.IsPinned = value; } }

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

        // このアイテムに紐付けらされたVectorSearchItem
        // UseFolderVectorSearchropertyがtrueの場合は、フォルダに設定されたVectorSearchItemを使用する
        public ObservableCollection<VectorSearchItem> VectorDBProperties { get; private set; } = [];

        private ObservableCollection<VectorSearchItem> UpdateVectorDBProperties() {
            ObservableCollection<VectorSearchItem> vectorDBProperties = [];
            if (UseFolderVectorSearchItem) {
                var items = GetFolder().GetVectorSearchProperties();
                vectorDBProperties = [.. items];
            }
            var value = Entity.ExtendedProperties.GetValueOrDefault("VectorDBProperties", null);
            if (value is string strValue) {
                vectorDBProperties = [.. VectorSearchItem.FromListJson(strValue)];
            } else if (value is List<VectorSearchItem> list) {
                vectorDBProperties = [.. list];
            } else {
                vectorDBProperties = [];
            }

            // Addイベント発生時の処理
            vectorDBProperties.CollectionChanged += (sender, e) => {
                if (e.NewItems != null) {
                    // Entityを更新
                    Entity.ExtendedProperties["VectorDBProperties"] = VectorSearchItem.ToListJson(vectorDBProperties);
                }
            };
            // Removeイベント発生時の処理
            vectorDBProperties.CollectionChanged += (sender, e) => {
                if (e.OldItems != null) {
                    // Entityを更新
                    Entity.ExtendedProperties["VectorDBProperties"] = VectorSearchItem.ToListJson(vectorDBProperties);
                }
            };
            // Clearイベント発生時の処理
            vectorDBProperties.CollectionChanged += (sender, e) => {
                // Entityを更新
                Entity.ExtendedProperties["VectorDBProperties"] = VectorSearchItem.ToListJson(vectorDBProperties);
            };
            return vectorDBProperties;
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
        public ChatSettings ChatSettings { get; set; }

        private ChatSettings LoadChatSettingsFromExtendedProperties() {
            var value = GetExtendedProperty<object>("ChatSettingsJson");
            ChatSettings chatSettings;
            if (value is string) {
                chatSettings = ChatSettings.FromJson(value.ToString() ?? "{}");
                return chatSettings;
            }
            return new ChatSettings();
        }

        private void SaveChatSettingsToExtendedProperties() {
            if (ChatSettings == null) {
                Entity.ExtendedProperties.Remove("ChatSettingsJson");
            } else {
                Entity.ExtendedProperties["ChatSettingsJson"] = ChatSettings.ToJson();
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
            return new ContentItemWrapper() {
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
        public string HeaderText {
            get {

                string header1 = "";

                // タイトルを追加
                header1 += $"[{PythonAILibStringResourcesJa.Instance.Title}]" + Description + "\n";
                // 作成日時文字列を追加
                header1 += $"[{PythonAILibStringResourcesJa.Instance.CreationDateTime}]" + CreatedAtString + "\n";
                // 更新日時文字列を追加
                header1 += $"[{PythonAILibStringResourcesJa.Instance.UpdateDate}]" + UpdatedAtString + "\n";
                // ベクトル化日時文字列を追加
                header1 += $"[{PythonAILibStringResourcesJa.Instance.VectorizedDate}]" + VectorizedAtString + "\n";
                // 貼り付け元のアプリケーション名を追加
                header1 += $"[{PythonAILibStringResourcesJa.Instance.SourceAppName}]" + SourceApplicationName + "\n";
                // 貼り付け元のアプリケーションのタイトルを追加
                header1 += $"[{PythonAILibStringResourcesJa.Instance.SourceTitle}]" + SourceApplicationTitle + "\n";
                // SourcePathを追加
                header1 += $"[{PythonAILibStringResourcesJa.Instance.SourcePath}]" + SourcePath + "\n";

                if (ContentType == ContentItemTypes.ContentItemTypeEnum.Text) {
                    header1 += $"[{PythonAILibStringResourcesJa.Instance.Type}]Text";
                } else if (ContentType == ContentItemTypes.ContentItemTypeEnum.Files) {
                    header1 += $"[{PythonAILibStringResourcesJa.Instance.Type}]File";
                } else if (ContentType == ContentItemTypes.ContentItemTypeEnum.Image) {
                    header1 += $"[{PythonAILibStringResourcesJa.Instance.Type}]Image";
                } else {
                    header1 += $"[{PythonAILibStringResourcesJa.Instance.Type}]Unknown";
                }
                // 文書の信頼度
                header1 += $"\n[{PythonAILibStringResourcesJa.Instance.DocumentReliability}]" + DocumentReliability + "%\n";
                // ★TODO フォルダーの説明を文章のカテゴリーの説明として追加
                var Folder = GetFolder();
                if (Folder != null && !string.IsNullOrEmpty(Folder.Description)) {
                    header1 += $"[{PythonAILibStringResourcesJa.Instance.DocumentCategorySummary}]" + Folder.Description + "\n";
                }

                // Tags
                header1 += $"[{PythonAILibStringResourcesJa.Instance.Tag}]" + TagsString() + "\n";
                return header1;
            }
        }

        // Load
        public virtual void Load(Action beforeAction, Action afterAction) { }

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


        public virtual VectorSearchItem GetMainVectorSearchItem() {
            return GetFolder().GetMainVectorSearchItem();
        }


        public virtual void Save() {
            // IdがTEMPORARY_ITEM_IDの場合は保存しない
            if (Id == TEMPORARY_ITEM_ID) {
                return;
            }
            if (ContentModified || DescriptionModified) {
                // 更新日時を設定
                UpdatedAt = DateTime.Now;
                // ベクトルを更新
                Task.Run(() => {
                    var item = GetMainVectorSearchItem();
                    string? vectorDBItemName = item.VectorDBItemName;
                    if (string.IsNullOrEmpty(vectorDBItemName)) {
                        LogWrapper.Error(PythonAILibStringResourcesJa.Instance.NoVectorDBSet);
                        return;
                    }
                    VectorEmbeddingItem vectorDBEntry = new(Id.ToString(), GetFolder().ContentFolderPath);
                    vectorDBEntry.SetMetadata(this);
                    VectorEmbeddingItem.UpdateEmbeddings(vectorDBItemName, vectorDBEntry);
                });
            }

            // ChatSettingsをExtendedPropertiesに保存
            SaveChatSettingsToExtendedProperties();
            ContentItemEntity.SaveItems([Entity]);
            ContentModified = false;
            DescriptionModified = false;

        }

        public void Delete() {
            ContentItemCommands.DeleteEmbeddings([this]);
            ContentItemEntity.DeleteItems([Entity]);
        }

        // FolderIdが一致するContentItemWrapperを取得
        public static List<T> GetItems<T>(ContentFolderWrapper folder) where T : ContentItemWrapper {
            using PythonAILibDBContext db = new();
            var items = db.ContentItems.Where(x => x.FolderId == folder.Id);
            List<T> result = [];
            foreach (var item in items) {
                if (Activator.CreateInstance(typeof(T)) is T newItem) {
                    newItem.Entity = item;
                    newItem.ChatSettings = newItem.LoadChatSettingsFromExtendedProperties();
                    result.Add(newItem);
                }
            }
            return result;
        }

        public static T? GetItem<T>(string id) where T : ContentItemWrapper, new() {
            using PythonAILibDBContext db = new();
            var item = db.ContentItems.FirstOrDefault(x => x.Id == id);
            if (item == null) {
                return null;
            }
            T result = new T() {
                Entity = item,
            };
            result.ChatSettings = result.LoadChatSettingsFromExtendedProperties();
            return result;
        }


        public static void DeleteItems(List<ContentItemWrapper> items) {
            ContentItemCommands.DeleteEmbeddings(items);
            ContentItemEntity.DeleteItems(items.Select(item => item.Entity).ToList());
        }

        public static void SaveItems(List<ContentItemWrapper> items, bool applyAutoProcess = false) {

            foreach (var item in items) {
                if (item.ContentModified || item.DescriptionModified) {
                    // 更新日時を設定
                    item.UpdatedAt = DateTime.Now;

                    // ベクトルを更新
                    Task.Run(async () => {
                        var searchItem = item.GetMainVectorSearchItem();
                        string? vectorDBItemName = searchItem.VectorDBItemName;
                        if (string.IsNullOrEmpty(vectorDBItemName)) {
                            LogWrapper.Error(PythonAILibStringResourcesJa.Instance.NoVectorDBSet);
                            return;
                        }
                        VectorEmbeddingItem vectorDBEntry = new(item.Id.ToString(), item.GetFolder().ContentFolderPath);
                        vectorDBEntry.SetMetadata(item);
                        VectorEmbeddingItem.UpdateEmbeddings(vectorDBItemName, vectorDBEntry);
                    });
                }
                // ChatSettingsをExtendedPropertiesに保存
                item.SaveChatSettingsToExtendedProperties();
            }
            if (applyAutoProcess) {
                // 自動処理を適用
            }
            ContentItemEntity.SaveItems(items.Select(item => item.Entity).ToList());
        }

        // ApplicationItemをJSON文字列に変換する
        public static string ToJson(ContentItemWrapper item) {
            return JsonSerializer.Serialize(item, JsonUtil.JsonSerializerOptions);
        }


        // JSON文字列をApplicationItemに変換する
        public static ContentItemWrapper? FromJson(string json) {
            ContentItemWrapper? item = JsonSerializer.Deserialize<ContentItemWrapper>(json, JsonUtil.JsonSerializerOptions);
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
