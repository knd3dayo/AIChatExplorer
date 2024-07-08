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
using ImageChat.ViewModel;
using WpfAppCommon.Model;

namespace ImageChat.View
{
    /// <summary>
    /// RagManagementWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ScreenShotCheckPromptWindow : Window {
        public ScreenShotCheckPromptWindow() {
            InitializeComponent();
        }

        public static void OpenScreenShotCheckPromptWindow(List<ScreenShotCheckICondition> conditions, Action<List<ScreenShotCheckICondition>> action) {
            ScreenShotCheckPromptWindow screenShotCheckPromptWindow = new();
            ScreenShotCheckPromptWindowViewModel viewModel = (ScreenShotCheckPromptWindowViewModel)screenShotCheckPromptWindow.DataContext;
            viewModel.Initialize(conditions, action);

            screenShotCheckPromptWindow.ShowDialog();
        }
    }
}
