using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace WpfAppCommon.Model {
    // TODO 多言語化

    public class ScreenShotCheckCondition {

        public static readonly string CheckTypeEqual = CommonStringResources.Instance.CheckTypeEqual;
        public static readonly string CheckTypeNotEqual = CommonStringResources.Instance.CheckTypeNotEqual;
        public static readonly string CheckTypeInclude = CommonStringResources.Instance.CheckTypeInclude;
        public static readonly string CheckTypeNotInclude = CommonStringResources.Instance.CheckTypeNotInclude;
        public static readonly string CheckTypeStartWith = CommonStringResources.Instance.CheckTypeStartWith;
        public static readonly string CheckTypeNotStartWith = CommonStringResources.Instance.CheckTypeNotStartWith;
        public static readonly string CheckTypeEndWith = CommonStringResources.Instance.CheckTypeEndWith;
        public static readonly string CheckTypeNotEndWith = CommonStringResources.Instance.CheckTypeNotEndWith;
        public static readonly string CheckTypeEmpty = CommonStringResources.Instance.CheckTypeEmpty;
        public static readonly string CheckTypeCheckBox = CommonStringResources.Instance.CheckTypeCheckBox;

        public ObservableCollection<string> CheckTypeList {
            get {
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
        public string TestString {
            get {
                return _testString;
            }
            set {
                _testString = value;
            }
        }

        // 設定項目
        private string _settingItem = "";
        [JsonPropertyName("setting_item")]
        public string SettingItem {
            get {
                return _settingItem;
            }
            set {
                _settingItem = value;
            }
        }
        // 設定値
        private string _settingValue = "";
        [JsonPropertyName("setting_value")]
        public string SettingValue {
            get {
                return _settingValue;
            }
            set {
                _settingValue = value;
            }
        }
        // チェック内容
        private string _checkTypeString = CheckTypeEqual;
        [JsonPropertyName("check_type_string")]
        public string CheckTypeString {
            get {
                return _checkTypeString;
            }
            set {
                _checkTypeString = value;
            }
        }

        public string ToPromptString() {
            string result = $"{CommonStringResources.Instance.SettingValueIs(SettingItem, SettingValue)}";
            // CheckType.CheckTypeがEqualの場合
            if (CheckTypeString == CheckTypeEqual) {
                result = $"{CommonStringResources.Instance.SettingValueIs(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotEqualの場合
            if (CheckTypeString == CheckTypeNotEqual) {
                result = $"{CommonStringResources.Instance.SettingValueIsNot(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがIncludeの場合
            if (CheckTypeString == CheckTypeInclude) {
                result = $"{CommonStringResources.Instance.SettingValueContains(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotIncludeの場合
            if (CheckTypeString == CheckTypeNotInclude) {
                result = $"{CommonStringResources.Instance.SettingValueNotContain(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがStartWithの場合
            if (CheckTypeString == CheckTypeStartWith) {
                result = $"{CommonStringResources.Instance.SettingValueStartsWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotStartWithの場合
            if (CheckTypeString == CheckTypeNotStartWith) {
                result = $"{CommonStringResources.Instance.SettingValueNotStartWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがEndWithの場合
            if (CheckTypeString == CheckTypeEndWith) {
                result = $"{CommonStringResources.Instance.SettingValueEndsWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotEndWithの場合
            if (CheckTypeString == CheckTypeNotEndWith) {
                result = $"{CommonStringResources.Instance.SettingValueNotEndWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがEmptyの場合
            if (CheckTypeString == CheckTypeEmpty) {
                result = $"{CommonStringResources.Instance.SettingValueIsEmpty(SettingItem)}";
            }
            // CheckType.CheckTypeがCheckBoxの場合
            if (CheckTypeString == CheckTypeCheckBox) {
                result = $"{CommonStringResources.Instance.SettingValueIsChecked(SettingItem, SettingValue)}";
            }
            return result;
        }

        // ScreenShotCheckItemのIEnumerableをJSONに変換する
        public static string ToJson(IEnumerable<ScreenShotCheckCondition> items) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(items, options);
        }
        // JSONをScreenShotCheckItemのListに変換する
        public static List<ScreenShotCheckCondition> FromJson(string json) {
            List<ScreenShotCheckCondition> result = [];
            var jsonObject = JsonSerializer.Deserialize<List<ScreenShotCheckCondition>>(json);
            if (jsonObject == null) {
                return result;
            }
            foreach (var item in jsonObject) {
                result.Add(item);
            }
            return result;

        }
    }
}
