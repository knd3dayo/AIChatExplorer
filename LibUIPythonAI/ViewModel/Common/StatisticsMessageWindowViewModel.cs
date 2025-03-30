using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.Statistics;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;

namespace LibUIPythonAI.ViewModel.Common {
    public class StatisticsMessageWindowViewModel : ObservableObject {
        public StatisticsMessageWindowViewModel() {
            Message = MainStatistics.GetStatisticsMessage();
            // クリアボタンを非表示にする
            ClearButtonVisibility = Visibility.Collapsed;
        }
        private string _message = string.Empty;
        public string Message {
            get { return _message; }
            set { _message = value; OnPropertyChanged(nameof(Message)); }
        }

        public CommonStringResources StringResources { get; set; } = CommonStringResources.Instance;

        // クリアボタンのVisible
        public Visibility ClearButtonVisibility { get; set; } = Visibility.Visible;


        public SimpleDelegateCommand<Window> CloseCommand => new((window) => {
            // ウィンドウを閉じる
            window.Close();

        });
    }
}
