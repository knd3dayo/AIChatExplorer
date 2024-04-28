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

    public partial class AutoProcessItem {
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
            Name = AutoProcessActionName.RunPythonScript.Name;
            DisplayName = displayName;
            Description = description;
            ScriptItem = scriptItem;
        }

        public bool IsCopyOrMoveOrMergeAction() {
            return Name == AutoProcessActionName.CopyToFolder.Name || Name == AutoProcessActionName.MoveToFolder.Name
                || Name == AutoProcessActionName.MergeAllItems.Name || Name == AutoProcessActionName.MergeItemsWithSameSourceApplicationTitle.Name;
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
            List<ScriptItem> items = [.. ClipboardAppFactory.Instance.GetClipboardDBController().GetScriptItems()];
            List<AutoProcessItem> result = new List<AutoProcessItem>();
            foreach (var item in items) {
                result.Add(new AutoProcessItem(item.Description, $"Pythonスクリプト{item.Description}を実行します", item));
            }
            return result;

        }

        public static ObservableCollection<AutoProcessItem> SystemAutoProcesses {
            get {
                ObservableCollection<AutoProcessItem> items = new ObservableCollection<AutoProcessItem>();

                // itemにフォルダにコピーするコマンドを追加
                items.Add(
                    new AutoProcessItem(AutoProcessActionName.CopyToFolder.Name, "フォルダにコピー", "クリップボードの内容を指定されたフォルダにコピーします")
                    );


                // itemにフォルダに移動するコマンドを追加
                items.Add(
                    new AutoProcessItem(AutoProcessActionName.MoveToFolder.Name, "フォルダに移動", "クリップボードの内容を指定されたフォルダに移動します")
                    );

                // itemにテキスト抽出コマンドを追加
                items.Add(
                    new AutoProcessItem(AutoProcessActionName.ExtractText.Name, "テキスト抽出", "クリップボードのテキストを抽出します")
                    );
                // itemにデータマスキングコマンドを追加
                // UseSpacyがTrueの場合のみ追加
                if (ClipboardAppConfig.UseSpacy) {
                    items.Add(
                        new AutoProcessItem(AutoProcessActionName.MaskData.Name, "データマスキング", "クリップボードのテキストをマスキングします")
                        );
                }
                // ファイルパスをフォルダ名とファイル名に分割するコマンドを追加
                items.Add(
                    new AutoProcessItem(AutoProcessActionName.SplitPathToFolderAndFileName.Name, "コピーしたファイルをフォルダパスとファイル名に分割", "コピーしたファイルをフォルダパスとファイル名に分割")
                    );
                // フォルダ内のアイテムを自動的にマージするコマンドを追加
                items.Add(
                    new AutoProcessItem(AutoProcessActionName.MergeAllItems.Name, "フォルダ内のアイテムをマージ", "フォルダ内のアイテムをマージします")
                    );
                // 同じSourceApplicationTitleを持つアイテムをマージするコマンドを追加
                items.Add(
                    new AutoProcessItem(AutoProcessActionName.MergeItemsWithSameSourceApplicationTitle.Name, "同じSourceApplicationTitleを持つアイテムをマージ", "同じSourceApplicationTitleを持つアイテムをマージします")
                    );


                var scriptItems = ClipboardAppFactory.Instance.GetClipboardDBController().GetScriptItems();
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
            if (name == AutoProcessActionName.CopyToFolder.Name) {
                return (args) => {
                    if (args.DestinationFolder == null) {
                        Tools.Warn("フォルダが選択されていません");
                        return args.ClipboardItem;
                    }
                    Tools.Info($"フォルダにコピーします{args.DestinationFolder.AbsoluteCollectionName}");
                    // DestinationFolderにコピー
                    ClipboardItem newItem = args.ClipboardItem.Copy();
                    args.DestinationFolder.AddItem(newItem, (actionMessage) => { });
                    // コピーの場合は元のアイテムを返す
                    return args.ClipboardItem;
                };
            }
            if (name == AutoProcessActionName.MoveToFolder.Name) {
                return (args) => {
                    if (args.DestinationFolder == null) {
                        Tools.Warn("フォルダが選択されていません");
                        return args.ClipboardItem;
                    }
                    // DestinationFolderに追加
                    ClipboardItem newItem = args.ClipboardItem.Copy();
                    ClipboardItem result = args.DestinationFolder.AddItem(newItem, (actionMessage) => { });
                    // 元のフォルダから削除
                    Tools.Info($"{args.ClipboardItem.CollectionName}から削除します");

                    args.ClipboardItem.Delete();

                    // Moveの場合は元のアイテムを返さない
                    return null;
                };
            }
            if (name == AutoProcessActionName.ExtractText.Name) {
                return (args) => {
                    return ClipboardItem.ExtractTextCommandExecute(args.ClipboardItem);
                };
            }
            if (name == AutoProcessActionName.MaskData.Name) {
                return (args) => {
                    return ClipboardItem.MaskDataCommandExecute(args.ClipboardItem);
                };
            }
            if (name == AutoProcessActionName.SplitPathToFolderAndFileName.Name) {
                return (args) => {
                    ClipboardItem.SplitFilePathCommandExecute(args.ClipboardItem);
                    return args.ClipboardItem;
                };
            }
            if (name == AutoProcessActionName.MergeAllItems.Name) {
                return (args) => {
                    ClipboardFolder folder = args.DestinationFolder ?? throw new ThisApplicationException("フォルダが選択されていません");

                    ClipboardFolder.MergeItemsCommandExecute(folder, args.ClipboardItem);
                    return args.ClipboardItem;
                };
            }
            if (name == AutoProcessActionName.MergeItemsWithSameSourceApplicationTitle.Name) {
                return (args) => {
                    ClipboardFolder folder = args.DestinationFolder ?? throw new ThisApplicationException("フォルダが選択されていません");
                    ClipboardFolder.MergeItemsBySourceApplicationTitleCommandExecute(folder, args.ClipboardItem);
                    return args.ClipboardItem;
                };
            }
            return (args) => {
                return args.ClipboardItem;
            };
        }

        public static Func<AutoProcessItemArgs, ClipboardItem?> RunPythonAction(ScriptItem item) {
            return (args) => {
                RunPythonScriptCommandExecute(item, args.ClipboardItem);
                return args.ClipboardItem;
            };

        }
        public ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder) {
            // NameがSys

            Func<AutoProcessItemArgs, ClipboardItem?> action = GetSystemAction(this.Name);
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
