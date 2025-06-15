using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using AIChatExplorer.ViewModel.Content;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.Content;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;

namespace AIChatExplorer.ViewModel.Main {
    public class MainPanelDataGridViewControlViewModel(AppViewModelCommands commands) : ObservableObject {

        private AppViewModelCommands Commands { get; set; } = commands;

        public TabControl? MyTabControl { get; set; } 
        public Action<bool> UpdateIndeterminateAction { get; set; } = (isIndeterminate) => { };

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
                    return SelectedItems[^1];
                }
                return null;
            }
        }

        public void UpdateView() {
            // 前回選択していたTabIndexを取得
            int lastSelectedTabIndex = SelectedItem?.SelectedTabIndex ?? 0;

            // SelectedTabIndexを更新する処理
            if (SelectedItem != null) {
                /**
                 * Task.Run(() => {
                    SelectedItem.ContentItem.Load(() => { }, () => {
                        MainUITask.Run(() => {
                            OnPropertyChanged(nameof(SelectedItem));
                        });
                    });
                });
                OnPropertyChanged(nameof(SelectedItem));
                **/
                // SourceがFileの場合は、ファイルの内容を読み込む
                if (SelectedItem.ContentItem.SourceType == ContentSourceType.File) {
                    ContentItemCommands.ExtractTexts([SelectedItem.ContentItem], () => { }, () => {
                        MainUITask.Run(() => {
                            SelectedItem.UpdateView(MyTabControl);
                            OnPropertyChanged(nameof(SelectedItem));
                        });
                    });
                }
                // 選択中のアイテムのSelectedTabIndexを更新する
                SelectedItem.LastSelectedTabIndex = lastSelectedTabIndex;
                SelectedItem.UpdateView(MyTabControl);
                OnPropertyChanged(nameof(SelectedItem));

            }
        }

        // アイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ApplicationItemSelectionChangedCommand => new((routedEventArgs) => {

            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid dataGrid) {

                if (dataGrid.SelectedItem is ContentItemViewModel applicationItemViewModel) {
                    // SelectedItemsをMainWindowViewModelにセット
                    SelectedItems.Clear();
                    foreach (ContentItemViewModel item in dataGrid.SelectedItems) {
                        SelectedItems.Add(item);
                    }
                    UpdateView();
                }
            }

        });

        // ピン留めの切り替え処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> ChangePinCommand => new((parameter) => {

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            foreach (ApplicationItemViewModel applicationItemViewModel in SelectedItems) {
                Commands.ChangePinCommand.Execute();
            }
        });

        #region アイテムのコンテキストメニューのInputBinding用のコマンド
        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((parameter) => {
            Commands.OpenContentAsFileCommand.Execute(this.SelectedItem);
        });

        // ベクトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateVectorCommand => new((parameter) => {
            Commands.GenerateVectorCommand.Execute(this.SelectedItems);
        });


        // ベクトル検索を実行する処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> VectorSearchCommand => new((parameter) => {
            Commands.VectorSearchCommand.Execute(SelectedItem);
        });

        #endregion

        #region アイテムのInputBinding用のコマンド
        // Ctrl + DeleteAsync が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            SelectedFolder?.DeleteDisplayedItemCommand.Execute();
        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand<object> DeleteItemCommand => new((parameter) => {
            Commands.DeleteItemsCommand.Execute(this.SelectedItems);
        });

        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutItemCommand => new((parameter) => {
            AppViewModelCommands.CutItemCommandExecute(this.SelectedItems);
        });

        // Ctrl + C が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CopyItemCommand => new((parameter) => {
            AppViewModelCommands.CopyToClipboardCommandExecute(this.SelectedItems);
        });

        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            AppViewModelCommands.OpenItemCommandExecute(this.SelectedItem);
        });


        #endregion



    }
}
