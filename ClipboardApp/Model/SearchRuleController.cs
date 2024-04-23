using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClipboardApp.Model {
    //検索条件ルールを管理するクラス
    public class SearchRuleController {
        
        // DBから検索条件ルールを取得する
        public static SearchRule? GetSearchRule(string name) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetSearchRule(name);
        }
        // コレクション名を指定して検索条件ルールを取得する
        public static SearchRule? GetSearchRuleByFolderName(string collectionName) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetSearchRuleByFolderName(collectionName);
        }


    }
}
