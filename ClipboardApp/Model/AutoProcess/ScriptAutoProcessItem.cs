using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Script;
using PythonAILib.PythonIF;
using PythonAILib.Model.AutoProcess;

namespace ClipboardApp.Model.AutoProcess {
    public class ScriptAutoProcessItem : SystemAutoProcessItem {
        public ScriptItem? ScriptItem { get; set; }

        public ScriptAutoProcessItem() { }

        public ScriptAutoProcessItem(ScriptItem scriptItem) {

            ScriptItem = scriptItem;
            Name = scriptItem.Name;
            DisplayName = scriptItem.Name;
            Description = scriptItem.Description;
            Type = TypeEnum.RunPythonScript;
        }

        public static List<ScriptAutoProcessItem> GetScriptAutoProcessItems() {

            // DBからスクリプトのScriptItemを取得
            var collection = PythonAILibManager.Instance.DataFactory.GetScriptCollection<ScriptItem>();
            List<ScriptItem> items = [.. collection.FindAll()];
            List<ScriptAutoProcessItem> result = [];
            foreach (var item in items) {
                result.Add(new ScriptAutoProcessItem(item));
            }
            return result;
        }
        public override ContentItem? Execute(ContentItem clipboardItem, ContentFolder? destinationFolder) {

            if (ScriptItem == null) {
                return null;
            }
            Func<AutoProcessItem, ContentItem?> action = RunPythonAction(ScriptItem);
            ContentItem? result = action(new AutoProcessItem(clipboardItem, destinationFolder));
            return result;
        }

        public static Func<AutoProcessItem, ContentItem?> RunPythonAction(ScriptItem item) {
            return (args) => {
                RunPythonScriptCommandExecute(item, args.ContentItem);
                return args.ContentItem;
            };

        }

        // 自動実行でPythonスクリプトを実行するコマンド
        public static void RunPythonScriptCommandExecute(ScriptItem scriptItem, ContentItem clipboardItem) {
            string inputJson = ClipboardItem.ToJson(clipboardItem);

            string result = PythonExecutor.PythonMiscFunctions.RunScript(scriptItem.Content, inputJson);
            ClipboardItem? resultItem = ClipboardItem.FromJson<ClipboardItem>(result);

            resultItem?.CopyTo(clipboardItem);

        }
    }

}
