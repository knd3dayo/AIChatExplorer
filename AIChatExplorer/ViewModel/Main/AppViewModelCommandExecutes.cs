using System.Collections.ObjectModel;
using System.Windows;
using AIChatExplorer.Model.Folders.Application;
using AIChatExplorer.Model.Folders.ClipboardHistory;
using AIChatExplorer.Model.Folders.ScreenShot;
using AIChatExplorer.View.Settings;
using AIChatExplorer.ViewModel.Content;
using AIChatExplorer.ViewModel.Folders.Application;
using AIChatExplorer.ViewModel.Folders.Search;
using AIChatExplorer.ViewModel.Settings;
using LibMain.Model.Content;
using LibMain.Model.Search;
using LibMain.Resources;
using LibMain.Utils.Common;
using LibUIImageChat.View;
using LibUIMergeChat.View;
using LibUINormalChat.View;
using LibUIMain.Resource;
using LibUIMain.Utils;
using LibUIMain.View.Item;
using LibUIMain.View.Search;
using LibUIMain.ViewModel.Chat;
using LibUIMain.ViewModel.Common;
using LibUIMain.ViewModel.Folder;
using LibUIMain.ViewModel.Item;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace AIChatExplorer.ViewModel.Main {
    public class AppViewModelCommandExecutes(Action<bool> updateIndeterminate, Action updateView) : CommonViewModelCommandExecutes(updateIndeterminate, updateView) {

        public override ObservableCollection<ContentItemViewModel> GetSelectedItems() {
            return MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems;
        }

        // OpenContentItemCommand
        public SimpleDelegateCommand<ContentItemViewModel> OpenItemCommand => new((itemViewModel) => {
            OpenItemCommandExecute(itemViewModel);
        });

        public SimpleDelegateCommand<ContentItemViewModel> OpenNormalChatWindowComman => new((itemViewModel) => {

            // QAChatControlのDrawerを開く
            OpenNormalChatWindowCommandExecute(itemViewModel);
        });

        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> CutItemCommand => new((itemViewModels) => {
            CutItemCommandExecute(itemViewModels);
        });



        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleClipboardMonitor => new((parameter) => {
            bool currentState = MainWindowViewModel.Instance.IsClipboardMonitoringActive;
            if (currentState) {
                StopClipboardMonitorCommandExecute();
            } else {
                // クリップボード監視を開始する
                MainWindowViewModel model = MainWindowViewModel.Instance;
                var rootFolderViewModel = model.RootFolderViewModelContainer.ClipboardHistoryFolderViewModel;
                if (rootFolderViewModel == null) {
                    LogWrapper.Error(CommonStringResources.Instance.RootFolderNotFound);
                    return;
                }
                // クリップボード履歴のルートフォルダを取得
                StartClipboardMonitorCommandExecute(rootFolderViewModel);
            }
        });

        // 画面監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleScreenMonitor => new(async (parameter) => {
            bool currentState = MainWindowViewModel.Instance.IsScreenMonitoringActive;
            if (currentState) {
                StopScreenMonitorCommandExecute();
            } else {
                MainWindowViewModel model = MainWindowViewModel.Instance;
                var screenShotHistoryFolderViewModel = model.RootFolderViewModelContainer.ScreenShotHistoryFolderViewModel;
                if (screenShotHistoryFolderViewModel != null) {
                    await StartScreenMonitorCommandExecute(screenShotHistoryFolderViewModel);
                }
            }
        });


        public static void OpenItemCommandExecute(ContentItemViewModel? itemViewModel) {
            if (itemViewModel == null) {
                return;
            }
            ContentFolderViewModel folderViewModel = itemViewModel.FolderViewModel;

            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(folderViewModel, itemViewModel,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    folderViewModel.FolderCommands.LoadFolderCommand.Execute();
                    LogWrapper.Info(CommonStringResources.Instance.Edited);
                });

            MainTabContent container = new(itemViewModel.ContentItem.Description, editItemControl);

            // UserControlをクローズする場合の処理を設定
            editItemControl.SetCloseUserControl(() => {
                MainWindowViewModel.Instance.MainTabManager.RemoveTabItem(container);
            });

            MainWindowViewModel.Instance.MainTabManager.AddTabItem(container);
        }

        // Command to open Image Chat
        public static void OpenImageChatWindowCommand(ContentItem item, System.Action action) {
            ImageChatWindow.OpenMainWindow(item, action);
        }

        // Command to Open Merge Chat
        public static void OpenMergeChatWindowCommand(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems) {
            MergeChatWindow.OpenWindow(folderViewModel, selectedItems);
        }

        // Command to Open Normal Chat
        public static void OpenNormalChatWindowCommandExecute(ContentItemViewModel itemViewModel) {
            QAChatStartupPropsBase qAChatStartupProps = new QAChatStartupProps(itemViewModel.ContentItem);
            NormalChatWindow.OpenWindow(qAChatStartupProps);
        }


        // Process when "Settings" is clicked in the menu
        public static void SettingCommandExecute() {
            // Open UserControl settings window
            SettingWindow.OpenSettingsWindow();
                }

        // Process to display the search window
        public static void OpenSearchWindowCommandExecute(SearchFolderViewModel searchFolderViewModel, System.Action action) {
            SearchRule? searchConditionRule = new() {
                SearchFolderId = searchFolderViewModel.Folder.Id,
            };
            SearchWindow.OpenSearchWindow(searchConditionRule, searchFolderViewModel.Folder, action);

        }

        // Process when Ctrl + X is pressed on a folder
        public static void CutFolderCommandExecute(MainPanelTreeViewControlViewModel model) {
            // Do not process if no folder is selected
            if (model.SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            model.CopiedFolder = model.SelectedFolder;
            // Set Cut Flag
            ClipboardController.Instance.CutFlag = ClipboardController.CutFlagEnum.Folder;
            // Set the selected folder to CopiedFolder
            ClipboardController.Instance.CopiedObjects = [model.SelectedFolder];
            LogWrapper.Info(CommonStringResources.Instance.Cut);
        }

        // Process when Ctrl + X is pressed on clipboard items; multiple items can be processed
        public static void CutItemCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels) {
            // Do not process if no items are selected
            if (itemViewModels.Count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            // Set Cut Flag
            ClipboardController.Instance.CutFlag = ClipboardController.CutFlagEnum.Item;
            // Set the selected items to CopiedItems
            ClipboardController.Instance.CopiedObjects.Clear();
            foreach (ApplicationItemViewModel item in itemViewModels) {
                ClipboardController.Instance.CopiedObjects.Add(item);
            }
            LogWrapper.Info(CommonStringResources.Instance.Cut);
        }

        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> CopyItemCommand => new((itemViewModels) => {
            CopyToClipboardCommandExecute(itemViewModels);
        });

        // Process when Ctrl + C is pressed
        public static void CopyToClipboardCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels) {

            // Do not process if no items are selected
            if (itemViewModels.Count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            // Reset Cut flag
            ClipboardController.Instance.CutFlag = ClipboardController.CutFlagEnum.None;
            // Set the selected items to CopiedItems
            ClipboardController.Instance.CopiedObjects.Clear();
            foreach (ApplicationItemViewModel item in itemViewModels) {
                ClipboardController.Instance.CopiedObjects.Add(item);
            }
            try {
                ClipboardController.Instance.SetDataObject(itemViewModels.Last().ContentItem);
                LogWrapper.Info(CommonStringResources.Instance.Copied);
            } catch (System.Exception e) {
                string message = $"{CommonStringResources.Instance.ErrorOccurredAndMessage}:\n{e.Message}\n{CommonStringResources.Instance.StackTrace}:\n{e.StackTrace}";
                LogWrapper.Error(message);
            }
        }
        // Command to start/stop clipboard monitoring
        public static void StartClipboardMonitorCommandExecute(ApplicationFolderViewModel targetFolderViewModel) {
            MainWindowViewModel model = MainWindowViewModel.Instance;
            var rootFolderViewModel = model.RootFolderViewModelContainer.GetApplicationRootFolderViewModel();
            if (rootFolderViewModel == null) {
                LogWrapper.Error(CommonStringResources.Instance.RootFolderNotFound);
                return;
            }
            model.IsClipboardMonitoringActive = true;
            ClipboardController.Instance.Start(
                (ApplicationFolder)targetFolderViewModel.Folder,
                async (applicationItem) => {
                    // Process when a clipboard item is added
                    // フォルダのルートフォルダに追加
                    await Task.Run(() => {
                        targetFolderViewModel.FolderCommands.AddItemCommand.Execute(new ApplicationItemViewModel(rootFolderViewModel, applicationItem));
                    });
                    // フォルダのルートフォルダを更新
                    MainUITask.Run(() => {
                        targetFolderViewModel.FolderCommands.LoadFolderCommand.Execute();
                    });
                });
            LogWrapper.Info(CommonStringResources.Instance.StartClipboardWatchMessage);
            // Notification
            model.NotifyPropertyChanged(nameof(model.IsClipboardMonitoringActive));
            // Change button text
            model.NotifyPropertyChanged(nameof(model.ClipboardMonitorButtonText));
        }

        public static void StopClipboardMonitorCommandExecute() {
            MainWindowViewModel model = MainWindowViewModel.Instance;
            model.IsClipboardMonitoringActive = false;
            ClipboardController.Instance.Stop();
            LogWrapper.Info(CommonStringResources.Instance.StopClipboardWatchMessage);
            // Notification
            model.NotifyPropertyChanged(nameof(model.IsClipboardMonitoringActive));
            // Change button text
            model.NotifyPropertyChanged(nameof(model.ClipboardMonitorButtonText));
        }
        // Command to start/stop screen monitoring
        public static async Task StartScreenMonitorCommandExecute(ApplicationFolderViewModel targetFolderViewModel) {
            MainWindowViewModel model = MainWindowViewModel.Instance;
            model.IsScreenMonitoringActive = true;
            // ScreenMonitoringInterval
            int ScreenMonitoringInterval = AIChatExplorerConfig.Instance.ScreenMonitoringInterval; // Default 10 seconds interval
            await ScreenShotController.Instance.Start(
                (ApplicationFolder)targetFolderViewModel.Folder,
                ScreenMonitoringInterval,
                (applicationItem) => {
                    var appViewModel = model.RootFolderViewModelContainer.GetApplicationRootFolderViewModel();
                    if (appViewModel == null) {
                        LogWrapper.Error(CommonStringResources.Instance.RootFolderNotFound);
                        return;
                    }
                    // フォルダのルートフォルダに追加
                    MainUITask.Run(() => {
                        targetFolderViewModel.FolderCommands.AddItemCommand.Execute(new ApplicationItemViewModel(appViewModel, applicationItem));
                        targetFolderViewModel.FolderCommands.LoadFolderCommand.Execute();
                    });
                });
            LogWrapper.Info(CommonStringResources.Instance.StartScreenWatchMessage);
            // Notification
            model.NotifyPropertyChanged(nameof(model.IsScreenMonitoringActive));
            // Change button text
            model.NotifyPropertyChanged(nameof(model.ScreenMonitorButtonText));
        }

        public static void StopScreenMonitorCommandExecute() {
            MainWindowViewModel model = MainWindowViewModel.Instance;
            model.IsScreenMonitoringActive = false;
            ScreenShotController.Instance.Stop();
            LogWrapper.Info(CommonStringResources.Instance.StopScreenWatchMessage);
            // Notification
            model.NotifyPropertyChanged(nameof(model.IsScreenMonitoringActive));
            // Change button text
            model.NotifyPropertyChanged(nameof(model.ScreenMonitorButtonText));
        }

        // Command to start/stop integrated monitoring
        public static async Task StartIntegratedMonitorCommandExecute(ApplicationFolderViewModel targetFolderViewModel) {
            MainWindowViewModel model = MainWindowViewModel.Instance;
            model.IsIntegratedMonitorActive = true;
            // いったんクリップボード監視を停止
            StopClipboardMonitorCommandExecute();
            // いったん画面監視を停止
            StopScreenMonitorCommandExecute();

            StartClipboardMonitorCommandExecute(targetFolderViewModel);
            await StartScreenMonitorCommandExecute(targetFolderViewModel);
            LogWrapper.Info(CommonStringResources.Instance.StartIntegratedMonitorMessage);
            // Notification
            model.NotifyPropertyChanged(nameof(model.IsIntegratedMonitorActive));
            // Change button text
            model.NotifyPropertyChanged(nameof(model.IntegratedMonitorButtonText));
        }
        public static void StopIntegratedMonitorCommandExecute() {
            MainWindowViewModel model = MainWindowViewModel.Instance;
            model.IsIntegratedMonitorActive = false;
            StopClipboardMonitorCommandExecute();
            StopScreenMonitorCommandExecute();
            LogWrapper.Info(CommonStringResources.Instance.StopIntegratedMonitorMessage);
            // Notification
            model.NotifyPropertyChanged(nameof(model.IsIntegratedMonitorActive));
            // Change button text
            model.NotifyPropertyChanged(nameof(model.IntegratedMonitorButtonText));
        }


        // -----------------------------------------------------------------------------------
        // プログレスインジケーター表示の処理

        // Process when Ctrl + V is pressed
        public override async Task PasteFromClipboardCommandExecute() {
            MainWindowViewModel windowViewModel = MainWindowViewModel.Instance;
            ContentFolderViewModel? folder = windowViewModel.MainPanelTreeViewControlViewModel?.SelectedFolder;
            if (folder is not ApplicationFolderViewModel SelectedFolder) {
                LogWrapper.Error(CommonStringResources.Instance.NoPasteFolder);
                return;
            }
            List<object> CopiedItems = ClipboardController.Instance.CopiedObjects;
            // Do not process if no folder is selected
            if (SelectedFolder == null || SelectedFolder.Folder is not ApplicationFolder clipboardFolder) {
                LogWrapper.Error(CommonStringResources.Instance.NoPasteFolder);
                return;
            }

            // If the source items are from within the app
            if (CopiedItems.Count > 0) {
                await SelectedFolder.PasteApplicationItemCommandExecute(
                    ClipboardController.Instance.CutFlag,
                    CopiedItems,
                    SelectedFolder
                );
                // Reset Cut flag
                ClipboardController.Instance.CutFlag = ClipboardController.CutFlagEnum.None;
                // Clear selected items after pasting
                CopiedItems.Clear();
            } else if (ClipboardController.LastClipboardChangedEventArgs != null) {
                // システムのクリップボードからのコピーの場合
                ProcessApplicationItem(clipboardFolder, ClipboardController.LastClipboardChangedEventArgs);
            }
        }

        private void ProcessApplicationItem(ApplicationFolder clipboardFolder, ClipboardChangedEventArgs lastClipboardChangedEventArgs) {
            UpdateIndeterminate(true);
            ClipboardController.ProcessClipboardItem(lastClipboardChangedEventArgs, clipboardFolder,
                async (applicationItem) => {
                    // Process when a clipboard item is added
                    await Task.Run(async () => {
                        // SaveAsync to folder if saveToFolder is true
                        await clipboardFolder.AddItemAsync(applicationItem);
                        // Process after pasting
                    }).ContinueWith((obj) => {
                        UpdateIndeterminate(false);
                        UpdateView();
                    });
                });
        }


    }
}
