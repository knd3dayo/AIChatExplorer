using System.Windows;
using LibUIImageChat.ViewModel;
using LibMain.Model.Image;

namespace LibUIImageChat.View
{
    /// <summary>
    /// RagManagementWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ScreenShotCheckPromptWindow : Window {
        public ScreenShotCheckPromptWindow() {
            InitializeComponent();
        }

        public static void OpenScreenShotCheckPromptWindow(List<ScreenShotCheckCondition> conditions, Action<List<ScreenShotCheckCondition>> action) {
            ScreenShotCheckPromptWindow screenShotCheckPromptWindow = new();
            screenShotCheckPromptWindow.DataContext = new ScreenShotCheckPromptWindowViewModel(conditions, action);

            screenShotCheckPromptWindow.ShowDialog();
        }
    }
}
