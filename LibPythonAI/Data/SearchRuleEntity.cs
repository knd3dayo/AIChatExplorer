using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Model.Content;
using PythonAILib.Model.Search;

namespace LibPythonAI.Data {



    public class SearchRuleEntity {

        private static JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };


        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";

        public string SearchConditionJson { get; set; } = "{}";

        [NotMapped]
        public SearchCondition SearchCondition {
            get {
                Dictionary<string, object>? dict = JsonSerializer.Deserialize<Dictionary<string, object>>(SearchConditionJson, jsonSerializerOptions);
                return SearchCondition.FromDict(dict ?? new());
            }
            set {
                SearchConditionJson = JsonSerializer.Serialize(value.ToDict(), jsonSerializerOptions);
            }
        }

        [Column("SEARCH_FOLDER_ID")]
        public string? SearchFolderId { get; set; }

        public ContentFolderEntity? SearchFolder { get; set; }

        [Column("TARGET_FOLDER_ID")]
        public string? TargetFolderId { get; set; }

        public ContentFolderEntity? TargetFolder { get; set; } 


    }
}
