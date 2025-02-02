using ClipboardApp.Settings;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Main {
    public class MainPanelViewModel : ObservableObject{



        public MainPanelTreeViewControlViewModel MainPanelTreeViewControlViewModel { get; set; } = new();

        public MainPanelDataGridViewControlViewModel MainPanelDataGridViewControlViewModel { get; set; } = new();


        // ShowProperties
        public bool ShowProperties {
            get {
                return ClipboardAppConfig.Instance.ShowProperties;
            }
            set {
                ClipboardAppConfig.Instance.ShowProperties = value;
                ClipboardAppConfig.Instance.Save();

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
