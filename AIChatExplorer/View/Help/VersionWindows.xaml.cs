using System.Windows;
using MaterialDesignThemes.Wpf;

namespace AIChatExplorer.View.Help {
    /// <summary>
    /// VersionWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class VersionWindow : Window {
        public VersionWindow() {
            InitializeComponent();
        }

        public static void OpenVersionWindow() {


            var versionWindow = new VersionWindow();
            versionWindow.ShowDialog();
        }
    }
}
