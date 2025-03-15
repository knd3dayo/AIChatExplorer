using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Resources;

namespace PythonAILib.Model.Image
{
    // TODO 多言語化

    public class ScreenShotCheckCondition
    {

        public static readonly string CheckTypeEqual = PythonAILibStringResources.Instance.CheckTypeEqual;
        public static readonly string CheckTypeNotEqual = PythonAILibStringResources.Instance.CheckTypeNotEqual;
        public static readonly string CheckTypeInclude = PythonAILibStringResources.Instance.CheckTypeInclude;
        public static readonly string CheckTypeNotInclude = PythonAILibStringResources.Instance.CheckTypeNotInclude;
        public static readonly string CheckTypeStartWith = PythonAILibStringResources.Instance.CheckTypeStartWith;
        public static readonly string CheckTypeNotStartWith = PythonAILibStringResources.Instance.CheckTypeNotStartWith;
        public static readonly string CheckTypeEndWith = PythonAILibStringResources.Instance.CheckTypeEndWith;
        public static readonly string CheckTypeNotEndWith = PythonAILibStringResources.Instance.CheckTypeNotEndWith;
        public static readonly string CheckTypeEmpty = PythonAILibStringResources.Instance.CheckTypeEmpty;
        public static readonly string CheckTypeCheckBox = PythonAILibStringResources.Instance.CheckTypeCheckBox;

        public ObservableCollection<string> CheckTypeList
        {
            get
            {
                // チェック用の定数のリストを返す
                return [
                    CheckTypeEqual,
                    CheckTypeNotEqual,
                    CheckTypeInclude,
                    CheckTypeNotInclude,
                    CheckTypeStartWith,
                    CheckTypeNotStartWith,
                    CheckTypeEndWith,
                    CheckTypeNotEndWith,
                    CheckTypeEmpty,
                    CheckTypeCheckBox,
                ];
            }
        }

        private string _testString = "";
        [JsonPropertyName("test_string")]
        public string TestString
        {
            get
            {
                return _testString;
            }
            set
            {
                _testString = value;
            }
        }

        // 設定項目
        private string _settingItem = "";
        [JsonPropertyName("setting_item")]
        public string SettingItem
        {
            get
            {
                return _settingItem;
            }
            set
            {
                _settingItem = value;
            }
        }
        // 設定値
        private string _settingValue = "";
        [JsonPropertyName("setting_value")]
        public string SettingValue
        {
            get
            {
                return _settingValue;
            }
            set
            {
                _settingValue = value;
            }
        }
        // チェック内容
        private string _checkTypeString = CheckTypeEqual;
        [JsonPropertyName("check_type_string")]
        public string CheckTypeString
        {
            get
            {
                return _checkTypeString;
            }
            set
            {
                _checkTypeString = value;
            }
        }

        public string ToPromptString()
        {
            string result = $"{PythonAILibStringResources.Instance.SettingValueIs(SettingItem, SettingValue)}";
            // CheckType.CheckTypeがEqualの場合
            if (CheckTypeString == CheckTypeEqual)
            {
                result = $"{PythonAILibStringResources.Instance.SettingValueIs(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotEqualの場合
            if (CheckTypeString == CheckTypeNotEqual)
            {
                result = $"{PythonAILibStringResources.Instance.SettingValueIsNot(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがIncludeの場合
            if (CheckTypeString == CheckTypeInclude)
            {
                result = $"{PythonAILibStringResources.Instance.SettingValueContains(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotIncludeの場合
            if (CheckTypeString == CheckTypeNotInclude)
            {
                result = $"{PythonAILibStringResources.Instance.SettingValueNotContain(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがStartWithの場合
            if (CheckTypeString == CheckTypeStartWith)
            {
                result = $"{PythonAILibStringResources.Instance.SettingValueStartsWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotStartWithの場合
            if (CheckTypeString == CheckTypeNotStartWith)
            {
                result = $"{PythonAILibStringResources.Instance.SettingValueNotStartWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがEndWithの場合
            if (CheckTypeString == CheckTypeEndWith)
            {
                result = $"{PythonAILibStringResources.Instance.SettingValueEndsWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotEndWithの場合
            if (CheckTypeString == CheckTypeNotEndWith)
            {
                result = $"{PythonAILibStringResources.Instance.SettingValueNotEndWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがEmptyの場合
            if (CheckTypeString == CheckTypeEmpty)
            {
                result = $"{PythonAILibStringResources.Instance.SettingValueIsEmpty(SettingItem)}";
            }
            // CheckType.CheckTypeがCheckBoxの場合
            if (CheckTypeString == CheckTypeCheckBox)
            {
                result = $"{PythonAILibStringResources.Instance.SettingValueIsChecked(SettingItem, SettingValue)}";
            }
            return result;
        }

        // ScreenShotCheckItemのIEnumerableをJSONに変換する
        public static string ToJson(IEnumerable<ScreenShotCheckCondition> items)
        {
            JsonSerializerOptions jsonSerializerOptions = new()
            {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return JsonSerializer.Serialize(items, options);
        }
        // JSONをScreenShotCheckItemのListに変換する
        public static List<ScreenShotCheckCondition> FromJson(string json)
        {
            List<ScreenShotCheckCondition> result = [];
            var jsonObject = JsonSerializer.Deserialize<List<ScreenShotCheckCondition>>(json);
            if (jsonObject == null)
            {
                return result;
            }
            foreach (var item in jsonObject)
            {
                result.Add(item);
            }
            return result;

        }
    }
}
