using ClipboardApp.Factory;
using ClipboardApp.Model.Folder;

namespace ClipboardApp.Model.Search {
    //検索条件ルールを管理するクラス
    public class SearchRuleController {

        // DBから検索条件ルールを取得する
        public static SearchRule? GetSearchRule(string name) {
            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetSearchRuleCollection();
            return collection.FindOne(x => x.Name == name);
        }
        // コレクション名を指定して検索条件ルールを取得する
        public static SearchRule? GetSearchRuleByFolder(ClipboardFolder folder) {
            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetSearchRuleCollection();
            var item = collection.FindOne(x => x.SearchFolder != null && x.SearchFolder.Id == folder.Id);
            return item;
        }
    }
}
