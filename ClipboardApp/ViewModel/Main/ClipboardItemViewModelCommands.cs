using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ClipboardApp.Common;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Item;
using ClipboardApp.View.Settings;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ImageChat.View;
using MergeChat.View;
using PythonAILib.Model.Content;
using PythonAILib.Model.File;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Prompt;
using PythonAILib.Model.Search;
using PythonAILibUI.ViewModel.Folder;
using PythonAILibUI.ViewModel.Item;
using QAChat.Resource;
using QAChat.View.Folder;
using QAChat.View.QAChatMain;
using QAChat.View.RAG;
using QAChat.View.Search;
using QAChat.View.VectorDB;
using QAChat.ViewModel;
using QAChat.ViewModel.Item;
using QAChat.ViewModel.VectorDB;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using ClipboardApp.View.Item;
using ClipboardApp.ViewModel.Common;
using QAChat.ViewModel.Folder;

namespace ClipboardApp.ViewModel.Main {
    public class ClipboardItemViewModelCommands : ContentItemViewModelCommands {

        public override SimpleDelegateCommand<ContentItemViewModel> OpenItemCommand => new((itemViewModel) => {
            
            ContentFolderViewModel folderViewModel = itemViewModel.FolderViewModel;

            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(folderViewModel, itemViewModel,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    folderViewModel.LoadFolderCommand.Execute();
                    LogWrapper.Info(CommonStringResources.Instance.Edited);
                });

            ClipboardAppTabContainer container = new(itemViewModel.ContentItem.Description, editItemControl);

            // UserControlをクローズする場合の処理を設定
            editItemControl.SetCloseUserControl(() => {
                MainWindowViewModel.Instance.RemoveTabItem(container);
            });

            MainWindowViewModel.Instance.AddTabItem(container);
        });


        // ピン留めの切り替えコマンド (複数選択可能)
        public SimpleDelegateCommand<ClipboardItemViewModel> ChangePinCommand => new((itemViewModel) => {
            foreach (var item in MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems) {
                item.IsPinned = !item.IsPinned;
                // ピン留めの時は更新日時を変更しない
                SaveClipboardItemCommand.Execute(item);
            }
        });

        // Command to start/stop clipboard monitoring
        public static void StartStopClipboardMonitorCommand() {
            MainWindowViewModel model = MainWindowViewModel.Instance;
            model.IsClipboardMonitoringActive = !model.IsClipboardMonitoringActive;
            if (model.IsClipboardMonitoringActive) {
                ClipboardController.Instance.Start(async (clipboardItem) => {
                    // Process when a clipboard item is added
                    // クリップボードフォルダのルートフォルダに追加
                    await Task.Run(() => {
                        model.RootFolderViewModelContainer.RootFolderViewModel?.AddItemCommand.Execute(new ClipboardItemViewModel(model.RootFolderViewModelContainer.RootFolderViewModel, clipboardItem));
                    });
                    // クリップボードフォルダのルートフォルダを更新
                    MainUITask.Run(() => {
                        model.RootFolderViewModelContainer.RootFolderViewModel?.LoadFolderCommand.Execute();
                    });
                });
                LogWrapper.Info(CommonStringResources.Instance.StartClipboardWatchMessage);
            } else {
                ClipboardController.Instance.Stop();
                LogWrapper.Info(CommonStringResources.Instance.StopClipboardWatchMessage);
            }
            // Notification
            model.NotifyPropertyChanged(nameof(model.IsClipboardMonitoringActive));
            // Change button text
            model.NotifyPropertyChanged(nameof(model.ClipboardMonitorButtonText));
        }

        // Command to open OpenAI Chat
        public override void OpenOpenAIChatWindowCommand(ContentItem? item) {
            if (item == null) {
                // チャット履歴用のItemの設定
                ClipboardFolder chatFolder = (ClipboardFolder)MainWindowViewModel.Instance.RootFolderViewModelContainer.ChatRootFolderViewModel.Folder;
                item = new(chatFolder.Id);
                // タイトルを日付 + 元のタイトルにする
                item.Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Chat";

            }
            QAChatStartupProps qAChatStartupProps = CreateQAChatStartupProps(item);
            QAChatMainWindow.OpenOpenAIChatWindow(qAChatStartupProps);
        }

        // Command to open Image Chat
        public void OpenImageChatWindowCommand(ContentItem item, Action action) {
            ImageChatMainWindow.OpenMainWindow(item, action);
        }

        // Command to Open Merge Chat
        public void OpenMergeChatWindowCommand(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems) {
            MergeChatMainWindow.OpenWindow(folderViewModel, selectedItems);
        }

        // Process when "RAG Management" is clicked in the menu
        public  void OpenRAGManagementWindowCommand() {
            // Open RARManagementWindow
            ListRAGSourceWindow.OpenRagManagementWindow();
        }

        // Process when "Vector DB Management" is clicked in the menu
        public  void OpenVectorDBManagementWindowCommand() {
            // Open VectorDBManagementWindow
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Edit, RootFolderViewModelContainer.FolderViewModels, (vectorDBItem) => { });
        }

        // Process when "Settings" is clicked in the menu
        public  void SettingCommandExecute() {
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
        public void OpenSearchWindowCommand(ClipboardFolder folder, Action action) {
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


        // Process when Ctrl + X is pressed on a folder
        public void CutFolderCommandExecute(MainPanelTreeViewControlViewModel model) {
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
        public void CutItemCommandExecute(MainPanelDataGridViewControlViewModel model) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = model.SelectedItems;
            // Do not process if no items are selected
            if (SelectedItems == null || SelectedItems.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            // Set Cut Flag
            ClipboardController.Instance.CutFlag = ClipboardController.CutFlagEnum.Item;
            // Set the selected items to CopiedItems
            ClipboardController.Instance.CopiedObjects.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                ClipboardController.Instance.CopiedObjects.Add(item);
            }
            LogWrapper.Info(CommonStringResources.Instance.Cut);
        }

        // Process when Ctrl + C is pressed
        public void CopyToClipboardCommandExecute(MainPanelDataGridViewControlViewModel model) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = model.SelectedItems;
            ClipboardItemViewModel? SelectedItem = model.SelectedItem;
            ClipboardFolderViewModel? SelectedFolder = model.SelectedFolder;
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
            ClipboardController.Instance.CutFlag = ClipboardController.CutFlagEnum.None;
            // Set the selected items to CopiedItems
            ClipboardController.Instance.CopiedObjects.Clear();
            foreach (ClipboardItemViewModel item in SelectedItems) {
                ClipboardController.Instance.CopiedObjects.Add(item);
            }
            try {
                ClipboardController.Instance.SetDataObject(SelectedItem.ContentItem);
                LogWrapper.Info(CommonStringResources.Instance.Copied);
            } catch (Exception e) {
                string message = $"{CommonStringResources.Instance.ErrorOccurredAndMessage}:\n{e.Message}\n{CommonStringResources.Instance.StackTrace}:\n{e.StackTrace}";
                LogWrapper.Error(message);
            }
        }

        // Command to open a folder
        public override void OpenFolder(ContentItem contentItem) {
            // Open the folder only if the ContentType is File
            if (contentItem.ContentType != ContentTypes.ContentItemTypes.Files) {
                LogWrapper.Error(CommonStringResources.Instance.CannotOpenFolderForNonFileContent);
                return;
            }
            string message = $"{CommonStringResources.Instance.ExecuteOpenFolder} {contentItem.FolderName}";
            LogWrapper.Info(message);

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
            message = $"{CommonStringResources.Instance.ExecuteOpenFolderSuccess} {contentItem.FolderName}";
            LogWrapper.Info(message);

        }

        // Command to open a file
        public override void OpenFile(ContentItem contentItem) {
            // Open the selected item
            ClipboardProcessController.OpenClipboardItemFile(contentItem, false);
        }

        // Command to open a file as a new file
        public override void OpenFileAsNewFile(ContentItem contentItem) {
            // Open the selected item
            ClipboardProcessController.OpenClipboardItemFile(contentItem, true);
        }

        // Command to perform vector search
        public void OpenVectorSearchWindowCommand(ContentFolder folder) {
            // Open vector search result window
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new();
            // Action when a vector DB item is selected
            vectorSearchWindowViewModel.SelectVectorDBItemAction = (vectorDBItems) => {
                ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                    vectorDBItems.Add(vectorDBItemBase);
                });
            };
            vectorSearchWindowViewModel.VectorSearchProperty = folder.GetMainVectorSearchProperty();
            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }

        // Command to perform vector search
        public  void OpenVectorSearchWindowCommand(ContentItem contentItem) {
            // Open vector search result window
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new();
            // Action when a vector DB item is selected
            vectorSearchWindowViewModel.SelectVectorDBItemAction = (vectorDBItems) => {
                ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                    vectorDBItems.Add(vectorDBItemBase);
                });
            };
            vectorSearchWindowViewModel.VectorSearchProperty = contentItem.GetMainVectorSearchProperty();
            vectorSearchWindowViewModel.InputText = contentItem.Content;
            // Execute vector search
            vectorSearchWindowViewModel.SendCommand.Execute(null);
            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }

        // Command to open text content as a file
        public override void OpenContentAsFile(ContentItem contentItem) {
            try {
                // Open the selected item
                ClipboardProcessController.OpenClipboardItemContent(contentItem);
            } catch (Exception e) {
                LogWrapper.Error(e.Message);
            }
        }

        // Process when Ctrl + Shift + M is pressed
        public void MergeItemWithHeaderCommandExecute(MainPanelDataGridViewControlViewModel model) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = model.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = model.SelectedFolder;
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
        public void MergeItemCommandExecute(MainPanelDataGridViewControlViewModel model) {
            ObservableCollection<ClipboardItemViewModel> SelectedItems = model.SelectedItems;
            ClipboardFolderViewModel? SelectedFolder = model.SelectedFolder;
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


        // -----------------------------------------------------------------------------------
        #region プログレスインジケーター表示の処理


        // Process when Ctrl + V is pressed
        public void PasteFromClipboardCommandExecute() {
            MainWindowViewModel windowViewModel = MainWindowViewModel.Instance;
            ClipboardFolderViewModel? SelectedFolder = windowViewModel.MainPanelTreeViewControlViewModel.SelectedFolder;
            List<object> CopiedItems = ClipboardController.Instance.CopiedObjects;
            // Do not process if no folder is selected
            if (SelectedFolder == null || SelectedFolder.Folder is not ClipboardFolder clipboardFolder) {
                LogWrapper.Error(CommonStringResources.Instance.NoPasteFolder);
                return;
            }

            // If the source items are from within the app
            if (CopiedItems.Count > 0) {
                SelectedFolder.PasteClipboardItemCommandExecute(
                    ClipboardController.Instance.CutFlag,
                    CopiedItems,
                    SelectedFolder
                );
                // Reset Cut flag
                ClipboardController.Instance.CutFlag = ClipboardController.CutFlagEnum.None;
                // Clear selected items after pasting
                CopiedItems.Clear();
            } else if (ClipboardController.LastClipboardChangedEventArgs != null) {

                MainWindowViewModel.Instance.UpdateIndeterminate(true);
                clipboardFolder.ProcessClipboardItem(ClipboardController.LastClipboardChangedEventArgs,
                    async (clipboardItem) => {
                        // Process when a clipboard item is added
                        await Task.Run(() => {
                            // Save to folder if saveToFolder is true
                            SelectedFolder?.AddItemCommand.Execute(new ClipboardItemViewModel(SelectedFolder, clipboardItem));
                            // Process after pasting
                        }).ContinueWith((obj) => {
                            MainUITask.Run(() => {
                                windowViewModel.MainPanelTreeViewControlViewModel.SelectedFolder?.LoadFolderCommand.Execute();
                            });
                            MainWindowViewModel.Instance.UpdateIndeterminate(false);
                        });
                    });
            }
        }


        // Command to reload the folder
        public void ReloadFolderCommand(MainPanelTreeViewControlViewModel model) {
            if (model.SelectedFolder == null) {
                return;
            }
            Task.Run(() => {
                // Display ProgressIndicator until processing is complete
                try {
                    model.UpdateIndeterminateAction(true);
                    model.SelectedFolder.LoadFolderCommand.Execute();
                    LogWrapper.Info(CommonStringResources.Instance.Reloaded);
                } finally {
                    model.UpdateIndeterminateAction(false);
                }
            });
        }



        // Command to generate vectors
        public  void GenerateVectorCommand(List<ContentItem> items, object afterExecuteAction) {

            List<Task> taskList = [];
            // Display ProgressIndicator
            MainWindowViewModel.Instance.UpdateIndeterminate(true);
            int count = items.Count;
            Task.Run(() => {
                object lockObject = new();
                int start_count = 0;
                ParallelOptions parallelOptions = new();
                // 10並列
                parallelOptions.MaxDegreeOfParallelism = 10;
                Parallel.For(0, count, parallelOptions, (i) => {
                    lock (lockObject) {
                        start_count++;
                    }
                    int index = i; // Store the current index in a separate variable to avoid closure issues
                    string message = $"{CommonStringResources.Instance.GenerateVectorInProgress} ({start_count}/{count})";
                    StatusText.Instance.UpdateInProgress(true, message);
                    ContentItem item = items[index];
                    ContentItemCommands.UpdateEmbedding(item);
                    // Save
                    item.Save(false);
                });
                // Execute if obj is an Action
                if (afterExecuteAction is Action action) {
                    action();
                }

                LogWrapper.Info(CommonStringResources.Instance.GenerateVectorCompleted);
                // Hide ProgressIndicator
                MainWindowViewModel.Instance.UpdateIndeterminate(false);
                StatusText.Instance.UpdateInProgress(false);
            });
        }

        // コンテキストメニューの「テキストを抽出」の実行用コマンド (複数選択可能)
        // 処理中はプログレスインジケータを表示
        public SimpleDelegateCommand<ClipboardItemViewModel> ExtractTextCommand => new((itemViewModel) => {

            List<Task> taskList = [];
            // プログレスインジケータを表示
            MainWindowViewModel.Instance.UpdateIndeterminate(true);
            int count = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems.Count;
            if (count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            Task.Run(() => {
                object lockObject = new();
                int start_count = 0;
                ParallelOptions parallelOptions = new();
                // 10並列
                parallelOptions.MaxDegreeOfParallelism = 10;
                Parallel.For(0, count, parallelOptions, (i) => {
                    int index = i; // Store the current index in a separate variable to avoid closure issues
                    lock (lockObject) {
                        start_count++;
                    }
                    string message = $"{CommonStringResources.Instance.TextExtractionInProgress} ({start_count}/{count})";
                    StatusText.Instance.UpdateInProgress(true, message);
                    var itemViewModel = MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems[index];

                    ContentItem item = itemViewModel.ContentItem;
                    if (item.ContentType == ContentTypes.ContentItemTypes.Text) {
                        LogWrapper.Error(CommonStringResources.Instance.CannotExtractTextForNonFileContent);
                        return;
                    }
                    ContentItemCommands.ExtractTextCommandExecute(item);
                    // Save the item
                    item.Save(false);
                });
                LogWrapper.Info(CommonStringResources.Instance.TextExtractionCompleted);
                MainWindowViewModel.Instance.UpdateIndeterminate(false);
                StatusText.Instance.UpdateInProgress(false);
            });
        });


        // Command to execute a prompt template (複数選択可能)
        public  void ExecutePromptTemplateCommand(List<ContentItem> contentItems, object afterExecuteAction, PromptItem promptItem) {
            List<Task> taskList = [];

            // promptNameからDescriptionを取得
            string description = promptItem.Description;

            LogWrapper.Info(CommonStringResources.Instance.PromptTemplateExecute(description));
            // プログレスインジケータを表示
            MainWindowViewModel.Instance.UpdateIndeterminate(true);
            int count = contentItems.Count;
            Task.Run(() => {
                object lockObject = new();
                int start_count = 0;
                ParallelOptions parallelOptions = new();
                // 10並列
                parallelOptions.MaxDegreeOfParallelism = 10;
                Parallel.For(0, count, parallelOptions, (i) => {
                    lock (lockObject) {
                        start_count++;
                    }
                    int index = i; // Store the current index in a separate variable to avoid closure issues
                    string message = $"{CommonStringResources.Instance.PromptTemplateInProgress(description)} ({start_count}/{count})";
                    StatusText.Instance.UpdateInProgress(true, message);
                    ContentItem item = contentItems[index];

                    ContentItemCommands.CreateChatResult(item, promptItem.Name);
                    // Save
                    item.Save(false);
                });
                // Execute if obj is an Action
                if (afterExecuteAction is Action action) {
                    action();
                }
                LogWrapper.Info(CommonStringResources.Instance.PromptTemplateExecuted(description));
                // プログレスインジケータを非表示
                MainWindowViewModel.Instance.UpdateIndeterminate(false);
                StatusText.Instance.UpdateInProgress(false);
            });

        }

        // Command to generate titles
        public void GenerateTitleCommand(List<ContentItem> items, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.TitleGeneration.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand(items, afterExecuteAction, promptItem);
        }

        // Command to generate background information
        public void GenerateBackgroundInfoCommand(List<ContentItem> items, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand(items, afterExecuteAction, promptItem);
        }


        // Command to generate a summary
        public void GenerateSummaryCommand(List<ContentItem> items, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.SummaryGeneration.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand(items, afterExecuteAction, promptItem);
        }

        // Command to generate a task list
        public  void GenerateTasksCommand(List<ContentItem> items, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.TasksGeneration.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand(items, afterExecuteAction, promptItem);
        }
        // Command to check the reliability of the document
        public void CheckDocumentReliabilityCommand(List<ContentItem> items, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.DocumentReliabilityCheck.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand(items, afterExecuteAction, promptItem);
        }

        #endregion
        // -----------------------------------------------------------------------------------

        public static QAChatStartupProps CreateQAChatStartupProps(ContentItem clipboardItem) {

            SearchRule rule = FolderManager.GlobalSearchCondition.Copy();

            MainWindowViewModel ActiveInstance = MainWindowViewModel.Instance;
            QAChatStartupProps props = new(clipboardItem) {
                // Saveアクション
                SaveCommand = (item, saveChatHistory) => {
                    bool flag = clipboardItem.GetFolder<ClipboardFolder>().FolderType != FolderTypeEnum.Chat;
                    clipboardItem.Save();

                    if (saveChatHistory && flag) {
                        // チャット履歴用のItemの設定
                        ClipboardFolder chatFolder = (ClipboardFolder)ActiveInstance.RootFolderViewModelContainer.ChatRootFolderViewModel.Folder;
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

                    FolderSelectWindow.OpenFolderSelectWindow(PythonAILibUI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels, (folder, finished) => {
                        if (finished) {
                            ClipboardItem chatHistoryItem = new(folder.Folder.Id);
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
                        }
                    });

                }
            };

            return props;
        }
        // QAChatButtonCommand
        public SimpleDelegateCommand<ContentItemViewModel> QAChatButtonCommand => new((itemViewModel) => {
            // QAChatControlのDrawerを開く
            OpenOpenAIChatWindowCommand(itemViewModel.ContentItem);
        });

        // ベクトル検索を実行するコマンド
        public SimpleDelegateCommand<ContentItemViewModel> VectorSearchCommand => new((itemViewModel) => {
            OpenVectorSearchWindowCommand(itemViewModel.ContentItem);
        });
    }
}
