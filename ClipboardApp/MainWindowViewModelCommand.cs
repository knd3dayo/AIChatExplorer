using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.AutoProcessRuleView;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.PythonScriptView.PythonScriptView;
using ClipboardApp.View.SearchView;
using ClipboardApp.View.TagView;
using QAChat.View.PromptTemplateWindow;
using QAChat.View.RAGWindow;
using QAChat.View.VectorDBWindow;
using WpfAppCommon.Control.Settings;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using WpfCommonApp.Control.StatusMessage;

namespace ClipboardApp {
    public partial class MainWindowViewModel {


        /// <summary>
        /// 終了コマンド
        /// </summary>
        public static void ExitCommandExecute() {
            // 終了確認ダイアログを表示。Yesならアプリケーションを終了
            MessageBoxResult result = MessageBox.Show("終了しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                Application.Current.Shutdown();
            }
        }
        /// <summary>
        /// クリップボード監視開始終了フラグを反転させる
        /// メニューの「開始」、「停止」をクリックしたときの処理
        /// </summary>
        /// <param name="windowViewModel"></param>
        public static void ToggleClipboardMonitorCommand(MainWindowViewModel windowViewModel) {
            windowViewModel.IsClipboardMonitor = !windowViewModel.IsClipboardMonitor;
            if (windowViewModel.IsClipboardMonitor) {
                ClipboardController.Start(async (clipboardItem) => {
                    // クリップボードアイテムが追加された時の処理
                    await Task.Run(() => {
                        RootFolderViewModel?.AddItemCommand.Execute(new ClipboardItemViewModel(clipboardItem));
                    });

                    Application.Current.Dispatcher.Invoke(() => {
                        windowViewModel.SelectedFolder?.LoadFolderCommand.Execute();
                    });
                });

                LogWrapper.Info(CommonStringResources.Instance.StartClipboardWatch);
            } else {
                ClipboardController.Stop();
                LogWrapper.Info(CommonStringResources.Instance.StopClipboardWatch);
            }
            // 通知
            windowViewModel.NotifyPropertyChanged(nameof(windowViewModel.IsClipboardMonitor));
            // ボタンのテキストを変更
            windowViewModel.NotifyPropertyChanged(nameof(windowViewModel.ClipboardMonitorButtonText));
        }

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public void FolderSelectionChangedCommandExecute(RoutedEventArgs routedEventArgs) {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ClipboardFolderViewModel clipboardItemFolderViewModel = (ClipboardFolderViewModel)treeView.SelectedItem;
            SelectedFolder = clipboardItemFolderViewModel;
            if (SelectedFolder != null) {
                // Load
                SelectedFolder.LoadFolderCommand.Execute();
            }

        }

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public void ClipboardItemSelectionChangedCommandExecute(RoutedEventArgs routedEventArgs) {
            // ListBoxの場合
            if (routedEventArgs.OriginalSource is ListBox) {
                ListBox listBox = (ListBox)routedEventArgs.OriginalSource;
                ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)listBox.SelectedItem;
                SelectedItem = clipboardItemViewModel;
                // SelectedItemsをMainWindowViewModelにセット
                SelectedItems.Clear();
                foreach (ClipboardItemViewModel item in listBox.SelectedItems) {
                    SelectedItems.Add(item);
                }
            }
            // DataGridの場合
            if (routedEventArgs.OriginalSource is DataGrid) {
                DataGrid dataGrid = (DataGrid)routedEventArgs.OriginalSource;
                ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)dataGrid.SelectedItem;
                SelectedItem = clipboardItemViewModel;
                // SelectedItemsをMainWindowViewModelにセット
                SelectedItems.Clear();
                foreach (ClipboardItemViewModel item in dataGrid.SelectedItems) {
                    SelectedItems.Add(item);
                }
            }
        }


        // OpenOpenAIWindowCommandExecute メニューの「OpenAIチャット」をクリックしたときの処理。選択中のアイテムは無視
        public void OpenOpenAIWindowCommandExecute() {
            ClipboardItemViewModel.OpenOpenAIChatWindowExecute(SelectedFolder, null);
        }
        // 画像エビデンスチェッカーを開くコマンド
        public static void OpenScreenshotCheckerWindowExecute() {
            ImageChat.MainWindow.OpenMainWindow(null, false);
        }

        // OpenRAGManagementWindowCommandExecute メニューの「RAG管理」をクリックしたときの処理。選択中のアイテムは無視
        public static void OpenRAGManagementWindowCommandExecute() {
            // RARManagementWindowを開く
            RagManagementWindow.OpenRagManagementWindow();
        }
        // OpenVectorDBManagementWindowCommandExecute メニューの「ベクトルDB管理」をクリックしたときの処理。選択中のアイテムは無視
        public static void OpenVectorDBManagementWindowCommandExecute() {
            // VectorDBManagementWindowを開く
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Edit, (vectorDBItem) => { });
        }

        // Ctrl + F が押された時の処理
        // 検索ウィンドウを表示する処理
        public static void SearchCommandExecute(ClipboardFolderViewModel? folderViewModel) {

            SearchRule? searchConditionRule;
            // 選択されたフォルダが検索フォルダの場合
            if (folderViewModel != null && folderViewModel.ClipboardItemFolder.FolderType == ClipboardFolder.FolderTypeEnum.Search) {
                searchConditionRule = SearchRuleController.GetSearchRuleByFolder(folderViewModel.ClipboardItemFolder);
                searchConditionRule ??= new() {
                        Type = SearchRule.SearchType.SearchFolder,
                        SearchFolder = folderViewModel.ClipboardItemFolder
                    };
            } else {
                searchConditionRule = ClipboardFolder.GlobalSearchCondition;
            }
            SearchWindow.OpenSearchWindow(searchConditionRule, folderViewModel, false, () => { folderViewModel?.LoadFolderCommand.Execute(); });

        }


        public static void ReloadCommandExecute(MainWindowViewModel windowViewModel) {
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            if (SelectedFolder == null) {
                return;
            }
            // -- 処理完了までProgressIndicatorを表示
            try {
                windowViewModel.IsIndeterminate = true;
                ClipboardFolderViewModel.ReloadCommandExecute(SelectedFolder);
            } finally {
                windowViewModel.IsIndeterminate = false;
            }
        }


        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public static void DeleteDisplayedItemCommandExecute(MainWindowViewModel windowViewModel) {
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            if (SelectedFolder == null) {
                LogWrapper.Error("フォルダが選択されていません");
                return;
            }
            ClipboardFolderViewModel.DeleteDisplayedItemCommandExecute(SelectedFolder);
        }

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public static void DeleteSelectedItemCommandExecute(MainWindowViewModel windowViewModel) {
            // 選択中のアイテムがない場合は処理をしない
            if (windowViewModel.SelectedItems.Count == 0) {
                LogWrapper.Error("選択中のアイテムがない");
                return;
            }
            if (windowViewModel.SelectedFolder == null) {
                LogWrapper.Error("選択中のフォルダがない");
                return;
            }

            ClipboardItemViewModel.DeleteSelectedItemCommandExecute(windowViewModel.SelectedFolder, windowViewModel.SelectedItems);
        }

        // メニューの「設定」をクリックしたときの処理
        public static void SettingCommandExecute() {
            // UserControlの設定ウィンドウを開く
            SettingsUserControl settingsControl = new();
            Window window = new() {
                SizeToContent = SizeToContent.Height,
                Title = CommonStringResources.Instance.SettingWindowTitle,
                Content = settingsControl
            };
            window.ShowDialog();

        }



        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public static void CutItemCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            ObservableCollection<ClipboardItemViewModel> CopiedItems = windowViewModel.CopiedItems;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                LogWrapper.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error("選択中のフォルダがない");
                return;
            }
            // Cut Flagを立てる
            windowViewModel.CutFlag = true;
            // CopiedItemsに選択中のアイテムをセット
            CopiedItems.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                CopiedItems.Add(item);
            }
            windowViewModel.CopiedItemFolder = windowViewModel.SelectedFolder;

            LogWrapper.Info("切り取りしました");

        }
        // Ctrl + C が押された時の処理
        public static void CopyToClipboardCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardItemViewModel? SelectedItem = windowViewModel.SelectedItem;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                LogWrapper.Error("選択中のアイテムがない");
                return;
            }
            if (SelectedItems.Count == 0) {
                LogWrapper.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error("選択中のフォルダがない");
                return;
            }

            // Cutフラグをもとに戻す
            windowViewModel.CutFlag = false;
            // CopiedItemsに選択中のアイテムをセット
            windowViewModel.CopiedItems.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                windowViewModel.CopiedItems.Add(item);
            }
            windowViewModel.CopiedItemFolder = windowViewModel.SelectedFolder;

            try {
                ClipboardController.SetDataObject(SelectedItem.ClipboardItem);
                LogWrapper.Info("コピーしました");

            } catch (Exception e) {
                string message = $"エラーが発生しました。\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
                LogWrapper.Error(message);
            }

        }
        // Ctrl + V が押された時の処理
        public static void PasteFromClipboardCommandExecute(MainWindowViewModel windowViewModel) {
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            ObservableCollection<ClipboardItemViewModel> CopiedItems = windowViewModel.CopiedItems;
            ClipboardFolderViewModel? CopiedItemFolder = windowViewModel.CopiedItemFolder;

            // 貼り付け先のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error("貼り付け先のフォルダがない");
                return;
            }

            // コピー元のアイテムがアプリ内のものである場合
            if (CopiedItems.Count > 0) {
                // コピー元のフォルダがない場合は処理をしない
                if (CopiedItemFolder == null) {
                    LogWrapper.Error("コピー元のフォルダがない");
                    return;
                }
                SelectedFolder.PasteClipboardItemCommandExecute(
                    windowViewModel.CutFlag,
                    CopiedItems,
                    CopiedItemFolder,
                    SelectedFolder
                    );
                // Cutフラグをもとに戻す
                windowViewModel.CutFlag = false;
                // 貼り付け後にコピー選択中のアイテムをクリア
                CopiedItems.Clear();
            } else if (ClipboardController.LastClipboardChangedEventArgs != null) {
                // コピー元のアイテムがない場合はシステムのクリップボードアイテムから貼り付け
                SelectedFolder.ClipboardItemFolder.ProcessClipboardItem( ClipboardController.LastClipboardChangedEventArgs,
                    async (clipboardItem) => {
                        // クリップボードアイテムが追加された時の処理
                        await Task.Run(() => {
                            SelectedFolder?.AddItemCommand.Execute(new ClipboardItemViewModel(clipboardItem));
                        });

                        Application.Current.Dispatcher.Invoke(() => {
                            windowViewModel.SelectedFolder?.LoadFolderCommand.Execute();
                        });
                    });
            }

        }
        // Ctrl + M が押された時の処理
        public static void MergeItemCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                LogWrapper.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error("選択中のフォルダがない");
                return;
            }
            SelectedFolder.MergeItemCommandExecute(
                SelectedFolder,
                SelectedItems,
                false
                );
        }
        // Ctrl + Shift + M が押された時の処理
        public static void MergeItemWithHeaderCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                LogWrapper.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error("選択中のフォルダがない");
                return;
            }
            SelectedFolder.MergeItemCommandExecute(
                SelectedFolder,
                SelectedItems,
                true
                );
        }

        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public static void OpenListPythonScriptWindowCommandExecute(object parameter) {
            PythonCommands.OpenListPythonScriptWindowCommandExecute(parameter);
        }
        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public static void OpenListPromptTemplateWindowCommandExecute(MainWindowViewModel windowViewModel) {
            // ListPromptTemplateWindowを開く
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Edit, (promptTemplateWindowViewModel, OpenAIExecutionModeEnum) => {
                // PromptTemplate = promptTemplateWindowViewModel.PromptItem;
            });
        }
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public void OpenListAutoProcessRuleWindowCommandExecute() {
            // ListAutoProcessRuleWindowを開く
            ListAutoProcessRuleWindow.OpenListAutoProcessRuleWindow(this);

        }
        // メニューの「タグ編集」をクリックしたときの処理
        public static void OpenTagWindowCommandExecute() {
            // TagWindowを開く
            TagWindow.OpenTagWindow(null, () => { });

        }
        // ステータスバーをクリックしたときの処理
        public static void OpenStatusMessageWindowCommand() {
            StatusMessageWindow userControl = new StatusMessageWindow();
            Window window = new() {
                Title = "Status Message",
                Content = userControl
            };
            StatusMessageWindowViewModel statusMessageWindowViewModel = (StatusMessageWindowViewModel)userControl.DataContext;
            statusMessageWindowViewModel.Initialize();
            window.ShowDialog();

        }

    }
}
