using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;


namespace LibUIMergeChat.ViewModel {
    public class MergeTargetPanelViewModel : CommonViewModelBase {

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
                return LibUIPythonAI.Utils.Tools.BoolToVisibility(ShowProperties);
            }
        }
    }

    public class MergeTargetDataGridViewControlViewModel : CommonViewModelBase {

        public MergeTargetDataGridViewControlViewModel(Action<bool> updateIndeterminateAction) {
            UpdateIndeterminateAction = updateIndeterminateAction;
        }
        public Action<bool> UpdateIndeterminateAction { get; set; }


        private ObservableCollection<ContentItemViewModel> _items = [];
        public ObservableCollection<ContentItemViewModel> Items {
            get {
                return _items;
            }
            set {
                _items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        // 選択中のアイテム(複数選択)
        private ObservableCollection<ContentItemViewModel> _selectedItems = [];
        public ObservableCollection<ContentItemViewModel> SelectedItems {
            get {
                return _selectedItems;

            }
            set {
                _selectedItems = value;

                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        public ContentItemViewModel? SelectedItem {
            get {
                // SelectedItemsの最後のアイテムを返す
                if (SelectedItems.Count > 0) {
                    return SelectedItems[SelectedItems.Count - 1];
                }
                return null;
            }
        }

        // CheckedItems
        private ObservableCollection<ContentItemViewModel> _checkedItems = [];
        public ObservableCollection<ContentItemViewModel> CheckedItems {
            get {
                _checkedItems.Clear();
                foreach (ContentItemViewModel item in Items) {
                    if (item.IsChecked) {
                        _checkedItems.Add(item);
                    }
                }
                return _checkedItems;
            }
        }
        // UpdateCheckedItems
        public void UpdateCheckedItems() {
            OnPropertyChanged(nameof(CheckedItems));

        }
        public DataGrid? MergeTargetSelectedDataGrid {
            get {
                return ThisUserControl?.FindName("MergeTargetSelectedDataGrid") as DataGrid;
            }
        }

        // MergeTargetSelectedDataGridのItemのうちIsCheckedがTrueのものを取得する。
        public ObservableCollection<ContentItemViewModel> CheckedItemsInMergeTargetSelectedDataGrid {
            get {
                ObservableCollection<ContentItemViewModel> checkedItems = [];
                if (MergeTargetSelectedDataGrid == null) {
                    return checkedItems;
                }
                foreach (ContentItemViewModel item in MergeTargetSelectedDataGrid.Items) {
                    if (item.IsChecked) {
                        checkedItems.Add(item);
                    }
                }
                return checkedItems;
            }
        }


        // アイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ApplicationItemSelectionChangedCommand => new((routedEventArgs) => {

            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                // 前回選択していたTabIndexを取得
                int lastSelectedIndex = SelectedItem?.SelectedTabIndex ?? 0;

                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;

                // SelectedItemsをMainWindowViewModelにセット
                SelectedItems.Clear();
                foreach (ContentItemViewModel item in dataGrid.SelectedItems) {
                    SelectedItems.Add(item);
                }

                // SelectedTabIndexを更新する処理
                if (SelectedItem != null) {
                    OnPropertyChanged(nameof(SelectedItem));
                    SelectedItem.SelectedTabIndex = lastSelectedIndex;
                }
            }

        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand<object> DeleteItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            // Itemsから選択中のアイテムを削除
            foreach (ContentItemViewModel item in SelectedItems) {
                Items.Remove(item);
            }
        });


        #region アイテムのInputBinding用のコマンド
        // Ctrl + DeleteAsync が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            Items.Clear();
        });


        public SimpleDelegateCommand<ContentItemViewModel> OpenSelectedItemCommand => new((item) => {
            // ★TODO 実装

        });

        #endregion



    }
    public class MergeTargetTreeViewControlViewModel : ObservableObject {

        public MergeTargetTreeViewControlViewModel(Action<ContentFolderViewModel> selectFolderAction, Action<bool> updateIndeterminateAction) {
            SelectFolderAction = selectFolderAction;
            UpdateIndeterminateAction = updateIndeterminateAction;
        }

        public Action<bool> UpdateIndeterminateAction { get; set; }

        public Action<ContentFolderViewModel> SelectFolderAction { get; set; } = (folder) => { };


        // 選択中のフォルダ
        private ContentFolderViewModel? _selectedFolder;
        public ContentFolderViewModel? SelectedFolder {
            get {

                return _selectedFolder;
            }
            set {
                if (value == null) {
                    _selectedFolder = null;
                } else {
                    _selectedFolder = value;
                    // Load
                    _selectedFolder.LoadFolderExecute(beforeAction: () => { }, afterAction: () => {
                        SelectFolderAction(_selectedFolder);
                    });
                }
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }
        public ObservableCollection<ContentFolderViewModel> FolderViewModels { get; set; } = RootFolderViewModelContainer.FolderViewModels;


        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ContentFolderViewModel applicationItemFolderViewModel = (ContentFolderViewModel)treeView.SelectedItem;
            SelectedFolder = applicationItemFolderViewModel;
        });

    }



}
