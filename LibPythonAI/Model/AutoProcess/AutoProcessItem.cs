using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Utils.Common;
using PythonAILib.Model.Content;
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
            AutoProcessItemInstance = entity;
        }

        public AutoProcessItemEntity AutoProcessItemInstance { get; set; }

        public string Name {
            get {
                return AutoProcessItemInstance.Name;
            }
            set {
                AutoProcessItemInstance.Name = value;
            }
        }
        public string DisplayName {
            get {
                return AutoProcessItemInstance.DisplayName;
            }
            set {
                AutoProcessItemInstance.DisplayName = value;
            }
        }
        public string Description {
            get {
                return AutoProcessItemInstance.Description;
            }
            set {
                AutoProcessItemInstance.Description = value;
            }
        }
        public TypeEnum TypeName {
            get {
                return AutoProcessItemInstance.TypeName;
            }
            set {
                AutoProcessItemInstance.TypeName = value;
            }
        }

        public ContentFolderWrapper? DestinationFolder {
            get {
                if (AutoProcessItemInstance.DestinationFolder == null) {
                    return null;
                }
                return new ContentFolderWrapper(AutoProcessItemInstance.DestinationFolder);
            }
            set {
                if (value == null) {
                    AutoProcessItemInstance.DestinationFolder = null;
                } else {
                    AutoProcessItemInstance.DestinationFolder = value.Entity;
                }
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

        public bool IsCopyOrMoveAction() {
            return Name == TypeEnum.CopyToFolder.ToString() || Name == TypeEnum.MoveToFolder.ToString();
        }

        public virtual void Execute(ContentItemWrapper clipboardItem, ContentFolderWrapper? destinationFolder) {

            Action<ContentItemWrapper> action = GetAction(TypeName, DestinationFolder);
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


    }
}
