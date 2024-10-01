using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClipboardApp.Factory;
using ClipboardApp.Model.Folder;

namespace ClipboardApp.Model.AutoProcess
{
    public class AutoProcessRuleController
    {

        // DBから自動処理ルールのコレクションを取得する
        public static ObservableCollection<AutoProcessRule> GetAutoProcessRules(ClipboardFolder targetFolder)
        {
            ObservableCollection<AutoProcessRule> rules = [.. ClipboardAppFactory.Instance.GetClipboardDBController().GetAutoProcessRules(targetFolder)];
            return rules;

        }
        // TypeがCopyTo または MoveToのルールをLiteDBから取得する。
        public static IEnumerable<AutoProcessRule> GetCopyToMoveToRules()
        {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetCopyToMoveToRules();
        }

    }
}
