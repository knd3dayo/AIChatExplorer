using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Item;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;


namespace LibUIMergeChat.ViewModel {
    public class MergeTargetPanelViewModel : CommonViewModelBase {

        public MergeTargetPanelViewModel(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems, Action<bool> updateIndeterminate) {
            SelectedItems = selectedItems;
            MergeTargetDataGridViewControlViewModel = new(updateIndeterminate);
            Action<ContentFolderViewModel> selectFolderAction = (folder) => {
                MergeTargetDataGridViewControlViewModel.Items = folder.Items;
            };

            MergeTargetTreeViewControlViewModel = new(selectFolderAction, updateIndeterminate) {
                // アイテムの選択処理
                // TreeViewのFolderを設定する
                SelectedFolder = folderViewModel
            };
        }

        public ObservableCollection<ContentItemViewModel> SelectedItems { get; set; } = [];

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

        // CheckedItems
        public ObservableCollection<ContentItemViewModel> CheckedItems { get; set; } = [];

        private void SubscribeToItemsPinnedChanged() {
            foreach (var item in Items) {
                item.PropertyChanged -= Item_PropertyChanged;
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        private void UnsubscribeFromItemsPinnedChanged() {
            foreach (var item in Items) {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        private void Item_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(ContentItemViewModel.IsChecked)) {
                // IsCheckedが変更されたときの処理をここに記述
                // 例: 必要に応じてUI更新や他のロジックを呼び出す
                if (sender is ContentItemViewModel item) {
                    if (item.IsChecked) {
                        // IsCheckedがTrueになった場合、CheckedItemsに追加
                        if (!CheckedItems.Contains(item)) {
                            CheckedItems.Add(item);
                        }
                    } else {
                        // IsCheckedがFalseになった場合、CheckedItemsから削除
                        if (CheckedItems.Contains(item)) {
                            CheckedItems.Remove(item);
                        }
                    }
                }
            }
        }

        private ObservableCollection<ContentItemViewModel> _items = [];

        // Itemsプロパティのsetterを修正
        public ObservableCollection<ContentItemViewModel> Items {
            get {
                return _items;
            }
            set {
                if (_items != null) {
                    UnsubscribeFromItemsPinnedChanged();
                    _items.CollectionChanged -= Items_CollectionChanged;
                }
                _items = value;
                if (_items != null) {
                    SubscribeToItemsPinnedChanged();
                    _items.CollectionChanged += Items_CollectionChanged;
                }
                OnPropertyChanged(nameof(Items));
            }
        }

        // Itemsコレクションの変更に対応
        private void Items_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.OldItems != null) {
                foreach (ContentItemViewModel item in e.OldItems) {
                    item.PropertyChanged -= Item_PropertyChanged;
                }
            }
            if (e.NewItems != null) {
                foreach (ContentItemViewModel item in e.NewItems) {
                    item.PropertyChanged += Item_PropertyChanged;
                }
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


        // Ctrl + DeleteAsync が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            Items.Clear();
        });


        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {

            if (SelectedItem == null) {
                return;
            }
            ContentFolderViewModel folderViewModel = SelectedItem.FolderViewModel;

            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(folderViewModel, SelectedItem,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    folderViewModel.FolderCommands.LoadFolderCommand.Execute();
                    LogWrapper.Info(CommonStringResources.Instance.Edited);
                });

            // editItemControlをWindowとして開く
            Window editItemWindow = new() {
                Content = editItemControl,
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            editItemControl.SetCloseUserControl(() => {
                editItemWindow.Close();
            });
            editItemWindow.ShowDialog();

        });

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
        public ObservableCollection<ContentFolderViewModel> FolderViewModels { get; set; } = FolderViewModelManagerBase.FolderViewModels;


        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ContentFolderViewModel applicationItemFolderViewModel = (ContentFolderViewModel)treeView.SelectedItem;
            SelectedFolder = applicationItemFolderViewModel;
        });

    }



}
