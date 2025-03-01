using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LiteDB;
using PythonAILib.Common;
using PythonAILib.Model.Tag;

namespace LibPythonAI.Data {
    public class TagItemEntity {
        // Id

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Tag { get; set; } = "";

        public bool IsPinned { get; set; } = false;

    }

}
