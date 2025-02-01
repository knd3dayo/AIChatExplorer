using ClipboardApp.Settings;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Main {
    public class MainPanelViewModel : ObservableObject{



        public MainPanelTreeViewControlViewModel MainPanelTreeViewControlViewModel { get; set; } = new();

        public MainPanelDataGridViewControlViewModel MainPanelDataGridViewControlViewModel { get; set; } = new();


        // ShowFooter
        public bool ShowFooter {
            get {
                return ClipboardAppConfig.Instance.ShowFooter;
            }
            set {
                ClipboardAppConfig.Instance.ShowFooter = value;
                ClipboardAppConfig.Instance.Save();

                OnPropertyChanged(nameof(ShowFooter));
                OnPropertyChanged(nameof(FooterVisibility));
            }
        }
        // FooterVisibility
        public Visibility FooterVisibility {
            get {
                return Tools.BoolToVisibility(ShowFooter);
            }
        }




    }
}
