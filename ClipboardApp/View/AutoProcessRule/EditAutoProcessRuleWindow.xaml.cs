using System.Windows;
using ClipboardApp.ViewModel.AutoProcess;

namespace ClipboardApp.View.AutoProcessRule {
    /// <summary>
    /// EditAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoProcessRuleWindow : Window {
        public EditAutoProcessRuleWindow() {
            InitializeComponent();
        }
        public static void OpenEditAutoProcessRuleWindow(PythonAILib.Model.AutoProcess.AutoProcessRule autoProcessRule, Action<PythonAILib.Model.AutoProcess.AutoProcessRule> afterUpdate) {
            EditAutoProcessRuleWindow editAutoProcessRuleWindow = new() {
                DataContext = new EditAutoProcessRuleWindowViewModel(autoProcessRule, afterUpdate)
            };
            editAutoProcessRuleWindow.ShowDialog();
        }
    }
}
