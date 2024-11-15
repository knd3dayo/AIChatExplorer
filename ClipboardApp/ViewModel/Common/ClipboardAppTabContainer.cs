using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ClipboardApp.ViewModel.Common {
    public class ClipboardAppTabContainer : ObservableObject {

        public ClipboardAppTabContainer(string tabName, UserControl tabContent) {
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
    }
}
