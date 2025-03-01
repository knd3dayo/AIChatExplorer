
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;

namespace LibPythonAI.Data {
    public class ContentFolderEntity {

        private static JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };


        [Key]
        public string Id { get; set;  } = Guid.NewGuid().ToString();

        // フォルダの種類の文字列
        public string FolderTypeString { get; set; } = "Normal";

        // 親フォルダのID
        [Column("PARENT_ID")]

        public string? ParentId { get; set; }

        public ContentFolderEntity? Parent { get; set; }

        // ルートフォルダか否か
        public bool IsRootFolder { get; set; } = false;

        //　フォルダ名
        public string FolderName { get; set; } = "";

        // Description
        public string Description { get; set; } = "";

        //　OS上のフォルダ名
        public string ContentOutputFolderPrefix { get; set; } = "";

        // public List<VectorDBProperty> VectorDBProperties { get; set; } = new();

        public string ExtendedPropertiesJson { get; set; } = "{}";

        // 拡張プロパティ (Dictionary<string, object> は EF でサポートされないため、Json で保存)
        [NotMapped]
        public Dictionary<string, object> ExtendedProperties {
            get {
                Dictionary<string, object>? items = JsonSerializer.Deserialize<Dictionary<string, object>>(ExtendedPropertiesJson, jsonSerializerOptions);
                return items ?? [];
            }
        }



    }
}
