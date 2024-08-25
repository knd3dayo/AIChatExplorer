using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ClipboardApp.View.AutoProcessRuleView;
using ClipboardApp.View.HelpView;
using ClipboardApp.View.SearchView;
using ClipboardApp.View.TagView;
using ClipboardApp.ViewModel;
using QAChat.View.PromptTemplateWindow;
using QAChat.View.RAGWindow;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel;
using WpfAppCommon.Control.Settings;
using WpfAppCommon.Model;
using WpfAppCommon.Model.ClipboardApp;
using WpfAppCommon.Utils;
using WpfCommonApp.Control.StatusMessage;

namespace ClipboardApp {
    public partial class MainWindowViewModel {




        // アプリケーションを終了する。
        // Ctrl + Q が押された時の処理
        // メニューの「終了」をクリックしたときの処理

        public static SimpleDelegateCommand<object> ExitCommand => new((parameter) => {
            // 終了確認ダイアログを表示。Yesならアプリケーションを終了
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmExit, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                Application.Current.Shutdown();
            }
        });


        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleClipboardMonitor => new((parameter) => {
            IsClipboardMonitor = !IsClipboardMonitor;
            if (IsClipboardMonitor) {
                ClipboardController.Start(async (clipboardItem) => {
                    // クリップボードアイテムが追加された時の処理
                    await Task.Run(() => {
                        RootFolderViewModel?.AddItemCommand.Execute(new ClipboardItemViewModel(RootFolderViewModel, clipboardItem));
                    });

                    MainUITask.Run(() => {
                        SelectedFolder?.LoadFolderCommand.Execute();
                    });
                });

                LogWrapper.Info(CommonStringResources.Instance.StartClipboardWatchMessage);
            } else {
                ClipboardController.Stop();
                LogWrapper.Info(CommonStringResources.Instance.StopClipboardWatchMessage);
            }
            // 通知
            NotifyPropertyChanged(nameof(IsClipboardMonitor));
            // ボタンのテキストを変更
            NotifyPropertyChanged(nameof(ClipboardMonitorButtonText));
        });

        // Windows通知監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleWindowsNotificationMonitor => new((parameter) => {
            IsWindowsNotificationMonitor = !IsWindowsNotificationMonitor;
            if (IsWindowsNotificationMonitor) {
                WindowsNotificationController.Start(RootFolderViewModel.ClipboardItemFolder, (item) => {
                    // クリップボードアイテムが追加された時の処理
                    RootFolderViewModel.AddItemCommand.Execute(new ClipboardItemViewModel(RootFolderViewModel, item));
                    MainUITask.Run(() => {
                        SelectedFolder?.LoadFolderCommand.Execute();
                    });
                });
                LogWrapper.Info(CommonStringResources.Instance.StartNotificationWatchMessage);

            } else {
                ClipboardController.Stop();
                LogWrapper.Info(CommonStringResources.Instance.StopNotificationWatchMessage);
            }
            // 通知
            NotifyPropertyChanged(nameof(IsWindowsNotificationMonitor));
            // ボタンのテキストを変更
            NotifyPropertyChanged(nameof(WindowsNotificationMonitorButtonText));
        });

        // フォルダが選択された時の処理
        // TreeViewで、SelectedItemChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> FolderSelectionChangedCommand => new((routedEventArgs) => {
            TreeView treeView = (TreeView)routedEventArgs.OriginalSource;
            ClipboardFolderViewModel clipboardItemFolderViewModel = (ClipboardFolderViewModel)treeView.SelectedItem;
            SelectedFolder = clipboardItemFolderViewModel;
            if (SelectedFolder != null) {
                // Load
                SelectedFolder.LoadFolderCommand.Execute();
            }
        });

        // クリップボードアイテムが選択された時の処理
        // ListBoxで、SelectionChangedが発生したときの処理
        public SimpleDelegateCommand<RoutedEventArgs> ClipboardItemSelectionChangedCommand => new((routedEventArgs) => {
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

        });

        // OpenOpenAIWindowCommandExecute メニューの「OpenAIチャット」をクリックしたときの処理。選択中のアイテムは無視
        public void OpenOpenAIWindowCommandExecute() {
            ClipboardFolderViewModel folderViewModel = SelectedFolder ?? RootFolderViewModel;
            // ダミーのアイテム

            ClipboardItemViewModel dummyItem = new(folderViewModel, new ClipboardItem(folderViewModel.ClipboardItemFolder.Id));
            dummyItem.OpenOpenAIChatWindowCommand.Execute();
        }
        // 画像エビデンスチェッカーを開くコマンド
        public void OpenScreenshotCheckerWindowExecute() {
            ImageChat.MainWindow.OpenMainWindow(null, false, () => {
                SelectedFolder?.LoadFolderCommand.Execute();
            });
        }

        // OpenRAGManagementWindowCommandExecute メニューの「RAG管理」をクリックしたときの処理。選択中のアイテムは無視
        public static void OpenRAGManagementWindowCommandExecute() {
            // RARManagementWindowを開く
            ListRAGSourceWindow.OpenRagManagementWindow();
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
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            ClipboardFolderViewModel.DeleteDisplayedItemCommandExecute(SelectedFolder);
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

        // ピン留めの切り替え処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> ChangePinCommand => new((parameter) => {

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(StringResources.NoItemSelected);
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(StringResources.FolderNotSelected);
                return;
            }

            foreach (ClipboardItemViewModel clipboardItemViewModel in SelectedItems) {
                clipboardItemViewModel.ChangePinCommand.Execute();
            }

            // フォルダ内のアイテムを再読み込み
            SelectedFolder.LoadFolderCommand.Execute();
        });


        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public static void CutItemCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            ObservableCollection<ClipboardItemViewModel> CopiedItems = windowViewModel.CopiedItems;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
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

            LogWrapper.Info(CommonStringResources.Instance.Cut);

        }
        // Ctrl + C が押された時の処理
        public static void CopyToClipboardCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardItemViewModel? SelectedItem = windowViewModel.SelectedItem;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            if (SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
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
                LogWrapper.Info(CommonStringResources.Instance.Copied);

            } catch (Exception e) {
                string message = $"{CommonStringResources.Instance.ErrorOccurredAndMessage}:\n{e.Message}\n{CommonStringResources.Instance.StackTrace}:\n{e.StackTrace}";
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
                LogWrapper.Error(CommonStringResources.Instance.NoPasteFolder);
                return;
            }

            // コピー元のアイテムがアプリ内のものである場合
            if (CopiedItems.Count > 0) {
                // コピー元のフォルダがない場合は処理をしない
                if (CopiedItemFolder == null) {
                    LogWrapper.Error(CommonStringResources.Instance.NoCopyFolder);
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
                SelectedFolder.ClipboardItemFolder.ProcessClipboardItem(ClipboardController.LastClipboardChangedEventArgs,
                    async (clipboardItem) => {
                        // クリップボードアイテムが追加された時の処理
                        await Task.Run(() => {
                            SelectedFolder?.AddItemCommand.Execute(new ClipboardItemViewModel(SelectedFolder, clipboardItem));
                        });

                        MainUITask.Run(() => {
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
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
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
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
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
            },
            // PromptItemを作成する関数
            () => { return new PromptItem(); });
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

        // クリップボードアイテムを作成する。
        // Ctrl + N が押された時の処理
        // メニューの「アイテム作成」をクリックしたときの処理
        public SimpleDelegateCommand<object> CreateItemCommand => new((parameter) => {
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            this.SelectedFolder.CreateItemCommandExecute();
        });

        // OpenOpenAIWindowCommandExecute メニューの「OpenAIチャット」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenOpenAIWindowCommand => new((parameter) => {
            OpenOpenAIWindowCommandExecute();
        });
        // OpenScreenshotCheckerWindowExecute メニューの「画像エビデンスチェッカー」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenScreenshotCheckerWindow => new((parameter) => {
            OpenScreenshotCheckerWindowExecute();
        });

        // OpenRAGManagementWindowCommandメニュー　「RAG管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenRAGManagementWindowCommand => new((parameter) => {
            OpenRAGManagementWindowCommandExecute();
        });
        // OpenVectorDBManagementWindowCommandメニュー　「ベクトルDB管理」をクリックしたときの処理。選択中のアイテムは無視
        public SimpleDelegateCommand<object> OpenVectorDBManagementWindowCommand => new((parameter) => {
            OpenVectorDBManagementWindowCommandExecute();
        });

        // Ctrl + F が押された時の処理
        public SimpleDelegateCommand<object> SearchCommand => new((parameter) => {
            SearchCommandExecute(SelectedFolder);
        });


        // Ctrl + R が押された時の処理
        public SimpleDelegateCommand<object> ReloadCommand => new((parameter) => {
            ReloadCommandExecute(this);
        });

        // Ctrl + Delete が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new((parameter) => {
            DeleteDisplayedItemCommandExecute(this);
        });

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public SimpleDelegateCommand<object> DeleteSelectedItemCommand => new((parameter) => {
            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show(StringResources.ConfirmDeleteSelectedItems, StringResources.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                // 選択中のアイテムを削除
                foreach (var item in SelectedItems) {
                    item.DeleteItemCommand.Execute();
                }
                // フォルダ内のアイテムを再読み込む
                SelectedFolder.LoadFolderCommand.Execute();
                LogWrapper.Info(StringResources.Deleted);
            }
        });


        // メニューの「設定」をクリックしたときの処理
        public static SimpleDelegateCommand<object> SettingCommand => new((parameter) => {
            SettingCommandExecute();
        });


        // 選択中のアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            this.SelectedFolder?.OpenItemCommand.Execute(this.SelectedItem);

        });

        // 選択したアイテムをテキストファイルとして開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((parameter) => {
            this.SelectedItem?.OpenContentAsFileCommand.Execute();
        });

        // 選択したアイテムを開く処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> OpenFileCommand => new((parameter) => {
            this.SelectedItem?.OpenFileCommand.Execute();
        });

        // タイトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateTitleCommand => new((parameter) => {
            // 選択中のアイテムすべてに対して処理を行う
            foreach (ClipboardItemViewModel item in SelectedItems) {
                item.GenerateTitleCommand.Execute(() => {
                    // フォルダ内のアイテムを再読み込み
                    MainUITask.Run(() => {
                        SelectedFolder?.LoadFolderCommand.Execute();
                    });
                });
            }
        });

        // 背景情報を生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateBackgroundInfoCommand => new((parameter) => {
            // 選択中のアイテムすべてに対して処理を行う
            foreach (ClipboardItemViewModel item in SelectedItems) {
                item.GenerateBackgroundInfoCommand.Execute();
            }
            // フォルダ内のアイテムを再読み込み
            SelectedFolder?.LoadFolderCommand.Execute();
        });

        // サマリーを生成する処理　複数アイテム処理可
        public SimpleDelegateCommand<object> GenerateSummaryCommand => new((parameter) => {
            // 選択中のアイテムすべてに対して処理を行う
            foreach (ClipboardItemViewModel item in SelectedItems) {
                item.GenerateSummaryCommand.Execute();
            }
            // フォルダ内のアイテムを再読み込み
            SelectedFolder?.LoadFolderCommand.Execute();
        });

        // ベクトルを生成する処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> GenerateVectorCommand => new((parameter) => {
            // 選択中のアイテムすべてに対して処理を行う
            foreach (ClipboardItemViewModel item in SelectedItems) {
                item.GenerateVectorCommand.Execute();
            }
            // フォルダ内のアイテムを再読み込み
            SelectedFolder?.LoadFolderCommand.Execute();
        });

        // ベクトル検索を実行する処理 複数アイテム処理不可
        public SimpleDelegateCommand<object> VectorSearchCommand => new((parameter) => {
            this.SelectedItem?.VectorSearchCommand.Execute();
        });


        // Ctrl + X が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CutItemCommand => new((parameter) => {
            CutItemCommandExecute(this);
        });
        // Ctrl + C が押された時の処理 複数アイテム処理可能
        public SimpleDelegateCommand<object> CopyItemCommand => new((parameter) => {
            CopyToClipboardCommandExecute(this);
        });
        // Ctrl + V が押された時の処理
        public SimpleDelegateCommand<object> PasteItemCommand => new((parameter) => {
            PasteFromClipboardCommandExecute(this);
        });

        // Ctrl + M が押された時の処理
        public SimpleDelegateCommand<object> MergeItemCommand => new((parameter) => {
            MergeItemCommandExecute(this);
        });
        // Ctrl + Shift + M が押された時の処理
        public SimpleDelegateCommand<object> MergeItemWithHeaderCommand => new((parameter) => {
            MergeItemWithHeaderCommandExecute(this);
        });

        // メニューの「Pythonスクリプトを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListPythonScriptWindowCommand => new((parameter) => {
            OpenListPythonScriptWindowCommandExecute(parameter);
        });

        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListPromptTemplateWindowCommand => new((parameter) => {
            OpenListPromptTemplateWindowCommandExecute(this);
        });
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenListAutoProcessRuleWindowCommand => new((parameter) => {
            OpenListAutoProcessRuleWindowCommandExecute();
        });
        // メニューの「タグ編集」をクリックしたときの処理
        public SimpleDelegateCommand<object> OpenTagWindowCommand => new((parameter) => {
            OpenTagWindowCommandExecute();
        });

        // バージョン情報画面を開く処理
        public SimpleDelegateCommand<object> OpenVersionInfoCommand => new((parameter) => {
            VersionWindow.OpenVersionWindow();
        });


    }
}
