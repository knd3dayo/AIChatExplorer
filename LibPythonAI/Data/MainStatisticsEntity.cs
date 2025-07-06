using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using LibPythonAI.Model.Statistics;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Data {
    public class MainStatisticsEntity {

        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        // DailyStatisticsJson
        public string DailyStatisticsJson { get; set; } = "[]";

        // 日毎のStatistics
        [NotMapped]
        public Dictionary<DateTime, DailyStatistics> DailyStatistics {
            get {
                Dictionary<DateTime, DailyStatistics>? items = JsonSerializer.Deserialize<Dictionary<DateTime, DailyStatistics>>(DailyStatisticsJson, JsonUtil.JsonSerializerOptions);
                return items ?? [];
            }
            set {
                DailyStatisticsJson = JsonSerializer.Serialize(value, JsonUtil.JsonSerializerOptions);
            }

        }
    }
}
