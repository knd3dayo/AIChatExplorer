using System.Collections.ObjectModel;
using PythonAILib.Model.AutoGen;
using QAChat.Model;

namespace QAChat.ViewModel.AutoGen {
    public class ListAutoGenItemWindowViewModel : QAChatViewModelBase {

        public ObservableCollection<AutoGenAgent> AutoGenAgents { get; set; } = [];

        public ObservableCollection<AutoGenTool> AutoGenTools { get; set; } = [];

        public ObservableCollection<AutoGenGroupChat> AutoGenGroupChats { get; set; } = [];

        public ListAutoGenItemWindowViewModel() {

            // Load AutoGenAgents
            var autoGenAgents = AutoGenAgent.FindAll();
            foreach (var item in autoGenAgents) {
                AutoGenAgents.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenAgents));

            // Load AutoGenTools
            var autoGenTools = AutoGenTool.FindAll();
            foreach (var item in autoGenTools) {
                AutoGenTools.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenTools));

            // Load AutoGenGroupChats
            var autoGenGroupChats = AutoGenGroupChat.FindAll();
            foreach (var item in autoGenGroupChats) {
                AutoGenGroupChats.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenGroupChats));
        }

        // SelectedAutoGenGroupChat
        public AutoGenGroupChat? SelectedAutoGenGroupChat { get; set; }

        // SelectedAutoGenAgent
        public AutoGenAgent? SelectedAutoGenAgent { get; set; }

        // SelectedAutoGenTool
        public AutoGenTool? SelectedAutoGenTool { get; set; }

    }
}
