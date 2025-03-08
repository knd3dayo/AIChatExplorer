using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PythonAILib.Common;
using PythonAILib.Model.File;
using PythonAILib.Model.VectorDB;
using PythonAILib.Utils.Git;

namespace LibPythonAI.Data {
    public class RAGSourceItemEntity {


        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? VectorDBItemId { get; set; }
        // VectorDBIte

        public string SourceURL { get; set; } = "";
        public string WorkingDirectory { get; set; } = "";

        public string LastIndexCommitHash { get; set; } = "";


        // 複数アイテムのSave
        public static void SaveItems(IEnumerable<RAGSourceItemEntity> items) {
            using PythonAILibDBContext db = new();
            foreach (var item in items) {
                var entity = db.RAGSourceItems.Find(item.Id);
                if (entity == null) {
                    db.RAGSourceItems.Add(item);
                } else {
                    db.RAGSourceItems.Entry(entity).CurrentValues.SetValues(item);
                }
            }
            db.SaveChanges();
        }

        // 複数アイテムのDelete
        public static void DeleteItems(IEnumerable<RAGSourceItemEntity> items) {
            using PythonAILibDBContext db = new();
            foreach (var item in items) {
                db.RAGSourceItems.Remove(item);
            }
            db.SaveChanges();
        }


        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            RAGSourceItemEntity other = (RAGSourceItemEntity)obj;
            return Id == other.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

    }
}
