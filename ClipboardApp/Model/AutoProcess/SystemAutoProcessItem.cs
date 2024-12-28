using LiteDB;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Model.Content;
using QAChat.Resource;
using WpfAppCommon.Utils;

namespace ClipboardApp.Model.AutoProcess {
    // TODO 多言語化
    public partial class SystemAutoProcessItem {
        public enum TypeEnum {
            Ignore,
            CopyToFolder,
            MoveToFolder,
            ExtractText,
            MergeAllItems,
            MergeItemsWithSameSourceApplicationTitle,
            RunPythonScript,
            PromptTemplate,
        }
        public ObjectId? Id { get; set; } = ObjectId.Empty;
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
            foreach (var item in SystemAutoProcesses) {
                if (item.Name == name) {
                    return item;
                }
            }
            throw new Exception(CommonStringResources.Instance.AutoProcessItemNotFound);
        }

        public virtual ContentItem? Execute(ContentItem clipboardItem, ContentFolder? destinationFolder) {

            Func<AutoProcessItem, ContentItem?> action = GetAction(Name);
            ContentItem? result = action(new AutoProcessItem(clipboardItem, destinationFolder));
            return result;
        }

        public static List<SystemAutoProcessItem> SystemAutoProcesses {
            get {
                List<SystemAutoProcessItem> items =
                [
                    // 無視するコマンドを追加
                    new SystemAutoProcessItem("Ignore", CommonStringResources.Instance.Ignore, CommonStringResources.Instance.DoNothing),
                    // itemにフォルダにコピーするコマンドを追加
                    new SystemAutoProcessItem("CopyToFolder", CommonStringResources.Instance.CopyToFolder, CommonStringResources.Instance.CopyClipboardContentToSpecifiedFolder),
                    // itemにフォルダに移動するコマンドを追加
                    new SystemAutoProcessItem("MoveToFolder",  CommonStringResources.Instance.MoveToFolder, CommonStringResources.Instance.MoveClipboardContentToSpecifiedFolder),
                    // itemにテキスト抽出コマンドを追加
                    new SystemAutoProcessItem("ExtractText", CommonStringResources.Instance.ExtractText, CommonStringResources.Instance.ExtractClipboardText),
                ];
                // itemにデータマスキングコマンドを追加
                // UseSpacyがTrueの場合のみ追加
                if (ClipboardAppConfig.Instance.UseSpacy) {
                    items.Add(
                        new SystemAutoProcessItem("MaskData", CommonStringResources.Instance.DataMasking, CommonStringResources.Instance.MaskClipboardText)
                        );
                }
                // フォルダ内のアイテムを自動的にマージするコマンドを追加
                items.Add(
                    new SystemAutoProcessItem("MergeAllItems", CommonStringResources.Instance.MergeItemsInFolder, CommonStringResources.Instance.MergeItemsInFolderDescription)
                    );
                // 同じSourceApplicationTitleを持つアイテムをマージするコマンドを追加
                items.Add(
                    new SystemAutoProcessItem("MergeItemsWithSameSourceApplicationTitle",
                        CommonStringResources.Instance.MergeItemsWithTheSameSourceApplicationTitle,
                        CommonStringResources.Instance.MergeItemsWithTheSameSourceApplicationTitleDescription)
                    );
                return items;
            }
        }

        public static Func<AutoProcessItem, ContentItem?> GetAction(string name) {
            if (name == TypeEnum.Ignore.ToString()) {
                return (args) => {
                    return null;
                };
            }
            if (name == TypeEnum.CopyToFolder.ToString()) {
                return (args) => {
                    if (args.DestinationFolder == null) {
                        LogWrapper.Warn(CommonStringResources.Instance.NoFolderSelected);
                        return args.ContentItem;
                    }

                    LogWrapper.Info($"{CommonStringResources.Instance.CopyToFolderDescription}:{args.DestinationFolder.FolderPath}");
                    ClipboardItem newItem = (ClipboardItem)args.ContentItem.Copy();

                    // Folderに追加
                    args.DestinationFolder.AddItem(newItem);

                    // コピーの場合は元のアイテムを返す
                    return args.ContentItem;
                };
            }
            if (name == TypeEnum.MoveToFolder.ToString()) {
                return (args) => {
                    if (args.DestinationFolder == null) {
                        LogWrapper.Warn(CommonStringResources.Instance.NoFolderSelected);
                        return args.ContentItem;
                    }
                    // Folderに追加
                    ClipboardItem newItem = (ClipboardItem)args.ContentItem.Copy();
                    args.DestinationFolder.AddItem(newItem);

                    args.ContentItem.Delete();

                    // Moveの場合は元のアイテムを返さない
                    return null;
                };
            }
            if (name == TypeEnum.ExtractText.ToString()) {
                return (args) => {
                    ContentItem.ExtractTextCommandExecute(args.ContentItem);
                    return (ClipboardItem)args.ContentItem;
                };
            }
            if (name == TypeEnum.MergeAllItems.ToString()) {
                return (args) => {
                    ContentFolder folder = args.DestinationFolder ?? throw new Exception(CommonStringResources.Instance.NoFolderSelected);

                    folder.MergeItems(args.ContentItem);
                    return args.ContentItem;
                };
            }
            if (name == TypeEnum.MergeItemsWithSameSourceApplicationTitle.ToString()) {
                return (args) => {
                    ContentFolder folder = args.DestinationFolder ?? throw new Exception(CommonStringResources.Instance.NoFolderSelected);

                    folder.MergeItemsBySourceApplicationTitleCommandExecute(args.ContentItem);
                    return args.ContentItem;
                };
            }
            return (args) => {
                return args.ContentItem;
            };
        }


    }

}
