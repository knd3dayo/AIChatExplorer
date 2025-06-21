using System.Windows;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Settings;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;

namespace AIChatExplorer.ViewModel.Main {
    public class MainPanelViewModel : CommonViewModelBase {

        public MainPanelViewModel(AppViewModelCommandExecutes commands) : base() {
            Commands = commands;
            MainPanelTreeViewControlViewModel = new MainPanelTreeViewControlViewModel(Commands);
            MainPanelDataGridViewControlViewModel = new MainPanelDataGridViewControlViewModel(Commands);
            CommonViewModelProperties.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(CommonViewModelProperties.MarkdownView)) {
                    MainPanelDataGridViewControlViewModel.UpdateView();
                }
            };
        }
        private TabControl? MyTabControl { get; set; }

        public override void OnLoadedAction() {
            base.OnLoadedAction();
            MyTabControl = ThisUserControl?.FindName("MyTabControl") as TabControl;
            MainPanelDataGridViewControlViewModel.MyTabControl = MyTabControl;

        }

        public AppViewModelCommandExecutes Commands { get; set; }
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
