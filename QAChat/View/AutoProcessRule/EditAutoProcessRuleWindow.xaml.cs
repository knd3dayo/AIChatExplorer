using System.Windows;
using QAChat.ViewModel.AutoProcess;
using QAChat.ViewModel.Folder;

namespace QAChat.View.AutoProcessRule {
    /// <summary>
    /// EditAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoProcessRuleWindow : Window {
        public EditAutoProcessRuleWindow() {
            InitializeComponent();
        }
        public static void OpenEditAutoProcessRuleWindow(PythonAILib.Model.AutoProcess.AutoProcessRule autoProcessRule, ContentFolderViewModel rootFolderViewModel, Action<PythonAILib.Model.AutoProcess.AutoProcessRule> afterUpdate) {
            EditAutoProcessRuleWindow editAutoProcessRuleWindow = new() {
                DataContext = new EditAutoProcessRuleWindowViewModel(autoProcessRule, rootFolderViewModel, afterUpdate)
            };
            editAutoProcessRuleWindow.ShowDialog();
        }
    }
}
