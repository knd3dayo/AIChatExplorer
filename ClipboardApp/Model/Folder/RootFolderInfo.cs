using LiteDB;
using static ClipboardApp.Model.Folder.ClipboardFolder;

namespace ClipboardApp.Model.Folder {
    public class RootFolderInfo {

        public string FolderName { get; set; } = "";
        public ObjectId Id { get; set; } = ObjectId.Empty;

        public FolderTypeEnum FolderType { get; set; } = FolderTypeEnum.Normal;

    }
}

