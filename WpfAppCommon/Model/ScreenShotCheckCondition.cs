using System.Collections.ObjectModel;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace WpfAppCommon.Model {

    public class ScreenShotCheckCondition {

        public static readonly string CheckTypeEqual = "等しい";
        public static readonly string CheckTypeNotEqual = "等しくない";
        public static readonly string CheckTypeInclude = "含む";
        public static readonly string CheckTypeNotInclude = "含まない";
        public static readonly string CheckTypeStartWith = "開始している";
        public static readonly string CheckTypeNotStartWith = "開始していない";
        public static readonly string CheckTypeEndWith = "終わっている";
        public static readonly string CheckTypeNotEndWith = "終わっていない";
        public static readonly string CheckTypeEmpty = "空である";
        public static readonly string CheckTypeCheckBox = "チェックボックス";

        public ObservableCollection<string> CheckTypeList {
            get {
                // チェック用の定数のリストを返す
                return new() {
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
                };
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
            string result = $"{SettingItem}の値は{SettingValue}である";
            // CheckType.CheckTypeがEqualの場合
            if (CheckTypeString == CheckTypeEqual) {
                result = $"{SettingItem}の値は{SettingValue}である";
            }
            // CheckType.CheckTypeがNotEqualの場合
            if (CheckTypeString == CheckTypeNotEqual) {
                result = $"{SettingItem}の値は{SettingValue}でない";
            }
            // CheckType.CheckTypeがIncludeの場合
            if (CheckTypeString == CheckTypeInclude) {
                result = $"{SettingItem}の値に{SettingValue}が含まれている";
            }
            // CheckType.CheckTypeがNotIncludeの場合
            if (CheckTypeString == CheckTypeNotInclude) {
                result = $"{SettingItem}の値に{SettingValue}が含まれていない";
            }
            // CheckType.CheckTypeがStartWithの場合
            if (CheckTypeString == CheckTypeStartWith) {
                result = $"{SettingItem}の値が{SettingValue}で始まっている";
            }
            // CheckType.CheckTypeがNotStartWithの場合
            if (CheckTypeString == CheckTypeNotStartWith) {
                result = $"{SettingItem}の値が{SettingValue}で始まっていない";
            }
            // CheckType.CheckTypeがEndWithの場合
            if (CheckTypeString == CheckTypeEndWith) {
                result = $"{SettingItem}の値が{SettingValue}で終わっている";
            }
            // CheckType.CheckTypeがNotEndWithの場合
            if (CheckTypeString == CheckTypeNotEndWith) {
                result = $"{SettingItem}の値が{SettingValue}で終わっていない";
            }
            // CheckType.CheckTypeがEmptyの場合
            if (CheckTypeString == CheckTypeEmpty) {
                result = $"{SettingItem}の値が空である";
            }
            // CheckType.CheckTypeがCheckBoxの場合
            if (CheckTypeString == CheckTypeCheckBox) {
                result = $"{SettingItem}のチェックボックスが{SettingValue}になっている";
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
