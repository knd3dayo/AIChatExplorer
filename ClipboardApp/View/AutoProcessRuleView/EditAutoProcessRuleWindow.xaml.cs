using System.Windows;
using ClipboardApp.Model.AutoProcess;
using ClipboardApp.ViewModel.AutoProcess;

namespace ClipboardApp.View.AutoProcessRuleView
{
    /// <summary>
    /// EditAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoProcessRuleWindow : Window {
        public EditAutoProcessRuleWindow() {
            InitializeComponent();
        }
        public static void OpenEditAutoProcessRuleWindow(EditAutoProcessRuleWindowViewModel.Mode mode, MainWindowViewModel? mainWindowViewModel, AutoProcessRule? autoProcessRule, Action<AutoProcessRule> afterUpdate) {
            EditAutoProcessRuleWindow editAutoProcessRuleWindow = new() {
                DataContext = new EditAutoProcessRuleWindowViewModel(mode, mainWindowViewModel, autoProcessRule, afterUpdate)
            };
            editAutoProcessRuleWindow.ShowDialog();
        }
    }
}
