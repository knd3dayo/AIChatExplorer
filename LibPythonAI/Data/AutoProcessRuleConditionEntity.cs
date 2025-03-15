using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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


        public static List<AutoProcessRuleConditionEntity> FromDictList(List<Dictionary<string, dynamic?>> dict) {

            List<AutoProcessRuleConditionEntity> result = [];
            foreach (var item in dict) {
                result.Add(FromDict(item));
            }
            return result;

        }

        public static AutoProcessRuleConditionEntity FromDict(Dictionary<string, dynamic?> dict) {
            AutoProcessRuleConditionEntity entity = new();

            if (dict.TryGetValue("Id", out dynamic? value1) && value1 is not null) {
                entity.Id = (string)value1;
            }
            if (dict.TryGetValue("ConditionType", out dynamic? value2) && value2 is not null) {
                entity.ConditionType = (AutoProcessRuleCondition.ConditionTypeEnum)value2;
            }
            if (dict.TryGetValue("ContentTypesJson", out dynamic? value3) && value3 is not null) {
                entity.ContentTypesJson = (string)value3;
            }
            if (dict.TryGetValue("Keyword", out dynamic? value4) && value4 is not null) {
                entity.Keyword = (string)value4;
            }
            if (dict.TryGetValue("MinLineCount", out dynamic? value5) && value5 is not null) {
                entity.MinLineCount = (int)value5;
            }
            if (dict.TryGetValue("MaxLineCount", out dynamic? value6) && value6 is not null) {
                entity.MaxLineCount = (int)value6;
            }

            return entity;
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

    }

}
