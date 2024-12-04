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

        public ListAutoGenItemWindowViewModel(ContentFolderViewModel rootFolderViewModel, bool selectGroupChatMode) {
            SelectGroupChatMode = selectGroupChatMode;
            RootFolderViewModel = rootFolderViewModel;
            LoadItems();
        }

        public ContentFolderViewModel RootFolderViewModel { get; set; }

        public bool SelectGroupChatMode { get; set; }

        public Visibility SelectGroupChatModeVisibility {
            get {
                return SelectGroupChatMode ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public Visibility NormalModeVisibility {
            get {
                return SelectGroupChatMode ? Visibility.Collapsed : Visibility.Visible;
            }
        }

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
        }

        // SelectedAutoGenGroupChat
        public AutoGenGroupChat? SelectedAutoGenGroupChat { get; set; }

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
        // OpenEditAutoGenGroupChatWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenGroupChatWindowCommand => new((parameter) => {
            if (SelectedAutoGenGroupChat == null) {
                return;
            }
            EditAutoGenGroupChatWindow.OpenWindow(SelectedAutoGenGroupChat, () => {
                LoadItems();
            });
        });

        // OpenEditAutoGenAgentWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenAgentWindowCommand => new((parameter) => {
            if (SelectedAutoGenAgent == null) {
                return;
            }
            EditAutoGenAgentWindow.OpenWindow(SelectedAutoGenAgent, RootFolderViewModel, () => {
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
                    EditAutoGenAgentWindow.OpenWindow(autoGenAgent, RootFolderViewModel, () => {
                        LoadItems();
                    });
                    break;
                case 2:
                    AutoGenTool autoGenTool = new();
                    EditAutoGenToolWindow.OpenWindow(autoGenTool, () => {
                        LoadItems();
                    });
                    break;
            }
        });

        // DeleteAutoGenGroupChatCommand
        public SimpleDelegateCommand<object> DeleteAutoGenGroupChatCommand => new((parameter) => {
            if (SelectedAutoGenGroupChat == null) {
                return;
            }
            SelectedAutoGenGroupChat.Delete();
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

        // AutoGenGroupChatSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> AutoGenGroupChatSelectionChangedCommand => new((routedEventArgs) => {
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                SelectedAutoGenGroupChat = (AutoGenGroupChat)dataGrid.SelectedItem;
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

        // SelectCommand
        public SimpleDelegateCommand<Window> SelectCommand => new((window) => {
            if (SelectedAutoGenGroupChat != null) {
                window.Close();
            }
        });
    }
}
