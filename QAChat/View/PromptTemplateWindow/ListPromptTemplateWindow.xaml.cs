using System.Windows;
using PythonAILib.Model;
using QAChat.ViewModel;
using WpfAppCommon.Model;
using WpfAppCommon.Model.QAChat;

namespace QAChat.View.PromptTemplateWindow
{
    /// <summary>
    /// ListAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListPromptTemplateWindow : Window {
        public ListPromptTemplateWindow() {
            InitializeComponent();
        }

        public static void OpenListPromptTemplateWindow(
            ListPromptTemplateWindowViewModel.ActionModeEum actionModeEum, Action<PromptItemViewModel, OpenAIExecutionModeEnum> callback, Func<PromptItemBase> createPromptItemFunction) {
            ListPromptTemplateWindow listPromptTemplateWindow = new();
            ListPromptTemplateWindowViewModel listPromptTemplateWindowViewModel = new (actionModeEum, callback, createPromptItemFunction);
            listPromptTemplateWindow.DataContext = listPromptTemplateWindowViewModel;
            listPromptTemplateWindow.ShowDialog();
        }
    }
}
