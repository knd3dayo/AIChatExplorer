using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.Model.Script;
using PythonAILib.PythonIF;

namespace PythonAILib.Model.AutoProcess {
    public class ScriptAutoProcessItem : SystemAutoProcessItem {
        public ScriptItem? ScriptItem { get; set; }

        public ScriptAutoProcessItem() { }

        public ScriptAutoProcessItem(ScriptItem scriptItem) {

            ScriptItem = scriptItem;
            Name = scriptItem.Name;
            DisplayName = scriptItem.Name;
            Description = scriptItem.Description;
            TypeName = TypeEnum.RunPythonScript;
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
        public override void Execute(ContentItem clipboardItem, ContentFolder? destinationFolder) {

            if (ScriptItem == null) {
                return;
            }
            Action<ContentItem> action = RunPythonAction(ScriptItem);
            ContentFolder? folder = ContentFolder.GetFolderById<ContentFolder>(DestinationFolderId);
            action(clipboardItem);
        }

        public static Action<ContentItem> RunPythonAction(ScriptItem item) {
            return (args) => {
                RunPythonScriptCommandExecute(item, args);
            };

        }

        // 自動実行でPythonスクリプトを実行するコマンド
        public static void RunPythonScriptCommandExecute(ScriptItem scriptItem, ContentItem clipboardItem) {
            string inputJson = ContentItem.ToJson(clipboardItem);

            string result = PythonExecutor.PythonMiscFunctions.RunScript(scriptItem.Content, inputJson);
            ContentItem? resultItem = ContentItem.FromJson<ContentItem>(result);

            resultItem?.CopyTo(clipboardItem);

        }
    }

}
