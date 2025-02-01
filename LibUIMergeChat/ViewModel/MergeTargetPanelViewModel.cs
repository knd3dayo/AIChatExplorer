using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PythonAILibUI.ViewModel.Folder;
using QAChat.Model;
using QAChat.ViewModel.Folder;
using WpfAppCommon.Utils;

namespace MergeChat.ViewModel {
    public  class MergeTargetPanelViewModel : QAChatViewModelBase{


        public MergeTargetPanelViewModel(MergeTargetDataGridViewControlViewModel dataGridView, MergeTargetTreeViewControlViewModel treeView) {
            MergeTargetDataGridViewControlViewModel = dataGridView;
            MergeTargetTreeViewControlViewModel = treeView;
        }

        public MergeTargetDataGridViewControlViewModel MergeTargetDataGridViewControlViewModel { get; set; }

        public MergeTargetTreeViewControlViewModel MergeTargetTreeViewControlViewModel { get; set; }

        private bool _showFooter = false;
        // ShowFooter
        public bool ShowFooter {
            get {
                return _showFooter;
            }
            set {
                _showFooter = value;
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
