using System.Collections.ObjectModel;
using System.Windows;
using LibUIPythonAI.ViewModel;
using PythonAILib.Model.AutoGen;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.VectorDB;
using LibUIPythonAI.Utils;
using LibPythonAI.Model.AutoGen;
using LibPythonAI.Model.VectorDB;
using LibUIPythonAI.Resource;

namespace LibUIPythonAI.ViewModel.AutoGen {
    public class EditAutoGenAgentViewModel : CommonViewModelBase {
        public EditAutoGenAgentViewModel(AutoGenAgent autoGenAgent, ObservableCollection<ContentFolderViewModel> rootFolderViewModels, Action afterUpdate) {
            AutoGenAgent = autoGenAgent;
            RootFolderViewModels = rootFolderViewModels;
            AfterUpdate = afterUpdate;
            LoadTools();
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

        // CodeExecution
        public bool CodeExecution {
            get { return AutoGenAgent.CodeExecution; }
            set {
                AutoGenAgent.CodeExecution = value;
                OnPropertyChanged();
            }
        }
        // Llm
        public string LLMConfigName {
            get { return AutoGenAgent.LLMConfigName; }
            set {
                AutoGenAgent.LLMConfigName = value;
                OnPropertyChanged();
            }
        }
        // LlmConfigList
        public ObservableCollection<string> LlmConfigNameList {
            get {
               var list = AutoGenLLMConfig.GetAutoGenLLMConfigList();
                ObservableCollection<string> llmConfigNames = [];
                foreach (var item in list) {
                    llmConfigNames.Add(item.Name);
                }
                return llmConfigNames;
            }
        }

        // VectorSearchProperty
        public ObservableCollection<VectorDBItemViewModel> VectorDBItems { get; set; } = [];

        public VectorDBItemViewModel? SelectedVectorDBItem { get; set; }

        // VectorDBSearchAgent
        public bool VectorDBSearchAgent {
            get { return AutoGenAgent.VectorDBSearchAgent; }
            set {
                AutoGenAgent.VectorDBSearchAgent = value;
                OnPropertyChanged(nameof(VectorDBItemsListBoxIsEnabled));
            }
        }
        // SelectedTabIndexが2の場合かつSelectVectorDBAtChatRun = falseの場合、「ベクトルDB選択」ボタンを表示するかどうか
        public Visibility VectorDBSelectionButtonVisibility => Tools.BoolToVisibility(SelectedTabIndex == 2 && VectorDBSearchAgent == false);

        // VectorDBItemsのListBoxをEnableにするかどうか
        public bool VectorDBItemsListBoxIsEnabled => VectorDBSearchAgent == false;
        // AutoGenTools
        public ObservableCollection<AutoGenToolViewModel> AutoGenTools { get; set; } = [];


        // SelectedTabIndex
        private int _selectedTabIndex = 0;
        public int SelectedTabIndex {
            get { return _selectedTabIndex; }
            set {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(VectorDBSelectionButtonVisibility));
            }
        }

        public void LoadTools() {
            // Load tools
            ObservableCollection<AutoGenToolViewModel> autoGenTools = [];
            foreach (AutoGenTool tool in AutoGenTool.GetAutoGenToolList()) {
                AutoGenToolViewModel toolViewModel = new(tool);
                if (AutoGenAgent.ToolNames.Contains(tool.Name)) {
                    toolViewModel.ToolIsChecked = true;
                } else {
                    toolViewModel.ToolIsChecked = false;
                }

                autoGenTools.Add(toolViewModel);
            }
            AutoGenTools = autoGenTools;
            OnPropertyChanged(nameof(AutoGenTools));
        }

        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {

            // Save
            AutoGenAgent.Save();
            AfterUpdate();
            window.Close();
        }, null, null);

        // ベクトルDBを追加するコマンド
        public SimpleDelegateCommand<object> AddVectorDBItemCommand => new((parameter) => {
            // フォルダを選択
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModels,  (selectedItem) => {
                var item = VectorDBItem.GetItemByName(selectedItem.VectorDBItemName);
                if (item == null) {
                    return;
                }
                VectorDBItems.Add(new VectorDBItemViewModel(item));
            });
            OnPropertyChanged(nameof(VectorDBItems));
        }, null, null);

        // 選択したVectorDBItemの編集画面を開くコマンド
        public SimpleDelegateCommand<object> OpenVectorDBItemCommand => new((parameter) => {
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModels, (selectedItem) => {

            });
        }, null, null);

        // RemoveVectorDBItemCommand
        public SimpleDelegateCommand<object> RemoveVectorDBItemCommand => new((parameter) => {
            if (SelectedVectorDBItem == null) {
                return;
            }
            VectorDBItems.Remove(SelectedVectorDBItem);
            OnPropertyChanged(nameof(VectorDBItems));
        }, null, null);

    }
}
