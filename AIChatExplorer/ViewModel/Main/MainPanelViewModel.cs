using System.Windows;
using AIChatExplorer.ViewModel.Settings;
using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using PythonAILib.Common;

namespace AIChatExplorer.ViewModel.Main {
    public class MainPanelViewModel: CommonViewModelBase {

        public MainPanelViewModel(AppViewModelCommands commands) : base() {
            Commands = commands;
            MainPanelTreeViewControlViewModel = new MainPanelTreeViewControlViewModel(Commands);
            MainPanelDataGridViewControlViewModel = new MainPanelDataGridViewControlViewModel(Commands);
            CommonViewModelProperties.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(CommonViewModelProperties.MarkdownView)) {
                    MainPanelDataGridViewControlViewModel.UpdateView();
                }
            };
        }


        public AppViewModelCommands Commands { get; set; }
        public MainPanelTreeViewControlViewModel MainPanelTreeViewControlViewModel { get; set; } 

        public MainPanelDataGridViewControlViewModel MainPanelDataGridViewControlViewModel { get; set; }


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
        public Visibility PropertiesVisibility => Tools.BoolToVisibility(ShowProperties);

        // MarkdownViewVisibility
        public Visibility MarkdownViewVisibility => Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView);

    }
}
