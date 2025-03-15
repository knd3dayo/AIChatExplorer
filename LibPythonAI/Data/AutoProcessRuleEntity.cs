using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibGit2Sharp;
using PythonAILib.Model.AutoProcess;
using PythonAILib.Utils.Common;

namespace LibPythonAI.Data {

    public class AutoProcessRuleEntity {

        private static JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string RuleName { get; set; } = "";

        // このルールを有効にするかどうか
        public bool IsEnabled { get; set; } = true;

        // 優先順位
        public int Priority { get; set; } = -1;

        public string ConditionsJson { get; set; } = "[]";

        private List<AutoProcessRuleConditionEntity>? _conditions { get; set; }

        [NotMapped]        
        public List<AutoProcessRuleConditionEntity> Conditions {
            get {
                if (_conditions == null) {
                    List<Dictionary<string, dynamic?>> dict = JsonUtil.ParseJsonArray(ConditionsJson);
                    _conditions = AutoProcessRuleConditionEntity.FromDictList(dict);
                }
                return _conditions;
            }
            set {
                _conditions = value;
            }

        }

        public string? AutoProcessItemId { get; set; }

        public string? TargetFolderId { get; set; }

        public string? DestinationFolderId { get; set; }

        public void SaveConditionsJson() {
            if (_conditions != null) {
                ConditionsJson = JsonSerializer.Serialize(_conditions.Select(c => c.ToDict()).ToList(), jsonSerializerOptions);
            }
        }

        public static void SaveItems(List<AutoProcessRuleEntity> items) {
            using PythonAILibDBContext db = new();
            foreach (var item in items) {
                item.SaveConditionsJson();
                var entity = db.AutoProcessRules.Find(item.Id);

                if (entity == null) {
                    db.AutoProcessRules.Add(item);
                } else {
                    db.AutoProcessRules.Entry(entity).CurrentValues.SetValues(item);
                }
            }
            db.SaveChanges();
        }

        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            AutoProcessRuleEntity other = (AutoProcessRuleEntity)obj;
            return Id == other.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }

    }
}


