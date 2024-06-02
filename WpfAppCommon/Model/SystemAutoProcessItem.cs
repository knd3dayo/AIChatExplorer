using LiteDB;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Model {
    public partial class SystemAutoProcessItem {
        public enum TypeEnum {
            CopyToFolder,
            MoveToFolder,
            ExtractText,
            MaskData,
            SplitPathToFolderAndFileName,
            MergeAllItems,
            MergeItemsWithSameSourceApplicationTitle,
            RunPythonScript,
            PromptTemplate,
        }
        public ObjectId? Id { get; set; } = LiteDB.ObjectId.Empty;
        public string Name { get; set; } = "";

        public string DisplayName { get; set; } = "";

        public string Description { get; set; } = "";

        public TypeEnum Type { get; set; } = TypeEnum.CopyToFolder;

        public SystemAutoProcessItem() {
        }

        // システムデフォルトのAutoProcessItemを作成
        public SystemAutoProcessItem(string name, string displayName, string description) : this() {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }

        public bool IsCopyOrMoveOrMergeAction() {
            return Name == TypeEnum.CopyToFolder.ToString() || Name == TypeEnum.MoveToFolder.ToString()
                || Name == TypeEnum.MergeAllItems.ToString() || Name == TypeEnum.MergeItemsWithSameSourceApplicationTitle.ToString();
        }

        public static SystemAutoProcessItem GetSystemAutoProcessItem(string name) {
            // システムデフォルトのAutoProcessItemを取得
            foreach (var item in SystemAutoProcessItem.SystemAutoProcesses) {
                if (item.Name == name) {
                    return item;
                }
            }
            throw new ThisApplicationException("AutoProcessItemが見つかりません");
        }

        public virtual ClipboardItem? Execute(ClipboardItem clipboardItem, ClipboardFolder? destinationFolder) {

            Func<AutoProcessItemArgs, ClipboardItem?> action = SystemAutoProcessItem.GetAction(this.Name);
            ClipboardItem? result = action(new AutoProcessItemArgs(clipboardItem, destinationFolder));
            return result;
        }

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
                return items;
            }
        }

        public static Func<AutoProcessItemArgs, ClipboardItem?> GetAction(string name) {
            if (name == TypeEnum.CopyToFolder.ToString()) {
                return (args) => {
                    if (args.DestinationFolder == null) {
                        Tools.Warn("フォルダが選択されていません");
                        return args.ClipboardItem;
                    }
                    Tools.Info($"フォルダにコピーします{args.DestinationFolder.CollectionName}");
                    // DestinationFolderをDBから取得
                    ClipboardFolder folder = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(args.DestinationFolder.CollectionName);

                    ClipboardItem newItem = args.ClipboardItem.Copy();

                    // Folderに追加
                    folder.AddItem(newItem);

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
                    // DestinationFolderをDBから取得
                    ClipboardFolder folder = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(args.DestinationFolder.CollectionName);

                    // Folderに追加
                    ClipboardItem newItem = args.ClipboardItem.Copy();
                    ClipboardItem result = folder.AddItem(newItem);
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
                    // DestinationFolderをDBから取得
                    folder = ClipboardAppFactory.Instance.GetClipboardDBController().GetFolder(args.DestinationFolder.CollectionName);

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
