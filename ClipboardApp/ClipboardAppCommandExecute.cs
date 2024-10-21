using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.Settings;
using ClipboardApp.Utils;
using ClipboardApp.View.SearchView;
using ClipboardApp.View.SelectVectorDBView;
using ClipboardApp.ViewModel;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using QAChat.Control;
using QAChat.Resource;
using QAChat.View.ImageChat;
using QAChat.View.RAGWindow;
using QAChat.View.VectorDBWindow;
using QAChat.ViewModel.VectorDBWindow;
using WpfAppCommon.Utils;

namespace ClipboardApp {
    public class ClipboardAppCommandExecute {

        // アプリケーション終了コマンド
        public static void ExitCommand() {
            // 終了確認ダイアログを表示。Yesならアプリケーションを終了
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmExit, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                Application.Current.Shutdown();
            }
        }
        // クリップボード監視を開始/停止するコマンド
        public static void StartStopClipboardMonitorCommand(MainWindowViewModel model) {
            model.IsClipboardMonitor = !model.IsClipboardMonitor;
            if (model.IsClipboardMonitor) {
                MainWindowViewModel.ClipboardController.Start(async (clipboardItem) => {
                    // クリップボードアイテムが追加された時の処理
                    await Task.Run(() => {
                        model.RootFolderViewModel?.AddItemCommand.Execute(new ClipboardItemViewModel(model.RootFolderViewModel, clipboardItem));
                    });

                    MainUITask.Run(() => {
                        model.SelectedFolder?.LoadFolderCommand.Execute();
                    });
                });

                LogWrapper.Info(CommonStringResources.Instance.StartClipboardWatchMessage);
            } else {
                MainWindowViewModel.ClipboardController.Stop();
                LogWrapper.Info(CommonStringResources.Instance.StopClipboardWatchMessage);
            }
            // 通知
            model.NotifyPropertyChanged(nameof(model.IsClipboardMonitor));
            // ボタンのテキストを変更
            model.NotifyPropertyChanged(nameof(model.ClipboardMonitorButtonText));


        }
        //  Windows通知監視開始終了フラグを反転させる
        public static void StartStopWindowsNotificationMonitorCommand(MainWindowViewModel model) {
            model.IsWindowsNotificationMonitor = !model.IsWindowsNotificationMonitor;
            if (model.IsWindowsNotificationMonitor) {
                WindowsNotificationController.Start(model.RootFolderViewModel.ClipboardItemFolder, (item) => {
                    // クリップボードアイテムが追加された時の処理
                    model.RootFolderViewModel.AddItemCommand.Execute(new ClipboardItemViewModel(model.RootFolderViewModel, item));
                    MainUITask.Run(() => {
                        model.SelectedFolder?.LoadFolderCommand.Execute();
                    });
                });
                LogWrapper.Info(CommonStringResources.Instance.StartNotificationWatchMessage);

            } else {
                MainWindowViewModel.ClipboardController.Stop();
                LogWrapper.Info(CommonStringResources.Instance.StopNotificationWatchMessage);
            }
            // 通知
            model.NotifyPropertyChanged(nameof(model.IsWindowsNotificationMonitor));
            // ボタンのテキストを変更
            model.NotifyPropertyChanged(nameof(model.WindowsNotificationMonitorButtonText));
        }

        // OpenAI Chatを開くコマンド
        public static void OpenOpenAIChatWindowCommand(ClipboardItem item) {
            QAChatStartupProps qAChatStartupProps = MainWindowViewModel.CreateQAChatStartupProps(item);
            QAChat.View.QAChatMain.QAChatMainWindow.OpenOpenAIChatWindow(qAChatStartupProps);
        }

        // 画像チャットを開くコマンド
        public static void OpenImageChatWindowCommand(ClipboardItem item, Action action) {
            ImageChatMainWindow.OpenMainWindow(item, action);
        }


        // メニューの「RAG管理」をクリックしたときの処理
        public static void OpenRAGManagementWindowCommand() {
            // RARManagementWindowを開く
            ListRAGSourceWindow.OpenRagManagementWindow();
        }



        // メニューの「ベクトルDB管理」をクリックしたときの処理
        public static void OpenVectorDBManagementWindowCommand() {
            // VectorDBManagementWindowを開く
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Edit, (vectorDBItem) => { });
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
        // 検索ウィンドウを表示する処理
        public static void OpenSearchWindowCommand(ClipboardFolder folder, Action action) {
            SearchRule? searchConditionRule;
            // 選択されたフォルダが検索フォルダの場合
            if (folder.FolderType == ClipboardFolder.FolderTypeEnum.Search) {
                searchConditionRule = SearchRuleController.GetSearchRuleByFolder(folder);
                searchConditionRule ??= new() {
                    Type = SearchRule.SearchType.SearchFolder,
                    SearchFolder = folder
                };
            } else {
                searchConditionRule = ClipboardFolder.GlobalSearchCondition;
            }
            SearchWindow.OpenSearchWindow(searchConditionRule, folder, false, action);
        }

        // フォルダをリロードするコマンド
        public static void ReloadFolderCommand(MainWindowViewModel model) {
            if (model.SelectedFolder == null) {
                return;
            }
            // -- 処理完了までProgressIndicatorを表示
            try {
                model.IsIndeterminate = true;
                ClipboardFolderViewModel.ReloadCommandExecute(model.SelectedFolder);
            } finally {
                model.IsIndeterminate = false;
            }
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
                SelectedItems
                );
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
                SelectedItems
                );
        }
        // フォルダのCtrl + X が押された時の処理
        public static void CutFolderCommandExecute(MainWindowViewModel windowViewModel) {

            // 選択中のフォルダがない場合は処理をしない
            if (windowViewModel.SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            windowViewModel.CopiedFolder = windowViewModel.SelectedFolder;
            // Cut Flagを立てる
            windowViewModel.CutFlag = MainWindowViewModel.CutFlagEnum.Folder;
            // CopiedFolderに選択中のフォルダをセット
            windowViewModel.CopiedObjects = [windowViewModel.SelectedFolder];

            LogWrapper.Info(CommonStringResources.Instance.Cut);
        }

        // クリップボードアイテムのCtrl + X が押された時の処理 複数アイテム処理可能
        public static void CutItemCommandExecute(MainWindowViewModel windowViewModel) {
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
            // Cut Flagを立てる
            windowViewModel.CutFlag = MainWindowViewModel.CutFlagEnum.Item;
            windowViewModel.CopiedFolder = windowViewModel.SelectedFolder;
            // CopiedItemsに選択中のアイテムをセット
            windowViewModel.CopiedObjects.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                windowViewModel.CopiedObjects.Add(item);
            }

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
            windowViewModel.CutFlag = MainWindowViewModel.CutFlagEnum.None;

            // CopiedItemsに選択中のアイテムをセット
            windowViewModel.CopiedObjects.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                windowViewModel.CopiedObjects.Add(item);
            }
            windowViewModel.CopiedFolder = windowViewModel.SelectedFolder;

            try {
                MainWindowViewModel.ClipboardController.SetDataObject(SelectedItem.ClipboardItem);
                LogWrapper.Info(CommonStringResources.Instance.Copied);

            } catch (Exception e) {
                string message = $"{CommonStringResources.Instance.ErrorOccurredAndMessage}:\n{e.Message}\n{CommonStringResources.Instance.StackTrace}:\n{e.StackTrace}";
                LogWrapper.Error(message);
            }

        }

        // Ctrl + V が押された時の処理
        public static void PasteFromClipboardCommandExecute(MainWindowViewModel windowViewModel) {
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            List<object> CopiedItems = windowViewModel.CopiedObjects;
            ClipboardFolderViewModel? CopiedItemFolder = windowViewModel.CopiedFolder;

            // 貼り付け先のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.NoPasteFolder);
                return;
            }
            // コピー元のフォルダがない場合は処理をしない
            if (CopiedItemFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.NoCopyFolder);
                return;
            }

            // コピー元のアイテムがアプリ内のものである場合
            if (CopiedItems.Count > 0) {
                SelectedFolder.PasteClipboardItemCommandExecute(
                    windowViewModel.CutFlag,
                    CopiedItems,
                    SelectedFolder
                    );

                // Cutフラグをもとに戻す
                windowViewModel.CutFlag = MainWindowViewModel.CutFlagEnum.None;
                // 貼り付け後にコピー選択中のアイテムをクリア
                CopiedItems.Clear();
            } else if (ClipboardController.LastClipboardChangedEventArgs != null) {
                // コピー元のアイテムがない場合はシステムのクリップボードアイテムから貼り付け
                SelectedFolder.ClipboardItemFolder.ProcessClipboardItem(ClipboardController.LastClipboardChangedEventArgs,
                    async (clipboardItem) => {
                        // クリップボードアイテムが追加された時の処理
                        await Task.Run(() => {
                            // saveToFolderがTrueの場合はフォルダに保存
                            SelectedFolder?.AddItemCommand.Execute(new ClipboardItemViewModel(SelectedFolder, clipboardItem));
                            // 貼り付け後の処理
                            MainUITask.Run(() => {
                                windowViewModel.SelectedFolder?.LoadFolderCommand.Execute();
                            });
                        });

                    });
            }

        }


        // フォルダを開くコマンド
        public static void OpenFolderCommand(ClipboardItem contentItem) {
            // ContentTypeがFileの場合のみフォルダを開く
            if (contentItem.ContentType != ContentTypes.ContentItemTypes.Files) {
                LogWrapper.Error(CommonStringResources.Instance.CannotOpenFolderForNonFileContent);
                return;
            }
            // Process.Startでフォルダを開く
            foreach (var item in contentItem.ClipboardItemFiles) {
                string? folderPath = item.FolderName;
                if (folderPath != null) {
                    var p = new Process {
                        StartInfo = new ProcessStartInfo(folderPath) {
                            UseShellExecute = true
                        }
                    };
                    p.Start();
                }
            }
        }

        // テキストを抽出するコマンド
        public static void ExtractTextCommand(ClipboardItem contentItem) {
            if (contentItem.ContentType != ContentTypes.ContentItemTypes.Files) {
                LogWrapper.Error(CommonStringResources.Instance.CannotExtractTextForNonFileContent);
                return;
            }
            contentItem.ExtractTextCommandExecute();
        }

        // ファイルを開くコマンド
        public static void OpenFileCommand(ClipboardItem contentItem) {
            // 選択中のアイテムを開く
            ClipboardProcessController.OpenClipboardItemFile(contentItem, false);
        }

        // ファイルを新規ファイルとして開くコマンド
        public static void OpenFileAsNewFileCommand(ClipboardItem contentItem) {
            // 選択中のアイテムを開く
            ClipboardProcessController.OpenClipboardItemFile(contentItem, true);
        }

        // タイトルを生成するコマンド
        public static async void GenerateTitleCommand(List<ClipboardItem> contentItem, object obj) {
            LogWrapper.Info(CommonStringResources.Instance.GenerateTitleInformation);
            await Task.Run(() => {
                foreach (var item in contentItem) {
                    item.CreateAutoTitleWithOpenAI();
                    // 保存
                    item.Save(false);
                }
                // objectがActionの場合は実行
                if (obj is Action action) {
                    action();
                }

            });
            LogWrapper.Info(CommonStringResources.Instance.GeneratedTitleInformation);
        }


        // プロンプトテンプレートを実行するコマンド
        public static async void ExecutePromptTemplateCommand(List<ClipboardItem> contentItem, object obj, string promptName) {
            LogWrapper.Info(PythonAILib.Resource.PythonAILibStringResources.Instance.PromptTemplateExecute(promptName));
            await Task.Run(() => {
                foreach (var item in contentItem) {
                    item.CreateChatResult(promptName);
                    // 保存
                    item.Save(false);
                }
                // objectがActionの場合は実行
                if (obj is Action action) {
                    action();
                }
            });
            LogWrapper.Info(PythonAILib.Resource.PythonAILibStringResources.Instance.PromptTemplateExecuted(promptName));
        }


        // 背景情報を生成するコマンド
        public static void GenerateBackgroundInfoCommand(List<ClipboardItem> contentItem, object obj) {
            string promptName = PromptItem.SystemDefinedPromptNames.BackgroundInformationGeneration.ToString();
            ExecutePromptTemplateCommand(contentItem, obj, promptName);
        }

        // サマリーを生成するコマンド
        public static void GenerateSummaryCommand(List<ClipboardItem> contentItem, object obj) {
            string promptName = PromptItem.SystemDefinedPromptNames.SummaryGeneration.ToString();
            ExecutePromptTemplateCommand(contentItem, obj, promptName);
        }

        // 課題リストを生成するコマンド
        public static void GenerateTasksCommand(List<ClipboardItem> contentItem, object obj) {
            string promptName = PromptItem.SystemDefinedPromptNames.TasksGeneration.ToString();
            ExecutePromptTemplateCommand(contentItem, obj, promptName);
        }

        // ベクトルを生成するコマンド
        public static async void GenerateVectorCommand(List<ClipboardItem> contentItem, object obj) {
            LogWrapper.Info(CommonStringResources.Instance.GenerateVector2);
            await Task.Run(() => {
                foreach (var item in contentItem) {
                    item.UpdateEmbedding();
                    // 保存
                    item.Save(false);
                }
                // objectがActionの場合は実行
                if (obj is Action action) {
                    action();
                }
            });
            LogWrapper.Info(CommonStringResources.Instance.GeneratedVector);
        }

        // ベクトル検索を実行するコマンド
        public static void OpenVectorSearchWindowCommand(ClipboardFolder folder) {
            // ベクトル検索結果ウィンドウを開く
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new();
            // ベクトルDBアイテムを選択したときのアクション
            vectorSearchWindowViewModel.SelectVectorDBItemAction = (vectorDBItems) => {
                SelectVectorDBWindow.OpenSelectVectorDBWindow(MainWindowViewModel.ActiveInstance.RootFolderViewModel, true, (selectedItems) => {
                    foreach (var item in selectedItems) {
                        vectorDBItems.Add(item);
                    }
                });
            };

            vectorSearchWindowViewModel.VectorDBItem = folder.GetVectorDBItem();

            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);

        }

        // ベクトル検索を実行するコマンド
        public static void OpenVectorSearchWindowCommand(ClipboardItem contentItem) {

            // ベクトル検索結果ウィンドウを開く
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new();
            // ベクトルDBアイテムを選択したときのアクション
            vectorSearchWindowViewModel.SelectVectorDBItemAction = (vectorDBItems) => {
                SelectVectorDBWindow.OpenSelectVectorDBWindow(MainWindowViewModel.ActiveInstance.RootFolderViewModel, true, (selectedItems) => {
                    foreach (var item in selectedItems) {
                        vectorDBItems.Add(item);
                    }
                });
            };


            if (ClipboardAppConfig.Instance.IncludeBackgroundInfoInEmbedding) {
                vectorSearchWindowViewModel.InputText = contentItem.Content + "\n" + contentItem.BackgroundInfo;
            } else {
                vectorSearchWindowViewModel.InputText = contentItem.Content;
            }
            vectorSearchWindowViewModel.VectorDBItem = contentItem.GetMainVectorDBItem();
            vectorSearchWindowViewModel.InputText = contentItem.Content;
            // ベクトル検索を実行
            vectorSearchWindowViewModel.SendCommand.Execute(null);


            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }

        // テキストをファイルとして開くコマンド
        public static void OpenContentAsFileCommand(ClipboardItem contentItem) {
            try {
                // 選択中のアイテムを開く
                ClipboardProcessController.OpenClipboardItemContent(contentItem);
            } catch (Exception e) {
                LogWrapper.Error(e.Message);
            }
        }

    }
}
