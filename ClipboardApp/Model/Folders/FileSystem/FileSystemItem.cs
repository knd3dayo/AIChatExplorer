using System.IO;
using PythonAILib.Common;
using PythonAILib.Model.Content;
using PythonAILib.PythonIF;

namespace ClipboardApp.Model.Folders.FileSystem {
    public class FileSystemItem : ContentItemWrapper {

        public static List<string> TargetMimeTypes { get; set; } = [
            "text/",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        ];

        // コンストラクタ
        public FileSystemItem(ContentItem item) : base(item) { }

        public FileSystemItem(LiteDB.ObjectId folderObjectId) : base(folderObjectId) { }

        public override FileSystemItem Copy() {
            return new(ContentItemInstance.Copy());
        }

        public override void Save(bool updateLastModifiedTime = true, bool applyAutoProcess = false) {
            ContentItemInstance.Save(false, applyAutoProcess);
        }

        public override void Load(Action beforeAction, Action afterAction) {
            // SourcePathのファイルが存在しない場合は、何もしない
            if (ContentItemInstance.SourcePath == null || !File.Exists(ContentItemInstance.SourcePath)) {
                return;
            }
            ContentItemCommands.ExtractTexts([this], beforeAction, afterAction);
        }
    }
}
