using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model.AutoGen;
using QAChat.Model;
using QAChat.View.VectorDB;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.VectorDB;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.AutoGen {
    public class EditAutoGenAgentViewModel : QAChatViewModelBase {
        public EditAutoGenAgentViewModel(AutoGenAgent autoGenAgent, ObservableCollection<ContentFolderViewModel> rootFolderViewModels, Action afterUpdate) {
            AutoGenAgent = autoGenAgent;
            RootFolderViewModels = rootFolderViewModels;
            AfterUpdate = afterUpdate;
            LoadTools();
            // TypeValue
            if (AutoGenAgent.TypeValue == "userproxy") {
                SelectedTypeValueIndex = 0;
            } else if (AutoGenAgent.TypeValue == "assistant") {
                SelectedTypeValueIndex = 1;
            }
            // HumanInputMode
            if (AutoGenAgent.HumanInputMode == "NEVER") {
                SelectedHumanInputModeIndex = 0;
            } else if (AutoGenAgent.HumanInputMode == "ALWAYS") {
                SelectedHumanInputModeIndex = 1;
            } else if (AutoGenAgent.HumanInputMode == "TERMINATE") {
                SelectedHumanInputModeIndex = 2;
            }
        }

        public ObservableCollection<ContentFolderViewModel> RootFolderViewModels { get; set; }

        public AutoGenAgent AutoGenAgent { get; set; }

        public Action AfterUpdate { get; set; }

        // Name
        public string Name {
            get { return AutoGenAgent.Name; }
            set {
                AutoGenAgent.Name = value;
                OnPropertyChanged();
            }
        }
        // Description
        public string Description {
            get { return AutoGenAgent.Description; }
            set {
                AutoGenAgent.Description = value;
                OnPropertyChanged();
            }
        }
        // SystemMessage
        public string SystemMessage {
            get { return AutoGenAgent.SystemMessage; }
            set {
                AutoGenAgent.SystemMessage = value;
                OnPropertyChanged();
            }
        }

        // SelectedTypeValueIndex
        private int _selectedTypeValueIndex = 0;
        public int SelectedTypeValueIndex {
            get { return _selectedTypeValueIndex; }
            set {
                _selectedTypeValueIndex = value;
                OnPropertyChanged();
            }
        }
        // SelectedHumanInputModeIndex
        private int _selectedHumanInputModeIndex = 0;
        public int SelectedHumanInputModeIndex {
            get { return _selectedHumanInputModeIndex; }
            set {
                _selectedHumanInputModeIndex = value;
                OnPropertyChanged();
            }
        }
        // termination_msg
        public string TerminationMsg {
            get { return AutoGenAgent.TerminationMsg; }
            set {
                AutoGenAgent.TerminationMsg = value;
                OnPropertyChanged();
            }
        }
        // CodeExecution
        public bool CodeExecution {
            get { return AutoGenAgent.CodeExecution; }
            set {
                AutoGenAgent.CodeExecution = value;
                OnPropertyChanged();
            }
        }
        // Llm
        public bool Llm {
            get { return AutoGenAgent.Llm; }
            set {
                AutoGenAgent.Llm = value;
                OnPropertyChanged();
            }
        }
        // VectorDBItem
        public ObservableCollection<VectorDBItemViewModel> VectorDBItems { get; set; } = [];

        public VectorDBItemViewModel? SelectedVectorDBItem { get; set; }

        // AutoGenTools
        public ObservableCollection<AutoGenToolViewModel> AutoGenTools { get; set; } = [];


        // SelectedTabIndex
        private int _selectedTabIndex = 0;
        public int SelectedTabIndex {
            get { return _selectedTabIndex; }
            set {
                _selectedTabIndex = value;
                OnPropertyChanged();
            }
        }

        public void LoadTools() {
            // Load tools
            ObservableCollection<AutoGenToolViewModel> autoGenTools = [];
            foreach (AutoGenTool tool in AutoGenTool.FindAll()) {
                AutoGenToolViewModel toolViewModel = new(tool);
                if (AutoGenAgent.ToolNamesForExecution.Contains(tool.Name)) {
                    toolViewModel.ToolsForExecutionIsChecked = true;
                } else {
                    toolViewModel.ToolsForExecutionIsChecked = false;
                }
                if (AutoGenAgent.ToolNamesForLlm.Contains(tool.Name)) {
                    toolViewModel.ToolsForLLMIsChecked = true;
                } else {
                    toolViewModel.ToolsForLLMIsChecked = false;
                }

                autoGenTools.Add(toolViewModel);
            }
            AutoGenTools = autoGenTools;
            OnPropertyChanged(nameof(AutoGenTools));
        }

        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            // TypeValue
            switch (SelectedTypeValueIndex) {
                case 0:
                    AutoGenAgent.TypeValue = "userproxy";
                    break;
                case 1:
                    AutoGenAgent.TypeValue = "assistant";
                    break;
            }
            // HumanInputMode
            switch (SelectedHumanInputModeIndex) {
                case 0:
                    AutoGenAgent.HumanInputMode = "NEVER";
                    break;
                case 1:
                    AutoGenAgent.HumanInputMode = "ALWAYS";
                    break;
                case 2:
                    AutoGenAgent.HumanInputMode = "TERMINATE";
                    break;
            }

            // Save
            AutoGenAgent.Save();
            AfterUpdate();
            window.Close();
        });

        // ベクトルDBを追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemCommand => new((parameter) => {
            // フォルダを選択
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModels,  (selectedItem) => {
                VectorDBItems.Add(new VectorDBItemViewModel(selectedItem));
            });
            OnPropertyChanged(nameof(VectorDBItems));
        });

        // 選択したVectorDBItemの編集画面を開くコマンド
        public SimpleDelegateCommand<object> OpenVectorDBItemCommand => new((parameter) => {
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModels, (selectedItem) => {

            });
        });


    }
}
