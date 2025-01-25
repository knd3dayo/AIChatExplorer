using System.Windows;
using PythonAILib.Model.Image;
using ImageChat.ViewModel;

namespace ImageChat.View
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
