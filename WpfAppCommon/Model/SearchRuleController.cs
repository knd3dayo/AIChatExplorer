using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppCommon.Model {
    //検索条件ルールを管理するクラス
    public class SearchRuleController {
        
        // DBから検索条件ルールを取得する
        public static SearchRule? GetSearchRule(string name) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetSearchRule(name);
        }
        // コレクション名を指定して検索条件ルールを取得する
        public static SearchRule? GetSearchRuleByFolder(ClipboardFolder folder) {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetSearchRuleByFolder(folder);
        }


    }
}
