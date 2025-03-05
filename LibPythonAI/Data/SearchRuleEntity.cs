using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Model.Search;

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

        public string? SearchFolderId { get; set; }


        public string? TargetFolderId { get; set; }


        public SearchRuleEntity Copy() {
            SearchRuleEntity clipboardItem = new();
            clipboardItem.Name = Name;
            clipboardItem.SearchCondition = SearchCondition;
            clipboardItem.SearchFolderId = SearchFolderId;
            clipboardItem.TargetFolderId = TargetFolderId;
            return clipboardItem;
        }

        public static void DeleteItems(List<SearchRuleEntity> items) {
            using PythonAILibDBContext db = new();
            foreach (var item in items) {
                db.SearchRules.Remove(item);
            }
            db.SaveChanges();
        }
        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            SearchRuleEntity item = (SearchRuleEntity)obj;
            return Id == item.Id;
        }

        public override int GetHashCode() {
            return Id.GetHashCode();
        }
    }
}
