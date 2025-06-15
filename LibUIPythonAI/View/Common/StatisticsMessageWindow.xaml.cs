using System.Windows;
using System.Windows.Controls;
using LibUIPythonAI.ViewModel.Common;

namespace LibUIPythonAI.View.Common {
    /// <summary>
    /// StatusMessageWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StatisticsMessageWindow : UserControl {

        public StatisticsMessageWindow() {
            InitializeComponent();
        }

        public static void OpenStatusMessageWindow(string title, StatisticsMessageWindowViewModel viewModel) {
            StatisticsMessageWindow userControl = new() {
                DataContext = viewModel
            };
            Window window = new() {
                Title = title,
                Content = userControl,
                Width = 800,
                Height = 450
            };

            window.ShowDialog();
        }

    }
}
