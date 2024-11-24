using System.Windows;
using PythonAILib.Model.Statistics;

namespace QAChat.View.Common.StatusMessage
{
    public class StatisticsMessageWindowViewModel : StatusMessageWindowViewModel
    {


        public StatisticsMessageWindowViewModel()
        {
            // MainStatisticsを取得
            MainStatistics mainStatistics = MainStatistics.GetMainStatistics();
            // 本日のトークン数
            long totalTokens = mainStatistics.GetTotalTokens();
            Message = PythonAILib.Resource.PythonAILibStringResources.Instance.TotalTokenFormat(totalTokens) + "\n\n";
            // 日次トークン数情報
            Message += PythonAILib.Resource.PythonAILibStringResources.Instance.DailyTokenCount + "\n";
            Dictionary<DateTime, DailyStatistics> keyValuePairs = mainStatistics.DailyStatistics;
            // 日毎のトークン数を表示
            foreach (KeyValuePair<DateTime, DailyStatistics> pair in keyValuePairs)
            {
                DailyStatistics dailyStatistics = pair.Value;
                string dailyMessage = PythonAILib.Resource.PythonAILibStringResources.Instance.DailyTokenFormat(dailyStatistics.Date.ToShortDateString(), dailyStatistics.TotalTokens);
                Message += dailyMessage + "\n";
            }

            // クリアボタンを非表示にする
            ClearButtonVisibility = Visibility.Collapsed;
        }

    }
}
