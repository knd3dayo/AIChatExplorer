using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Utils.Common;

namespace LibPythonAI.Data {
    public class TagItemEntity {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Tag { get; set; } = "";

        public bool IsPinned { get; set; } = false;

        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            TagItemEntity entity = (TagItemEntity)obj;
            return Id == entity.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
