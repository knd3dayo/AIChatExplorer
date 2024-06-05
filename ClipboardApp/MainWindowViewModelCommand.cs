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
using WpfAppCommon;
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
                        RootFolderViewModel?.AddItem(new ClipboardItemViewModel(clipboardItem));
                    });

                    Application.Current.Dispatcher.Invoke(() => {
                        windowViewModel.SelectedFolder?.Load();
                    });
                });

                Tools.Info(StringResources.Instance.StartClipboardWatch);
            } else {
                ClipboardController.Stop();
                Tools.Info(StringResources.Instance.StopClipboardWatch);
            }
            // 通知
            windowViewModel.NotifyPropertyChanged(nameof(windowViewModel.IsClipboardMonitor));
            // ボタンのテキストを変更
            windowViewModel.NotifyPropertyChanged(nameof(windowViewModel.ClipboardMonitorButtonText));
        }

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public void FolderSelectionChangedCommandExecute(object parameter) {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ClipboardFolderViewModel clipboardItemFolderViewModel = (ClipboardFolderViewModel)treeView.SelectedItem;
            SelectedFolder = clipboardItemFolderViewModel;
            // Load
            if (SelectedFolder != null) {
                SelectedFolder.Load();
            }

        }

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public void ClipboardItemSelectionChangedCommandExecute(object parameter) {
            RoutedEventArgs routedEventArgs = (RoutedEventArgs)parameter;
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
        /// <summary>
        /// クリップボードアイテムを新規作成する処理
        /// 作成後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public void CreateItemCommandExecute(ClipboardFolderViewModel? folderViewModel) {
            if (folderViewModel == null) {
                Tools.Error("フォルダが選択されていません。");
                return;
            }
            EditItemWindow.OpenEditItemWindow(folderViewModel, null, () => {
                // フォルダ内のアイテムを再読み込み
                folderViewModel.Load();
                Tools.Info("追加しました");
            });
        }




        // OpenOpenAIWindowCommandExecute メニューの「OpenAIチャット」をクリックしたときの処理。選択中のアイテムは無視
        public static void OpenOpenAIWindowCommandExecute() {
            ClipboardItemViewModel.OpenOpenAIChatWindowExecute(null);
        }
        // 画像エビデンスチェッカーを開くコマンド
        public static void OpenScreenshotCheckerWindowExecute() {
            ImageChat.MainWindow.OpenMainWindow(false);
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
            if (folderViewModel != null && folderViewModel.IsSearchFolder) {
                string absoluteCollectionName = folderViewModel.CollectionName;
                searchConditionRule = SearchRuleController.GetSearchRuleByFolderName(absoluteCollectionName);
                if (searchConditionRule == null) {
                    searchConditionRule = new() {
                        Type = SearchRule.SearchType.SearchFolder
                    };
                    folderViewModel.SetSearchFolder(searchConditionRule);

                }
            } else {
                searchConditionRule = ClipboardFolder.GlobalSearchCondition;
            }
            SearchWindow.OpenSearchWindow(searchConditionRule, folderViewModel, () => { folderViewModel?.Load(); });

        }


        public static  void ReloadCommandExecute(MainWindowViewModel windowViewModel) {
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
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardFolderViewModel.DeleteDisplayedItemCommandExecute(SelectedFolder);
        }

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public static void DeleteSelectedItemCommandExecute(MainWindowViewModel windowViewModel) {
            // 選択中のアイテムがない場合は処理をしない
            if (windowViewModel.SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            if (windowViewModel.SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
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
                Title = StringResources.Instance.SettingWindowTitle,
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
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
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

            Tools.Info("切り取りしました");

        }
        // Ctrl + C が押された時の処理
        public static void CopyToClipboardCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardItemViewModel? SelectedItem = windowViewModel.SelectedItem;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
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
                SelectedItem.SetDataObject();
                Tools.Info("コピーしました");

            } catch (Exception e) {
                string message = $"エラーが発生しました。\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
                Tools.Error(message);
            }

        }
        // Ctrl + V が押された時の処理
        public static void PasteFromClipboardCommandExecute(MainWindowViewModel windowViewModel) {
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            ObservableCollection<ClipboardItemViewModel> CopiedItems = windowViewModel.CopiedItems;
            ClipboardFolderViewModel? CopiedItemFolder = windowViewModel.CopiedItemFolder;

            // コピー元のアイテムがない場合は処理をしない
            if (CopiedItems.Count == 0) {
                Tools.Info("コピー元のアイテムがない");
                return;
            }
            // コピー元のフォルダがない場合は処理をしない
            if (CopiedItemFolder == null) {
                Tools.Error("コピー元のフォルダがない");
                return;
            }
            // 貼り付け先のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("貼り付け先のフォルダがない");
                return;
            }
            ClipboardItemViewModel.PasteClipboardItemCommandExecute(
                windowViewModel.CutFlag,
                CopiedItems,
                CopiedItemFolder,
                SelectedFolder
                );
            // Cutフラグをもとに戻す
            windowViewModel.CutFlag = false;
            // 貼り付け後にコピー選択中のアイテムをクリア
            CopiedItems.Clear();

        }
        // Ctrl + M が押された時の処理
        public static void MergeItemCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemViewModel.MergeItemCommandExecute(
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
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }
            ClipboardItemViewModel.MergeItemCommandExecute(
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

        // コンテキストメニューの「ファイルのパスを分割」の実行用コマンド
        public static void SplitFilePathCommandExecute(MainWindowViewModel windowViewModel) {
            ClipboardItemViewModel? SelectedItem = windowViewModel.SelectedItem;
            // 選択中のアイテムを取得
            if (SelectedItem == null) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 処理が終わるまでProgressIndicatorを表示
            windowViewModel.IsIndeterminate = true;
            try {
                // ファイルパスを分割
                SelectedItem.SplitFilePathCommandExecute();
                // 保存
                SelectedItem.Save();

            } catch (ThisApplicationException ex) {
                Tools.Error(ex.Message);
            } finally {
                windowViewModel.IsIndeterminate = false;
            }
        }


    }
}
