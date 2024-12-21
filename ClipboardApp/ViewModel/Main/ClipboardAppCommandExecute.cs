using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Search;
using ClipboardApp.Utils;
using ClipboardApp.View.Search;
using ClipboardApp.View.Settings;
using ClipboardApp.ViewModel.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.Prompt;
using QAChat.Resource;
using QAChat.View.Folder;
using QAChat.View.QAChatMain;
using QAChat.View.RAG;
using QAChat.View.VectorDB;
using QAChat.ViewModel;
using QAChat.ViewModel.VectorDB;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Main
{
    public class ClipboardAppCommandExecute {

        public ClipboardAppCommandExecute(ClipboardItemViewModel clipboardItemViewModel) {
            ClipboardItemViewModel = clipboardItemViewModel;
        }
        private ClipboardItemViewModel ClipboardItemViewModel { get; }

        // フォルダを開くコマンド
        public SimpleDelegateCommand<object> OpenFolderCommand => new((parameter) => {
            ClipboardAppCommandExecute.OpenFolder(ClipboardItemViewModel.ClipboardItem);
        });

        // テキストをファイルとして開くコマンド
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((obj) => {
            ClipboardAppCommandExecute.OpenContentAsFile(ClipboardItemViewModel.ClipboardItem);
        });

        // ファイルを開くコマンド
        public SimpleDelegateCommand<object> OpenFileCommand => new((obj) => {
            ClipboardAppCommandExecute.OpenFile(ClipboardItemViewModel.ClipboardItem);
        });


        // ファイルを新規ファイルとして開くコマンド
        public SimpleDelegateCommand<object> OpenFileAsNewFileCommand => new((obj) => {
            ClipboardAppCommandExecute.OpenFileAsNewFile(ClipboardItemViewModel.ClipboardItem);
        });

        // QAChatButtonCommand
        public SimpleDelegateCommand<object> QAChatButtonCommand => new((obj) => {
            // QAChatControlのDrawerを開く
            ClipboardAppCommandExecute.OpenOpenAIChatWindowCommand(ClipboardItemViewModel.ClipboardItem);
        });

        // ベクトル検索を実行するコマンド
        public SimpleDelegateCommand<object> VectorSearchCommand => new((obj) => {
            ClipboardAppCommandExecute.OpenVectorSearchWindowCommand(ClipboardItemViewModel.ClipboardItem);
        });

        // コンテキストメニューの「テキストを抽出」の実行用コマンド
        public SimpleDelegateCommand<object> ExtractTextCommand => new((parameter) => {
            if (ClipboardItemViewModel == null) {
                LogWrapper.Error("クリップボードアイテムが選択されていません");
                return;
            }
            ClipboardAppCommandExecute.ExtractText(ClipboardItemViewModel.ClipboardItem);
            int index = ClipboardItemViewModel.SelectedTabIndex;
            ClipboardItemViewModel.SelectedTabIndex = index;
        });

        // ピン留めの切り替えコマンド
        public SimpleDelegateCommand<object> ChangePinCommand => new((obj) => {
            ClipboardItemViewModel.IsPinned = !ClipboardItemViewModel.IsPinned;
            // ピン留めの時は更新日時を変更しない
            SaveClipboardItemCommand.Execute(false);
        });

        // アイテム保存
        public SimpleDelegateCommand<bool> SaveClipboardItemCommand => new(ClipboardItemViewModel.ClipboardItem.Save);

        // Delete
        public SimpleDelegateCommand<ClipboardItemViewModel> DeleteItemCommand => new((obj) => {
            ClipboardItemViewModel.ClipboardItem.Delete();
        });






        /// <summary>
        /// Application exit command
        /// </summary>
        public static void ExitCommand() {
            // Display exit confirmation dialog. If Yes, exit the application
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmExit, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                Application.Current.Shutdown();
            }
        }
        // Command to start/stop clipboard monitoring
        public static void StartStopClipboardMonitorCommand() {
            MainWindowViewModel model = MainWindowViewModel.Instance;
            model.IsClipboardMonitoringActive = !model.IsClipboardMonitoringActive;
            if (model.IsClipboardMonitoringActive) {
                MainWindowViewModel.ClipboardController.Start(async (clipboardItem) => {
                    // Process when a clipboard item is added
                    await Task.Run(() => {
                        model.RootFolderViewModelContainer.RootFolderViewModel?.AddItemCommand.Execute(new ClipboardItemViewModel(model.RootFolderViewModelContainer.RootFolderViewModel, clipboardItem));
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
            // Notification
            model.NotifyPropertyChanged(nameof(model.IsClipboardMonitoringActive));
            // Change button text
            model.NotifyPropertyChanged(nameof(model.ClipboardMonitorButtonText));
        }

        // Toggle flag to start/stop Windows notification monitoring
        public static void StartStopWindowsNotificationMonitorCommand() {
            MainWindowViewModel model = MainWindowViewModel.Instance;
            model.IsWindowsNotificationMonitorActive = !model.IsWindowsNotificationMonitorActive;
            if (model.IsWindowsNotificationMonitorActive) {
                WindowsNotificationController.Start(model.RootFolderViewModelContainer.RootFolderViewModel.ClipboardItemFolder, (item) => {
                    // Process when a clipboard item is added
                    model.RootFolderViewModelContainer.RootFolderViewModel.AddItemCommand.Execute(new ClipboardItemViewModel(model.RootFolderViewModelContainer.RootFolderViewModel, item));
                    MainUITask.Run(() => {
                        model.SelectedFolder?.LoadFolderCommand.Execute();
                    });
                });
                LogWrapper.Info(CommonStringResources.Instance.StartNotificationWatchMessage);
            } else {
                MainWindowViewModel.ClipboardController.Stop();
                LogWrapper.Info(CommonStringResources.Instance.StopNotificationWatchMessage);
            }
            // Notification
            model.NotifyPropertyChanged(nameof(model.IsWindowsNotificationMonitorActive));
            // Change button text
            model.NotifyPropertyChanged(nameof(model.WindowsNotificationMonitorButtonText));
        }

        // Command to open OpenAI Chat
        public static void OpenOpenAIChatWindowCommand(ClipboardItem? item) {
            if (item == null) {
                // チャット履歴用のItemの設定
                ClipboardFolder chatFolder = MainWindowViewModel.Instance.RootFolderViewModelContainer.ChatRootFolderViewModel.ClipboardItemFolder;
                item = new(chatFolder.Id);
                // タイトルを日付 + 元のタイトルにする
                item.Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Chat";

            }
            QAChatStartupProps qAChatStartupProps = CreateQAChatStartupProps(item);
            QAChatMainWindow.OpenOpenAIChatWindow(qAChatStartupProps);
        }

        // Command to open Image Chat
        public static void OpenImageChatWindowCommand(Model.ClipboardItem item, Action action) {
            ImageChatMainWindow.OpenMainWindow(item, action);
        }

        // Process when "RAG Management" is clicked in the menu
        public static void OpenRAGManagementWindowCommand() {
            // Open RARManagementWindow
            ListRAGSourceWindow.OpenRagManagementWindow();
        }

        // Process when "Vector DB Management" is clicked in the menu
        public static void OpenVectorDBManagementWindowCommand() {
            // Open VectorDBManagementWindow
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Edit, MainWindowViewModel.Instance.RootFolderViewModelContainer.RootFolderViewModel, (vectorDBItem) => { });
        }

        // Process when "Settings" is clicked in the menu
        public static void SettingCommandExecute() {
            // Open UserControl settings window
            SettingsUserControl settingsControl = new();
            Window window = new() {
                SizeToContent = SizeToContent.Height,
                Title = CommonStringResources.Instance.SettingWindowTitle,
                Content = settingsControl
            };
            window.ShowDialog();
        }

        // Process to display the search window
        public static void OpenSearchWindowCommand(ClipboardFolder folder, Action action) {
            SearchRule? searchConditionRule;
            // If the selected folder is a search folder
            if (folder.FolderType == FolderTypeEnum.Search) {
                searchConditionRule = SearchRuleController.GetSearchRuleByFolder(folder);
                searchConditionRule ??= new() {
                    Type = SearchRule.SearchType.SearchFolder,
                    SearchFolder = folder
                };
            } else {
                searchConditionRule = FolderManager.GlobalSearchCondition;
            }
            SearchWindow.OpenSearchWindow(searchConditionRule, folder, false, action);
        }

        // Command to reload the folder
        public static void ReloadFolderCommand(MainWindowViewModel model) {
            if (model.SelectedFolder == null) {
                return;
            }
            // Display ProgressIndicator until processing is complete
            try {
                model.UpdateIndeterminate(true);
                model.SelectedFolder.LoadFolderCommand.Execute();
                LogWrapper.Info(CommonStringResources.Instance.Reloaded);
            } finally {
                model.UpdateIndeterminate(false);
            }
        }

        // Process when Ctrl + Shift + M is pressed
        public static void MergeItemWithHeaderCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            // Do not process if no items are selected
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // Do not process if no folder is selected
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            SelectedFolder.MergeItemCommandExecute(
                SelectedFolder,
                SelectedItems
            );
        }
        // Process when Ctrl + M is pressed
        public static void MergeItemCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            // Do not process if no items are selected
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // Do not process if no folder is selected
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            SelectedFolder.MergeItemCommandExecute(
                SelectedFolder,
                SelectedItems
            );
        }

        // Process when Ctrl + X is pressed on a folder
        public static void CutFolderCommandExecute(MainWindowViewModel windowViewModel) {
            // Do not process if no folder is selected
            if (windowViewModel.SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            windowViewModel.CopiedFolder = windowViewModel.SelectedFolder;
            // Set Cut Flag
            windowViewModel.CutFlag = MainWindowViewModel.CutFlagEnum.Folder;
            // Set the selected folder to CopiedFolder
            windowViewModel.CopiedObjects = [windowViewModel.SelectedFolder];
            LogWrapper.Info(CommonStringResources.Instance.Cut);
        }

        // Process when Ctrl + X is pressed on clipboard items; multiple items can be processed
        public static void CutItemCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            // Do not process if no items are selected
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // Do not process if no folder is selected
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            // Set Cut Flag
            windowViewModel.CutFlag = MainWindowViewModel.CutFlagEnum.Item;
            windowViewModel.CopiedFolder = windowViewModel.SelectedFolder;
            // Set the selected items to CopiedItems
            windowViewModel.CopiedObjects.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                windowViewModel.CopiedObjects.Add(item);
            }
            LogWrapper.Info(CommonStringResources.Instance.Cut);
        }

        // Process when Ctrl + C is pressed
        public static void CopyToClipboardCommandExecute(MainWindowViewModel windowViewModel) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;
            ClipboardItemViewModel? SelectedItem = windowViewModel.SelectedItem;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            // Do not process if no items are selected
            if (SelectedItem == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // Do not process if no folder is selected
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.FolderNotSelected);
                return;
            }
            // Reset Cut flag
            windowViewModel.CutFlag = MainWindowViewModel.CutFlagEnum.None;
            // Set the selected items to CopiedItems
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

        // Process when Ctrl + V is pressed
        public static void PasteFromClipboardCommandExecute() {
            MainWindowViewModel windowViewModel = MainWindowViewModel.Instance;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            List<object> CopiedItems = windowViewModel.CopiedObjects;
            ClipboardFolderViewModel? CopiedItemFolder = windowViewModel.CopiedFolder;
            // Do not process if no folder is selected
            if (SelectedFolder == null) {
                LogWrapper.Error(CommonStringResources.Instance.NoPasteFolder);
                return;
            }
            // If the source items are from within the app
            if (CopiedItems.Count > 0) {
                SelectedFolder.PasteClipboardItemCommandExecute(
                    windowViewModel.CutFlag,
                    CopiedItems,
                    SelectedFolder
                );
                // Reset Cut flag
                windowViewModel.CutFlag = MainWindowViewModel.CutFlagEnum.None;
                // Clear selected items after pasting
                CopiedItems.Clear();
            } else if (ClipboardController.LastClipboardChangedEventArgs != null) {
                // If there are no source items, paste from the system clipboard
                SelectedFolder.ClipboardItemFolder.ProcessClipboardItem(ClipboardController.LastClipboardChangedEventArgs,
                    async (clipboardItem) => {
                        // Process when a clipboard item is added
                        await Task.Run(() => {
                            // Save to folder if saveToFolder is true
                            SelectedFolder?.AddItemCommand.Execute(new ClipboardItemViewModel(SelectedFolder, clipboardItem));
                            // Process after pasting
                            MainUITask.Run(() => {
                                windowViewModel.SelectedFolder?.LoadFolderCommand.Execute();
                            });
                        });
                    });
            }
        }
        // Command to open a folder
        public static void OpenFolder(Model.ClipboardItem contentItem) {
            // Open the folder only if the ContentType is File
            if (contentItem.ContentType != ContentTypes.ContentItemTypes.Files) {
                LogWrapper.Error(CommonStringResources.Instance.CannotOpenFolderForNonFileContent);
                return;
            }
            // Open the folder with Process.Start
            string? folderPath = contentItem.FolderName;
            if (folderPath != null) {
                var p = new Process {
                    StartInfo = new ProcessStartInfo(folderPath) {
                        UseShellExecute = true
                    }
                };
                p.Start();
            }
        }

        // Command to extract text
        public static void ExtractText(Model.ClipboardItem contentItem) {
            // プログレスインジケータを表示
            MainWindowViewModel.Instance.UpdateIndeterminate(true);
            try {
                if (contentItem.ContentType == ContentTypes.ContentItemTypes.Text) {
                    LogWrapper.Error(CommonStringResources.Instance.CannotExtractTextForNonFileContent);
                    return;
                }
                contentItem.ExtractTextCommandExecute();
                // 保存を行う
                contentItem.Save(false);
            } finally {
                MainWindowViewModel.Instance.UpdateIndeterminate(false);
            }
        }

        // Command to open a file
        public static void OpenFile(Model.ClipboardItem contentItem) {
            // Open the selected item
            ClipboardProcessController.OpenClipboardItemFile(contentItem, false);
        }

        // Command to open a file as a new file
        public static void OpenFileAsNewFile(Model.ClipboardItem contentItem) {
            // Open the selected item
            ClipboardProcessController.OpenClipboardItemFile(contentItem, true);
        }

        // Command to generate titles
        public static async void GenerateTitleCommand(List<Model.ClipboardItem> contentItem, object afterExecuteAction) {
            LogWrapper.Info(CommonStringResources.Instance.GenerateTitleInformation);
            await Task.Run(() => {
                foreach (var item in contentItem) {
                    item.CreateAutoTitleWithOpenAI();
                    // Save
                    item.Save(false);
                }
                // Execute if obj is an Action
                if (afterExecuteAction is Action action) {
                    action();
                }
            });
            LogWrapper.Info(CommonStringResources.Instance.GeneratedTitleInformation);
        }

        // Command to execute a prompt template
        public static async void ExecutePromptTemplateCommand(List<Model.ClipboardItem> contentItem, object afterExecuteAction, string promptName) {
            LogWrapper.Info(PythonAILib.Resource.PythonAILibStringResources.Instance.PromptTemplateExecute(promptName));
            await Task.Run(() => {
                foreach (var item in contentItem) {
                    item.CreateChatResult(promptName);
                    // Save
                    item.Save(false);
                }
                // Execute if obj is an Action
                if (afterExecuteAction is Action action) {
                    action();
                }
            });
            LogWrapper.Info(PythonAILib.Resource.PythonAILibStringResources.Instance.PromptTemplateExecuted(promptName));
        }

        // Command to generate background information
        public static void GenerateBackgroundInfoCommand(List<Model.ClipboardItem> contentItem, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString();
            ExecutePromptTemplateCommand(contentItem, afterExecuteAction, promptName);
        }

        // Command to generate a summary
        public static void GenerateSummaryCommand(List<Model.ClipboardItem> contentItem, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.SummaryGeneration.ToString();
            ExecutePromptTemplateCommand(contentItem, afterExecuteAction, promptName);
        }

        // Command to generate a task list
        public static void GenerateTasksCommand(List<Model.ClipboardItem> contentItem, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.TasksGeneration.ToString();
            ExecutePromptTemplateCommand(contentItem, afterExecuteAction, promptName);
        }
        // Command to check the reliability of the document
        public static void CheckDocumentReliabilityCommand(List<Model.ClipboardItem> contentItem, object afterExecuteAction) {
            foreach (var item in contentItem) {
                item.CheckDocumentReliability();
                // Save
                item.Save(false);
            }
            // Execute if obj is an Action
            if (afterExecuteAction is Action action) {
                action();
            }
        }

        // Command to generate vectors
        public static async void GenerateVectorCommand(List<Model.ClipboardItem> contentItem, object afterExecuteAction) {
            LogWrapper.Info(CommonStringResources.Instance.GenerateVector2);
            await Task.Run(() => {
                foreach (var item in contentItem) {
                    item.UpdateEmbedding();
                    // Save
                    item.Save(false);
                }
                // Execute if obj is an Action
                if (afterExecuteAction is Action action) {
                    action();
                }
            });
            LogWrapper.Info(CommonStringResources.Instance.GeneratedVector);
        }

        // Command to perform vector search
        public static void OpenVectorSearchWindowCommand(ClipboardFolder folder) {
            // Open vector search result window
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new();
            // Action when a vector DB item is selected
            vectorSearchWindowViewModel.SelectVectorDBItemAction = (vectorDBItems) => {
                ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, MainWindowViewModel.Instance.RootFolderViewModelContainer.RootFolderViewModel, (vectorDBItemBase) => {
                    vectorDBItems.Add(vectorDBItemBase);
                });
            };
            vectorSearchWindowViewModel.VectorDBItem = folder.GetVectorDBItem();
            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }

        // Command to perform vector search
        public static void OpenVectorSearchWindowCommand(Model.ClipboardItem contentItem) {
            // Open vector search result window
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new();
            // Action when a vector DB item is selected
            vectorSearchWindowViewModel.SelectVectorDBItemAction = (vectorDBItems) => {
                ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, MainWindowViewModel.Instance.RootFolderViewModelContainer.RootFolderViewModel, (vectorDBItemBase) => {
                    vectorDBItems.Add(vectorDBItemBase);
                });
            };
            vectorSearchWindowViewModel.VectorDBItem = contentItem.GetMainVectorDBItem();
            vectorSearchWindowViewModel.InputText = contentItem.Content;
            // Execute vector search
            vectorSearchWindowViewModel.SendCommand.Execute(null);
            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }

        // Command to open text content as a file
        public static void OpenContentAsFile(Model.ClipboardItem contentItem) {
            try {
                // Open the selected item
                ClipboardProcessController.OpenClipboardItemContent(contentItem);
            } catch (Exception e) {
                LogWrapper.Error(e.Message);
            }
        }

        public static QAChatStartupProps CreateQAChatStartupProps(ClipboardItem clipboardItem) {

            SearchRule rule = FolderManager.GlobalSearchCondition.Copy();

            MainWindowViewModel ActiveInstance = MainWindowViewModel.Instance;
            QAChatStartupProps props = new(clipboardItem) {

                // フォルダ選択アクション
                SelectVectorDBItemAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select,
                        ActiveInstance.RootFolderViewModelContainer.RootFolderViewModel, (vectorDBItemBase) => {
                            vectorDBItems.Add(vectorDBItemBase);
                        });

                },
                // フォルダ編集アクション
                EditVectorDBItemAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Edit,
                        ActiveInstance.RootFolderViewModelContainer.RootFolderViewModel, (vectorDBItemBase) => {
                            vectorDBItems.Add(vectorDBItemBase);
                        });

                },
                // Saveアクション
                SaveCommand = (item, saveChatHistory) => {
                    bool flag = clipboardItem.GetFolder<ClipboardFolder>().FolderType != FolderTypeEnum.Chat;
                    clipboardItem.Save();
   
                    if (saveChatHistory && flag) {
                        // チャット履歴用のItemの設定
                        ClipboardFolder chatFolder = ActiveInstance.RootFolderViewModelContainer.ChatRootFolderViewModel.ClipboardItemFolder;
                        ClipboardItem chatHistoryItem = new(chatFolder.Id);
                        clipboardItem.CopyTo(chatHistoryItem);
                        if (!string.IsNullOrEmpty(clipboardItem.Description)) {
                            chatHistoryItem.Description += " " + clipboardItem.Description;
                        }
                        // タイトルを日付 + 元のタイトルにする
                        chatHistoryItem.Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Chat";
                        chatHistoryItem.Save();
                    }
                },
                // ExportChatアクション
                ExportChatCommand = (chatHistory) => {
                    ClipboardFolderViewModel? folderViewModel = ActiveInstance.SelectedFolder ?? ActiveInstance.RootFolderViewModelContainer.RootFolderViewModel;

                    FolderSelectWindow.OpenFolderSelectWindow(folderViewModel, (folder) => {
                        Model.ClipboardItem chatHistoryItem = new(folder.Folder.Id);
                        // タイトルを日付 + 元のタイトルにする
                        chatHistoryItem.Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Chat";
                        if (!string.IsNullOrEmpty(clipboardItem.Description)) {
                            chatHistoryItem.Description += " " + clipboardItem.Description;
                        }
                        // chatHistoryItemの内容をテキスト化
                        string chatHistoryText = "";
                        foreach (var item in chatHistory) {
                            chatHistoryText += $"--- {item.Role} ---\n";
                            chatHistoryText += item.ContentWithSources + "\n\n";
                        }
                        chatHistoryItem.Content = chatHistoryText;
                        chatHistoryItem.Save();

                    });

                }
            };

            return props;
        }


    }
}
