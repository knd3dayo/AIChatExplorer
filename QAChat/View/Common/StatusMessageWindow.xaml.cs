using System.Windows;
using System.Windows.Controls;
using QAChat.ViewModel.Common;

namespace QAChat.View.Common {
    /// <summary>
    /// StatusMessageWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StatusMessageWindow : UserControl {

        public StatusMessageWindow() {
            InitializeComponent();
        }

        public static void OpenStatusMessageWindow(string title, StatusMessageWindowViewModel viewModel) {
            StatusMessageWindow userControl = new() {
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
