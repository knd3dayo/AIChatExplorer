using PythonAILib.PythonIF;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public  class ScriptAutoProcessItem : SystemAutoProcessItem{
        public ScriptItem? ScriptItem { get; set; }

        public ScriptAutoProcessItem() {
        }

        public ScriptAutoProcessItem(ScriptItem scriptItem){

            ScriptItem = scriptItem;
            Name = scriptItem.Name;
            DisplayName = scriptItem.Name;
            Description = scriptItem.Description;
            Type = TypeEnum.RunPythonScript;

        }


        public static List<ScriptAutoProcessItem> GetScriptAutoProcessItems() {
            // DBからスクリプトのScriptItemを取得
            List<ScriptItem> items = [.. ClipboardAppFactory.Instance.GetClipboardDBController().GetScriptItems()];
            List<ScriptAutoProcessItem> result = [];
            foreach (var item in items) {
                result.Add(new ScriptAutoProcessItem(item));
            }
            return result;

        }
        public override ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder) {
            
            if (ScriptItem == null) {
                return null;
            }
            Func<AutoProcessItemArgs, ClipboardItem?> action = RunPythonAction(ScriptItem);
            ClipboardItem? result = action(new AutoProcessItemArgs(clipboardItem, destinationFolder));
            return result;
        }

        public static Func<AutoProcessItemArgs, ClipboardItem?> RunPythonAction(ScriptItem item) {
            return (args) => {
                RunPythonScriptCommandExecute(item, args.ClipboardItem);
                return args.ClipboardItem;
            };

        }

        // 自動実行でPythonスクリプトを実行するコマンド
        public static void RunPythonScriptCommandExecute(ScriptItem scriptItem, ClipboardItem clipboardItem) {
            string inputJson = ClipboardItem.ToJson(clipboardItem);

            string result = PythonExecutor.PythonFunctions.RunScript(scriptItem.Content, inputJson);
            ClipboardItem? resultItem = ClipboardItem.FromJson(result, (message) => {
                LogWrapper.Info("Pythonスクリプトを実行しました");

            });
            resultItem?.CopyTo(clipboardItem);

        }
    }

}
