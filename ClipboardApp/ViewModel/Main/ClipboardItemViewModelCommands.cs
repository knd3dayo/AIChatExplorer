using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ClipboardApp.Common;
using ClipboardApp.Model.Folder;
using ClipboardApp.Model.Item;
using ClipboardApp.View.Settings;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Folders.Clipboard;
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
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Main {
    public class ClipboardItemViewModelCommands : ContentItemViewModelCommands {

        #region プログレスインジケーター不要の処理

        // ベクトル検索を実行するコマンド
        public SimpleDelegateCommand<ContentItemViewModel> VectorSearchCommand => new((itemViewModel) => {
            OpenVectorSearchWindowCommand(itemViewModel.ContentItem);
        });

        // ピン留めの切り替えコマンド (複数選択可能)
        public SimpleDelegateCommand<ClipboardItemViewModel> ChangePinCommand => new((itemViewModel) => {
            foreach (var item in MainWindowViewModel.Instance.SelectedItems) {
                item.IsPinned = !item.IsPinned;
                // ピン留めの時は更新日時を変更しない
                SaveClipboardItemCommand.Execute(item);
            }
        });


        // 選択中のContentItemBaseを開く
        public override void OpenItem(ContentItem contentItem) {
            throw new System.NotImplementedException();
        }

        // 選択中のContentItemBaseを削除
        public override void RemoveItem(ContentItem contentItem) {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Application exit command
        /// </summary>
        public override void ExitCommand() {
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
                ClipboardController.Instance.Start(async (clipboardItem) => {
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
        public override void OpenImageChatWindowCommand(ContentItem item, Action action) {
            ImageChatMainWindow.OpenMainWindow(item, action);
        }

        // Process when "RAG Management" is clicked in the menu
        public override void OpenRAGManagementWindowCommand() {
            // Open RARManagementWindow
            ListRAGSourceWindow.OpenRagManagementWindow();
        }

        // Process when "Vector DB Management" is clicked in the menu
        public override void OpenVectorDBManagementWindowCommand() {
            // Open VectorDBManagementWindow
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Edit, RootFolderViewModelContainer.FolderViewModels, (vectorDBItem) => { });
        }

        // Process when "Settings" is clicked in the menu
        public override void SettingCommandExecute() {
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
        public void CutFolderCommandExecute(MainWindowViewModel windowViewModel) {
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
        public void CutItemCommandExecute(MainWindowViewModel windowViewModel) {
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
        public void CopyToClipboardCommandExecute(MainWindowViewModel windowViewModel) {
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
        public override void OpenVectorSearchWindowCommand(ContentFolder folder) {
            // Open vector search result window
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new();
            // Action when a vector DB item is selected
            vectorSearchWindowViewModel.SelectVectorDBItemAction = (vectorDBItems) => {
                ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                    vectorDBItems.Add(vectorDBItemBase);
                });
            };
            vectorSearchWindowViewModel.VectorDBItem = folder.MainVectorDBItem;
            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }

        // Command to perform vector search
        public override void OpenVectorSearchWindowCommand(ContentItem contentItem) {
            // Open vector search result window
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new();
            // Action when a vector DB item is selected
            vectorSearchWindowViewModel.SelectVectorDBItemAction = (vectorDBItems) => {
                ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
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
        public override void OpenContentAsFile(ContentItem contentItem) {
            try {
                // Open the selected item
                ClipboardProcessController.OpenClipboardItemContent(contentItem);
            } catch (Exception e) {
                LogWrapper.Error(e.Message);
            }
        }

        // Process when Ctrl + Shift + M is pressed
        public void MergeItemWithHeaderCommandExecute(MainWindowViewModel windowViewModel) {
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
        public void MergeItemCommandExecute(MainWindowViewModel windowViewModel) {
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

        // Process when Ctrl + V is pressed
        public void PasteFromClipboardCommandExecute() {
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
                if (SelectedFolder.Folder is not ClipboardFolder clipboardFolder) {
                    return;
                }
                clipboardFolder.ProcessClipboardItem(ClipboardController.LastClipboardChangedEventArgs,
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


        #endregion

        // -----------------------------------------------------------------------------------
        #region プログレスインジケーター表示の処理

        // コンテキストメニューの「テキストを抽出」の実行用コマンド (複数選択可能)
        // 処理中はプログレスインジケータを表示
        public SimpleDelegateCommand<ClipboardItemViewModel> ExtractTextCommand => new((itemViewModel) => {
            Task.Run(() => {
                // プログレスインジケータを表示
                MainWindowViewModel.Instance.UpdateIndeterminate(true);
                try {
                    foreach (var itemViewModel in MainWindowViewModel.Instance.SelectedItems) {
                        ContentItem item = itemViewModel.ContentItem;
                        if (item.ContentType == ContentTypes.ContentItemTypes.Text) {
                            LogWrapper.Error(CommonStringResources.Instance.CannotExtractTextForNonFileContent);
                            return;
                        }
                        ContentItemCommands.ExtractTextCommandExecute(item);
                        // 保存を行う
                        item.Save(false);
                    }
                } finally {
                    MainWindowViewModel.Instance.UpdateIndeterminate(false);
                }
            });
        });

        // Command to reload the folder
        public void ReloadFolderCommand(MainWindowViewModel model) {
            if (model.SelectedFolder == null) {
                return;
            }
            Task.Run(() => {
                // Display ProgressIndicator until processing is complete
                try {
                    model.UpdateIndeterminate(true);
                    model.SelectedFolder.LoadFolderCommand.Execute();
                    LogWrapper.Info(CommonStringResources.Instance.Reloaded);
                } finally {
                    model.UpdateIndeterminate(false);
                }
            });
        }
        // Command to generate titles
        public override async void GenerateTitleCommand(List<ContentItem> contentItem, object afterExecuteAction) {
            await Task.Run(() => {
                LogWrapper.Info(CommonStringResources.Instance.GenerateTitleInformation);
                try {
                    // プログレスインジケータを表示
                    MainWindowViewModel.Instance.UpdateIndeterminate(true);
                    foreach (var item in contentItem) {
                        ContentItemCommands.CreateAutoTitleWithOpenAI(item);
                        // Save
                        item.Save(false);
                    }
                    // Execute if obj is an Action
                    if (afterExecuteAction is Action action) {
                        action();
                    }
                    LogWrapper.Info(CommonStringResources.Instance.GeneratedTitleInformation);
                } finally {
                    // プログレスインジケータを非表示
                    MainWindowViewModel.Instance.UpdateIndeterminate(false);
                }
            });
        }

        // Command to execute a prompt template (複数選択可能)
        public override async void ExecutePromptTemplateCommand(List<ContentItem> contentItem, object afterExecuteAction, string promptName) {
            await Task.Run(() => {
                try {
                    LogWrapper.Info(PythonAILib.Resource.PythonAILibStringResources.Instance.PromptTemplateExecute(promptName));
                    // プログレスインジケータを表示
                    MainWindowViewModel.Instance.UpdateIndeterminate(true);
                    foreach (var item in contentItem) {
                        ContentItemCommands.CreateChatResult(item, promptName);
                        // Save
                        item.Save(false);
                    }
                    // Execute if obj is an Action
                    if (afterExecuteAction is Action action) {
                        action();
                    }
                    LogWrapper.Info(PythonAILib.Resource.PythonAILibStringResources.Instance.PromptTemplateExecuted(promptName));
                } finally {
                    // プログレスインジケータを非表示
                    MainWindowViewModel.Instance.UpdateIndeterminate(false);
                }
            });
        }

        // Command to generate background information
        public override void GenerateBackgroundInfoCommand(List<ContentItem> contentItem, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString();
            ExecutePromptTemplateCommand(contentItem, afterExecuteAction, promptName);
        }

        // Command to generate a summary
        public override void GenerateSummaryCommand(List<ContentItem> contentItem, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.SummaryGeneration.ToString();
            ExecutePromptTemplateCommand(contentItem, afterExecuteAction, promptName);
        }

        // Command to generate a task list
        public override void GenerateTasksCommand(List<ContentItem> contentItem, object afterExecuteAction) {
            string promptName = SystemDefinedPromptNames.TasksGeneration.ToString();
            ExecutePromptTemplateCommand(contentItem, afterExecuteAction, promptName);
        }
        // Command to check the reliability of the document
        public override void CheckDocumentReliabilityCommand(List<ContentItem> contentItem, object afterExecuteAction) {
            Task.Run(() => {
                try {
                    // プログレスインジケータを表示
                    MainWindowViewModel.Instance.UpdateIndeterminate(true);
                    foreach (var item in contentItem) {
                        ContentItemCommands.CheckDocumentReliability(item);
                        // Save
                        item.Save(false);
                    }
                    // Execute if obj is an Action
                    if (afterExecuteAction is Action action) {
                        action();
                    }
                } finally {
                    // プログレスインジケータを表示
                    MainWindowViewModel.Instance.UpdateIndeterminate(false);
                }
            });
        }

        // Command to generate vectors
        public override async void GenerateVectorCommand(List<ContentItem> contentItem, object afterExecuteAction) {
            await Task.Run(() => {
                try {
                    LogWrapper.Info(CommonStringResources.Instance.GenerateVector2);
                    // プログレスインジケータを表示
                    MainWindowViewModel.Instance.UpdateIndeterminate(true);

                    foreach (var item in contentItem) {
                        ContentItemCommands.UpdateEmbedding(item);
                        // Save
                        item.Save(false);
                    }
                    // Execute if obj is an Action
                    if (afterExecuteAction is Action action) {
                        action();
                    }
                    LogWrapper.Info(CommonStringResources.Instance.GeneratedVector);
                } finally {
                    // プログレスインジケータを非表示
                    MainWindowViewModel.Instance.UpdateIndeterminate(false);
                }
            });
        }

        #endregion
        // -----------------------------------------------------------------------------------

        public static QAChatStartupProps CreateQAChatStartupProps(ContentItem clipboardItem) {

            SearchRule rule = FolderManager.GlobalSearchCondition.Copy();

            MainWindowViewModel ActiveInstance = MainWindowViewModel.Instance;
            QAChatStartupProps props = new(clipboardItem) {

                // フォルダ選択アクション
                SelectVectorDBItemAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select,
                        PythonAILibUI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                            vectorDBItems.Add(vectorDBItemBase);
                        });

                },
                // フォルダ編集アクション
                EditVectorDBItemAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Edit,
                        PythonAILibUI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                            vectorDBItems.Add(vectorDBItemBase);
                        });

                },
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

                    FolderSelectWindow.OpenFolderSelectWindow(PythonAILibUI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels, (folder) => {
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

                    });

                }
            };

            return props;
        }


    }
}
