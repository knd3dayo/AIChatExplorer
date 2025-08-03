using System.Windows;
using AIChatExplorer.ViewModel.Settings;

namespace AIChatExplorer.View.Settings {
    /// <summary>
    /// SimpleSettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingWindow : Window {
        public SettingWindow() {
            InitializeComponent();
        }

        public static void OpenSettingsWindow() {
            SettingWindow settingsControl = new() {
                DataContext = new SettingUserControlViewModel()
            };
            settingsControl.ShowDialog();
        }
    }
}
