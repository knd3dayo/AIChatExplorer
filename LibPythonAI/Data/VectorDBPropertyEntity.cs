using System.ComponentModel.DataAnnotations;

namespace LibPythonAI.Data {
    public class VectorDBPropertyEntity {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        //TopK
        public int TopK { get; set; }

        public string? FolderId { get; set; }

        // VectorDBItemId
        public string? VectorDBItemId { get; set; }

        // ContentType
        public string ContentType { get; set; } = string.Empty;


        // Equals
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            VectorDBPropertyEntity other = (VectorDBPropertyEntity)obj;
            return Id == other.Id;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
