using System.Windows;
using AIChatExplorer.ViewModel.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Utils;

namespace AIChatExplorer.ViewModel.Main {
    public class MainPanelViewModel(AppViewModelCommands commands) : ObservableObject {
        public MainPanelTreeViewControlViewModel MainPanelTreeViewControlViewModel { get; set; } = new MainPanelTreeViewControlViewModel(commands);

        public MainPanelDataGridViewControlViewModel MainPanelDataGridViewControlViewModel { get; set; } = new MainPanelDataGridViewControlViewModel(commands);


        // ShowProperties
        public bool ShowProperties {
            get {
                return AIChatExplorerConfig.Instance.ShowProperties;
            }
            set {
                AIChatExplorerConfig.Instance.ShowProperties = value;
                AIChatExplorerConfig.Instance.Save();

                OnPropertyChanged(nameof(ShowProperties));
                OnPropertyChanged(nameof(PropertiesVisibility));
            }
        }
        // PropertiesVisibility
        public Visibility PropertiesVisibility {
            get {
                return Tools.BoolToVisibility(ShowProperties);
            }
        }




    }
}
