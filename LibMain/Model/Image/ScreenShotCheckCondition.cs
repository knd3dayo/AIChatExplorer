using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using LibMain.Resources;
using LibMain.Utils.Common;

namespace LibMain.Model.Image {
    // TODO 多言語化

    public class ScreenShotCheckCondition {

        public static readonly string CheckTypeEqual = PythonAILibStringResourcesJa.Instance.CheckTypeEqual;
        public static readonly string CheckTypeNotEqual = PythonAILibStringResourcesJa.Instance.CheckTypeNotEqual;
        public static readonly string CheckTypeInclude = PythonAILibStringResourcesJa.Instance.CheckTypeInclude;
        public static readonly string CheckTypeNotInclude = PythonAILibStringResourcesJa.Instance.CheckTypeNotInclude;
        public static readonly string CheckTypeStartWith = PythonAILibStringResourcesJa.Instance.CheckTypeStartWith;
        public static readonly string CheckTypeNotStartWith = PythonAILibStringResourcesJa.Instance.CheckTypeNotStartWith;
        public static readonly string CheckTypeEndWith = PythonAILibStringResourcesJa.Instance.CheckTypeEndWith;
        public static readonly string CheckTypeNotEndWith = PythonAILibStringResourcesJa.Instance.CheckTypeNotEndWith;
        public static readonly string CheckTypeEmpty = PythonAILibStringResourcesJa.Instance.CheckTypeEmpty;
        public static readonly string CheckTypeCheckBox = PythonAILibStringResourcesJa.Instance.CheckTypeCheckBox;

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
            string result = $"{PythonAILibStringResourcesJa.Instance.SettingValueIs(SettingItem, SettingValue)}";
            // CheckType.CheckTypeがEqualの場合
            if (CheckTypeString == CheckTypeEqual) {
                result = $"{PythonAILibStringResourcesJa.Instance.SettingValueIs(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotEqualの場合
            if (CheckTypeString == CheckTypeNotEqual) {
                result = $"{PythonAILibStringResourcesJa.Instance.SettingValueIsNot(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがIncludeの場合
            if (CheckTypeString == CheckTypeInclude) {
                result = $"{PythonAILibStringResourcesJa.Instance.SettingValueContains(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotIncludeの場合
            if (CheckTypeString == CheckTypeNotInclude) {
                result = $"{PythonAILibStringResourcesJa.Instance.SettingValueNotContain(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがStartWithの場合
            if (CheckTypeString == CheckTypeStartWith) {
                result = $"{PythonAILibStringResourcesJa.Instance.SettingValueStartsWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotStartWithの場合
            if (CheckTypeString == CheckTypeNotStartWith) {
                result = $"{PythonAILibStringResourcesJa.Instance.SettingValueNotStartWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがEndWithの場合
            if (CheckTypeString == CheckTypeEndWith) {
                result = $"{PythonAILibStringResourcesJa.Instance.SettingValueEndsWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがNotEndWithの場合
            if (CheckTypeString == CheckTypeNotEndWith) {
                result = $"{PythonAILibStringResourcesJa.Instance.SettingValueNotEndWith(SettingItem, SettingValue)}";
            }
            // CheckType.CheckTypeがEmptyの場合
            if (CheckTypeString == CheckTypeEmpty) {
                result = $"{PythonAILibStringResourcesJa.Instance.SettingValueIsEmpty(SettingItem)}";
            }
            // CheckType.CheckTypeがCheckBoxの場合
            if (CheckTypeString == CheckTypeCheckBox) {
                result = $"{PythonAILibStringResourcesJa.Instance.SettingValueIsChecked(SettingItem, SettingValue)}";
            }
            return result;
        }

        // ScreenShotCheckItemのIEnumerableをJSONに変換する
        public static string ToJson(IEnumerable<ScreenShotCheckCondition> items) {
            return JsonSerializer.Serialize(items, JsonUtil.JsonSerializerOptions);
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
