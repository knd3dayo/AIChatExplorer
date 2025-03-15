
using System.ComponentModel.DataAnnotations;

namespace LibPythonAI.Data {
    public class ContentFolderRootEntity {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // フォルダの種類の文字列
        public string FolderTypeString { get; set; } = "Normal";

        //　OS上のフォルダ名
        public string ContentOutputFolderPrefix { get; set; } = "";

        public static ContentFolderRootEntity? GetFolderRoot(string? id) {
            if (id == null) {
                return null;
            }
            using PythonAILibDBContext context = new();
            var folder = context.ContentFolderRoots
                .FirstOrDefault(x => x.Id == id);
            return folder;
        }

        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            ContentFolderEntity other = (ContentFolderEntity)obj;
            return Id == other.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
