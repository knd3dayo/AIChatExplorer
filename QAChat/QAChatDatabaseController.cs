using LiteDB;
using QAChat.Model;

namespace QAChat {
    public class QAChatDatabaseController {

        // プロンプトテンプレートを保存するコレクション名
        public const string PromptTemplateCollectionName = "PromptTemplate";

        private static LiteDatabase? db;

        public static LiteDatabase GetClipboardDatabase() {
            if (db == null) {
                db = new LiteDatabase("QAChat.db");
                // 前回までのトランザクションのチェックポイント処理を行う
                db.Checkpoint();
            }
            return db;
        }

        public static void UpsertPromptTemplate(PromptItem promptItem) {
            var db = GetClipboardDatabase();

            var col = db.GetCollection<PromptItem>(PromptTemplateCollectionName);
            col.Upsert(promptItem);
        }
        // プロンプトテンプレートを取得する
        public static PromptItem? GetPromptTemplate(string name) {
            var col = GetClipboardDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            return col.FindOne(x => x.Name == name);
        }
        // 引数として渡されたプロンプトテンプレートを削除する
        public static void DeletePromptTemplate(PromptItem promptItem) {
            var col = GetClipboardDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            col.Delete(promptItem.Id);
        }

        // プロンプトテンプレートを全て取得する
        public static ICollection<PromptItem> GetAllPromptTemplates() {
            ICollection<PromptItem> collation = new List<PromptItem>();
            var col = GetClipboardDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            foreach (var item in col.FindAll()) {
                collation.Add(item);
            }
            return collation;
        }
        // プロンプトテンプレートを全て削除する
        public static void DeleteAllPromptTemplates() {
            var col = GetClipboardDatabase().GetCollection<PromptItem>(PromptTemplateCollectionName);
            col.DeleteAll();
        }
    }
}
