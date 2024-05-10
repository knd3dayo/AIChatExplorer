using WpfAppCommon.Utils;
using static WpfAppCommon.Model.SystemAutoProcessItem;

namespace WpfAppCommon.Model {
    public class AutoProcessItemSystemActions {

        public static List<SystemAutoProcessItem> SystemAutoProcesses {
            get {
                List<SystemAutoProcessItem> items =
                [
                    // itemにフォルダにコピーするコマンドを追加
                    new SystemAutoProcessItem("CopyToFolder", "フォルダにコピー", "クリップボードの内容を指定されたフォルダにコピーします")
,
                    // itemにフォルダに移動するコマンドを追加
                    new SystemAutoProcessItem("MoveToFolder", "フォルダに移動", "クリップボードの内容を指定されたフォルダに移動します")
,
                    // itemにテキスト抽出コマンドを追加
                    new SystemAutoProcessItem("ExtractText", "テキスト抽出", "クリップボードのテキストを抽出します")
,
                ];
                // itemにデータマスキングコマンドを追加
                // UseSpacyがTrueの場合のみ追加
                if (ClipboardAppConfig.UseSpacy) {
                    items.Add(
                        new SystemAutoProcessItem("MaskData", "データマスキング", "クリップボードのテキストをマスキングします")
                        );
                }
                // ファイルパスをフォルダ名とファイル名に分割するコマンドを追加
                items.Add(
                    new SystemAutoProcessItem("SplitPathToFolderAndFileName", "コピーしたファイルをフォルダパスとファイル名に分割", "コピーしたファイルをフォルダパスとファイル名に分割")
                    );
                // フォルダ内のアイテムを自動的にマージするコマンドを追加
                items.Add(
                    new SystemAutoProcessItem("MergeAllItems", "フォルダ内のアイテムをマージ", "フォルダ内のアイテムをマージします")
                    );
                // 同じSourceApplicationTitleを持つアイテムをマージするコマンドを追加
                items.Add(
                    new SystemAutoProcessItem("MergeItemsWithSameSourceApplicationTitle", "同じSourceApplicationTitleを持つアイテムをマージ", "同じSourceApplicationTitleを持つアイテムをマージします")
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
                    items.Add(new SystemAutoProcessItem(scriptItem.Description, $"Pythonスクリプト{scriptItem.Description}を実行します", scriptItem));
                }
                return items;
            }
        }

        public static Func<AutoProcessItemArgs, ClipboardItem?> GetSystemAction(string name) {
            if (name == TypeEnum.CopyToFolder.ToString()) {
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
            if (name == TypeEnum.MoveToFolder.ToString()) {
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

                    args.ClipboardItem.Delete();

                    // Moveの場合は元のアイテムを返さない
                    return null;
                };
            }
            if (name == TypeEnum.ExtractText.ToString()) {
                return (args) => {
                    return ClipboardItem.ExtractTextCommandExecute(args.ClipboardItem);
                };
            }
            if (name == TypeEnum.MaskData.ToString()) {
                return (args) => {
                    return args.ClipboardItem.MaskDataCommandExecute();
                };
            }
            if (name == TypeEnum.SplitPathToFolderAndFileName.ToString()) {
                return (args) => {
                    args.ClipboardItem.SplitFilePathCommandExecute();
                    return args.ClipboardItem;
                };
            }
            if (name == TypeEnum.MergeAllItems.ToString()) {
                return (args) => {
                    ClipboardFolder folder = args.DestinationFolder ?? throw new ThisApplicationException("フォルダが選択されていません");

                    folder.MergeItemsCommandExecute(args.ClipboardItem);
                    return args.ClipboardItem;
                };
            }
            if (name == TypeEnum.MergeItemsWithSameSourceApplicationTitle.ToString()) {
                return (args) => {
                    ClipboardFolder folder = args.DestinationFolder ?? throw new ThisApplicationException("フォルダが選択されていません");
                    folder.MergeItemsBySourceApplicationTitleCommandExecute(args.ClipboardItem);
                    return args.ClipboardItem;
                };
            }
            return (args) => {
                return args.ClipboardItem;
            };
        }


    }
}
