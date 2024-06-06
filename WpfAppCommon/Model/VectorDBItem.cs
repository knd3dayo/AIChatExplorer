using System.ComponentModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using WpfAppCommon.Factory;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {

    /// <summary>
    /// VectorDBの種類。現在はFaiss,Chroma(インメモリ)のみ
    /// </summary>
    public enum VectorDBTypeEnum {
        [Description("Faiss")]
        Faiss = 0,
        [Description("Chroma")]
        Chroma = 1,
        [Description("PGVector")]
        PGVector = 2,
        [Description("Custom")]
        Custom = 3,

    }
    /// <summary>
    /// VectorDBのアイテム
    /// </summary>
    public class VectorDBItem {

        // システムk共通のベクトルDBの名前
        public static string SystemCommonVectorDBName = "SystemCommonVectorDB";
        // システム共通のベクトルDB
        public static VectorDBItem SystemCommonVectorDB {
            get {
                // DBからベクトルDBを取得
                var item = GetItems().FirstOrDefault(item => item.Name == SystemCommonVectorDBName);
                if (item == null) {
                    item = new VectorDBItem() {
                        Id = LiteDB.ObjectId.Empty,
                        Name = SystemCommonVectorDBName,
                        Description = "システム共通のベクトルDBです。",
                        Type = VectorDBTypeEnum.Chroma,
                        VectorDBURL = "clipboard_vector_db",
                        IsEnabled = true
                    };
                    item.Save();
                }
                return item;

            }
        }

        public LiteDB.ObjectId Id { get; set; } = LiteDB.ObjectId.Empty;

        // 名前
        [JsonPropertyName("VectorDBName")]
        public string Name { get; set; } = "";
        // 説明
        [JsonPropertyName("VectorDBDescription")]
        public string Description { get; set; } = "ユーザーからの質問に基づき過去ドキュメントを検索するための汎用ベクトルDBです。";

        [JsonPropertyName("VectorDBURL")]
        public string VectorDBURL { get; set; } = "";

        [JsonIgnore]
        public VectorDBTypeEnum Type { get; set; } = VectorDBTypeEnum.Faiss;

        // VectorDBTypeString
        [JsonPropertyName("VectorDBTypeString")]
        public string VectorDBTypeString {
            get {
                return Type.ToString();
            }
        }

        // 有効かどうか
        [JsonIgnore]
        public bool IsEnabled { get; set; } = true;

        // Save
        public void Save() {
            // DBControllerのインスタンスを取得
            IClipboardDBController dbController = ClipboardAppFactory.Instance.GetClipboardDBController();
            // UpsertItemメソッドを呼び出して保存
            dbController.UpsertVectorDBItem(this);

        }

        // Delete
        public void Delete() {
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
            return dbController.GetVectorDBItems();
        }
        // IsEnabled=Trueのアイテムを取得
        public static IEnumerable<VectorDBItem> GetEnabledItems() {
            return GetItems().Where(item => item.IsEnabled);
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

        public void UpdateIndex(ClipboardItem clipboardItem) {

            // TODO コレクション名を設定する。
            PythonExecutor.PythonFunctions.UpdateVectorDBIndex(IPythonFunctions.VectorDBUpdateMode.update, clipboardItem, this);
        }

        public void DeleteIndex(ClipboardItem clipboardItem) {
            // TODO コレクション名を設定する。
            PythonExecutor.PythonFunctions.UpdateVectorDBIndex(IPythonFunctions.VectorDBUpdateMode.delete, clipboardItem, this);
        }

        // TestLangChain
        public void TestLangChain() {
            try {

                ChatResult result = PythonExecutor.PythonFunctions.LangChainChat([this], "Hello", []);
                if (string.IsNullOrEmpty(result.Response)) {
                    Tools.Error("[NG]:LangChainの実行に失敗しました。");
                } else {
                    string Message = "[OK]:LangChainの実行が可能です。";
                    Tools.Info(Message, true);
                }
            } catch (Exception ex) {
                string Message = "[NG]:LangChainの実行に失敗しました。\n[メッセージ]" + ex.Message + "\n[スタックトレース]" + ex.StackTrace;
                Tools.Error(Message);
            }
        }

    }
}
