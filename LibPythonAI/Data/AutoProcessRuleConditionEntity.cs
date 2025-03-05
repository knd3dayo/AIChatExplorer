using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Data;
using PythonAILib.Model.File;

namespace PythonAILib.Model.AutoProcess {
    public class AutoProcessRuleConditionEntity {

        private static JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // 条件の種類
        public AutoProcessRuleCondition.ConditionTypeEnum ConditionType { get; set; } = AutoProcessRuleCondition.ConditionTypeEnum.AllItems;

        // ContentTypesJson
        // ContentTypesJson
        public string ContentTypesJson { get; set; } = "[]";

        // アイテムのタイプ種類のリスト
        [NotMapped]
        public List<ContentTypes.ContentItemTypes> ContentTypes {
            get {
                List<ContentTypes.ContentItemTypes> result = [];
                foreach (var item in JsonSerializer.Deserialize<List<int>>(ContentTypesJson, jsonSerializerOptions) ?? []) {
                    result.Add((ContentTypes.ContentItemTypes)item);
                }
                return result;
            }
            set {
                List<int> result = [];
                foreach (var item in value) {
                    result.Add((int)item);
                }
                ContentTypesJson = JsonSerializer.Serialize(result, jsonSerializerOptions);
            }
        }

        // 条件のキーワード
        public string Keyword { get; set; } = "";

        public int MinLineCount { get; set; } = -1;

        public int MaxLineCount { get; set; } = -1;

        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> result = new();
            result["ConditionType"] = ConditionType;
            result["ContentTypesJson"] = ContentTypesJson;
            result["Keyword"] = Keyword;
            result["MinLineCount"] = MinLineCount;
            result["MaxLineCount"] = MaxLineCount;
            return result;
        }
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), jsonSerializerOptions);
        }

        // Equals , GetHashCodeのオーバーライド
        public override bool Equals(object? obj) {
            if (obj == null || GetType() != obj.GetType()) {
                return false;
            }
            AutoProcessRuleConditionEntity entity = (AutoProcessRuleConditionEntity)obj;
            return Id == entity.Id;
        }
        public override int GetHashCode() {
            return Id.GetHashCode();
        }


        public static AutoProcessRuleConditionEntity? FromDict(Dictionary<string, object>? dict) {
            if (dict == null) {
                return null;
            }
            AutoProcessRuleConditionEntity entity = new() {
                ConditionType = (AutoProcessRuleCondition.ConditionTypeEnum)dict["ConditionType"],
                ContentTypesJson = (string)dict["ContentTypesJson"],
                Keyword = (string)dict["Keyword"],
                MinLineCount = (int)dict["MinLineCount"],
                MaxLineCount = (int)dict["MaxLineCount"]
            };
            return entity;

        }

        public static AutoProcessRuleConditionEntity? FromJson(string json) {
            return FromDict(JsonSerializer.Deserialize<Dictionary<string, object>>(json, jsonSerializerOptions));
        }

    }

}
