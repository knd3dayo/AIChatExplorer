using System.Collections.ObjectModel;
using WpfApp1.Utils;
using LiteDB;
using WK.Libraries.SharpClipboardNS;

namespace WpfApp1.Model {
    // 自動処理の引数用のクラス
    public class AutoProcessItemArgs {

        public ClipboardItem ClipboardItem { get; set; }
        public ClipboardItemFolder? DestinationFolder { get; set; }

        public AutoProcessItemArgs(ClipboardItem clipboardItem, ClipboardItemFolder? destinationFolder) {
            ClipboardItem = clipboardItem;
            DestinationFolder = destinationFolder;
        }
    }

    public class AutoProcessItem {
        // System処理の名前
        public class ActionName {
            public bool IsSystemAction { get; set;}
            public string Name { get; set; }
           
            public  ActionName(bool isSystemAction, string name) {
                IsSystemAction = isSystemAction;
                Name = name;
            }

            public static ActionName CopyToFolder = new ActionName(true, "CopyToFolder");
            public static ActionName MoveToFolder = new ActionName(true, "MoveToFolder");
            public static ActionName ExtractText = new ActionName(true, "ExtractText");
            public static ActionName MaskData = new ActionName(true, "GetMaskedString");
            public static ActionName SplitPathToFolderAndFileName = new ActionName(true, "SplitPathToFolderAndFileName");

            public static ActionName RunPythonScript = new ActionName(false, "RunPythonScript");

        }
        public ObjectId? Id { get; set; }
        public string Name { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public string Description { get; set; } = "";
        public ScriptItem? ScriptItem { get; set; }

        public AutoProcessItem() {
        }

        // システムデフォルトのAutoProcessItemを作成
        private AutoProcessItem(string name, string displayName, string description) : this() {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }
        // ユーザーが作成したスクリプトのAutoProcessItemを作成
        public AutoProcessItem(string displayName, string description, ScriptItem scriptItem) : this() {
            Name = ActionName.RunPythonScript.Name;
            DisplayName = displayName;
            Description = description;
            ScriptItem = scriptItem;
        }

        public bool IsCopyOrMoveAction() {
            return Name == ActionName.CopyToFolder.Name || Name == ActionName.MoveToFolder.Name;
        }

        public static AutoProcessItem GetSystemAutoProcessItem(string name) {
            // システムデフォルトのAutoProcessItemを取得
            foreach (var item in SystemAutoProcesses) {
                if (item.Name == name) {
                    return item;
                }
            }
            throw new ThisApplicationException("AutoProcessItemが見つかりません");
        }

        public static List<AutoProcessItem> GetScriptAutoProcessItems() {
            // DBからスクリプトのScriptItemを取得
            List<ScriptItem> items = ClipboardDatabaseController.GetScriptItems();
            List<AutoProcessItem> result = new List<AutoProcessItem>();
            foreach (var item in items) {
                result.Add(new AutoProcessItem(item.Description, $"Pythonスクリプト{item.Description}を実行します", item));
            }
            return result;

        }

        private static ObservableCollection<AutoProcessItem> SystemAutoProcesses {
            get {
                ObservableCollection<AutoProcessItem> items = new ObservableCollection<AutoProcessItem>();

                // itemにフォルダにコピーするコマンドを追加
                items.Add(
                    new AutoProcessItem(ActionName.CopyToFolder.Name, "フォルダにコピー", "クリップボードの内容を指定されたフォルダにコピーします")
                    );


                // itemにフォルダに移動するコマンドを追加
                items.Add(
                    new AutoProcessItem(ActionName.MoveToFolder.Name,"フォルダに移動", "クリップボードの内容を指定されたフォルダに移動します")
                    );

                // itemにテキスト抽出コマンドを追加
                items.Add(
                    new AutoProcessItem(ActionName.ExtractText.Name,"テキスト抽出", "クリップボードのテキストを抽出します")
                    );
                // itemにデータマスキングコマンドを追加
                items.Add(
                    new AutoProcessItem(ActionName.MaskData.Name,"データマスキング", "クリップボードのテキストをマスキングします")
                    );

                // ファイルパスをフォルダ名とファイル名に分割するコマンドを追加
                items.Add(
                    new AutoProcessItem(ActionName.SplitPathToFolderAndFileName.Name, "コピーしたファイルをフォルダパスとファイル名に分割", "コピーしたファイルをフォルダパスとファイル名に分割")
                    );

                var scriptItems = ClipboardController.GetScriptItems();
                // スクリプトを追加
                foreach (var scriptItem in scriptItems) {
                    if (scriptItem.Type != ScriptType.Python) {
                        continue;
                    }
                    if (scriptItem.Description == null) {
                        continue;
                    }
                    if (scriptItem.Content == null) {
                        continue;
                    }
                    items.Add(new AutoProcessItem(scriptItem.Description, $"Pythonスクリプト{scriptItem.Description}を実行します", scriptItem));
                }
                return items;
            }
        }

        // LIteDBにはFuncを保存できないので、AutoProcessItemにはActionTypeを保存し、ActionTypeごとのFuncを返す、staticメソッドを作成

        public static Func<AutoProcessItemArgs, ClipboardItem?> GetSystemAction(string name) {
            if (name == ActionName.CopyToFolder.Name) {
                return (args) => {
                    if (args.DestinationFolder == null) {
                        Tools.Warn("フォルダが選択されていません");
                        return args.ClipboardItem;
                    }
                    Tools.Info($"フォルダにコピーします{args.DestinationFolder.AbsoluteCollectionName}");
                    // DestinationFolderにコピー
                    ClipboardItem newItem = args.ClipboardItem.Copy();
                    args.DestinationFolder.AddItem(newItem);
                    // コピーの場合は元のアイテムを返す
                    return args.ClipboardItem;
                };
            }
            if (name == ActionName.MoveToFolder.Name) {
                return (args) => {
                    if (args.DestinationFolder == null) {
                        Tools.Warn("フォルダが選択されていません");
                        return args.ClipboardItem;
                    }
                    // DestinationFolderに追加
                    ClipboardItem newItem = args.ClipboardItem.Copy();
                    ClipboardItem result = args.DestinationFolder.AddItem(newItem);
                    // 元のフォルダから削除
                    Tools.Info($"{args.ClipboardItem.CollectionName}から削除します");
                    ClipboardDatabaseController.DeleteItem(args.ClipboardItem);
                    // Moveの場合は元のアイテムを返さない
                    return null;
                };
            }
            if (name == ActionName.ExtractText.Name) { 
                    return (args) => {
                        return AutoProcessCommand.ExtractTextCommandExecute(args.ClipboardItem);
                    };
                }
            if (name == ActionName.MaskData.Name) {
                return (args) => {
                    return AutoProcessCommand.MaskDataCommandExecute(args.ClipboardItem);
                };
            }
            if (name == ActionName.SplitPathToFolderAndFileName.Name) {
                return (args) => {
                    AutoProcessCommand.SplitFilePathCommandExecute(args.ClipboardItem);
                    return args.ClipboardItem;
                };
            }
            return (args) => {
                return args.ClipboardItem;
            };
        }

        public static Func<AutoProcessItemArgs, ClipboardItem?> RunPythonAction(ScriptItem item) {
            return (args) => {
                AutoProcessCommand.RunPythonScriptCommandExecute(item, args.ClipboardItem);
                return args.ClipboardItem;
            };

        }
        public ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardItemFolder? destinationFolder) {
            // NameがSys

            Func<AutoProcessItemArgs, ClipboardItem?> action = GetSystemAction(this.Name);
            ClipboardItem? result = action(new AutoProcessItemArgs(clipboardItem, destinationFolder));
            return result;
        }

    }

}
