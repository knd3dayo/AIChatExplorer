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

    }
}
