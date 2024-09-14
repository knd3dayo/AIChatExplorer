using System.Windows;
using PythonAILib.Model;
using QAChat.ViewModel.ImageChat;

namespace QAChat.View.ImageChat
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
            ScreenShotCheckPromptWindowViewModel viewModel = (ScreenShotCheckPromptWindowViewModel)screenShotCheckPromptWindow.DataContext;
            viewModel.Initialize(conditions, action);

            screenShotCheckPromptWindow.ShowDialog();
        }
    }
}
