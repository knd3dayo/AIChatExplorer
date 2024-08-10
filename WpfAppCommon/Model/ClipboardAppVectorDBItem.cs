using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using WpfAppCommon.Factory;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {

    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class ClipboardAppVectorDBItem : VectorDBItem {

        // システム共通のベクトルDBの名前
        public static string SystemCommonVectorDBName = "SystemCommonVectorDB";
        // システム共通のベクトルDB
        public static VectorDBItem SystemCommonVectorDB {
            get {
                // DBからベクトルDBを取得
                var item = GetItems(true).FirstOrDefault(item => item.Name == SystemCommonVectorDBName);
                if (item == null) {
                    string docDBPath = Path.Combine(ClipboardAppConfig.AppDataFolder, "clipboard_doc_store.db");
                    string vectorDBPath = Path.Combine(ClipboardAppConfig.AppDataFolder, "clipboard_vector_db.db");
                    item = new ClipboardAppVectorDBItem() {
                        Id = LiteDB.ObjectId.Empty,
                        Name = SystemCommonVectorDBName,
                        Description = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。",
                        Type = VectorDBTypeEnum.Chroma,
                        VectorDBURL = vectorDBPath,
                        DocStoreURL = $"sqlite:///{docDBPath}",
                        IsUseMultiVectorRetriever = true,
                        IsEnabled = true,
                        IsSystem = true
                    };
                    item.Save();
                }
                // IsSystemフラグ導入前のバージョンへの対応
                item.IsSystem = true;
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
        public static IEnumerable<VectorDBItem> GetItems(bool includeSystemVectorDB) {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // GetItemsメソッドを呼び出して取得
            IEnumerable<VectorDBItem> items = dbController.GetVectorDBItems();
            if (!includeSystemVectorDB) {
                items = items.Where(item => !item.IsSystem && item.Name != SystemCommonVectorDBName);
            }

            return items;
        }
        public static IEnumerable<VectorDBItem> GetEnabledItems(bool includeSystemVectorDB) {
            return GetItems(includeSystemVectorDB).Where(item => item.IsEnabled);
        }

        public static VectorDBItem GetFolderVectorDBItem(ClipboardFolder folder) {
            // SystemCommonVectorDBからコピーを作成
            ClipboardAppVectorDBItem item = new() {
                Id = SystemCommonVectorDB.Id,
                Name = folder.FolderName,
                CollectionName = folder.Id.ToString(),
                // ★TODO デスクリプションはLangChainのToolのDescriptionに使用するが、適切なものでない場合はToolsが選択されないためとりあえず空文字に設定する。
                // Description = folder.FolderName,
                Description = "",
                Type = SystemCommonVectorDB.Type,
                VectorDBURL = SystemCommonVectorDB.VectorDBURL,
                DocStoreURL = SystemCommonVectorDB.DocStoreURL,
                IsUseMultiVectorRetriever = SystemCommonVectorDB.IsUseMultiVectorRetriever,
                IsEnabled = SystemCommonVectorDB.IsEnabled,
                IsSystem = SystemCommonVectorDB.IsSystem
            };
            return item;
        }

        // GetItemById
        public static VectorDBItem? GetItemById(LiteDB.ObjectId id) {
            return GetItems(true).FirstOrDefault(item => item.Id == id);
        }



        public override void UpdateIndex(IPythonFunctions.ContentInfo clipboard) {
            // CollectionNameの設定
            PythonExecutor.PythonFunctions.UpdateVectorDBIndex(ClipboardAppConfig.CreateOpenAIProperties(), clipboard, this);
        }

        public override void DeleteIndex(IPythonFunctions.ContentInfo clipboard) {

            PythonExecutor.PythonFunctions.UpdateVectorDBIndex(ClipboardAppConfig.CreateOpenAIProperties(), clipboard, this);
        }

        // TestLangChain
        public void TestLangChain() {
            try {
                ChatRequest chatController = new(ClipboardAppConfig.CreateOpenAIProperties());
                List<ChatItem> chatItems = [new ChatItem(ChatItem.UserRole, "こんにちは")];
                chatController.ChatHistory = chatItems;
                chatController.ChatMode = OpenAIExecutionModeEnum.LangChain;
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
