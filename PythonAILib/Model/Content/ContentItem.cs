using System.IO;
using System.Windows.Media.Imaging;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Image;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils.Common;
using PythonAILib.Utils.Python;

namespace PythonAILib.Model.Content
{
    public class ContentItem {

        // 日時のダミー初期値。2000/1/1 0:0:0
        private static readonly DateTime InitialDateTime = new(2000, 1, 1, 0, 0, 0);

        // コンストラクタ
        public ContentItem() {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        public ObjectId Id { get; set; } = ObjectId.Empty;
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
        // ReferenceVectorDBItemsがフォルダのReferenceVectorDBItemsと同期済みかどうか
        public bool IsReferenceVectorDBItemsSynced { get; set; } = false;



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
                // ピン留め中かどうか
                if (IsPinned) {
                    header1 += $"[{PythonAILibStringResources.Instance.Pinned}]\n";
                }
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


        // ReferenceVectorDBItems
        public  List<VectorDBItem> GetReferenceVectorDBItems {

            get {
                // IsReferenceVectorDBItemsSyncedがTrueの場合はそのまま返す
                if (IsReferenceVectorDBItemsSynced) {
                    return ReferenceVectorDBItems;
                }
                // folderを取得
                var folder = GetFolder<ContentFolder>();
                if (folder == null) {
                    return [];
                }
                ReferenceVectorDBItems = new(folder.ReferenceVectorDBItems);
                IsReferenceVectorDBItemsSynced = true;
                return ReferenceVectorDBItems;

            }
            set {
                ReferenceVectorDBItems = value;
            }
        }
        // Collectionに対応するClipboardFolderを取得
        public T GetFolder<T>() where T: ContentFolder{
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
                    UpdateCache();
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

        // キャッシュを更新する
        // Base64Stringを参照する場合とテキスト抽出を行う場合にキャッシュを更新する。
        // 対象ファイルがない場合は何もしない。
        public void UpdateCache() {
            if (FilePath == null || System.IO.File.Exists(FilePath) == false) {
                return;
            }
            LastModified = new System.IO.FileInfo(FilePath).LastWriteTime.Ticks;

            byte[] bytes = System.IO.File.ReadAllBytes(FilePath);
            CachedBase64String = Convert.ToBase64String(bytes);
        }

        // テキストを抽出する
        public void ExtractText() {
            // キャッシュを更新
            UpdateCache();

            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            ChatRequestContext chatRequestContext = new() {
                VectorDBItems = ReferenceVectorDBItems,
                OpenAIProperties = openAIProperties
            };

            string base64 = Base64String;

            try {

                if (ContentTypes.IsImageData(base64)) {
                    string result = ChatUtil.ExtractTextFromImage(chatRequestContext, [base64]);
                    if (string.IsNullOrEmpty(result) == false) {
                        Content = result;
                    }
                } else {
                    // ファイル名から拡張子を取得
                    string extension = Path.GetExtension(FilePath);
                    string text = PythonExecutor.PythonAIFunctions.ExtractBase64ToText(base64, extension);
                    Content = text;
                }

            } catch (UnsupportedFileTypeException) {
                LogWrapper.Info(PythonAILibStringResources.Instance.UnsupportedFileType);
            }
        }

        #endregion


        public virtual VectorDBItem GetMainVectorDBItem() {
            VectorDBItem item = VectorDBItem.SystemCommonVectorDB;
            item.CollectionName = CollectionId.ToString();
            return item;
        }
        // 参照用のベクトルDBのリストのプロパティ
        private List<VectorDBItem> _referenceVectorDBItems = [];
        public virtual List<VectorDBItem> ReferenceVectorDBItems {
            get {
                return _referenceVectorDBItems;
            }
            set {
                _referenceVectorDBItems = value;
            }
        }

        public virtual void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            Task.Run(() => {
                UpdateEmbedding(VectorDBUpdateMode.delete);
            });
            libManager.DataFactory.GetItemCollection<ContentItem>().Delete(Id);
        }

        public virtual void Save(bool updateLastModifiedTime = true) {
            PythonAILibManager libManager = PythonAILibManager.Instance;

            if (updateLastModifiedTime) {
                // 更新日時を設定
                UpdatedAt = DateTime.Now;
            }
            libManager.DataFactory.GetItemCollection<ContentItem>().Upsert(this);
        }

        // OpenAIを使用してイメージからテキスト抽出する。
        public virtual void ExtractImageWithOpenAI() {
            ExtractText();
        }

        // テキストを抽出」を実行するコマンド
        public virtual ContentItem ExtractTextCommandExecute() {
            ExtractText();
            LogWrapper.Info($"{PythonAILibStringResources.Instance.TextExtracted}");
            return this;
        }
        // OpenAIを使用してタイトルを生成する
        public virtual void CreateAutoTitleWithOpenAI() {
            // ContentTypeがTextの場合
            if (ContentType == ContentTypes.ContentItemTypes.Text) {
                CreateChatResult(SystemDefinedPromptNames.TitleGeneration.ToString());
                return;
            }
            // ContentTypeがFiles,の場合
            if (ContentType == ContentTypes.ContentItemTypes.Files) {
                // ファイル名をタイトルとして使用
                Description += FileName;
                return;
            }
            // ContentTypeがImageの場合
            Description = "Image";
        }

        // PromptItemの内容でチャットを実行して結果をPromptChatResultに保存する
        public void CreateChatResult(PromptItem promptItem) {

            // Contentがない場合は処理しない
            if (string.IsNullOrEmpty(Content)) {
                return;
            }

            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            List<VectorDBItem> vectorDBItems = promptItem.ChatType switch {
                OpenAIExecutionModeEnum.OpenAIRAG => ReferenceVectorDBItems,
                OpenAIExecutionModeEnum.LangChain => ReferenceVectorDBItems,
                _ => []
            };
            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorDBItems = vectorDBItems,
                OpenAIProperties = openAIProperties
            };
            // ヘッダー情報とコンテンツ情報を結合
            // ★TODO タグ情報を追加する
            string contentText = HeaderText + "\n" + Content;

            // PromptResultTypeがTextContentの場合
            if (promptItem.PromptResultType == PromptResultTypeEnum.TextContent) {
                string result = ChatUtil.CreateTextChatResult(chatRequestContext, promptItem, contentText);
                if (string.IsNullOrEmpty(result) == false) {
                    // PromptChatResultに結果を保存
                    PromptChatResult.SetTextContent(promptItem.Name, result);
                    // PromptOutputTypeがOverwriteTitleの場合はDescriptionに結果を保存
                    if (promptItem.PromptOutputType == PromptOutputTypeEnum.OverwriteTitle) {
                        Description = result;
                    }
                    // PromptOutputTypeがOverwriteContentの場合はContentに結果を保存
                    if (promptItem.PromptOutputType == PromptOutputTypeEnum.OverwriteContent) {
                        Content = result;
                    }
                }
                return;
            }

            // PromptResultTypeがTableContentの場合
            if (promptItem.PromptResultType == PromptResultTypeEnum.TableContent) {
                Dictionary<string, dynamic?> response = ChatUtil.CreateTableChatResult(chatRequestContext, promptItem, contentText);
                // resultからキー:resultを取得
                if (response.ContainsKey("result") == false) {
                    return;
                }
                dynamic? result = response["result"];
                // resultがない場合は処理しない
                if (result == null) {
                    return;
                }
                if (result.Count > 0) {
                    // resultからDynamicDictionaryObjectを作成
                    List<Dictionary<string, object>> resultDictList = [];
                    foreach (var item in result) {
                        resultDictList.Add(item);
                    }
                    // PromptChatResultに結果を保存
                    PromptChatResult.SetTableContent(promptItem.Name, resultDictList);
                }
                return;
            }
            // PromptResultTypeがListの場合
            if (promptItem.PromptResultType == PromptResultTypeEnum.ListContent) {
                List<string> response = ChatUtil.CreateListChatResult(chatRequestContext, promptItem, contentText);
                if (response.Count > 0) {
                    // PromptChatResultに結果を保存
                    PromptChatResult.SetListContent(promptItem.Name, response);
                }
                return;
            }
        }
        // ExecuteSystemDefinedPromptを実行する
        public void CreateChatResult(string promptName) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            // システム定義のPromptItemを取得
            PromptItem promptItem = libManager.DataFactory.GetPromptCollection<PromptItem>().FindAll().FirstOrDefault(x => x.Name == promptName) ?? throw new Exception("PromptItem not found");
            // CreateChatResultを実行
            CreateChatResult(promptItem);
        }

        // 文章の信頼度を判定する
        public void CheckDocumentReliability() {

            CreateChatResult(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString());
            // PromptChatResultからキー：DocumentReliabilityCheck.ToString()の結果を取得
            string result = PromptChatResult.GetTextContent(SystemDefinedPromptNames.DocumentReliabilityCheck.ToString());
            // resultがない場合は処理しない
            if (string.IsNullOrEmpty(result)) {
                return;
            }
            // ChatUtl.CreateDictionaryChatResultを実行
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorDBItems = ReferenceVectorDBItems,
                OpenAIProperties = openAIProperties
            };

            Dictionary<string, dynamic?> response = ChatUtil.CreateDictionaryChatResult(chatRequestContext, new PromptItem() {
                ChatType = OpenAIExecutionModeEnum.OpenAIRAG,
                Prompt = PromptStringResource.Instance.DocumentReliabilityDictionaryPrompt
            }, result);
            // responseからキー：reliabilityを取得
            if (response.ContainsKey("reliability") == false) {
                return;
            }
            dynamic? reliability = response["reliability"];

            int reliabilityValue = int.Parse(reliability?.ToString() ?? "0");

            // DocumentReliabilityにreliabilityを設定
            DocumentReliability = reliabilityValue;
            // responseからキー：reasonを取得
            if (response.ContainsKey("reason")) {
                dynamic? reason = response["reason"];
                // DocumentReliabilityReasonにreasonを設定
                DocumentReliabilityReason = reason?.ToString() ?? "";
            }
        }

        // ベクトル検索を実行する
        public List<VectorDBEntry> VectorSearch(List<VectorDBItem> vectorDBItems) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            // ChatRequestContextを作成
            ChatRequestContext chatRequestContext = new() {
                VectorDBItems = vectorDBItems,
                OpenAIProperties = openAIProperties
            };

            string contentText = Content;
            // VectorSearchRequestを作成
            VectorSearchRequest request = new() {
                Query = contentText,
                SearchKWArgs = new Dictionary<string, object> {
                    ["k"] = 10
                }
            };
            // ベクトル検索を実行
            List<VectorDBEntry> results = PythonExecutor.PythonAIFunctions.VectorSearch(chatRequestContext, request);
            return results;
        }

        // ベクトルを更新する
        public virtual void UpdateEmbedding(VectorDBUpdateMode mode) {

            // VectorDBItemを取得
            VectorDBItem folderVectorDBItem = GetMainVectorDBItem();

            if (mode == VectorDBUpdateMode.delete) {
                // IPythonAIFunctions.ClipboardInfoを作成
                VectorDBEntry vectorDBEntry = new(this.Id.ToString());

                // Embeddingを削除
                folderVectorDBItem.DeleteIndex(vectorDBEntry);
                return;
            }
            if (mode == VectorDBUpdateMode.update) {
                // IPythonAIFunctions.ClipboardInfoを作成
                VectorDBEntry vectorDBEntry = new(this.Id.ToString());
                // タイトルとHeaderTextを追加
                string description = Description + "\n" + HeaderText;
                if (ContentType == ContentTypes.ContentItemTypes.Text) {
                    vectorDBEntry.UpdateSourceInfo(description, Content, VectorSourceType.Clipboard, "", "", "", "");
                    // Embeddingを保存
                    folderVectorDBItem.UpdateIndex(vectorDBEntry);
                } else {
                    if (IsImage()) {
                        // 画像からテキスト抽出
                        vectorDBEntry.UpdateSourceInfo(description, Content, VectorSourceType.File, FilePath, "", "", Base64String);
                        // Embeddingを保存
                        folderVectorDBItem.UpdateIndex(vectorDBEntry);
                    } else {
                        vectorDBEntry.UpdateSourceInfo(description, Content, VectorSourceType.File, FilePath, "", "", "");
                        // Embeddingを保存
                        folderVectorDBItem.UpdateIndex(vectorDBEntry);
                    }
                }
                // ベクトル化日時を更新
                VectorizedAt = DateTime.Now;

            }
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
                CreateChatResult(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
                return PromptChatResult.GetTextContent(SystemDefinedPromptNames.BackgroundInformationGeneration.ToString()); ;
            });

            // すべてのタスクが完了するまで待機
            Task.WaitAll(task1);
            // 背景情報を更新 taskの結果がNullでない場合は追加
            if (task1.Result != null) {
                BackgroundInfo += task1.Result;
            }
        }

        // Embeddingを更新する
        public void UpdateEmbedding() {
            UpdateEmbedding(VectorDBUpdateMode.update);
        }


    }
}
