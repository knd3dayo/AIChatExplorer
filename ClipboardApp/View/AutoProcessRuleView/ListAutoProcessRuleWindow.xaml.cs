using System.Windows;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.AutoProcessRuleView {
    /// <summary>
    /// ListAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListAutoProcessRuleWindow : Window {
        public ListAutoProcessRuleWindow() {
            InitializeComponent();
        }
        public static void OpenListAutoProcessRuleWindow(MainWindowViewModel viewModel) {
            ListAutoProcessRuleWindow listAutoProcessRuleWindow = new();
            listAutoProcessRuleWindow.DataContext = new ListAutoProcessRuleWindowViewModel(viewModel);
            listAutoProcessRuleWindow.ShowDialog();
        }
    }
}
