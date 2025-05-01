using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LibUIPythonAI.ViewModel;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibPythonAI.Utils.Common;
using PythonAILib.Resources;

namespace LibUIMergeChat.ViewModel {
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


        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ClipboardItemSelectionChangedCommand => new((routedEventArgs) => {

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


        #region クリップボードアイテムのInputBinding用のコマンド
        // Ctrl + DeleteAsync が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            Items.Clear();
        });


        public SimpleDelegateCommand<ContentItemViewModel> OpenSelectedItemCommand => new((item) => {
            // ★TODO 実装

        });

        #endregion



    }
}
