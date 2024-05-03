using System.Collections.ObjectModel;
using LiteDB;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    // 自動処理の引数用のクラス
    public class AutoProcessItemArgs {

        public ClipboardItem ClipboardItem { get; set; }
        public ClipboardFolder? DestinationFolder { get; set; }

        public AutoProcessItemArgs(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder) {
            ClipboardItem = clipboardItem;
            DestinationFolder = destinationFolder;
        }
    }

    public partial class SystemAutoProcessItem {
        public enum TypeEnum {
            CopyToFolder,
            MoveToFolder,
            ExtractText,
            MaskData,
            SplitPathToFolderAndFileName,
            MergeAllItems,
            MergeItemsWithSameSourceApplicationTitle,
            RunPythonScript
        }
        public ObjectId? Id { get; set; } = LiteDB.ObjectId.Empty;
        public string Name { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public string Description { get; set; } = "";
        public ScriptItem? ScriptItem { get; set; }

        public TypeEnum Type { get; set; } = TypeEnum.CopyToFolder;

        public SystemAutoProcessItem() {
        }

        // システムデフォルトのAutoProcessItemを作成
        public SystemAutoProcessItem(string name, string displayName, string description) : this() {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }
        // ユーザーが作成したスクリプトのAutoProcessItemを作成
        public SystemAutoProcessItem(string displayName, string description, ScriptItem scriptItem) : this() {
            Name = AutoProcessActionName.RunPythonScript.Name;
            DisplayName = displayName;
            Description = description;
            ScriptItem = scriptItem;
        }

        public bool IsCopyOrMoveOrMergeAction() {
            return Name == AutoProcessActionName.CopyToFolder.Name || Name == AutoProcessActionName.MoveToFolder.Name
                || Name == AutoProcessActionName.MergeAllItems.Name || Name == AutoProcessActionName.MergeItemsWithSameSourceApplicationTitle.Name;
        }

        public static SystemAutoProcessItem GetSystemAutoProcessItem(string name) {
            // システムデフォルトのAutoProcessItemを取得
            foreach (var item in AutoProcessItemSystemActions.SystemAutoProcesses) {
                if (item.Name == name) {
                    return item;
                }
            }
            throw new ThisApplicationException("AutoProcessItemが見つかりません");
        }

        public static List<SystemAutoProcessItem> GetScriptAutoProcessItems() {
            // DBからスクリプトのScriptItemを取得
            List<ScriptItem> items = [.. ClipboardAppFactory.Instance.GetClipboardDBController().GetScriptItems()];
            List<SystemAutoProcessItem> result = [];
            foreach (var item in items) {
                result.Add(new SystemAutoProcessItem(item.Description, $"Pythonスクリプト{item.Description}を実行します", item));
            }
            return result;

        }


        public static Func<AutoProcessItemArgs, ClipboardItem?> RunPythonAction(ScriptItem item) {
            return (args) => {
                RunPythonScriptCommandExecute(item, args.ClipboardItem);
                return args.ClipboardItem;
            };

        }
        public ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder) {
            // NameがSys

            Func<AutoProcessItemArgs, ClipboardItem?> action = AutoProcessItemSystemActions.GetSystemAction(this.Name);
            ClipboardItem? result = action(new AutoProcessItemArgs(clipboardItem, destinationFolder));
            return result;
        }

        // 自動実行でPythonスクリプトを実行するコマンド
        public static void RunPythonScriptCommandExecute(ScriptItem scriptItem, ClipboardItem clipboardItem) {
            string inputJson = ClipboardItem.ToJson(clipboardItem);

            string result = PythonExecutor.PythonFunctions.RunScript(scriptItem.Content, inputJson);
            ClipboardItem? resultItem = ClipboardItem.FromJson(result, (message) => {
                Tools.Info("Pythonスクリプトを実行しました");

            });
            resultItem?.CopyTo(clipboardItem);

        }
    }

}
