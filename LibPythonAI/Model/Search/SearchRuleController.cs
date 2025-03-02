
using LibPythonAI.Data;
using PythonAILib.Common;
using PythonAILib.Model.Content;

namespace PythonAILib.Model.Search {
    //検索条件ルールを管理するクラス
    public class SearchRuleController {

        // DBから検索条件ルールを取得する
        public static SearchRule? GetSearchRule(string name) {
            using PythonAILibDBContext db = new();
            var item = db.SearchRules.FirstOrDefault(x => x.Name == name);
            if (item != null) {
                return new SearchRule(item);
            }
            return null;

        }

        // コレクション名を指定して検索条件ルールを取得する
        public static SearchRule? GetSearchRuleByFolder(ContentFolderWrapper folder) {
            using PythonAILibDBContext db = new();
            var item = db.SearchRules.FirstOrDefault(x => x.SearchFolder == folder.Entity);
            if (item != null) {
                return new SearchRule(item);
            }
            return null;
        }
    }
}
