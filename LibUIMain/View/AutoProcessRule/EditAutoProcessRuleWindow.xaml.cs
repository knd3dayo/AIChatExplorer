using System.Collections.ObjectModel;
using System.Windows;
using LibUIMain.ViewModel.AutoProcess;
using LibUIMain.ViewModel.Folder;

namespace LibUIMain.View.AutoProcessRule {
    /// <summary>
    /// EditAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoProcessRuleWindow : Window {
        public EditAutoProcessRuleWindow() {
            InitializeComponent();
        }
        public static void OpenEditAutoProcessRuleWindow(
            LibMain.Model.AutoProcess.AutoProcessRule autoProcessRule, ObservableCollection<ContentFolderViewModel> rootFolderViewModel, Action<LibMain.Model.AutoProcess.AutoProcessRule> afterUpdate) {
            EditAutoProcessRuleWindow editAutoProcessRuleWindow = new() {
                DataContext = new EditAutoProcessRuleWindowViewModel(autoProcessRule, rootFolderViewModel, afterUpdate)
            };
            editAutoProcessRuleWindow.ShowDialog();
        }
    }
}
