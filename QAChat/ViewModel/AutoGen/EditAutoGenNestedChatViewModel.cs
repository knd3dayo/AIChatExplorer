using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.AutoGen;
using QAChat.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.AutoGen {
    public class EditAutoGenNestedChatViewModel : QAChatViewModelBase {


        public EditAutoGenNestedChatViewModel(AutoGenNestedChat autoGenNestedChat, Action afterUpdate) {

            AutoGenNestedChat = autoGenNestedChat;
            AfterUpdate = afterUpdate;

            // ★ TODO LoadGroupChatList, LoadNormalChatListを実装
            // LoadAutoGenAgents();
        }

        public AutoGenNestedChat AutoGenNestedChat { get; set; }

        public Action AfterUpdate { get; set; }
        // Name
        public string Name {
            get => AutoGenNestedChat.Name;
            set {
                AutoGenNestedChat.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        // Description
        public string Description {
            get => AutoGenNestedChat.Description;
            set {
                AutoGenNestedChat.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        // InitAgentName
        public string InitAgentName {
            get => AutoGenNestedChat.InitAgentName;
            set {
                AutoGenNestedChat.InitAgentName = value;
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

        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            // AutoGenNestedChatのAgentNamesを更新


            // InitAgentNameを更新
            if (InitAgent != null) { 
                AutoGenNestedChat.InitAgentName = InitAgent.AutoGenAgent.Name;
            }
            AutoGenNestedChat.Save();
            AfterUpdate();

            window.Close();

        });

    }
}
