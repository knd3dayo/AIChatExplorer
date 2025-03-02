using System.Collections.ObjectModel;
using System.Windows;
using LibUIPythonAI.ViewModel.AutoProcess;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.View.AutoProcessRule {
    /// <summary>
    /// EditAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoProcessRuleWindow : Window {
        public EditAutoProcessRuleWindow() {
            InitializeComponent();
        }
        public static void OpenEditAutoProcessRuleWindow(LibPythonAI.Model.AutoProcess.AutoProcessRule autoProcessRule, ObservableCollection<ContentFolderViewModel> rootFolderViewModel, Action<LibPythonAI.Model.AutoProcess.AutoProcessRule> afterUpdate) {
            EditAutoProcessRuleWindow editAutoProcessRuleWindow = new() {
                DataContext = new EditAutoProcessRuleWindowViewModel(autoProcessRule, rootFolderViewModel, afterUpdate)
            };
            editAutoProcessRuleWindow.ShowDialog();
        }
    }
}
