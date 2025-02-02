using System.Windows;
using PythonAILib.Model.Statistics;

namespace LibUIPythonAI.ViewModel.Common {
    public class StatisticsMessageWindowViewModel : StatusMessageWindowViewModel {
        public StatisticsMessageWindowViewModel() {
            Message = MainStatistics.GetStatisticsMessage();
            // クリアボタンを非表示にする
            ClearButtonVisibility = Visibility.Collapsed;
        }

    }
}
