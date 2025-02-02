using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibUIPythonAI.ViewModel;
using PythonAILib.Model.AutoGen;
using LibUIPythonAI.View.AutoGen;
using LibUIPythonAI.ViewModel.Folder;
using WpfAppCommon.Utils;

namespace LibUIPythonAI.ViewModel.AutoGen {
    public class ListAutoGenItemWindowViewModel : ChatViewModelBase {

        public ObservableCollection<AutoGenAgent> AutoGenAgents { get; set; } = [];

        public ObservableCollection<AutoGenTool> AutoGenTools { get; set; } = [];

        public ObservableCollection<AutoGenGroupChat> AutoGenGroupChats { get; set; } = [];

        public ObservableCollection<AutoGenLLMConfig> AutoGenLLMConfigs { get; set; } = [];

        public ListAutoGenItemWindowViewModel(ObservableCollection<ContentFolderViewModel> rootFolderViewModels, bool selectGroupChatMode) {
            SelectGroupChatMode = selectGroupChatMode;
            RootFolderViewModels = rootFolderViewModels;
            LoadItems();
        }

        public ObservableCollection<ContentFolderViewModel> RootFolderViewModels { get; set; } = [];

        public bool SelectGroupChatMode { get; set; }

        public Visibility SelectGroupChatModeVisibility => Tools.BoolToVisibility(SelectGroupChatMode);

        public Visibility NormalModeVisibility => Tools.BoolToVisibility(!SelectGroupChatMode);
        public void LoadItems() {
            // Load AutoGenLLMConfigs
            // Clear
            AutoGenLLMConfigs.Clear();

            var autoGenLLMConfigs = AutoGenLLMConfig.GetAutoGenLLMConfigList();
            foreach (var item in autoGenLLMConfigs) {
                AutoGenLLMConfigs.Add(item);
            }

            // Load AutoGenTools
            // Clear
            AutoGenTools.Clear();

            var autoGenTools = AutoGenTool.GetAutoGenToolList();
            foreach (var item in autoGenTools) {
                AutoGenTools.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenTools));

            // Load AutoGenAgents
            // Clear
            AutoGenAgents.Clear();

            var autoGenAgents = AutoGenAgent.GetAutoGenAgentList();
            foreach (var item in autoGenAgents) {
                AutoGenAgents.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenAgents));

            // Load AutoGenGroupChats
            // Clear
            AutoGenGroupChats.Clear();

            var autoGenGroupChats = AutoGenGroupChat.GetAutoGenChatList();
            foreach (var item in autoGenGroupChats) {
                AutoGenGroupChats.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenGroupChats));
        }
        // SelectedAutoGenLLMConfig
        public AutoGenLLMConfig? SelectedAutoGenLLMConfig { get; set; }

        // SelectedAutoGenAgent
        public AutoGenAgent? SelectedAutoGenAgent { get; set; }

        // SelectedAutoGenTool
        public AutoGenTool? SelectedAutoGenTool { get; set; }

        // SelectedIndex
        private int _selectedTabIndex = 0;
        public int SelectedTabIndex {
            get => _selectedTabIndex;
            set {
                _selectedTabIndex = value;
                OnPropertyChanged(nameof(SelectedTabIndex));
            }
        }
        // OpenEditAutoGenLLMConfigWindowCommand
        public SimpleDelegateCommand<object> OpenEditAutoGenLLMConfigWindowCommand => new((parameter) => {
            if (SelectedAutoGenLLMConfig == null) {
                return;
            }
            EditAutoGenLLMConfigWindow.OpenWindow(SelectedAutoGenLLMConfig, () => {
                LoadItems();
            });
        });

        // OpenEditAutoGenAgentWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenAgentWindowCommand => new((parameter) => {
            if (SelectedAutoGenAgent == null) {
                return;
            }
            EditAutoGenAgentWindow.OpenWindow(SelectedAutoGenAgent, RootFolderViewModels, () => {
                LoadItems();
            });
        });

        // OpenEditAutoGenToolWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenToolWindowCommand => new((parameter) => {
            if (SelectedAutoGenTool == null) {
                return;
            }
            EditAutoGenToolWindow.OpenWindow(SelectedAutoGenTool, () => {
                LoadItems();
            });
        });

        // AddItem
        public SimpleDelegateCommand<object> AddItemCommand => new((parameter) => {
            switch (SelectedTabIndex) {
                case 0:
                    AutoGenGroupChat autoGenGroupChat = new();
                    EditAutoGenGroupChatWindow.OpenWindow(autoGenGroupChat, () => {
                        LoadItems();
                    });
                    break;
                case 1:
                    AutoGenAgent autoGenAgent = new();
                    EditAutoGenAgentWindow.OpenWindow(autoGenAgent, RootFolderViewModels, () => {
                        LoadItems();
                    });
                    break;
                case 2:
                    AutoGenTool autoGenTool = new();
                    EditAutoGenToolWindow.OpenWindow(autoGenTool, () => {
                        LoadItems();
                    });
                    break;
                case 3:
                    AutoGenLLMConfig autoGenLLMConfig = new();
                    EditAutoGenLLMConfigWindow.OpenWindow(autoGenLLMConfig, () => {
                        LoadItems();
                    });
                    break;
            }
        });

        // DeleteAutoGenLLMConfigCommand
        public SimpleDelegateCommand<object> DeleteAutoGenLLMConfigCommand => new((parameter) => {
            if (SelectedAutoGenLLMConfig == null) {
                return;
            }
            SelectedAutoGenLLMConfig.Delete();
            LoadItems();
        });

        // DeleteAutoGenAgentCommand
        public SimpleDelegateCommand<object> DeleteAutoGenAgentCommand => new((parameter) => {
            if (SelectedAutoGenAgent == null) {
                return;
            }
            SelectedAutoGenAgent.Delete();
            LoadItems();
        });

        // DeleteAutoGenToolCommand
        public SimpleDelegateCommand<object> DeleteAutoGenToolCommand => new((parameter) => {
            if (SelectedAutoGenTool == null) {
                return;
            }
            SelectedAutoGenTool.Delete();
            LoadItems();
        });

        // AutoGenLLMConfigSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> AutoGenLLMConfigSelectionChangedCommand => new((routedEventArgs) => {
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                SelectedAutoGenLLMConfig = (AutoGenLLMConfig)dataGrid.SelectedItem;
            }
        });

        // AutoGenAgentSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> AutoGenAgentSelectionChangedCommand => new((routedEventArgs) => {
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                SelectedAutoGenAgent = (AutoGenAgent)dataGrid.SelectedItem;
            }
        });

        // AutoGenToolSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> AutoGenToolSelectionChangedCommand => new((routedEventArgs) => {
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                SelectedAutoGenTool = (AutoGenTool)dataGrid.SelectedItem;
            }
        });

        // SelectedAutoGenGroupChat
        public AutoGenGroupChat? SelectedAutoGenGroupChat { get; set; }

        // OpenEditAutoGenGroupChatWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenGroupChatWindowCommand => new((parameter) => {
            if (SelectedAutoGenGroupChat == null) {
                return;
            }
            EditAutoGenGroupChatWindow.OpenWindow(SelectedAutoGenGroupChat, () => {
                LoadItems();
            });
        });

        // DeleteAutoGenGroupChatCommand
        public SimpleDelegateCommand<object> DeleteAutoGenGroupChatCommand => new((parameter) => {
            if (SelectedAutoGenGroupChat == null) {
                return;
            }
            SelectedAutoGenGroupChat.Delete();
            LoadItems();
        });

        // AutoGenGroupChatSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> AutoGenGroupChatSelectionChangedCommand => new((routedEventArgs) => {
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                SelectedAutoGenGroupChat = (AutoGenGroupChat)dataGrid.SelectedItem;
            }
        });

        // SelectCommand
        public SimpleDelegateCommand<Window> SelectCommand => new((window) => {
            if (SelectedAutoGenGroupChat != null) {
                window.Close();
            }
        });
    }
}
