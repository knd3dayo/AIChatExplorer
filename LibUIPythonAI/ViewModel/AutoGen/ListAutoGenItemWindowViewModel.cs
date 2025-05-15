using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibPythonAI.Model.AutoGen;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.AutoGen;
using LibUIPythonAI.ViewModel.Folder;

namespace LibUIPythonAI.ViewModel.AutoGen {
    public class ListAutoGenItemWindowViewModel : CommonViewModelBase {

        public ListAutoGenItemWindowViewModel(ObservableCollection<ContentFolderViewModel> rootFolderViewModels, bool selectGroupChatMode) {
            SelectGroupChatMode = selectGroupChatMode;
            RootFolderViewModels = rootFolderViewModels;
            Task.Run(async () => {
                await LoadItems();
            });
        }

        private ObservableCollection<AutoGenLLMConfig> _autoGenLLMConfigs = [];
        public ObservableCollection<AutoGenLLMConfig> AutoGenLLMConfigs {
            get => _autoGenLLMConfigs;
            set {
                _autoGenLLMConfigs = value;
                OnPropertyChanged(nameof(AutoGenLLMConfigs));
            }
        }
        
        private ObservableCollection<AutoGenTool> _autoGenTools = [];
        public ObservableCollection<AutoGenTool> AutoGenTools {
            get => _autoGenTools;
            set {
                _autoGenTools = value;
                OnPropertyChanged(nameof(AutoGenTools));
            }
        }

        private ObservableCollection<AutoGenAgent> _autoGenAgents = [];
        public ObservableCollection<AutoGenAgent> AutoGenAgents {
            get => _autoGenAgents;
            set {
                _autoGenAgents = value;
                OnPropertyChanged(nameof(AutoGenAgents));
            }
        }

        private ObservableCollection<AutoGenGroupChat> _autoGenGroupChats = [];

        public ObservableCollection<AutoGenGroupChat> AutoGenGroupChats {
            get => _autoGenGroupChats;
            set {
                _autoGenGroupChats = value;
                OnPropertyChanged(nameof(AutoGenGroupChats));
            }
        }


        public ObservableCollection<ContentFolderViewModel> RootFolderViewModels { get; set; } = [];

        public bool SelectGroupChatMode { get; set; }

        public Visibility SelectGroupChatModeVisibility => Tools.BoolToVisibility(SelectGroupChatMode);

        public Visibility NormalModeVisibility => Tools.BoolToVisibility(!SelectGroupChatMode);

        private async Task LoadLLMConfigListAsync() {
            // LLMConfigList
            ObservableCollection<AutoGenLLMConfig> llmConfigList = [];
            var autoGenLLMConfigs = await AutoGenLLMConfig.GetAutoGenLLMConfigList();
            foreach (var item in autoGenLLMConfigs) {
                llmConfigList.Add(item);
            }
            // MainUIのUIスレッドで実行する
            MainUITask.Run(() => {
                AutoGenLLMConfigs = llmConfigList;
            });
        }
        private async Task LoadAutoGenToolListAsync() {
            // AutoGenToolList
            ObservableCollection<AutoGenTool> autoGenToolList = [];
            var autoGenTools = await AutoGenTool.GetAutoGenToolListAsync();
            foreach (var item in autoGenTools) {
                autoGenToolList.Add(item);
            }
            // MainUIのUIスレッドで実行する
            MainUITask.Run(() => {
                AutoGenTools = autoGenToolList;
            });
        }

        private void LoadAutoGenAgentList() {
            // AutoGenAgentList
            ObservableCollection<AutoGenAgent> autoGenAgentList = [];
            var autoGenAgents = AutoGenAgent.GetAutoGenAgentList();
            foreach (var item in autoGenAgents) {
                autoGenAgentList.Add(item);
            }
            // MainUIのUIスレッドで実行する
            MainUITask.Run(() => {
                AutoGenAgents = autoGenAgentList;
            });

        }
        private void LoadAutoGenGroupChatList() {
            // AutoGenGroupChatList
            ObservableCollection<AutoGenGroupChat> autoGenGroupChatList = [];
            var autoGenGroupChats = AutoGenGroupChat.GetAutoGenChatList();
            foreach (var item in autoGenGroupChats) {
                autoGenGroupChatList.Add(item);
            }
            // MainUIのUIスレッドで実行する
            MainUITask.Run(() => {
                AutoGenGroupChats = autoGenGroupChatList;
            });
        }

        private async Task LoadItems() {

            await LoadLLMConfigListAsync();

            // Load AutoGenTools
            await LoadAutoGenToolListAsync();

            // Load AutoGenAgents
            LoadAutoGenAgentList();
            // Load AutoGenGroupChats
            LoadAutoGenGroupChatList();

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
                Task.Run(async () => {
                    await LoadItems();
                });
            });
        });

        // OpenEditAutoGenAgentWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenAgentWindowCommand => new((parameter) => {
            if (SelectedAutoGenAgent == null) {
                return;
            }
            EditAutoGenAgentWindow.OpenWindow(SelectedAutoGenAgent, RootFolderViewModels, () => {
                Task.Run(async () => {
                    await LoadItems();
                });
            });
        });

        // OpenEditAutoGenToolWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenToolWindowCommand => new((parameter) => {
            if (SelectedAutoGenTool == null) {
                return;
            }
            EditAutoGenToolWindow.OpenWindow(SelectedAutoGenTool, () => {
                Task.Run(async () => {
                    await LoadItems();
                });
            });
        });

        // AddItem
        public SimpleDelegateCommand<object> AddItemCommand => new((parameter) => {
            switch (SelectedTabIndex) {
                case 0:
                    AutoGenGroupChat autoGenGroupChat = new();
                    EditAutoGenGroupChatWindow.OpenWindow(autoGenGroupChat, () => {
                        Task.Run(async () => {
                            await LoadItems();
                        });
                    });
                    break;
                case 1:
                    AutoGenAgent autoGenAgent = new();
                    EditAutoGenAgentWindow.OpenWindow(autoGenAgent, RootFolderViewModels, () => {
                        Task.Run(async () => {
                            await LoadItems();
                        });
                    });
                    break;
                case 2:
                    AutoGenTool autoGenTool = new();
                    EditAutoGenToolWindow.OpenWindow(autoGenTool, () => {
                        Task.Run(async () => {
                            await LoadItems();
                        });
                    });
                    break;
                case 3:
                    AutoGenLLMConfig autoGenLLMConfig = new();
                    EditAutoGenLLMConfigWindow.OpenWindow(autoGenLLMConfig, () => {
                        Task.Run(async () => {
                            await LoadItems();
                        });
                    });
                    break;
            }
        });

        // DeleteAutoGenLLMConfigCommand
        public SimpleDelegateCommand<object> DeleteAutoGenLLMConfigCommand => new((parameter) => {
            if (SelectedAutoGenLLMConfig == null) {
                return;
            }
            Task.Run(async () => {
                await SelectedAutoGenLLMConfig.Delete();
                await LoadItems();
            });
        });

        // DeleteAutoGenAgentCommand
        public SimpleDelegateCommand<object> DeleteAutoGenAgentCommand => new((parameter) => {
            if (SelectedAutoGenAgent == null) {
                return;
            }
            Task.Run(async () => {
                SelectedAutoGenAgent.Delete();
                await LoadItems();
            });
        });

        // DeleteAutoGenToolCommand
        public SimpleDelegateCommand<object> DeleteAutoGenToolCommand => new((parameter) => {
            if (SelectedAutoGenTool == null) {
                return;
            }
            Task.Run(async () => {
                await SelectedAutoGenTool.Delete();
                await LoadItems();
            });
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
                Task.Run(async () => {
                    await LoadItems();
                });
            });
        });

        // DeleteAutoGenGroupChatCommand
        public SimpleDelegateCommand<object> DeleteAutoGenGroupChatCommand => new((parameter) => {
            if (SelectedAutoGenGroupChat == null) {
                return;
            }
            Task.Run(async () => {
                SelectedAutoGenGroupChat.Delete();
                await LoadItems();
            });
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
