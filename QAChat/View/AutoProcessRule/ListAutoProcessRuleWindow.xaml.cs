using System.Windows;
using QAChat.ViewModel.AutoProcess;
using QAChat.ViewModel.Folder;

namespace QAChat.View.AutoProcessRule {
    /// <summary>
    /// ListAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ListAutoProcessRuleWindow : Window {
        public ListAutoProcessRuleWindow() {
            InitializeComponent();
        }
        public static void OpenListAutoProcessRuleWindow(ContentFolderViewModel viewModel) {
            ListAutoProcessRuleWindow listAutoProcessRuleWindow = new();
            listAutoProcessRuleWindow.DataContext = new ListAutoProcessRuleWindowViewModel(viewModel);
            listAutoProcessRuleWindow.ShowDialog();
        }
    }
}
