using LibPythonAI.Model.Content;
using LibPythonAI.PythonIF;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Model.AutoProcess {

    public enum AutoProcessItemTypeEnum {
        SystemDefined,
        ModifiedSystemDefined,
        UserDefined,
    }

    public enum AutoProcessActionTypeEnum {
        Ignore,
        CopyToFolder,
        MoveToFolder,
        ExtractText,
        PromptTemplate,
    }

    // 自動処理の引数用のクラス
    public class AutoProcessItem {

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public AutoProcessActionTypeEnum TypeName { get; set; } = AutoProcessActionTypeEnum.CopyToFolder;

        public AutoProcessItemTypeEnum ItemType { get; set; } = AutoProcessItemTypeEnum.UserDefined;

        private static bool _isInitialized = false;

        private static List<AutoProcessItem> _items = [];
        public static async Task LoadItemsAsync() {
            // PythonExecutor.PythonAIFunctions.GetAutoProcessItemsAsync() が非同期メソッドであれば直接awaitする
            _items = await PythonExecutor.PythonAIFunctions.GetAutoProcessItemsAsync();
            if (_items != null) {
                _isInitialized = true;
            }
        }

        public static async Task<List<AutoProcessItem>> GetItems() {
            if (!_isInitialized) {
                await LoadItemsAsync();
            }
            return _items;
        }

        public static List<AutoProcessItem> GetSystemDefinedItems() {
            return _items.Where(x => x.ItemType == AutoProcessItemTypeEnum.SystemDefined).ToList();
        }

        // GetItemById
        public static async Task<AutoProcessItem?> GetItemById(string? id) {
            var items = await GetItems();
            return items.FirstOrDefault(x => x.Id == id);
        }

        // SaveAsync
        public async Task SaveAsync() {
            await PythonExecutor.PythonAIFunctions.UpdateAutoProcessItemAsync(new AutoProcessItemRequest(this));
        }
        // DeleteAsync
        public async Task DeleteAsync() {
            await PythonExecutor.PythonAIFunctions.DeleteAutoProcessItemAsync(new AutoProcessItemRequest(this));
        }

        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "id", Id },
                { "display_name", DisplayName },
                { "description", Description },
                { "auto_process_item_type", (int)ItemType },
                { "action_type", (int)TypeName }
            };
            return dict;
        }
        // ToDictList
        public static List<Dictionary<string, object>> ToDictList(List<AutoProcessItem> autoProcessItems) {
            return autoProcessItems.Select(item => item.ToDict()).ToList();
        }

        // FromDict
        public static AutoProcessItem FromDict(Dictionary<string, object> dict) {
            AutoProcessItem item = new() {
                Id = dict["id"] as string ?? Guid.NewGuid().ToString(),
                DisplayName = dict["display_name"] as string ?? "",
                Description = dict["description"] as string ?? "",
                ItemType = (AutoProcessItemTypeEnum)(Decimal)(dict["auto_process_item_type"] ?? AutoProcessItemTypeEnum.UserDefined),
                TypeName = (AutoProcessActionTypeEnum)(Decimal)(dict["action_type"] ?? AutoProcessActionTypeEnum.CopyToFolder)
            };
            return item;
        }


        public Func<ContentItemWrapper, Task> GetAction(AutoProcessActionTypeEnum typeEnum, ContentFolderWrapper? destinationFolder) {
            if (typeEnum == AutoProcessActionTypeEnum.Ignore) {
                return (args) => Task.CompletedTask;
            }
            if (typeEnum == AutoProcessActionTypeEnum.CopyToFolder) {
                return async (args) => {
                    if (destinationFolder == null) {
                        LogWrapper.Warn(PythonAILibStringResourcesJa.Instance.NoFolderSelected);
                        return;
                    }

                    LogWrapper.Info($"{PythonAILibStringResourcesJa.Instance.CopyToFolderDescription}:{destinationFolder.FolderName}");
                    ContentItemWrapper newItem = args.Copy();
                    // Folderに追加
                    await destinationFolder.AddItemAsync(newItem);
                };
            }
            if (typeEnum == AutoProcessActionTypeEnum.MoveToFolder) {
                return async (args) => {
                    if (destinationFolder == null) {
                        LogWrapper.Warn(PythonAILibStringResourcesJa.Instance.NoFolderSelected);
                        return;
                    }
                    // Folderに移動
                    await args.MoveToAsync(destinationFolder);

                };
            }
            if (typeEnum == AutoProcessActionTypeEnum.ExtractText) {
                return async (args) => {
                    List<ContentItemWrapper> contentItemWrappers = [args];
                    await ContentItemCommands.ExtractTextsAsync(contentItemWrappers);
                };
            }

            return (args) => Task.CompletedTask;
        }

        public bool IsCopyOrMoveAction() {
            return TypeName == AutoProcessActionTypeEnum.CopyToFolder || TypeName == AutoProcessActionTypeEnum.MoveToFolder;
        }

        public virtual async Task ExecuteAsync(ContentItemWrapper applicationItem, ContentFolderWrapper? destinationFolder)
        {
            var action = GetAction(TypeName, destinationFolder);
            await action(applicationItem);
        }


    }
}
