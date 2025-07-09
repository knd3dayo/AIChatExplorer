namespace LibPythonAI.Model.Folders {
    public class FolderTypeEnum {

        public string Name { get; private set; } = "";

        public FolderTypeEnum(string name) {
            Name = name;
        }
        // Equals
        public override bool Equals(object? obj) {
            return obj is FolderTypeEnum type &&
                   Name == type.Name;
        }
        // GetHashCode
        public override int GetHashCode() {
            return HashCode.Combine(Name);
        }

        // Normal,
        public static FolderTypeEnum Normal { get; } = new("Normal");
        // Search,
        public static FolderTypeEnum Search { get; } = new("Search");
        // ImageCheck,
        public static FolderTypeEnum ImageCheck { get; } = new("ImageCheck");
        // Chat,
        public static FolderTypeEnum Chat { get; } = new("Chat");
        // FileSystem,
        public static FolderTypeEnum FileSystem { get; } = new("FileSystem");
        // ShortCut,
        public static FolderTypeEnum ShortCut { get; } = new("ShortCut");
        // Outlook,
        public static FolderTypeEnum Outlook { get; } = new("Outlook");

    }
}

