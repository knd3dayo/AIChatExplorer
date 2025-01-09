using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.AutoGen;
using QAChat.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.AutoGen {
    public class EditAutoGenNormalChatViewModel : QAChatViewModelBase {


        public EditAutoGenNormalChatViewModel(AutoGenNormalChat autoGenNormalChat, Action afterUpdate) {

            AutoGenNormalChat = autoGenNormalChat;
            AfterUpdate = afterUpdate;

            LoadAutoGenAgents();
        }

        public AutoGenNormalChat AutoGenNormalChat { get; set; }

        public Action AfterUpdate { get; set; }
        // Name
        public string Name {
            get => AutoGenNormalChat.Name;
            set {
                AutoGenNormalChat.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        // Description
        public string Description {
            get => AutoGenNormalChat.Description;
            set {
                AutoGenNormalChat.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        // InitAgentName
        public string InitAgentName {
            get => AutoGenNormalChat.InitAgentName;
            set {
                AutoGenNormalChat.InitAgentName = value;
                OnPropertyChanged(nameof(InitAgentName));
            }
        }

        // AutoGenAgents
        public ObservableCollection<AutoGenAgentViewModel> AutoGenAgents { get; set; } = [];

        // SelectedIndex
        private int _selectedTabIndex = 0;
        public int SelectedTabIndex {
            get => _selectedTabIndex;
            set {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }

        // InitAgent
        public AutoGenAgentViewModel? InitAgent { get; set; }

        public void LoadAutoGenAgents() {
            // AutoGenAgentのリストを取得
            ObservableCollection<AutoGenAgentViewModel> autoGenAgents = [];
            foreach (AutoGenAgent item in AutoGenAgent.FindAll()) {
                AutoGenAgentViewModel autoGenAgentViewModel = new(item);
                // itemのAgentNamesにAutoGenNormalChatのAgentNamesが含まれている場合はIsCheckedをTrueにする
                if (AutoGenNormalChat.AgentNames.Contains(item.Name)) {
                    autoGenAgentViewModel.IsChecked = true;
                } else {
                    autoGenAgentViewModel.IsChecked = false;
                }
                autoGenAgents.Add(autoGenAgentViewModel);
            }
            AutoGenAgents = autoGenAgents;
            OnPropertyChanged(nameof(AutoGenAgents));

            // InitAgentNameからAutoGenAgentを取得
            InitAgent = AutoGenAgents.Select((item) => item).FirstOrDefault((item) => item.AutoGenAgent.Name == AutoGenNormalChat.InitAgentName);
            OnPropertyChanged(nameof(InitAgent));

        }


        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            // AutoGenNormalChatのAgentNamesを更新
            AutoGenNormalChat.AgentNames = [];
            foreach (AutoGenAgentViewModel item in AutoGenAgents) {
                if (item.IsChecked) {
                    AutoGenNormalChat.AgentNames.Add(item.AutoGenAgent.Name);
                }
            }
            // InitAgentNameを更新
            if (InitAgent != null) { 
                AutoGenNormalChat.InitAgentName = InitAgent.AutoGenAgent.Name;
            }
            AutoGenNormalChat.Save();
            AfterUpdate();

            window.Close();

        });

    }
}
