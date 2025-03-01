using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using PythonAILib.Model.Statistics;

namespace LibPythonAI.Data {
    public class MainStatisticsEntity {
        private static JsonSerializerOptions jsonSerializerOptions = new() {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
            WriteIndented = true
        };

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // DailyStatisticsJson
        public string DailyStatisticsJson { get; set; } = "[]";

        // 日毎のStatistics
        [NotMapped]
        public Dictionary<DateTime, DailyStatistics> DailyStatistics {
            get {
                Dictionary<DateTime, DailyStatistics>? items = JsonSerializer.Deserialize<Dictionary<DateTime, DailyStatistics>>(DailyStatisticsJson, jsonSerializerOptions);
                return items ?? [];
            }
            set {
                DailyStatisticsJson = JsonSerializer.Serialize(value, jsonSerializerOptions);
            }

        }
    }
}
