using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using PythonAILib.Resources;

namespace LibPythonAI.Model.AutoProcess {
    // 自動処理の引数用のクラス
    public class AutoProcessItem {

        public enum TypeEnum {
            Ignore,
            CopyToFolder,
            MoveToFolder,
            ExtractText,
            PromptTemplate,
        }

        public AutoProcessItem(AutoProcessItemEntity entity) {
            Entity = entity;
        }

        public AutoProcessItemEntity Entity { get; set; }

        // Id
        public string Id {  get => Entity.Id; }

        public string Name {
            get {
                return Entity.Name;
            }
            set {
                Entity.Name = value;
            }
        }
        public string DisplayName {
            get {
                return Entity.DisplayName;
            }
            set {
                Entity.DisplayName = value;
            }
        }
        public string Description {
            get {
                return Entity.Description;
            }
            set {
                Entity.Description = value;
            }
        }
        public TypeEnum TypeName {
            get {
                return Entity.TypeName;
            }
            set {
                Entity.TypeName = value;
            }
        }

        public ContentFolderWrapper? DestinationFolder {
            get {
                return ContentFolderWrapper.GetFolderById(Entity.DestinationFolderId);
            }
            set {
                Entity.DestinationFolderId = value?.Id;
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
                    // Folderに移動
                    args.MoveTo(destinationFolder);

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

        public bool IsCopyOrMoveAction() {
            return Name == TypeEnum.CopyToFolder.ToString() || Name == TypeEnum.MoveToFolder.ToString();
        }

        public virtual void Execute(ContentItemWrapper clipboardItem, ContentFolderWrapper? destinationFolder) {

            Action<ContentItemWrapper> action = GetAction(TypeName, destinationFolder);
            action(clipboardItem);

        }


        private static List<AutoProcessItem>? _systemAutoProcesses;

        public static List<AutoProcessItem> SystemAutoProcesses {
            get {
                if (_systemAutoProcesses == null) {
                    _systemAutoProcesses = Init();
                }
                return _systemAutoProcesses;
            }
        }

        public static List<AutoProcessItem> Init() {

            List<AutoProcessItem> items = [];

            using PythonAILibDBContext db = new();
            // Ignoreコマンド
            var ignore = db.AutoProcessItems.Where(x => x.Name == "Ignore").FirstOrDefault();
            if (ignore == null) {
                ignore = new AutoProcessItemEntity() {
                    Name = "Ignore",
                    DisplayName = PythonAILibStringResources.Instance.Ignore,
                    Description = PythonAILibStringResources.Instance.DoNothing,
                    TypeName = TypeEnum.Ignore
                };
                db.AutoProcessItems.Add(ignore);
            }
            items.Add(new AutoProcessItem(ignore));

            // CopyToFolderコマンド
            var copyToFolder = db.AutoProcessItems.Where(x => x.Name == "CopyToFolder").FirstOrDefault();
            if (copyToFolder == null) {
                copyToFolder = new AutoProcessItemEntity() {
                    Name = "CopyToFolder",
                    DisplayName = PythonAILibStringResources.Instance.CopyToFolder,
                    Description = PythonAILibStringResources.Instance.CopyClipboardContentToSpecifiedFolder,
                    TypeName = TypeEnum.CopyToFolder
                };
                db.AutoProcessItems.Add(copyToFolder);
            }
            items.Add(new AutoProcessItem(copyToFolder));

            // MoveToFolderコマンド
            var moveToFolder = db.AutoProcessItems.Where(x => x.Name == "MoveToFolder").FirstOrDefault();
            if (moveToFolder == null) {
                moveToFolder = new AutoProcessItemEntity() {
                    Name = "MoveToFolder",
                    DisplayName = PythonAILibStringResources.Instance.MoveToFolder,
                    Description = PythonAILibStringResources.Instance.MoveClipboardContentToSpecifiedFolder,
                    TypeName = TypeEnum.MoveToFolder
                };
                db.AutoProcessItems.Add(moveToFolder);
            }
            items.Add(new AutoProcessItem(moveToFolder));

            // ExtractTextコマンド
            var extractText = db.AutoProcessItems.Where(x => x.Name == "ExtractText").FirstOrDefault();
            if (extractText == null) {
                extractText = new AutoProcessItemEntity() {
                    Name = "ExtractText",
                    DisplayName = PythonAILibStringResources.Instance.ExtractText,
                    Description = PythonAILibStringResources.Instance.ExtractClipboardText,
                    TypeName = TypeEnum.ExtractText
                };
                db.AutoProcessItems.Add(extractText);
            }
            items.Add(new AutoProcessItem(extractText));

            db.SaveChanges();

            return items;

        }

        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            AutoProcessItem item = (AutoProcessItem)obj;
            return item.Entity == Entity;
        }
        public override int GetHashCode() {
            return Entity.GetHashCode();
        }

        // GetItemById
        public static AutoProcessItem? GetItemById(string? id) {
            if (id == null) {
                return null;
            }
            using PythonAILibDBContext db = new();
            var item = db.AutoProcessItems.Find(id);
            if (item == null) {
                return null;
            }
            return new AutoProcessItem(item);
        }

        // GetItemsByType
        public static List<AutoProcessItem> GetItemsByType(TypeEnum? typeEnum) {
            if (typeEnum == null) {
                return [];
            }
            using PythonAILibDBContext db = new();
            var items = db.AutoProcessItems.Where(x => x.TypeName == typeEnum);
            return items.Select(x => new AutoProcessItem(x)).ToList();
        }
    }
}
