using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibMain.Model.Content;
using LibMain.Utils.Common;

namespace LibMain.Model.AutoProcess {
    public class AutoProcessRuleCondition {

        public enum ConditionTypeEnum {
            AllItems,
            DescriptionContains,
            ContentContains,
            SourceApplicationNameContains,
            SourceApplicationTitleContains,
            SourceApplicationPathContains,
            ContentTypeIs,

        }


        public string Id { get; set; } = Guid.NewGuid().ToString();

        // 条件の種類
        public ConditionTypeEnum ConditionType { get; set; } = ConditionTypeEnum.AllItems;

        // ContentTypesJson
        // ContentTypesJson
        public string ContentTypesJson { get; set; } = "[]";

        // アイテムのタイプ種類のリスト
        [NotMapped]
        public List<ContentItemTypes.ContentItemTypeEnum> ContentTypes {
            get {
                List<ContentItemTypes.ContentItemTypeEnum> result = [];
                foreach (var item in JsonSerializer.Deserialize<List<int>>(ContentTypesJson, JsonUtil.JsonSerializerOptions) ?? []) {
                    result.Add((ContentItemTypes.ContentItemTypeEnum)item);
                }
                return result;
            }
            set {
                List<int> result = [];
                foreach (var item in value) {
                    result.Add((int)item);
                }
                ContentTypesJson = JsonSerializer.Serialize(result, JsonUtil.JsonSerializerOptions);
            }
        }

        // 条件の種類
        public ConditionTypeEnum Type { get; set; } = ConditionTypeEnum.AllItems;

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


        public static List<AutoProcessRuleCondition> FromDictList(List<Dictionary<string, dynamic?>> dict) {

            List<AutoProcessRuleCondition> result = [];
            foreach (var item in dict) {
                result.Add(FromDict(item));
            }
            return result;

        }

        public static AutoProcessRuleCondition FromDict(Dictionary<string, dynamic?> dict) {
            AutoProcessRuleCondition entity = new();

            if (dict.TryGetValue("Id", out dynamic? value1) && value1 is not null) {
                entity.Id = (string)value1;
            }
            if (dict.TryGetValue("ConditionType", out dynamic? value2) && value2 is not null) {
                entity.ConditionType = (ConditionTypeEnum)value2;
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

        //ApplicationItemのDescriptionが指定したキーワードを含むかどうか
        public bool IsDescriptionContains(ContentItem applicationItem, string keyword) {
            // DescriptionがNullの場合はFalseを返す
            if (applicationItem.Description == null) {
                return false;
            }
            LogWrapper.Info("Description:" + applicationItem.Description);
            LogWrapper.Info("Keyword:" + keyword);
            LogWrapper.Info("Contains:" + applicationItem.Description.Contains(keyword));

            return applicationItem.Description.Contains(keyword);

        }
        //ApplicationItemのContentが指定したキーワードを含むかどうか
        public bool IsContentContains(ContentItem applicationItem, string keyword) {
            // ContentがNullの場合はFalseを返す
            if (applicationItem.Content == null) {
                return false;
            }
            return applicationItem.Content.Contains(keyword);
        }
        // ApplicationItemのSourceApplicationNameが指定したキーワードを含むかどうか
        public bool IsSourceApplicationNameContains(ContentItem applicationItem, string keyword) {
            // SourceApplicationNameがnullの場合は、falseを返す
            if (applicationItem.SourceApplicationName == null) {
                return false;
            }
            return applicationItem.SourceApplicationName.Contains(keyword);
        }
        // ApplicationItemのSourceApplicationTitleが指定したキーワードを含むかどうか
        public bool IsSourceApplicationTitleContains(ContentItem applicationItem, string keyword) {
            // SourceApplicationTitleがnullの場合は、falseを返す
            if (applicationItem.SourceApplicationTitle == null) {
                return false;
            }
            return applicationItem.SourceApplicationTitle.Contains(keyword);
        }
        // ApplicationItemのSourceApplicationPathが指定したキーワードを含むかどうか
        public bool IsSourceApplicationPathContains(ContentItem applicationItem, string keyword) {
            // SourceApplicationPathがnullの場合は、falseを返す
            if (applicationItem.SourceApplicationPath == null) {
                return false;
            }
            return applicationItem.SourceApplicationPath != null && applicationItem.SourceApplicationPath.Contains(keyword);
        }

        // ApplicationItemのContentの行数が指定した行数以上かどうか
        public bool IsContentLineCountOver(ContentItem applicationItem) {
            // MinLineCountが-1の場合はTrueを返す
            if (MinLineCount == -1) {
                return true;
            }
            // ContentがNullの場合はFalseを返す
            if (applicationItem.Content == null) {
                return false;
            }
            return applicationItem.Content.Split('\n').Length >= MinLineCount;
        }
        // ApplicationItemのContentの行数が指定した行数以下かどうか
        public bool IsContentLineCountUnder(ContentItem applicationItem) {
            // MaxLineCountが-1の場合はTrueを返す
            if (MaxLineCount == -1) {
                return true;
            }
            // ContentがNullの場合はFalseを返す
            if (applicationItem.Content == null) {
                return false;
            }
            return applicationItem.Content.Split('\n').Length <= MaxLineCount;
        }

        // ConditionTypeに対応する関数を実行してBoolを返す
        // ★TODO SearchConditionと共通化する
        public bool CheckCondition(ContentItem applicationItem) {
            return Type switch {
                ConditionTypeEnum.DescriptionContains => IsDescriptionContains(applicationItem, Keyword),
                ConditionTypeEnum.ContentContains => IsContentContains(applicationItem, Keyword),
                ConditionTypeEnum.SourceApplicationNameContains => IsSourceApplicationNameContains(applicationItem, Keyword),
                ConditionTypeEnum.SourceApplicationTitleContains => IsSourceApplicationTitleContains(applicationItem, Keyword),
                ConditionTypeEnum.SourceApplicationPathContains => IsSourceApplicationPathContains(applicationItem, Keyword),
                ConditionTypeEnum.ContentTypeIs => CheckContentTypeIs(applicationItem),
                _ => false,
            };
        }

        // ContentTypeIsの条件にマッチするかどうか
        public bool CheckContentTypeIs(ContentItem applicationItem) {
            if (ContentTypes.Contains(applicationItem.ContentType) == false) {
                return false;
            }
            if (applicationItem.ContentType == ContentItemTypes.ContentItemTypeEnum.Text) {
                return IsContentLineCountOver(applicationItem) && IsContentLineCountUnder(applicationItem);
            }
            return true;
        }

    }

}
