using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppCommon.Model {
    public class AutoProcessRuleController {

        // DBから自動処理ルールのコレクションを取得する
        public static ObservableCollection<AutoProcessRule> GetAutoProcessRules(ClipboardItemFolder? targetFolder) {
            ObservableCollection<AutoProcessRule> rules = [.. ClipboardAppFactory.Instance.GetClipboardDBController().GetAutoProcessRules(targetFolder)];
            return rules;

        }
        // DBから自動処理ルールのコレクションを取得する
        public static ObservableCollection<AutoProcessRule> GetAllAutoProcessRules() {
            ObservableCollection<AutoProcessRule> rules = [.. ClipboardAppFactory.Instance.GetClipboardDBController().GetAllAutoProcessRules()];
            return rules;
        }
        // TypeがCopyTo または MoveToのルールをLiteDBから取得する。
        public static IEnumerable<AutoProcessRule> GetCopyToMoveToRules() {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetCopyToMoveToRules();
        }

    }
}
