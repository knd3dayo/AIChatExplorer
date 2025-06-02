using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Model.AutoGen;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;

namespace LibUIPythonAI.ViewModel.AutoGen {
    public class EditAutoGenGroupChatViewModel : CommonViewModelBase {


        public EditAutoGenGroupChatViewModel(AutoGenGroupChat autoGenGroupChat, Action afterUpdate) {

            AutoGenGroupChat = autoGenGroupChat;
            AfterUpdate = afterUpdate;

            Task.Run(async () => {
                await LoadAutoGenAgentsAsync();
                await LoadLLMConfigAsync();
            });
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
        // LLMConfigList
        private ObservableCollection<AutoGenLLMConfig> _LLMConfigList = new();
        public ObservableCollection<AutoGenLLMConfig> LLMConfigList {
            get => _LLMConfigList;
            set {
                _LLMConfigList = value;
                OnPropertyChanged(nameof(LLMConfigList));
            }
        }

        // InitAgent
        private AutoGenLLMConfig? _llmConfig;
        public AutoGenLLMConfig? LLMConfig {
            get => _llmConfig;
            set {
                _llmConfig = value;
                OnPropertyChanged(nameof(LLMConfig));
            }
        }

        public async Task LoadAutoGenAgentsAsync() {
            // AutoGenAgentのリストを取得
            ObservableCollection<AutoGenAgentViewModel> autoGenAgents = [];
            var list = await AutoGenAgent.GetAutoGenAgentList();
            foreach (AutoGenAgent item in list) {
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
        }
        // LLMConfigを読み込む
        public async Task LoadLLMConfigAsync() {
            List<AutoGenLLMConfig> list = await AutoGenLLMConfig.GetAutoGenLLMConfigListAsync();
            var config  = list.FirstOrDefault(x => x.Name == LLMConfigName);

            // MainUIスレッドで実行する
            MainUITask.Run(() => {
                if (config != null) {
                    LLMConfig = config;
                }
            });
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
            Task.Run(async () => {
                // AutoGenGroupChatを保存
                await AutoGenGroupChat.SaveAsync();
            });
            AfterUpdate();

            window.Close();

        }, null, null);

    }
}
