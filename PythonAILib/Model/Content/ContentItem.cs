using System.Collections.Generic;
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

        public virtual List<ContentAttachedItem> ClipboardItemFiles { get; set; } = [];

        public virtual VectorDBItem GetMainVectorDBItem() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            VectorDBItem item = libManager.DataFactory.GetMainVectorDBItem();
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
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            Task.Run(() => {
                UpdateEmbedding(VectorDBUpdateMode.delete);
            });

            // ファイルが存在する場合は削除
            foreach (var fileObjectId in FileObjectIds) {
                ContentAttachedItem? file = libManager.DataFactory.GetAttachedItem(fileObjectId);
                file?.Delete();
            }
            libManager.DataFactory.DeleteItem(this);
        }

        public virtual void Save(bool contentIsModified = true) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);

            if (contentIsModified) {
                // ★TODO DBControllerに処理を移動する。
                // ファイルを保存
                SaveFiles();

                // Embeddingを更新
                Task.Run(() => {
                    UpdateEmbedding();
                });
            }
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
            // ContentTypeがTextの場合
            if (ContentType == ContentTypes.ContentItemTypes.Text) {
                CreateChatResult(PromptItem.SystemDefinedPromptNames.TitleGeneration.ToString());
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

            // Contentがない場合は処理しない
            if (string.IsNullOrEmpty(Content)) {
                return;
            }

            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            OpenAIProperties openAIProperties = libManager.ConfigParams.GetOpenAIProperties();
            List<VectorDBItem> vectorDBItems = promptItem.ChatType switch {
                OpenAIExecutionModeEnum.OpenAIRAG => ReferenceVectorDBItems,
                OpenAIExecutionModeEnum.LangChain => ReferenceVectorDBItems,
                _ => []
            };

            // ヘッダー情報とコンテンツ情報を結合
            // ★TODO タグ情報を追加する
            string contentText = HeaderText + "\n" + Content;
            // PromptResultTypeがTextContentの場合
            if (promptItem.PromptResultType == PromptItem.PromptResultTypeEnum.TextContent) {
                string result = ChatUtil.CreateTextChatResult(openAIProperties, vectorDBItems, promptItem, contentText);
                if (string.IsNullOrEmpty(result) == false) {
                    // PromptChatResultに結果を保存
                    PromptChatResult.SetTextContent(promptItem.Name, result);
                    // PromptOutputTypeがOverwriteTitleの場合はDescriptionに結果を保存
                    if (promptItem.PromptOutputType == PromptItem.PromptOutputTypeEnum.OverwriteTitle) {
                        Description = result;
                    }
                    // PromptOutputTypeがOverwriteContentの場合はContentに結果を保存
                    if (promptItem.PromptOutputType == PromptItem.PromptOutputTypeEnum.OverwriteContent) {
                        Content = result;
                    }
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
                    // resultからDynamicDictionaryObjectを作成
                    List<Dictionary<string, object>> resultDict = [];
                    foreach (var item in result) {
                        resultDict.Add(item);
                    }
                    // PromptChatResultに結果を保存
                    PromptChatResult.SetComplexContent(promptItem.Name, result);
                }
                return;
            }
            // PromptResultTypeがListの場合
            if (promptItem.PromptResultType == PromptItem.PromptResultTypeEnum.ListContent) {
                List<string> response = ChatUtil.CreateListChatResult(openAIProperties, vectorDBItems, promptItem, contentText);
                if (response.Count > 0) {
                    // PromptChatResultに結果を保存
                    PromptChatResult.SetListContent(promptItem.Name, response);
                }
                return;
            }
        }
        // ExecuteSystemDefinedPromptを実行する
        public void CreateChatResult(string promptName) {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            // システム定義のPromptItemを取得
            PromptItem? promptItem = libManager.DataFactory.GetPromptTemplateByName(promptName) ?? throw new Exception("PromptItem not found");
            // CreateChatResultを実行
            CreateChatResult(promptItem);
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

        // ベクトルを更新する
        public void UpdateEmbedding(VectorDBUpdateMode mode) {

            if (mode == VectorDBUpdateMode.delete) {
                // VectorDBItemを取得
                VectorDBItem folderVectorDBItem = GetMainVectorDBItem();
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
                VectorDBItem folderVectorDBItem = GetMainVectorDBItem();
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
