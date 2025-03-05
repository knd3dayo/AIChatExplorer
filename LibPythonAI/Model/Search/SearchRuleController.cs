
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Search;
using PythonAILib.Common;

namespace PythonAILib.Model.Search {
    //検索条件ルールを管理するクラス
    public class SearchRuleController {

        // DBから検索条件ルールを取得する
        public static SearchRule? GetSearchRule(string name) {
            return SearchRule.GetItemByName(name);
        }

        // コレクション名を指定して検索条件ルールを取得する
        public static SearchRule? GetSearchRuleByFolder(ContentFolderWrapper folder) {
            return SearchRule.GetItemByFolder(folder);
        }
    }
}
