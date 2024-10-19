using System.Windows;
using PythonAILib.Model.Statistics;

namespace QAChat.Control.StatusMessage {
    public class StatisticsMessageWindowViewModel : StatusMessageWindowViewModel {


        public StatisticsMessageWindowViewModel() {
            // MainStatisticsを取得
            MainStatistics mainStatistics = MainStatistics.GetMainStatistics();
            // 本日のトークン数
            long totalTokens = mainStatistics.GetTotalTokens();
            Message = $"総トークン数: {totalTokens} トークン\n\n";
            // 日次トークン数情報
            Message += "日次トークン数\n";
            Dictionary<DateTime, DailyStatistics> keyValuePairs = mainStatistics.DailyStatistics;
            // 日毎のトークン数を表示
            foreach (KeyValuePair<DateTime, DailyStatistics> pair in keyValuePairs) {
                DailyStatistics dailyStatistics = pair.Value;
                Message += $"{dailyStatistics.Date.ToShortDateString()}のトークン数: {dailyStatistics.TotalTokens} トークン\n";
            }

            // クリアボタンを非表示にする
            ClearButtonVisibility = Visibility.Collapsed;
        }

    }
}
