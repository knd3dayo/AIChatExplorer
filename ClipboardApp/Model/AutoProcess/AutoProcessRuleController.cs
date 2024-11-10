using System.Collections.ObjectModel;
using ClipboardApp.Factory;
using ClipboardApp.Model.Folder;

namespace ClipboardApp.Model.AutoProcess {
    public class AutoProcessRuleController {

        // DBから自動処理ルールのコレクションを取得する
        public static ObservableCollection<AutoProcessRule> GetAutoProcessRules(ClipboardFolder targetFolder) {
            ObservableCollection<AutoProcessRule> rules = [];
            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetAutoProcessRuleCollection();
            foreach (var i in targetFolder.AutoProcessRuleIds) {
                var item = collection.FindById(i);
                if (item != null) {
                    rules.Add(item);
                }
            }
            return rules;

        }
        // TypeがCopyTo または MoveToのルールをLiteDBから取得する。
        public static IEnumerable<AutoProcessRule> GetCopyToMoveToRules() {
            var collection = ClipboardAppFactory.Instance.GetClipboardDBController().GetAutoProcessRuleCollection();
            var items = collection.FindAll().Where(
                x => x.RuleAction != null
                && (x.RuleAction.Name == SystemAutoProcessItem.TypeEnum.CopyToFolder.ToString()
                    || x.RuleAction.Name == SystemAutoProcessItem.TypeEnum.MoveToFolder.ToString()));
            return items;
        }

    }
}
