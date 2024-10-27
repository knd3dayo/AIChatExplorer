using LiteDB;
using PythonAILib.Resource;
using QAChat;

namespace PythonAILib.Model.Statistics {
    public class MainStatistics {

        public static MainStatistics GetMainStatistics() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            return libManager.DataFactory.GetStatistics();
        }

        // Id
        public ObjectId Id { get; set; } = ObjectId.NewObjectId();
        // 日毎のStatistics
        public Dictionary<DateTime, DailyStatistics> DailyStatistics { get; set; } = [];


        // 本日のStatisticsを取得するメソッド
        public DailyStatistics GetTodayStatistics() {
            DateTime today = DateTime.Today;
            if (DailyStatistics.ContainsKey(today)) {
                return DailyStatistics[today];
            }
            DailyStatistics todayStatistics = new() {
                Date = today
            };
            DailyStatistics.Add(today, todayStatistics);
            return todayStatistics;
        }
        // 指定した日のStatisticsを取得するメソッド
        public DailyStatistics GetDailyStatistics(DateTime date) {
            // dateの年月日のみを取得
            DateTime dateOnly = date.Date;
            if (DailyStatistics.ContainsKey(dateOnly)) {
                return DailyStatistics[dateOnly];
            }
            // dateOnlyのStatisticsが存在しない場合は新規作成
            DailyStatistics newStatistics = new() {
                Date = dateOnly
            };
            return newStatistics;
        }
        // DailyStatisticsを更新するメソッド
        public void UpdateDailyStatistics(DailyStatistics dailyStatistics) {
            DateTime date = dailyStatistics.Date;
            DailyStatistics[date] = dailyStatistics;
        }
        // 本日のトークン数を追加するメソッド
        public void AddTodayTokens(long tokens, string modelName) {
            DailyStatistics todayStatistics = GetTodayStatistics();
            todayStatistics.TotalTokens += tokens;
            // modelNameのトークン数を追加
            if (todayStatistics.TokenCounts.ContainsKey(modelName)) {
                todayStatistics.TokenCounts[modelName] += tokens;
            } else {
                todayStatistics.TokenCounts[modelName] = tokens;
            }
            // 本日のStatisticsを更新
            UpdateDailyStatistics(todayStatistics);
            // 保存
            Save();
        }

        // トータルトークン数を取得するメソッド
        public long GetTotalTokens() {
            long totalTokens = 0;
            foreach (DailyStatistics dailyStatistics in DailyStatistics.Values) {
                totalTokens += dailyStatistics.TotalTokens;
            }
            return totalTokens;
        }

        // Save
        public void Save() {
            PythonAILibManager libManager = PythonAILibManager.Instance ?? throw new Exception(PythonAILibStringResources.Instance.PythonAILibManagerIsNotInitialized);
            libManager.DataFactory.UpsertStatistics(this);
        }

        
    }
}
