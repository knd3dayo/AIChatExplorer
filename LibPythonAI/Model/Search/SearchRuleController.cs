
using PythonAILib.Common;
using PythonAILib.Model.Content;

namespace PythonAILib.Model.Search {
    //検索条件ルールを管理するクラス
    public class SearchRuleController {

        // DBから検索条件ルールを取得する
        public static SearchRule? GetSearchRule(string name) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetSearchRuleCollection();

            return collection.FindOne(x => x.Name == name);
        }
        // コレクション名を指定して検索条件ルールを取得する
        public static SearchRule? GetSearchRuleByFolder(ContentFolder folder) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            var collection = libManager.DataFactory.GetSearchRuleCollection();
            var item = collection.FindOne(x => x.SearchFolder != null && x.SearchFolder.Id == folder.Id);
            return item;
        }
    }
}
