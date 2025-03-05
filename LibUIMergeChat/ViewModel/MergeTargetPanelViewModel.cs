using System.Collections.ObjectModel;
using System.Windows;
using LibUIPythonAI.ViewModel;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.Utils;

namespace LibUIMergeChat.ViewModel {
    public class MergeTargetPanelViewModel : ChatViewModelBase {

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
                var selectedItemIds = new HashSet<string>(SelectedItems.Select(item => item.ContentItem.Id));
                foreach (ContentItemViewModel item in MergeTargetDataGridViewControlViewModel.Items) {
                    var id = item.ContentItem.Id;
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

        private bool _showProperties = false;
        // ShowProperties
        public bool ShowProperties {
            get {
                return _showProperties;
            }
            set {
                _showProperties = value;
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
