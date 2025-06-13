using System.Windows;
using LibUIPythonAI.Resource;
using AIChatExplorer.View.Main;
using AIChatExplorer.ViewModel.Settings;
using AIChatExplorer.ViewModel.Main;
using LibPythonAI.PythonIF;

namespace AIChatExplorer {
    /// <summary>
    /// StartupWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class StartupWindow : Window {
        public StartupWindow() {
            InitializeComponent();
            StartupWindowViewModel startupWindowViewModel = new ();
            this.DataContext = startupWindowViewModel;

            startupWindowViewModel.Startup(this);
        }
    }
}
