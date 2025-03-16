using System.Windows;
using System.Windows.Controls;
using LibUIPythonAI.Resource;

namespace AIChatExplorer.View.Settings {
    /// <summary>
    /// SimpleSettingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SettingsUserControl : UserControl {
        public SettingsUserControl() {
            InitializeComponent();
        }

        public static void OpenSettingsWindow() {
            SettingsUserControl settingsControl = new();
            Window window = new() {
                Title = CommonStringResources.Instance.SettingWindowTitle,
                Content = settingsControl,
                Margin = new Thickness(0,0,0,0)
                
            };
            window.ShowDialog();
        }
    }
}
