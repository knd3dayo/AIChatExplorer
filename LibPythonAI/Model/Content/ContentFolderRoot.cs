using LibPythonAI.Data;

namespace LibPythonAI.Model.Content {
    public class ContentFolderRoot {


        public ContentFolderRootEntity Entity { get; private set; } = new ContentFolderRootEntity();
        public string Id {
            get {
                return Entity.Id;
            }
        }

        // フォルダの種類の文字列
        public string FolderTypeString {
            get => Entity.FolderTypeString;
            set => Entity.FolderTypeString = value;
        }
        //　OS上のフォルダ名
        public string ContentOutputFolderPrefix {
            get => Entity.ContentOutputFolderPrefix;
            set => Entity.ContentOutputFolderPrefix = value;
        }

        public virtual void Save() {
            ContentFolderRootEntity.SaveItems([Entity]);
        }

        public static ContentFolderRoot? GetFolderRootByFolderType(string folderType) {
            using PythonAILibDBContext context = new();
            var folder = context.ContentFolderRoots
                .FirstOrDefault(x => x.FolderTypeString == folderType);
            if (folder == null) {
                return null;
            }
            ContentFolderRoot contentFolderRoot = new() {
                Entity = folder
            };
            return contentFolderRoot;
        }

    }

}
