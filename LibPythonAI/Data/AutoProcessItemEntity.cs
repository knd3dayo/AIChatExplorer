using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LibPythonAI.Model.AutoProcess;
using PythonAILib.Resources;

namespace LibPythonAI.Data {
    // 自動処理の引数用のクラス
    public class AutoProcessItemEntity {


        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = "";
        public string DisplayName { get; set; } = "";
        public string Description { get; set; } = "";
        public AutoProcessItem.TypeEnum TypeName { get; set; } = AutoProcessItem.TypeEnum.CopyToFolder;

        public string? DestinationFolderId { get; set; }

        // GetItemById
        public static AutoProcessItemEntity? GetItemById(string? id) {
            if (id == null) {
                return null;
            }
            using PythonAILibDBContext db = new();
            return db.AutoProcessItems.FirstOrDefault(x => x.Id == id);
        }

        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            AutoProcessItemEntity other = (AutoProcessItemEntity)obj;
            return Id == other.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
