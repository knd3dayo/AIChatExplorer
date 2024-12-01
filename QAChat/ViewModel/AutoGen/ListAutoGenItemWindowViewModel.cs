using System.Collections.ObjectModel;
using PythonAILib.Model.AutoGen;
using QAChat.Model;
using QAChat.View.AutoGen;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.AutoGen {
    public class ListAutoGenItemWindowViewModel : QAChatViewModelBase {

        public static string StringResGroupChat { get; set; } = "グループチャット";
        public static string StringResAgent { get; set; } = "エージェント";
        public static string StringResTool { get; set; } = "ツール";
        public static string StringResName { get; set; } = "名前";
        public static string StringResDescription { get; set; } = "説明";
        public static string StringResSourcePath { get; set; } = "ソースパス";
        public static string StringResSave { get; set; } = "保存";
        public static string StringResDelete { get; set; } = "削除";
        public static string StringResAdd { get; set; } = "追加";
        public static string StringResCancel { get; set; } = "キャンセル";




        public ObservableCollection<AutoGenAgent> AutoGenAgents { get; set; } = [];

        public ObservableCollection<AutoGenTool> AutoGenTools { get; set; } = [];

        public ObservableCollection<AutoGenGroupChat> AutoGenGroupChats { get; set; } = [];

        public ListAutoGenItemWindowViewModel() {

            // Load AutoGenAgents
            var autoGenAgents = AutoGenAgent.FindAll();
            foreach (var item in autoGenAgents) {
                AutoGenAgents.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenAgents));

            // Load AutoGenTools
            var autoGenTools = AutoGenTool.FindAll();
            foreach (var item in autoGenTools) {
                AutoGenTools.Add(item);
            }
            OnPropertyChanged(nameof(AutoGenTools));

            // Load AutoGenGroupChats
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
            EditAutoGenGroupChatWindow.OpenWindow(SelectedAutoGenGroupChat);
        });

        // OpenEditAutoGenAgentWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenAgentWindowCommand => new((parameter) => {
            if (SelectedAutoGenAgent == null) {
                return;
            }
            EditAutoGenAgentWindow.OpenWindow(SelectedAutoGenAgent);
        });

        // OpenEditAutoGenToolWindow
        public SimpleDelegateCommand<object> OpenEditAutoGenToolWindowCommand => new((parameter) => {
            if (SelectedAutoGenTool == null) {
                return;
            }
            EditAutoGenToolWindow.OpenWindow(SelectedAutoGenTool);
        });

        // AddItem
        public SimpleDelegateCommand<object> AddItemCommand => new((parameter) => {
            switch (SelectedTabIndex) {
                case 0:
                    AutoGenGroupChat autoGenGroupChat = new AutoGenGroupChat();
                    EditAutoGenGroupChatWindow.OpenWindow(autoGenGroupChat);
                    break;
                case 1:
                    AutoGenAgent autoGenAgent = new AutoGenAgent();
                    EditAutoGenAgentWindow.OpenWindow(autoGenAgent);
                    break;
                case 2:
                    AutoGenTool autoGenTool = new AutoGenTool();
                    EditAutoGenToolWindow.OpenWindow(autoGenTool);
                    break;
            }
        });
    }
}
