namespace LibPythonAI.Model.Statistics {

    // 日次の統計情報
    public class DailyStatistics {

        public DateTime Date { get; set; }

        public long TotalTokens { get; set; }

        // モデル毎のトークン数
        public Dictionary<string, long> TokenCounts { get; set; } = [];

    }
}
