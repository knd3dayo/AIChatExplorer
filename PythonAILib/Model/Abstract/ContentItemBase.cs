using PythonAILib.Model.Abstract;
using PythonAILib.Model.Chat;
using PythonAILib.Model.File;
using PythonAILib.Model.Image;
using PythonAILib.Model.VectorDB;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using PythonAILib.Utils;

namespace PythonAILib.Model {
    public abstract class ContentItemBase {

        // 生成日時
        public DateTime CreatedAt { get; set; }

        // 更新日時
        public DateTime UpdatedAt { get; set; }

        // クリップボードの内容
        public string Content { get; set; } = "";

        //説明
        public string Description { get; set; } = "";


        // 背景情報
        public string BackgroundInfo { get; set; } = "";

        // サマリー
        public string Summary { get; set; } = "";

        // クリップボードの内容の種類
        public ContentTypes.ContentItemTypes ContentType { get; set; }

        // OpenAIチャットのChatItemコレクション
        // LiteDBの同一コレクションで保存されているオブジェクト。ClipboardItemオブジェクト生成時にロード、Save時に保存される。
        public List<ChatIHistorytem> ChatItems { get; set; } = [];

        // Issues
        public List<IssueItemBase> Issues { get; set; } = [];

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

        public abstract VectorDBItemBase GetVectorDBItem();

        public abstract void CopyTo(ContentItemBase newItem);
        public abstract void Delete();

        public abstract void Save(bool contentIsModified = true);

        public abstract void UpdateEmbedding(VectorDBUpdateMode mode);

        public abstract ContentItemBase Copy();
        // 
        public virtual List<ContentAttachedItemBase> ClipboardItemFiles { get; set; } = [];

        // OpenAIを使用してイメージからテキスト抽出する。
        public void ExtractImageWithOpenAI() {

            foreach (var file in ClipboardItemFiles) {
                file.ExtractText();
            }
        }

        // テキストを抽出」を実行するコマンド
        public ContentItemBase ExtractTextCommandExecute() {

            foreach (var clipboardItemFile in ClipboardItemFiles) {
                clipboardItemFile.ExtractText();
                LogWrapper.Info($"{PythonAILibStringResources.Instance.TextExtracted}");
            }
            return this;
        }

        // OpenAIを使用してタイトルを生成する
        public void CreateAutoTitleWithOpenAI(PromptItemBase promptItem, OpenAIProperties openAIProperties) {
            // ContentTypeがTextの場合
            if (ContentType == ContentTypes.ContentItemTypes.Text) {
                // Contentがない場合は処理しない
                if (string.IsNullOrEmpty(Content)) {
                    return;
                }

                // ChatRequest.CreateTitleを実行
                string result = ChatUtil.CreateTitle(openAIProperties, Content, promptItem.Prompt);

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

        // 自動でサマリーを付与するコマンド
        public void CreateSummary(PromptItemBase promptItem, OpenAIProperties openAIProperties) {
            string contentText = Content;
            // contentTextがない場合は処理しない
            if (string.IsNullOrEmpty(contentText)) {
                return;
            }
            string result = ChatUtil.CreateSummary(openAIProperties, contentText, promptItem.Prompt);
            if (string.IsNullOrEmpty(result) == false) {
                Summary = result;
            }
        }
        // 課題リストを作成する
        public void CreateIssues(OpenAIProperties openAIProperties, List<VectorDBItemBase> vectorDBItems, PromptItemBase promptItem) {
            string contentText = Content;
            // contentTextがない場合は処理しない
            if (string.IsNullOrEmpty(contentText)) {
                return;
            }
            List<string> result = ChatUtil.CreateIssues(openAIProperties, vectorDBItems, contentText, promptItem.Prompt);

            // IssueItemをクリア
            Issues.Clear();

            foreach (var item in result) {
                IssueItemBase issueItem = new() {
                    Title = "",
                    Content = item,
                    Action = ""
                };
                // IssueItemを追加
                Issues.Add(issueItem);
            }
        }


        // ベクトル検索を実行する
        public List<VectorSearchResult> VectorSearchCommandExecute(OpenAIProperties openAIProperties, VectorDBItemBase vectorDBItem, bool IncludeBackgroundInfo) {
            string contentText = Content;
            // IncludeBackgroundInfoInEmbeddingの場合はBackgroundInfoを含める
            if (IncludeBackgroundInfo) {
                contentText += $"\n---{PythonAILibStringResources.Instance.BackgroundInformation}--\n{BackgroundInfo}";
            }
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

    }
}
