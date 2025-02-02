using System.Collections.ObjectModel;
using System.Windows;
using QAChat.Model;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.Item;
using WpfAppCommon.Utils;

namespace MergeChat.ViewModel {
    public class MergeTargetPanelViewModel : QAChatViewModelBase {

        private bool _initialized = false;

        public MergeTargetPanelViewModel(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems, Action<bool> updateIndeterminate) {
            SelectedItems = selectedItems;

            MergeTargetDataGridViewControlViewModel = new(updateIndeterminate);
            Action<ContentFolderViewModel> selectFolderAction = (folder) => {
                MergeTargetDataGridViewControlViewModel.Items = folder.Items;
                FolderSelectionChanged();
            };
            MergeTargetTreeViewControlViewModel = new(selectFolderAction, updateIndeterminate) {
                // アイテムの選択処理
                // TreeViewのFolderを設定する
                SelectedFolder = folderViewModel
            };
        }
        public ObservableCollection<ContentItemViewModel> SelectedItems { get; set; } = [];

        private void FolderSelectionChanged() {
            if (SelectedItems.Count == 0 || _initialized) {
                // SelectedItemsがない場合は、SelectedFolderのItemsのIsCheckedを設定する
                foreach (ContentItemViewModel item in MergeTargetDataGridViewControlViewModel.Items) {
                    item.IsChecked = true;
                }
            } else {
                // SelectedItemsがある場合はSelectedItemsとマッチするアイテムのIsCheckedを設定する
                var selectedItemIds = new HashSet<string>(SelectedItems.Select(item => item.ContentItem.Id.ToString()));
                foreach (ContentItemViewModel item in MergeTargetDataGridViewControlViewModel.Items) {
                    var id = item.ContentItem.Id.ToString();
                    item.IsChecked = selectedItemIds.Contains(id);
                }
            }
            UpdateCheckedItems();
            _initialized = true;

        }

        public void UpdateCheckedItems() {
            MergeTargetDataGridViewControlViewModel.UpdateCheckedItems();
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
