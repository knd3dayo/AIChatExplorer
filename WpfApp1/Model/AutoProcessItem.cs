using System.Collections.ObjectModel;
using System.IO;
using LiteDB;
using WK.Libraries.SharpClipboardNS;
using WpfApp1.Utils;

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
        public enum ActionType {
            CopyToFolder,
            MoveToFolder,
            ExtractText,
            MaskData,
            SplitPathToFolderAndFileName,
            RunPythonScript,
        }

        public ObjectId? Id { get; set; }
        public string Name { get; set; } = "";
        public ActionType Type { get; set; }

        public string Description { get; set; } = "";
        public ScriptItem? ScriptItem { get; set; }

        public AutoProcessItem() {
        }
        public AutoProcessItem(string name, string description, ActionType actionType) : this() {
            Name = name;
            Description = description;
            Type = actionType;
        }

        public static ObservableCollection<AutoProcessItem> AutoProcessItemTemplates {
            get {
                ObservableCollection<AutoProcessItem> items = new ObservableCollection<AutoProcessItem>();

                // itemにフォルダにコピーするコマンドを追加
                items.Add(
                    new AutoProcessItem(
                    "フォルダにコピー", "クリップボードの内容を指定されたフォルダにコピーします", ActionType.CopyToFolder)
                    );

                // itemにフォルダに移動するコマンドを追加
                items.Add(
                    new AutoProcessItem(
                    "フォルダに移動", "クリップボードの内容を指定されたフォルダに移動します", ActionType.MoveToFolder)
                    );

                // itemにテキスト抽出コマンドを追加
                items.Add(
                    new AutoProcessItem(
                        "テキスト抽出", "クリップボードのテキストを抽出します", ActionType.ExtractText)
                    );
                // itemにデータマスキングコマンドを追加
                items.Add(
                    new AutoProcessItem("データマスキング", "クリップボードのテキストをマスキングします", ActionType.ExtractText)
                    );

                // ファイルパスをフォルダ名とファイル名に分割するコマンドを追加
                items.Add(
                    new AutoProcessItem(
                        "コピーしたファイルをフォルダパスとファイル名に分割", "コピーしたファイルをフォルダパスとファイル名に分割",
                        ActionType.SplitPathToFolderAndFileName)
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
                    items.Add(
                        new AutoProcessItem(scriptItem.Description, $"Pythonスクリプト{scriptItem.Description}を実行します", ActionType.RunPythonScript));
                }
                return items;
            }
        }

        // LIteDBにはFuncを保存できないので、AutoProcessItemにはActionTypeを保存し、ActionTypeごとのFuncを返す、staticメソッドを作成

        public static Func<AutoProcessItemArgs, ClipboardItem?> GetAction(AutoProcessItem autoProcessItem) {
            switch (autoProcessItem.Type) {
                case AutoProcessItem.ActionType.CopyToFolder:
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
                case AutoProcessItem.ActionType.MoveToFolder:
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
                case AutoProcessItem.ActionType.ExtractText:
                    return (args) => {
                        return PythonCommands.AutoExtractTextCommandExecute(args.ClipboardItem);
                    };
                case AutoProcessItem.ActionType.MaskData:
                    return (args) => {
                        return PythonCommands.AutoMaskDataCommandExecute(args.ClipboardItem);
                    };
                case AutoProcessItem.ActionType.SplitPathToFolderAndFileName:
                    return (args) => {
                        // Contentがファイルの場合に処理を行う
                        if (args.ClipboardItem.ContentType == SharpClipboard.ContentTypes.Files) {
                            string path = args.ClipboardItem.Content;

                            // ファイルパスをフォルダ名とファイル名に分割
                            string? folderPath = Path.GetDirectoryName(path);
                            if (folderPath == null) {
                                Tools.Warn("フォルダパスが取得できません");
                                return args.ClipboardItem;
                            }
                            string? fileName = Path.GetFileName(path);
                            args.ClipboardItem.Content = folderPath + "\n" + fileName;
                            // ContentTypeをTextに変更
                            args.ClipboardItem.ContentType = SharpClipboard.ContentTypes.Text;
                        }
                        return args.ClipboardItem;
                    };
                case AutoProcessItem.ActionType.RunPythonScript:
                    return (args) => {
                        if (autoProcessItem.ScriptItem == null) {
                            Tools.Warn("スクリプトが選択されていません");
                            return args.ClipboardItem;
                        }
                        return PythonCommands.AutoRunPythonScriptCommandExecute(autoProcessItem.ScriptItem, args.ClipboardItem);
                    };
                default:
                    return (args) => {
                        return args.ClipboardItem;
                    };
            }
        }

        public ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardItemFolder? destinationFolder) {
            Func<AutoProcessItemArgs, ClipboardItem?> action = GetAction(this);
            ClipboardItem? result = action(new AutoProcessItemArgs(clipboardItem, destinationFolder));
            return result;
        }
    }

}
