using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Resource;
using QAChat.ViewModel.Folder;
using QAChat.ViewModel.Item;
using WpfAppCommon.Utils;

namespace MergeChat.ViewModel {
    public class MergeTargetDataGridViewControlViewModel : ObservableObject {

        public Action<bool> UpdateIndeterminateAction { get; set; } = (isIndeterminate) => { };

        public ObservableCollection<ContentItemViewModel> Items { get; } = [];

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

        private ContentFolderViewModel? _selectedFolder;
        public ContentFolderViewModel? SelectedFolder {
            get {
                return _selectedFolder;
            }
            set {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
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
                ContentItemViewModel? ContentItemViewModel = (ContentItemViewModel)dataGrid.SelectedItem;
                if (ContentItemViewModel == null) {
                    return;
                }

                // SelectedItemsをMainWindowViewModelにセット
                SelectedItems.Clear();
                foreach (ContentItemViewModel item in dataGrid.SelectedItems) {
                    SelectedItems.Add(item);
                }
                // SelectedTabIndexを更新する処理
                if (SelectedItem != null) {
                    SelectedItem.SelectedTabIndex = lastSelectedIndex;
                }
                OnPropertyChanged(nameof(SelectedItem));
            }

        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand<object> DeleteItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // Itemsから選択中のアイテムを削除
            foreach (ContentItemViewModel item in SelectedItems) {
                Items.Remove(item);
            }
        });


        #region クリップボードアイテムのInputBinding用のコマンド
        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            Items.Clear();
        });

        #endregion



    }
}
