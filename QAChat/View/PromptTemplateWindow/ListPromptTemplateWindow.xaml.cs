using System.Windows;
using PythonAILib.Model;
using WpfAppCommon.Model;

namespace QAChat.View.PromptTemplateWindow {
    /// <summary>
    /// ListAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListPromptTemplateWindow : Window {
        public ListPromptTemplateWindow() {
            InitializeComponent();
        }

        public static void OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum actionModeEum, Action<PromptItemViewModel, OpenAIExecutionModeEnum> callback) {
            ListPromptTemplateWindow listPromptTemplateWindow = new();
            ListPromptTemplateWindowViewModel listPromptTemplateWindowViewModel = (ListPromptTemplateWindowViewModel)listPromptTemplateWindow.DataContext;
            listPromptTemplateWindowViewModel.Initialize(actionModeEum, callback);
            listPromptTemplateWindow.ShowDialog();
        }
    }
}
