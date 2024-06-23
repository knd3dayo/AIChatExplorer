using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using QAChat.Model;
using WpfAppCommon.Factory;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {

    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class ClipboardAppVectorDBItem: VectorDBItem {

        // システム共通のベクトルDBの名前
        public static string SystemCommonVectorDBName = "SystemCommonVectorDB";
        // システム共通のベクトルDB
        public static VectorDBItem SystemCommonVectorDB {
            get {
                // DBからベクトルDBを取得
                var item = GetItems().FirstOrDefault(item => item.Name == SystemCommonVectorDBName);
                if (item == null) {
                    item = new ClipboardAppVectorDBItem() {
                        Id = LiteDB.ObjectId.Empty,
                        Name = SystemCommonVectorDBName,
                        Description = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。",
                        Type = VectorDBTypeEnum.Chroma,
                        VectorDBURL = "clipboard_vector_db",
                        DocStoreURL = "sqlite:///clipboard_doc_store",
                        IsUseMultiVectorRetriever = true,
                        IsEnabled = true
                    };
                    item.Save();
                }
                return item;

            }
        }


        public ClipboardAppVectorDBItem() {
        }

        // Save
        public override void Save() {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // UpsertItemメソッドを呼び出して保存
            dbController.UpsertVectorDBItem(this);

        }

        // Delete
        public override void Delete() {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // DeleteItemメソッドを呼び出して削除
            dbController.DeleteVectorDBItem(this);
        }
        // Get
        public static IEnumerable<VectorDBItem> GetItems() {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // GetItemsメソッドを呼び出して取得
            IEnumerable<VectorDBItem> items = dbController.GetVectorDBItems();
            return items;
        }
        // IsEnabled=Trueのアイテムを取得
        public static IEnumerable<VectorDBItem> GetEnabledItems() {
            return GetItems().Where(item => item.IsEnabled);
        }

        // IsEnabled=Trueのアイテムを取得して、SystemCommonVectorDBのCollectionNameを指定した文字列に置換する
        public static IEnumerable<VectorDBItem> GetEnabledItemsWithSystemCommonVectorDBCollectionName(string? name, string? description) {
            if (name == null) {
                return GetEnabledItems();
            }
            return GetItems().Where(item => item.IsEnabled).Select(item => {
                if (item.Name == SystemCommonVectorDBName) {
                    item.CollectionName = name;
                    if (description != null) {
                        item.Description += description;
                    }

                }

                return item;
            });
        }
        // GetItemById
        public static VectorDBItem? GetItemById(LiteDB.ObjectId id) {
            return GetItems().FirstOrDefault(item => item.Id == id);
        }

        // Json文字列化する
        public static string ToJson(IEnumerable<VectorDBItem> items) {
            var options = new JsonSerializerOptions {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            return System.Text.Json.JsonSerializer.Serialize(items, options);
        }

        public override void UpdateIndex(IPythonFunctions.ClipboardInfo clipboard) {

            PythonExecutor.PythonFunctions.UpdateVectorDBIndex(ClipboardAppConfig.CreateOpenAIProperties(), clipboard, this);
        }

        public override void DeleteIndex(IPythonFunctions.ClipboardInfo clipboard) {

            PythonExecutor.PythonFunctions.UpdateVectorDBIndex(ClipboardAppConfig.CreateOpenAIProperties(), clipboard, this);
        }

        // TestLangChain
        public void TestLangChain() {
            try {
                ChatRequest chatController = new(ClipboardAppConfig.CreateOpenAIProperties());
                List<ChatItem> chatItems = [new ChatItem(ChatItem.UserRole, "こんにちは")];
                chatController.ChatHistory = chatItems;
                chatController.ChatMode = OpenAIExecutionModeEnum.RAG;
                ChatResult? result = chatController.ExecuteChat();
                if (string.IsNullOrEmpty(result?.Response)) {
                    LogWrapper.Error("[NG]:LangChainの実行に失敗しました。");
                } else {
                    string Message = "[OK]:LangChainの実行が可能です。";
                    LogWrapper.Info(Message);
                }
            } catch (Exception ex) {
                string Message = "[NG]:LangChainの実行に失敗しました。\n[メッセージ]" + ex.Message + "\n[スタックトレース]" + ex.StackTrace;
                LogWrapper.Error(Message);
            }
        }

    }
}
