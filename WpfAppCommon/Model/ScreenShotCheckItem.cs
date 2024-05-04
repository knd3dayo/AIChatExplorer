using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace WpfAppCommon.Model {

    public class ScreenShotCheckResults {
        [JsonPropertyName("results")]
        public List<ScreenShotCheckItem> Results { get; set; } = [];

        public static List<ScreenShotCheckItem> FromJson(string json) {
            List<ScreenShotCheckItem> result = [];
            var jsonObject = JsonSerializer.Deserialize<ScreenShotCheckResults>(json);
            if (jsonObject == null) {
                return result;
            }
            var results = jsonObject.Results;
            foreach (var item in results) {
                result.Add(item);
            }
            return result;

        }
    }
    public class ScreenShotCheckItem {
        // 結果のEnum
        public enum ResultEnum {
            Empty,
            OK,
            NG,
            Unknown,
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
        // 結果
        private ResultEnum _result = ResultEnum.Empty;
        [JsonIgnore]
        public ResultEnum Result {
            get {
                return _result;
            }
            set {
                _result = value;
            }
        }
        // 結果の文字列
        [JsonPropertyName("result_string")]
        public string ResultString {
            get {
                return _result switch {
                    ResultEnum.Empty => "",
                    ResultEnum.OK => "OK",
                    ResultEnum.NG => "NG",
                    ResultEnum.Unknown => "Unknown",
                    _ => "",
                };

            }set {
                _result = value switch {
                    "OK" => ResultEnum.OK,
                    "NG" => ResultEnum.NG,
                    "Unknown" => ResultEnum.Unknown,
                    _ => ResultEnum.Empty,
                };
            }
        }
        // 画像データ内の実際の設定値
        private string _actualValue = "";
        [JsonPropertyName("actual_value")]
        public string ActualValue {
            get {
                return _actualValue;
            }
            set {
                _actualValue = value;
            }
        }



        // ScreenShotCheckItemのIEnumerableをJSONに変換する
        public static string ToJson(IEnumerable<ScreenShotCheckItem> items) {
            JsonSerializerOptions jsonSerializerOptions = new() {
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                WriteIndented = true
            };
            JsonSerializerOptions options = jsonSerializerOptions;
            return System.Text.Json.JsonSerializer.Serialize(items, options);
        }
        // JSONをScreenShotCheckItemのListに変換する
        public static List<ScreenShotCheckItem> FromJson(string json) {
            List<ScreenShotCheckItem> result = [];
            var jsonObject = JsonSerializer.Deserialize<List<ScreenShotCheckItem>>(json);
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
