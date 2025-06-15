
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

        // 複数アイテムのSave
        public static void SaveItems(List<ContentFolderRootEntity> items) {
            using PythonAILibDBContext context = new();
            foreach (var item in items) {
                var ExistingItem = context.ContentFolderRoots.FirstOrDefault(x => x.Id == item.Id);
                if (ExistingItem == null) {
                    context.ContentFolderRoots.Add(item);
                } else {
                    context.Entry(ExistingItem).CurrentValues.SetValues(item);
                }
            }
            context.SaveChanges();
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
