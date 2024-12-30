using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model.AutoGen;
using QAChat.Model;
using QAChat.View.AutoGen;
using QAChat.ViewModel.Folder;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.AutoGen {
    public class ListAutoGenItemWindowViewModel : QAChatViewModelBase {

        public ObservableCollection<AutoGenAgent> AutoGenAgents { get; set; } = [];

        public ObservableCollection<AutoGenTool> AutoGenTools { get; set; } = [];

        public ObservableCollection<AutoGenGroupChat> AutoGenGroupChats { get; set; } = [];

        public ObservableCollection<AutoGenNormalChat> AutoGenNormalChats { get; set; } = [];

        public ObservableCollection<AutoGenNestedChat> AutoGenNestedChats { get; set; } = [];

        public ListAutoGenItemWindowViewModel(ObservableCollection<ContentFolderViewModel> rootFolderViewModels, bool selectGroupChatMode) {
            SelectGroupChatMode = selectGroupChatMode;
            RootFolderViewModels = rootFolderViewModels;
            LoadItems();
        }

        public ObservableCollection<ContentFolderViewModel> RootFolderViewModels { get; set; } = [];

        public bool SelectGroupChatMode { get; set; }

        public Visibility SelectGroupChatModeVisibility => Tools.BoolToVisibility(SelectGroupChatMode);

        public Visibility NormalModeVisibility  => Tools.BoolToVisibility(!SelectGroupChatMode);
        public void LoadItems() {
            AutoGenAgents = [];
            // Load AutoGenAgents
            var autoGenAgents = AutoGenAgent.FindAll();
            foreach (var item in autoGenAgents) {
                AutoGenAgents.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenAgents));

            // Load AutoGenTools
            AutoGenTools = [];
            var autoGenTools = AutoGenTool.FindAll();
            foreach (var item in autoGenTools) {
                AutoGenTools.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenTools));

            // Load AutoGenGroupChats
            AutoGenGroupChats = [];
            var autoGenGroupChats = AutoGenGroupChat.FindAll();
            foreach (var item in autoGenGroupChats) {
                AutoGenGroupChats.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenGroupChats));

            // Load AutoGenNormalChats
            AutoGenNormalChats = [];
            var autoGenNormalChats = AutoGenNormalChat.FindAll();
            foreach (var item in autoGenNormalChats) {
                AutoGenNormalChats.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenNormalChats));

            // Load AutoGenNestedChats
            AutoGenNestedChats = [];
            var autoGenNestedChats = AutoGenNestedChat.FindAll();
            foreach (var item in autoGenNestedChats) {
                AutoGenNestedChats.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenNestedChats));
        }

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
                    AutoGenNormalChat autoGenNormalChat = new();
                    EditAutoGenNormalChatWindow.OpenWindow(autoGenNormalChat, () => {
                        LoadItems();
                    });
                    break;
                case 1:
                    AutoGenGroupChat autoGenGroupChat = new();
                    EditAutoGenGroupChatWindow.OpenWindow(autoGenGroupChat, () => {
                        LoadItems();
                    });
                    break;
                case 2:
                    AutoGenNestedChat autoGenNestedChat = new();
                    EditAutoGenNestedChatWindow.OpenWindow(autoGenNestedChat, () => {
                        LoadItems();
                    });
                    break;
                case 3:
                    AutoGenAgent autoGenAgent = new();
                    EditAutoGenAgentWindow.OpenWindow(autoGenAgent, RootFolderViewModels, () => {
                        LoadItems();
                    });
                    break;
                case 4:
                    AutoGenTool autoGenTool = new();
                    EditAutoGenToolWindow.OpenWindow(autoGenTool, () => {
                        LoadItems();
                    });
                    break;
            }
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
        // SelectedAutoGenNormalChat
        public AutoGenNormalChat? SelectedAutoGenNormalChat { get; set; }

        // OpenEditAutoGenNormalChatWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenNormalChatWindowCommand => new((parameter) => {
            if (SelectedAutoGenNormalChat == null) {
                return;
            }
            EditAutoGenNormalChatWindow.OpenWindow(SelectedAutoGenNormalChat, () => {
                LoadItems();
            });
        });

        // DeleteAutoGenNormalChatCommand
        public SimpleDelegateCommand<object> DeleteAutoGenNormalChatCommand => new((parameter) => {
            if (SelectedAutoGenNormalChat == null) {
                return;
            }
            SelectedAutoGenNormalChat.Delete();
            LoadItems();
        });
        // AutoGenNormalChatSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> AutoGenNormalChatSelectionChangedCommand => new((routedEventArgs) => {
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                SelectedAutoGenNormalChat = (AutoGenNormalChat)dataGrid.SelectedItem;
            }
        });

        // SelectedAutoGenNestedChat
        public AutoGenNestedChat? SelectedAutoGenNestedChat { get; set; }

        // OpenEditAutoGenNestedChatWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenNestedChatWindowCommand => new((parameter) => {
            if (SelectedAutoGenNestedChat == null) {
                return;
            }
            EditAutoGenNestedChatWindow.OpenWindow(SelectedAutoGenNestedChat, () => {
                LoadItems();
            });
        });
        // DeleteAutoGenNestedChatCommand
        public SimpleDelegateCommand<object> DeleteAutoGenNestedChatCommand => new((parameter) => {
            if (SelectedAutoGenNestedChat == null) {
                return;
            }
            SelectedAutoGenNestedChat.Delete();
            LoadItems();
        });

        // AutoGenNestedChatSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> AutoGenNestedChatSelectionChangedCommand => new((routedEventArgs) => {
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                SelectedAutoGenNestedChat = (AutoGenNestedChat)dataGrid.SelectedItem;
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
