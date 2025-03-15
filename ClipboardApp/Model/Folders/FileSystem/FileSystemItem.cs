using System.IO;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
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
        public FileSystemItem(ContentItemEntity item) : base(item) { }

        public FileSystemItem(ContentFolderEntity folder) : base(folder) { }

        public override FileSystemItem Copy() {
            return new(Entity.Copy());
        }


        public override void Load(Action beforeAction, Action afterAction) {
            // SourcePathのファイルが存在しない場合は、何もしない
            if (SourcePath == null || !File.Exists(SourcePath)) {
                return;
            }
            ContentItemCommands.ExtractTexts([this], beforeAction, afterAction);
        }
    }
}
