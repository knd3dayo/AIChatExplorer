using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using ClipboardApp.Factory;
using ClipboardApp.Model.AutoProcess;
using ClipboardApp.Model.Folder;
using LiteDB;
using PythonAILib.Model.Chat;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.VectorDB;
using QAChat.Resource;
using WpfAppCommon.Utils;


namespace ClipboardApp.Model {
    public partial class ClipboardItem : ContentItem {
        // コンストラクタ
        public ClipboardItem(LiteDB.ObjectId folderObjectId) {


            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
            CollectionId = folderObjectId;
        }

        // プロパティ
        public string FolderPath {
            get {
                // FolderObjectIdからClipboardFolderを取得
                ClipboardFolder? folder = (ClipboardFolder?)ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>().FindById(CollectionId);
                if (folder == null) {
                    return "";
                }
                return folder.FolderPath;
            }
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


        // ReferenceVectorDBItems
        public override List<VectorDBItem> ReferenceVectorDBItems {

            get {
                // IsReferenceVectorDBItemsSyncedがTrueの場合はそのまま返す
                if (IsReferenceVectorDBItemsSynced) {
                    return base.ReferenceVectorDBItems;
                }
                // folderを取得
                ClipboardFolder folder = GetFolder();
                base.ReferenceVectorDBItems = new(folder.ReferenceVectorDBItems);
                IsReferenceVectorDBItemsSynced = true;
                return base.ReferenceVectorDBItems;

            }
            set {
                base.ReferenceVectorDBItems = value;
            }
        }


        // -------------------------------------------------------------------
        // インスタンスメソッド
        // -------------------------------------------------------------------

        // 別フォルダに移動
        public void MoveToFolder(ClipboardFolder folder) {
            CollectionId = folder.Id;
            Save();
        }
        // 別フォルダにコピー
        public void CopyToFolder(ClipboardFolder folder) {
            ClipboardItem newItem = Copy();
            newItem.CollectionId = folder.Id;
            newItem.Save();
        }

        public ClipboardItem Copy() {
            ClipboardItem newItem = new(this.CollectionId);
            CopyTo(newItem);
            return newItem;

        }

        public void CopyTo(ContentItem newItem) {
            if (newItem is not ClipboardItem) {
                return;
            }
            ClipboardItem clipboardItem = (ClipboardItem)newItem;
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
            newItem.ChatItems = new List<ChatContentItem>(ChatItems);

        }

        // ベクトルDBを返す。
        public override VectorDBItem GetMainVectorDBItem() {
            return GetFolder().GetVectorDBItem();
        }

        public void MergeItems(List<ClipboardItem> items) {
            // itemsが空の場合は何もしない
            if (items.Count == 0) {
                return;
            }

            string mergeText = "\n";
            mergeText += "---\n";

            foreach (var item in items) {
                // itemが自分自身の場合はスキップ
                if (item.Id == Id) {
                    continue;
                }
                // Contentを追加
                mergeText += item.Content + "\n";
            }
            // mergeTextをContentに追加
            Content += mergeText;

            // Tagsのマージ。重複を除外して追加
            Tags.UnionWith(items.SelectMany(item => item.Tags));

            // マージしたアイテムを削除
            foreach (var item in items) {
                // itemが自分自身の場合はスキップ
                if (item.Id == Id) {
                    continue;
                }
                item.Delete();
            }
            // 保存
            Save();

        }

        // Collectionに対応するClipboardFolderを取得
        public ClipboardFolder GetFolder(Type? objectType = null) {
            ClipboardFolder? folder = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolderCollection<ClipboardFolder>().FindById(CollectionId);
            return folder ?? throw new Exception(CommonStringResources.Instance.CannotGetFolder);
        }

        //--------------------------------------------------------------------------------
        // staticメソッド
        //--------------------------------------------------------------------------------

        // ClipboardItemをJSON文字列に変換する
        public static string ToJson(ClipboardItem item) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(item, options);
        }


        // JSON文字列をClipboardItemに変換する
        public static ClipboardItem? FromJson(string json) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            var options = jsonSerializerOptions;
            ClipboardItem? item = System.Text.Json.JsonSerializer.Deserialize<ClipboardItem>(json, options);
            if (item == null) {
                LogWrapper.Error(CommonStringResources.Instance.FailedToParseJSONString);
                return null;
            }
            return item;

        }

        // 自分自身をDBに保存する
        public override void Save(bool contentIsModified = true) {

            base.Save(contentIsModified);
        }

        // 自分自身をDBから削除する
        public override void Delete() {
            base.Delete();
        }

        // 自動処理を適用する処理
        public ClipboardItem? ApplyAutoProcess() {

            ClipboardItem? result = this;
            // AutoProcessRulesを取得
            var AutoProcessRules = AutoProcessRuleController.GetAutoProcessRules(this.GetFolder());
            foreach (var rule in AutoProcessRules) {
                LogWrapper.Info($"{CommonStringResources.Instance.ApplyAutoProcessing} {rule.GetDescriptionString()}");
                result = rule.RunAction(result);
                // resultがNullの場合は処理を中断
                if (result == null) {
                    LogWrapper.Info(CommonStringResources.Instance.ItemsDeletedByAutoProcessing);
                    return null;
                }
            }
            return result;
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

    }
}
