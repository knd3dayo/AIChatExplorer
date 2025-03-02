using System.ComponentModel.DataAnnotations;

namespace LibPythonAI.Data {
    public class TagItemEntity {
        // Id

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Tag { get; set; } = "";

        public bool IsPinned { get; set; } = false;

    }

}
