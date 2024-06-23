using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfAppCommon.Model;

namespace WpfAppCommon.Control.Settings {
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
                Content = settingsControl
            };
            window.ShowDialog();
        }
    }
}
