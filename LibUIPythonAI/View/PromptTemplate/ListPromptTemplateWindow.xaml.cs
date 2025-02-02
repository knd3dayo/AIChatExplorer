using System.Windows;
using LibUIPythonAI.ViewModel.PromptTemplate;
using PythonAILib.Model.Chat;

namespace LibUIPythonAI.View.PromptTemplate {
    /// <summary>
    /// ListAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListPromptTemplateWindow : Window {
        public ListPromptTemplateWindow() {
            InitializeComponent();
        }

        public static void OpenListPromptTemplateWindow(
            ListPromptTemplateWindowViewModel.ActionModeEum actionModeEum, Action<PromptItemViewModel, OpenAIExecutionModeEnum> callback) {
            ListPromptTemplateWindow listPromptTemplateWindow = new();
            ListPromptTemplateWindowViewModel listPromptTemplateWindowViewModel = new(actionModeEum, callback);
            listPromptTemplateWindow.DataContext = listPromptTemplateWindowViewModel;
            listPromptTemplateWindow.ShowDialog();
        }
    }
}
