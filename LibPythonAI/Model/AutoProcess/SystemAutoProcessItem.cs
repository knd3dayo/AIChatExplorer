using LibPythonAI.Utils.Common;
using PythonAILib.Model.Content;
using PythonAILib.Resources;

namespace PythonAILib.Model.AutoProcess {

    public class SystemAutoProcessItem : AutoProcessItem {
        public SystemAutoProcessItem() {
        }

        // システムデフォルトのAutoProcessItemを作成
        public SystemAutoProcessItem(string name, string displayName, string description) : this() {
            Name = name;
            DisplayName = displayName;
            Description = description;
        }

        public bool IsCopyOrMoveAction() {
            return Name == TypeEnum.CopyToFolder.ToString() || Name == TypeEnum.MoveToFolder.ToString();
        }

        public virtual void Execute(ContentItemWrapper clipboardItem, ContentFolderWrapper? destinationFolder) {
            // DestinationFolderIdからFolderを取得
            ContentFolder? folder = ContentFolder.GetFolderById<ContentFolder>(DestinationFolderId);
            ContentFolderWrapper? destinationFolderWrapper = folder == null ? null : new ContentFolderWrapper(folder);
            Action<ContentItemWrapper> action = GetAction(TypeName, destinationFolderWrapper);
            action(clipboardItem);

        }

        public static List<SystemAutoProcessItem> SystemAutoProcesses {
            get {
                List<SystemAutoProcessItem> items =
                [
                    // 無視するコマンドを追加
                    new SystemAutoProcessItem("Ignore", PythonAILibStringResources.Instance.Ignore, PythonAILibStringResources.Instance.DoNothing),
                    // itemにフォルダにコピーするコマンドを追加
                    new SystemAutoProcessItem("CopyToFolder", PythonAILibStringResources.Instance.CopyToFolder, PythonAILibStringResources.Instance.CopyClipboardContentToSpecifiedFolder),
                    // itemにフォルダに移動するコマンドを追加
                    new SystemAutoProcessItem("MoveToFolder",  PythonAILibStringResources.Instance.MoveToFolder, PythonAILibStringResources.Instance.MoveClipboardContentToSpecifiedFolder),
                    // itemにテキスト抽出コマンドを追加
                    new SystemAutoProcessItem("ExtractText", PythonAILibStringResources.Instance.ExtractText, PythonAILibStringResources.Instance.ExtractClipboardText),
                ];

                /** ★TODO Implement processing based on automatic processing rules.
                // itemにデータマスキングコマンドを追加
                // UseSpacyがTrueの場合のみ追加
                if (ClipboardAppConfig.Instance.UseSpacy) {
                    items.Add(
                        new SystemAutoProcessItem("MaskData", PythonAILibStringResources.Instance.DataMasking, PythonAILibStringResources.Instance.MaskClipboardText)
                        );
                }
                // フォルダ内のアイテムを自動的にマージするコマンドを追加
                items.Add(
                    new SystemAutoProcessItem("MergeAllItems", PythonAILibStringResources.Instance.MergeItemsInFolder, PythonAILibStringResources.Instance.MergeItemsInFolderDescription)
                    );
                // 同じSourceApplicationTitleを持つアイテムをマージするコマンドを追加
                items.Add(
                    new SystemAutoProcessItem("MergeItemsWithSameSourceApplicationTitle",
                        PythonAILibStringResources.Instance.MergeItemsWithTheSameSourceApplicationTitle,
                        PythonAILibStringResources.Instance.MergeItemsWithTheSameSourceApplicationTitleDescription)
                    );

                **/

                return items;
            }
        }

        public static Action<ContentItemWrapper> GetAction(TypeEnum typeEnum, ContentFolderWrapper? destinationFolder) {
            if (typeEnum == TypeEnum.Ignore) {
                return (args) => {
                    return;
                };
            }
            if (typeEnum == TypeEnum.CopyToFolder) {
                return (args) => {
                    if (destinationFolder == null) {
                        LogWrapper.Warn(PythonAILibStringResources.Instance.NoFolderSelected);
                        return;
                    }

                    LogWrapper.Info($"{PythonAILibStringResources.Instance.CopyToFolderDescription}:{destinationFolder.ContentFolderPath}");
                    ContentItemWrapper newItem = args.Copy();
                    // Folderに追加
                    destinationFolder.AddItem(newItem);
                };
            }
            if (typeEnum == TypeEnum.MoveToFolder) {
                return (args) => {
                    if (destinationFolder == null) {
                        LogWrapper.Warn(PythonAILibStringResources.Instance.NoFolderSelected);
                        return;
                    }
                    // Folderに追加
                    ContentItemWrapper newItem = args.Copy();
                    destinationFolder.AddItem(newItem);
                    args.Delete();

                };
            }
            if (typeEnum == TypeEnum.ExtractText) {
                return (args) => {
                    List<ContentItemWrapper> contentItemWrappers = [args];
                    ContentItemCommands.ExtractTexts(contentItemWrappers, () => { }, () => { });
                };
            }

            return (args) => {
                return;
            };
        }


    }

}
