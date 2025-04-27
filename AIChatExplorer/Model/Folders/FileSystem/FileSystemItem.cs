using System.IO;
using LibPythonAI.Data;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;

namespace AIChatExplorer.Model.Folders.FileSystem {
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

        public override void Save() {
            if (ContentModified || DescriptionModified) {
                // ベクトルを更新
                Task.Run(() => {
                    VectorDBProperty.UpdateEmbeddings(GetFolder().GetMainVectorSearchProperty());
                });
                ContentModified = false;
                DescriptionModified = false;
            }

            ContentItemEntity.SaveItems([Entity]);
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
