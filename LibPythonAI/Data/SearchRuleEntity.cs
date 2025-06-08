using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Model.Search;
using LibPythonAI.Utils.Common;

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

        private SearchCondition? _searchCondition;
        [NotMapped]
        public SearchCondition SearchCondition {
            get {
                if (_searchCondition == null) {
                    Dictionary<string, dynamic?> dict = JsonUtil.ParseJson(SearchConditionJson);
                    _searchCondition = SearchCondition.FromDict(dict ?? new());
                }
                return _searchCondition;
            }
            set {
                SearchConditionJson = JsonSerializer.Serialize(value.ToDict(), jsonSerializerOptions);
            }
        }

        public string? SearchFolderId { get; set; }

        public string? TargetFolderId { get; set; }

        // 検索対象フォルダ配下を検索するかどうか
        public bool IsIncludeSubFolder { get; set; }

        // 全てのフォルダを検索するかどうか
        public bool IsGlobalSearch { get; set; }


        public void SaveSearchConditionJson() {
            if (_searchCondition != null) {
                SearchConditionJson = JsonSerializer.Serialize(_searchCondition.ToDict(), jsonSerializerOptions);
            }
        }

        public SearchRuleEntity Copy() {
            SearchRuleEntity applicationItem = new();
            applicationItem.Name = Name;
            applicationItem.SearchCondition = SearchCondition;
            applicationItem.SearchFolderId = SearchFolderId;
            applicationItem.TargetFolderId = TargetFolderId;
            return applicationItem;
        }

        public static void DeleteItems(List<SearchRuleEntity> items) {
            using PythonAILibDBContext db = new();
            foreach (var item in items) {
                db.SearchRules.Remove(item);
            }
            db.SaveChanges();
        }
        // SaveItems
        public static void SaveItems(List<SearchRuleEntity> items) {
            using PythonAILibDBContext db = new();
            foreach (var item in items) {
                item.SaveSearchConditionJson();
                var entity = db.SearchRules.Find(item.Id);
                if (entity == null) {
                    db.SearchRules.Add(item);
                } else {
                    db.SearchRules.Entry(entity).CurrentValues.SetValues(item);
                }
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
