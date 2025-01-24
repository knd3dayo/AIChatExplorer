using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.AutoGen;
using QAChat.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.AutoGen {
    public class EditAutoGenGroupChatViewModel : QAChatViewModelBase {


        public EditAutoGenGroupChatViewModel(AutoGenGroupChat autoGenGroupChat, Action afterUpdate) {

            AutoGenGroupChat = autoGenGroupChat;
            AfterUpdate = afterUpdate;

            LoadAutoGenAgents();
        }

        public AutoGenGroupChat AutoGenGroupChat { get; set; }

        public Action AfterUpdate { get; set; }
        // Name
        public string Name {
            get => AutoGenGroupChat.Name;
            set {
                AutoGenGroupChat.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        // Description
        public string Description {
            get => AutoGenGroupChat.Description;
            set {
                AutoGenGroupChat.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        // InitAgentName
        public string LLMConfigName {
            get => AutoGenGroupChat.LLMConfigName;
            set {
                AutoGenGroupChat.LLMConfigName = value;
                OnPropertyChanged(nameof(LLMConfigName));
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
        public AutoGenLLMConfig? LLMConfig { get; set; }

        public void LoadAutoGenAgents() {
            // AutoGenAgentのリストを取得
            ObservableCollection<AutoGenAgentViewModel> autoGenAgents = [];
            foreach (AutoGenAgent item in AutoGenAgent.GetAutoGenAgentList()) {
                AutoGenAgentViewModel autoGenAgentViewModel = new(item);
                // itemのAgentNamesにAutoGenGroupChatのAgentNamesが含まれている場合はIsCheckedをTrueにする
                if (AutoGenGroupChat.AgentNames.Contains(item.Name)) {
                    autoGenAgentViewModel.IsChecked = true;
                } else {
                    autoGenAgentViewModel.IsChecked = false;
                }
                autoGenAgents.Add(autoGenAgentViewModel);
            }
            AutoGenAgents = autoGenAgents;
            OnPropertyChanged(nameof(AutoGenAgents));

            // InitAgentNameからAutoGenAgentを取得
            LLMConfig = AutoGenLLMConfig.GetAutoGenLLMConfigList().FirstOrDefault(x => x.Name == AutoGenGroupChat.LLMConfigName);
            OnPropertyChanged(nameof(LLMConfig));

        }


        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            // AutoGenGroupChatのAgentNamesを更新
            AutoGenGroupChat.AgentNames = [];
            foreach (AutoGenAgentViewModel item in AutoGenAgents) {
                if (item.IsChecked) {
                    AutoGenGroupChat.AgentNames.Add(item.AutoGenAgent.Name);
                }
            }
            // LLMConfigを更新
            if (LLMConfig != null) { 
                AutoGenGroupChat.LLMConfigName = LLMConfig.Name;
            }
            AutoGenGroupChat.Save();
            AfterUpdate();

            window.Close();

        });

    }
}
