
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Utils.Common;

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

        public string? ParentId { get; set; }

        // ルートフォルダか否か
        public bool IsRootFolder { get; set; } = false;

        //　フォルダ名
        public string FolderName { get; set; } = "";

        // Description
        public string Description { get; set; } = "";

        //　OS上のフォルダ名
        public string ContentOutputFolderPrefix { get; set; } = "";

        public List<VectorDBPropertyEntity> VectorDBProperties { get; set; } = new();

        public string ExtendedPropertiesJson { get; set; } = "{}";

        private Dictionary<string, object?>? _extendedProperties;
        // 拡張プロパティ (Dictionary<string, object> は EF でサポートされないため、Json で保存)
        [NotMapped]
        public Dictionary<string, object?> ExtendedProperties {
            get {
                if (_extendedProperties == null) {
                    _extendedProperties = JsonUtil.ParseJson(ExtendedPropertiesJson);
                }
                return _extendedProperties ?? [];
            }
        }

        public void SaveExtendedPropertiesJson() {
            if (_extendedProperties != null) {
                ExtendedPropertiesJson = JsonSerializer.Serialize(ExtendedProperties, jsonSerializerOptions);
            }
        }

    }
}
