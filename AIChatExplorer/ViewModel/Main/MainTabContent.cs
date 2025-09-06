using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibUIMain.Utils;

namespace AIChatExplorer.ViewModel.Main {
    public class MainTabContent : ObservableObject {

        public static double HeaderWidthStatic { get; set; } = 200;

        private double _headerWidth = HeaderWidthStatic;
        public double HeaderWidth {
            get { return _headerWidth; }
            set {
                _headerWidth = value; OnPropertyChanged(nameof(HeaderWidth));
            }
        }

        public MainTabContent(string tabName, UserControl tabContent) {
            _tabName = tabName;
            _tabContent = tabContent;
        }
        // TabName
        private string _tabName;
        public string TabName {
            get { return _tabName; }
            set {
                _tabName = value; OnPropertyChanged(nameof(TabName));

            }
        }
        // TabContent
        private UserControl _tabContent;
        public UserControl TabContent {
            get { return _tabContent; }
            set {
                _tabContent = value; OnPropertyChanged(nameof(TabContent));

            }
        }
        // CloseButtonVisibility
        private Visibility _closeButtonVisibility = Visibility.Visible;
        public Visibility CloseButtonVisibility {
            get { return _closeButtonVisibility; }
            set {
                _closeButtonVisibility = value; OnPropertyChanged(nameof(CloseButtonVisibility));
            }
        }

        // CloseTabCommand
        public SimpleDelegateCommand<object> CloseTabCommand => new((param) => {
            if (param is MainTabContent tabContainer) {
                MainWindowViewModel.Instance.MainTabManager.RemoveTabItem(tabContainer);
            }
        });
    }
}
