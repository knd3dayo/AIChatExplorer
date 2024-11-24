using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonAILib.Model.AutoGen;
using QAChat.Model;

namespace QAChat.ViewModel.AutoGen {
    public class EditAutoGenViewModel : QAChatViewModelBase{

        public ObservableCollection<AutoGenAgent> AutoGenAgents { get; set; } = [];

        public ObservableCollection<AutoGenTool> AutoGenTools { get; set; } = [];

        public ObservableCollection<AutoGenGroupChat> AutoGenGroupChats { get; set; } = [];

        public EditAutoGenViewModel() {

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

    }
}
