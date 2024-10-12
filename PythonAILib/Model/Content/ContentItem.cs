using LiteDB;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Image;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils;
using QAChat;

namespace PythonAILib.Model.Content {
    public class ContentItem {

        public ObjectId Id { get; set; } = ObjectId.Empty;

        // ClipboardFolderのObjectId
        public ObjectId CollectionId { get; set; } = ObjectId.Empty;


        // ファイルのObjectId
        public List<ObjectId> FileObjectIds { get; set; } = [];


        // ファイル
        // LiteDBの別コレクションで保存されているオブジェクト。LiteDBからはLoad**メソッドで取得する。Saveメソッドで保存する
        protected List<ContentAttachedItem> _attachedItems = [];
        public List<ContentAttachedItem> AttachedItems {
            get {
                if (_attachedItems.Count() == 0) {
                    LoadFiles();
                }
                return _attachedItems;
            }
        }
        protected void LoadFiles() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            foreach (var fileObjectId in FileObjectIds) {
                ContentAttachedItem? file = libManager.DataFactory.GetAttachedItem(fileObjectId);
                if (file != null) {
                    _attachedItems.Add(file);
                }
            }
        }

        protected void SaveFiles() {
            //Fileを保存
            FileObjectIds = [];
            foreach (ContentAttachedItem file in _attachedItems) {
                file.Save();
                // ClipboardItemFileをSaveした後にIdが設定される。そのあとでFileObjectIdsに追加
                FileObjectIds.Add(file.Id);
            }
        }


        // 生成日時
        public DateTime CreatedAt { get; set; }

        // 更新日時
        public DateTime UpdatedAt { get; set; }

        // クリップボードの内容
        public string Content { get; set; } = "";

        //説明
        public string Description { get; set; } = "";


        // クリップボードの内容の種類
        public ContentTypes.ContentItemTypes ContentType { get; set; }

        // OpenAIチャットのChatItemコレクション
        // LiteDBの同一コレクションで保存されているオブジェクト。ClipboardItemオブジェクト生成時にロード、Save時に保存される。
        public List<ChatHistoryItem> ChatItems { get; set; } = [];

        #region プロンプトテンプレートに基づくチャットの結果
        public PromptChatResult PromptChatResult { get; set; } = new();

        // 背景情報
        [BsonIgnore]
        public string BackgroundInfo {
            get {
                return PromptChatResult.GetTextContent(PromptItem.SystemDefinedPromptNames.BackgroundInformationGeneration.ToString());
            }
            set {
                PromptChatResult.SetTextContent(PromptItem.SystemDefinedPromptNames.BackgroundInformationGeneration.ToString(), value);
            }
        }

        // サマリー
        [BsonIgnore]
        public string Summary {
            get {
                return PromptChatResult.GetTextContent(PromptItem.SystemDefinedPromptNames.SummaryGeneration.ToString());
            }
            set {
                PromptChatResult.SetTextContent(PromptItem.SystemDefinedPromptNames.SummaryGeneration.ToString(), value);
            }
        }

        // Tasks
        [BsonIgnore]
        public List<Dictionary<string,string>> Tasks {
            get {
                List<Dictionary<string, string>>? tasks = PromptChatResult.GetComplexContent(PromptItem.SystemDefinedPromptNames.TasksGeneration.ToString());
                return tasks ?? [];
            }
            set {
                PromptChatResult.SetComplexContent(PromptItem.SystemDefinedPromptNames.TasksGeneration.ToString(), value);
            }
        }

        #endregion

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

        // タグ表示用の文字列
        public string TagsString() {
            return string.Join(",", Tags);
        }

        [BsonIgnore]
        public string? HeaderText {
            get {
                string header1 = "";
                // 更新日時文字列を追加
                header1 += $"[{PythonAILibStringResources.Instance.UpdateDate}]" + UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 作成日時文字列を追加
                header1 += $"[{PythonAILibStringResources.Instance.CreationDateTime}]" + CreatedAt.ToString("yyyy/MM/dd HH:mm:ss") + "\n";
                // 貼り付け元のアプリケーション名を追加
                header1 += $"[{PythonAILibStringResources.Instance.SourceAppName}]" + SourceApplicationName + "\n";
                // 貼り付け元のアプリケーションのタイトルを追加
                header1 += $"[{PythonAILibStringResources.Instance.SourceTitle}]" + SourceApplicationTitle + "\n";
                // Tags
                header1 += $"[{PythonAILibStringResources.Instance.Tag}]" + TagsString() + "\n";
                // ピン留め中かどうか
                if (IsPinned) {
                    header1 += $"[{PythonAILibStringResources.Instance.Pinned}]\n";
                }

                if (ContentType == ContentTypes.ContentItemTypes.Text) {
                    return header1 + $"[{PythonAILibStringResources.Instance.Type}]Text";
                } else if (ContentType == ContentTypes.ContentItemTypes.Files) {
                    return header1 + $"[{PythonAILibStringResources.Instance.Type}]File";
                } else if (ContentType == ContentTypes.ContentItemTypes.Image) {
                    return header1 + $"[{PythonAILibStringResources.Instance.Type}]Image";
                } else {
                    return header1 + $"[{PythonAILibStringResources.Instance.Type}]Unknown";
                }
            }
        }



        public string UpdatedAtString {
            get {
                return UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");
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


        // 
        public virtual List<ContentAttachedItem> ClipboardItemFiles { get; set; } = [];

        public virtual VectorDBItem GetVectorDBItem() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            VectorDBItem item = libManager.DataFactory.GetSystemVectorDBItem();
            item.CollectionName = CollectionId.ToString();
            return item;

        }

        public virtual void Delete() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.DeleteItem(this);

        }

        public virtual void Save(bool contentIsModified = true) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.UpsertItem(this, contentIsModified);

        }

        // OpenAIを使用してイメージからテキスト抽出する。
        public void ExtractImageWithOpenAI() {

            foreach (var file in ClipboardItemFiles) {
                file.ExtractText();
            }
        }

        // テキストを抽出」を実行するコマンド
        public ContentItem ExtractTextCommandExecute() {

            foreach (var clipboardItemFile in ClipboardItemFiles) {
                clipboardItemFile.ExtractText();
                LogWrapper.Info($"{PythonAILibStringResources.Instance.TextExtracted}");
            }
            return this;
        }

        // OpenAIを使用してタイトルを生成する
        public void CreateAutoTitleWithOpenAI() {

            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            PromptItem promptItem = libManager.DataFactory.GetSystemPromptTemplateByName(PromptItem.SystemDefinedPromptNames.TitleGeneration.ToString()) ?? throw new Exception("PromptItem not found");

            // ContentTypeがTextの場合
            if (ContentType == ContentTypes.ContentItemTypes.Text) {
                // Contentがない場合は処理しない
                if (string.IsNullOrEmpty(Content)) {
                    return;
                }

                // contentの文字数が4096文字を超える場合は4096文字までに制限
                string contentText = Content.Length > 4096 ? Content[..4096] : Content;

                // ChatRequest.CreateTitleを実行
                string result = ChatUtil.CreateTextChatResult(openAIProperties, [],  promptItem, contentText);

                if (string.IsNullOrEmpty(result) == false) {
                    Description = result;
                }
                return;
            }
            // ContentTypeがFiles,の場合
            if (ContentType == ContentTypes.ContentItemTypes.Files) {
                // FileObjectIdsがない場合は処理しない
                if (ClipboardItemFiles.Count == 0) {
                    return;
                }
                foreach (var clipboardItemFile in ClipboardItemFiles) {
                    string path = clipboardItemFile.FilePath;
                    if (string.IsNullOrEmpty(path)) {
                        continue;
                    }
                    // ファイルのフルパスをタイトルとして使用
                    Description += path + " ";
                }
                return;
            }

            // ContentTypeがImageの場合
            Description = "Image";
        }

        // PromptItemの内容でチャットを実行して結果をPromptChatResultに保存する
        public void CreateChatResult(PromptItem promptItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            List<VectorDBItem> vectorDBItems = promptItem.ChatType switch {
                OpenAIExecutionModeEnum.OpenAIRAG => [ libManager.DataFactory.GetSystemVectorDBItem()],
                OpenAIExecutionModeEnum.LangChain => [libManager.DataFactory.GetSystemVectorDBItem()],
                OpenAIExecutionModeEnum.AnalyzeAndDictionarize => [ libManager.DataFactory.GetSystemVectorDBItem()],
                _ => []
            };

            // Contentがない場合は処理しない
            if (string.IsNullOrEmpty(Content)) {
                return;
            }
            // ヘッダー情報とコンテンツ情報を結合
            // ★TODO タグ情報を追加する
            string contentText = HeaderText + "\n" + Content;
            // PromptResultTypeがTextContentの場合
            if (promptItem.PromptResultType == PromptItem.PromptResultTypeEnum.TextContent) {
                string result = ChatUtil.CreateTextChatResult(openAIProperties, vectorDBItems, promptItem, contentText);
                if (string.IsNullOrEmpty(result) == false) {
                    // PromptChatResultに結果を保存
                    PromptChatResult.SetTextContent(promptItem.Name, result);
                }
                return;
            }
            // PromptResultTypeがComplexContentの場合
            if (promptItem.PromptResultType == PromptItem.PromptResultTypeEnum.ComplexContent) {
                Dictionary<string, dynamic?> response = ChatUtil.CreateComplexChatResult(openAIProperties, vectorDBItems, promptItem, contentText);
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
                    // PromptChatResultに結果を保存
                    PromptChatResult.SetComplexContent(promptItem.Name, result);
                }
                return;
            }
            // PromptResultTypeがListの場合
            if (promptItem.PromptResultType == PromptItem.PromptResultTypeEnum.List) {
                List<string> response = ChatUtil.CreateListChatResult(openAIProperties, vectorDBItems, promptItem, contentText);
                if (response.Count > 0) {
                    // PromptChatResultに結果を保存
                    PromptChatResult.SetListContent(promptItem.Name, response);
                }
                return;
            }
        }



        // 自動でサマリーを付与するコマンド
        public void CreateSummary() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            // システム定義のPromptItemを取得
            PromptItem? promptItem = libManager.DataFactory.GetPromptTemplateByName(PromptItem.SystemDefinedPromptNames.SummaryGeneration.ToString()) ?? throw new Exception("PromptItem not found");
            // CreateChatResultを実行
            CreateChatResult(promptItem);
        }

        // 課題リストを作成する
        public void CreateTasks() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);

            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();

            VectorDBItem vectorDBItem = libManager.DataFactory.GetSystemVectorDBItem();
            vectorDBItem.CollectionName = CollectionId.ToString();

            string contentText = Content;
            // contentTextがない場合は処理しない
            if (string.IsNullOrEmpty(contentText)) {
                return;
            }
            PromptItem promptItem = PromptItem.GetPromptItemByName(PromptItem.SystemDefinedPromptNames.TasksGeneration.ToString()) ?? throw new Exception("PromptItem not found");
            Dictionary<string, dynamic?> response = ChatUtil.CreateComplexChatResult(openAIProperties, [vectorDBItem], promptItem, contentText);

            // TaskItemをクリア
            Tasks.Clear();

            // resultからキー:resultを取得
            if (response.ContainsKey("result") == false) {
                return;
            }
            dynamic? result = response["result"] ;
            // resultがない場合は処理しない
            if (result == null) {
                return;
            }

            foreach (var item in result) {
                // todo と actionを取得
                Tasks.Add(item);
            }
        }

        public string? CreateNormalBackgroundInfo() {
            PromptItem promptItem = PromptItem.GetPromptItemByName(PromptItem.SystemDefinedPromptNames.BackgroundInformationGeneration.ToString()) ?? throw new Exception("PromptItem not found");
            // CreateChatResultを実行
            CreateChatResult(promptItem);
            return BackgroundInfo;
        }


        // 日本語の文章を解析する
        public string? CreateAnalyzedJapaneseSentence() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            VectorDBItem vectorDBItem = libManager.DataFactory.GetSystemVectorDBItem();
            vectorDBItem.CollectionName = CollectionId.ToString();

            // PromptItemの生成
            PromptItem promptItem = new PromptItem() {
                Name = "AnalyzeJapaneseSentence",
                Prompt = PromptStringResource.Instance.AnalyzeJapaneseSentenceRequest,
                PromptResultType = PromptItem.PromptResultTypeEnum.TextContent,
                ChatType = OpenAIExecutionModeEnum.OpenAIRAG
            };
            // CreateChatResultを実行
            CreateChatResult(promptItem);
            // "AnalyzeJapaneseSentence"の結果を返す
            return PromptChatResult.GetTextContent("AnalyzeJapaneseSentence");
        }

        // QAを作成する
        public string? CreateQA() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            VectorDBItem vectorDBItem = libManager.DataFactory.GetSystemVectorDBItem();
            vectorDBItem.CollectionName = CollectionId.ToString();

            string result;
            string contentText = Content;
            // contentTextがない場合は処理しない
            if (string.IsNullOrEmpty(contentText)) {
                return null;
            }

            List<string> promptList =
            [
                PromptStringResource.Instance.GenerateQuestionRequest,
                PromptStringResource.Instance.AnswerRequest,
            ];

            result = ChatUtil.CreateTextChatResult(OpenAIExecutionModeEnum.OpenAIRAG,libManager.ConfigParams.GetOpenAIProperties(), [vectorDBItem], promptList, contentText);
            if (string.IsNullOrEmpty(result) == false) {
                BackgroundInfo += "\n" + result;
            }
            return result;
        }

        // ベクトル検索を実行する
        public List<VectorSearchResult> VectorSearchCommandExecute(VectorDBItem vectorDBItem) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            string contentText = Content;
            // VectorSearchRequestを作成
            VectorSearchRequest request = new() {
                Query = contentText,
                SearchKWArgs = new Dictionary<string, object> {
                    ["k"] = 10
                }
            };
            // ベクトル検索を実行
            List<VectorSearchResult> results = PythonExecutor.PythonAIFunctions.VectorSearch(openAIProperties, vectorDBItem, request);
            return results;
        }
        public void UpdateEmbedding(VectorDBUpdateMode mode) {

            if (mode == VectorDBUpdateMode.delete) {
                // VectorDBItemを取得
                VectorDBItem folderVectorDBItem = GetVectorDBItem();
                // IPythonAIFunctions.ClipboardInfoを作成
                ContentInfo clipboardInfo = new(VectorDBUpdateMode.delete, this.Id.ToString(), this.Content);
                // Embeddingを削除
                folderVectorDBItem.DeleteIndex(clipboardInfo);
                return;
            }
            if (mode == VectorDBUpdateMode.update) {
                // IPythonAIFunctions.ClipboardInfoを作成
                // タイトルとHeaderTextを追加
                string content = Description + "\n" + HeaderText + "\n" + Content;

                ContentInfo clipboardInfo = new(VectorDBUpdateMode.update, this.Id.ToString(), content);

                // VectorDBItemを取得
                VectorDBItem folderVectorDBItem = GetVectorDBItem();
                // Embeddingを保存
                folderVectorDBItem.UpdateIndex(clipboardInfo);
            }
        }

        // Embeddingを更新する
        public void UpdateEmbedding() {
            UpdateEmbedding(VectorDBUpdateMode.update);
        }


    }
}
