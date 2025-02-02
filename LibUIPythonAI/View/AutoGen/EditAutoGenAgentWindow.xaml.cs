using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.AutoGen;
using LibUIPythonAI.ViewModel.AutoGen;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.View.AutoGen {
    /// <summary>
    /// EditAutoGenAgentWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoGenAgentWindow : Window {
        public EditAutoGenAgentWindow() {
            InitializeComponent();
        }
        public static void OpenWindow(AutoGenAgent autoGenAgent, ObservableCollection<ContentFolderViewModel> rootFolderViewModels, Action afterUpdate) {
            var window = new EditAutoGenAgentWindow();
            window.DataContext = new EditAutoGenAgentViewModel(autoGenAgent, rootFolderViewModels, afterUpdate);
            window.ShowDialog();
        }
    }

}
