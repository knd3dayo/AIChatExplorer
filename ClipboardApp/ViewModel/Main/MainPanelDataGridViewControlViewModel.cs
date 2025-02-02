using ClipboardApp.ViewModel.Content;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.Settings;
using System.Windows;
using System.Windows.Controls;
using WpfAppCommon.Utils;
using QAChat.Resource;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using ClipboardApp.ViewModel.Folders.Clipboard;

namespace ClipboardApp.ViewModel.Main {
    public class MainPanelDataGridViewControlViewModel : ObservableObject{

        public Action<bool> UpdateIndeterminateAction { get; set; } = (isIndeterminate) => { };


        private ClipboardFolderViewModel? _selectedFolder;
        public ClipboardFolderViewModel? SelectedFolder {
            get {
                return _selectedFolder;
            }
            set {
                _selectedFolder = value;
                OnPropertyChanged(nameof(SelectedFolder));
            }
        }

        public ClipboardItemViewModelCommands Commands { get; } = new();

        // 選択中のアイテム(複数選択)
        private ObservableCollection<ClipboardItemViewModel> _selectedItems = [];
        public ObservableCollection<ClipboardItemViewModel> SelectedItems {
            get {
                return _selectedItems;

            }
            set {
                _selectedItems = value;

                OnPropertyChanged(nameof(SelectedItems));
            }
        }

        public ClipboardItemViewModel? SelectedItem {
            get {
                // SelectedItemsの最後のアイテムを返す
                if (SelectedItems.Count > 0) {
                    return SelectedItems[SelectedItems.Count - 1];
                }
                return null;
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
                ClipboardItemViewModel? clipboardItemViewModel = (ClipboardItemViewModel)dataGrid.SelectedItem;
                if (clipboardItemViewModel == null) {
                    return;
                }

                // SelectedItemsをMainWindowViewModelにセット
                SelectedItems.Clear();
                foreach (ClipboardItemViewModel item in dataGrid.SelectedItems) {
                    SelectedItems.Add(item);
                }
                // SelectedTabIndexを更新する処理
                if (SelectedItem != null) {
                    SelectedItem.SelectedTabIndex = lastSelectedIndex;
                }
                OnPropertyChanged(nameof(SelectedItem));
            }

        });

        // ピン留めの切り替え処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> ChangePinCommand => new((parameter) => {

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            foreach (ClipboardItemViewModel clipboardItemViewModel in SelectedItems) {
               Commands.ChangePinCommand.Execute();
            }
        });
        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand<object> DeleteItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteSelectedItems, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                UpdateIndeterminateAction(true);
                List<Task> taskLIst = [];
                // 選択中のアイテムを削除
                foreach (var item in SelectedItems) {
                    Task task = Task.Run(() => {
                        Commands.DeleteItemCommand.Execute(item);
                    });
                    taskLIst.Add(task);
                }
                // 全ての削除処理が終了した後、後続処理を実行
                Task.WhenAll(taskLIst).ContinueWith((task) => {
                    // フォルダ内のアイテムを再読み込む
                    MainUITask.Run(() => {
                        var folders = SelectedItems.Select(x => x.FolderViewModel).DistinctBy(x => x.Folder.Id);
                        foreach (var folder in folders) {
                            folder.LoadFolderCommand.Execute();
                        }
                    });
                    LogWrapper.Info(CommonStringResources.Instance.Deleted);
                    UpdateIndeterminateAction(false);
                });
            }
        });

        #region クリップボードアイテムのコンテキストメニューのInputBinding用のコマンド
        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((parameter) => {
            Commands.OpenContentAsFileCommand.Execute(this.SelectedItem);
        });

        // タイトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateTitleCommand => new((parameter) => {
            Commands.GenerateTitleCommand(SelectedItems.Select(x => x.ContentItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    var folders = SelectedItems.Select(x => x.FolderViewModel).DistinctBy(x => x.Folder.Id);
                    foreach (var folder in folders) {
                        folder.LoadFolderCommand.Execute();
                    }
                });
            });
        });

        // 背景情報を生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateBackgroundInfoCommand => new((parameter) => {
            Commands.GenerateBackgroundInfoCommand(SelectedItems.Select(x => x.ContentItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    var folders = SelectedItems.Select(x => x.FolderViewModel).DistinctBy(x => x.Folder.Id);
                    foreach (var folder in folders) {
                        folder.LoadFolderCommand.Execute();
                    }
                });
            });
        });

        // サマリーを生成する処理　複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateSummaryCommand => new((parameter) => {
            Commands.GenerateSummaryCommand(SelectedItems.Select(x => x.ContentItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    var folders = SelectedItems.Select(x => x.FolderViewModel).DistinctBy(x => x.Folder.Id);
                    foreach (var folder in folders) {
                        folder.LoadFolderCommand.Execute();
                    }
                });
            });
        });

        // 課題リストを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateTasksCommand => new((parameter) => {
            Commands.GenerateTasksCommand(SelectedItems.Select(x => x.ContentItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    var folders = SelectedItems.Select(x => x.FolderViewModel).DistinctBy(x => x.Folder.Id);
                    foreach (var folder in folders) {
                        folder.LoadFolderCommand.Execute();
                    }
                });
            });
        });
        // 文書の信頼度を判定する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> CheckDocumentReliabilityCommand => new((parameter) => {
            Commands.CheckDocumentReliabilityCommand(SelectedItems.Select(x => x.ContentItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    var folders = SelectedItems.Select(x => x.FolderViewModel).DistinctBy(x => x.Folder.Id);
                    foreach (var folder in folders) {
                        folder.LoadFolderCommand.Execute();
                    }
                });
            });
        });


        // ベクトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateVectorCommand => new((parameter) => {
            Commands.GenerateVectorCommand(SelectedItems.Select(x => x.ContentItem).ToList(), () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    var folders = SelectedItems.Select(x => x.FolderViewModel).DistinctBy(x => x.Folder.Id);
                    foreach (var folder in folders) {
                        folder.LoadFolderCommand.Execute();
                    }
                });
            });
        });


        // プロンプトテンプレートを実行
        public SimpleDelegateCommand<Tuple<ClipboardItemViewModel, PromptItem>> ExecutePromptTemplateCommand => new((tuple) => {
            ClipboardItemViewModel itemViewModel = tuple.Item1;
            PromptItem promptItem = tuple.Item2;
            List<ContentItem> contentItems = SelectedItems.Select(x => x.ContentItem).ToList();
            Commands.ExecutePromptTemplateCommand(contentItems, () => {
                // フォルダ内のアイテムを再読み込み
                MainUITask.Run(() => {
                    var folders = SelectedItems.Select(x => x.FolderViewModel).DistinctBy(x => x.Folder.Id);
                    foreach (var folder in folders) {
                        folder.LoadFolderCommand.Execute();
                    }
                });
            }, promptItem);
        });

        // ベクトル検索を実行する処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> VectorSearchCommand => new((parameter) => {
            Commands.VectorSearchCommand.Execute(SelectedItem);
        });


        #endregion

        #region クリップボードアイテムのInputBinding用のコマンド
        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            ClipboardFolderViewModel.DeleteDisplayedItemCommandExecute(SelectedFolder);
        });


        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutItemCommand => new((parameter) => {
            Commands.CutItemCommandExecute(this);
        });
        // Ctrl + C が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CopyItemCommand => new((parameter) => {
            Commands.CopyToClipboardCommandExecute(this);
        });
        // Ctrl + M が押された時の処理
        public SimpleDelegateCommand<object> MergeItemCommand => new((parameter) => {
            ClipboardItemViewModelCommands commands = new();
            Commands.MergeItemCommandExecute(this);
        });

        // Ctrl + Shift + M が押された時の処理
        public SimpleDelegateCommand<object> MergeItemWithHeaderCommand => new((parameter) => {
            ClipboardItemViewModelCommands commands = new();
            Commands.MergeItemWithHeaderCommandExecute(this);
        });

        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            Commands.OpenItemCommand.Execute(this.SelectedItem);

        });


        #endregion



    }
}
