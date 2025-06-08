using System.Collections.ObjectModel;
using System.Windows;
using AIChatExplorer.Model.Folders.Clipboard;
using AIChatExplorer.Model.Item;
using AIChatExplorer.Model.Main;
using AIChatExplorer.View.Settings;
using AIChatExplorer.ViewModel.Content;
using AIChatExplorer.ViewModel.Folders.Application;
using AIChatExplorer.ViewModel.Folders.Search;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Search;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibUIAutoGenChat.View.Chat;
using LibUIImageChat.View;
using LibUIMergeChat.View;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.AutoProcessRule;
using LibUIPythonAI.View.Chat;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.View.Item;
using LibUIPythonAI.View.PromptTemplate;
using LibUIPythonAI.View.Search;
using LibUIPythonAI.View.Tag;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Chat;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.ViewModel.PromptTemplate;
using LibUIPythonAI.ViewModel.VectorDB;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace AIChatExplorer.ViewModel.Main {
    public class AppViewModelCommands(Action<bool> updateIndeterminate, Action updateView) : ContentItemViewModelCommands(updateIndeterminate, updateView) {


        // フォルダを開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenFolderCommand => new((itemViewModel) => {
            OpenFolderExecute(itemViewModel);
        });

        // テキストをファイルとして開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenContentAsFileCommand => new((itemViewModel) => {
            OpenFolderExecute(itemViewModel);
        });

        // ファイルを開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenFileCommand => new((itemViewModel) => {
            OpenFolderExecute(itemViewModel);
        });

        // ファイルを新規ファイルとして開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenFileAsNewFileCommand => new((itemViewModel) => {
            OpenFileAsNewFileExecute(itemViewModel);
        });


        // アイテム保存
        public SimpleDelegateCommand<ContentItemViewModel> SaveApplicationItemCommand => new((itemViewModel) => {
            itemViewModel.ContentItem.Save();
        });


        // OpenContentItemCommand
        public SimpleDelegateCommand<ContentItemViewModel> OpenItemCommand => new((itemViewModel) => {
            OpenItemCommandExecute(itemViewModel);
        });

        public SimpleDelegateCommand<ContentItemViewModel> OpenOpenAIChatWindowCommand => new((itemViewModel) => {

            // QAChatControlのDrawerを開く
            OpenOpenAIChatWindowCommandExecute(itemViewModel);
        });

        public SimpleDelegateCommand<ContentItemViewModel> OpenVectorSearchWindowCommand => new((itemViewModel) => {
            OpenVectorSearchWindowCommandExecute(itemViewModel);
        });
        // OpenFolderVectorSearchWindowCommand
        public SimpleDelegateCommand<ContentFolderViewModel> OpenFolderVectorSearchWindowCommand => new((folderViewModel) => {
            OpenFolderVectorSearchWindowCommandExecute(folderViewModel);
        });

        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> CutItemCommand => new((itemViewModels) => {
            CutItemCommandExecute(itemViewModels);
        });


        // ピン留めの切り替えコマンド (複数選択可能)
        public SimpleDelegateCommand<ApplicationItemViewModel> ChangePinCommand => new((itemViewModel) => {
            foreach (var item in MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems) {
                if (item is ApplicationItemViewModel applicationItemViewModel) {
                    applicationItemViewModel.IsPinned = !applicationItemViewModel.IsPinned;
                    // ピン留めの時は更新日時を変更しない
                    SaveApplicationItemCommand.Execute(applicationItemViewModel);
                }
            }
        });

        // クリップボード監視開始終了フラグを反転させる
        // メニューの「開始」、「停止」をクリックしたときの処理
        public SimpleDelegateCommand<object> ToggleClipboardMonitor => new((parameter) => {
            StartStopClipboardMonitorCommandExecute();
        });

        public static SimpleDelegateCommand<object> ExitCommand => new((parameter) => {
            // Display exit confirmation dialog. If Yes, exit the application
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmExit, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                Application.Current.Shutdown();
            }
        });


        //---------------------


        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public static void OpenListPromptTemplateWindowCommandExecute() {
            // ListPromptTemplateWindowを開く
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Edit, (promptTemplateWindowViewModel, OpenAIExecutionModeEnum) => { });
        }
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public static void OpenListAutoProcessRuleWindowCommandExecute() {
            // ListAutoProcessRuleWindowを開く
            ListAutoProcessRuleWindow.OpenListAutoProcessRuleWindow(LibUIPythonAI.ViewModel.Folder.RootFolderViewModelContainer.FolderViewModels);

        }
        // メニューの「タグ編集」をクリックしたときの処理
        public static void OpenTagWindowCommandExecute() {
            // TagWindowを開く
            TagWindow.OpenTagWindow(null, () => { });

        }

        public static void OpenItemCommandExecute(ContentItemViewModel? itemViewModel) {
            if (itemViewModel == null) {
                return;
            }
            ContentFolderViewModel folderViewModel = itemViewModel.FolderViewModel;

            EditItemControl editItemControl = EditItemControl.CreateEditItemControl(folderViewModel, itemViewModel,
                () => {
                    // フォルダ内のアイテムを再読み込み
                    folderViewModel.LoadFolderCommand.Execute();
                    LogWrapper.Info(CommonStringResources.Instance.Edited);
                });

            MainTabContent container = new(itemViewModel.ContentItem.Description, editItemControl);

            // UserControlをクローズする場合の処理を設定
            editItemControl.SetCloseUserControl(() => {
                MainWindowViewModel.Instance.MainTabManager.RemoveTabItem(container);
            });

            MainWindowViewModel.Instance.MainTabManager.AddTabItem(container);
        }

        // Command to open OpenAI Chat
        public static void OpenOpenAIChatWindowCommandExecute(ContentItemViewModel itemViewModel) {

            QAChatStartupProps qAChatStartupProps = CreateQAChatStartupProps(itemViewModel.ContentItem);
            ChatWindow.OpenOpenAIChatWindow(qAChatStartupProps);
        }

        public static void OpenOpenAIChatWindowCommandExecute() {
            // チャット履歴用のItemの設定
            ApplicationFolderViewModel chatFolderViewModel = MainWindowViewModel.Instance.RootFolderViewModelContainer.ChatRootFolderViewModel;
            // チャット履歴用のItemの設定
            ApplicationItem item = new(chatFolderViewModel.Folder.Entity) {
                // タイトルを日付 + 元のタイトルにする
                Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + CommonStringResources.Instance.ChatHeader + CommonStringResources.Instance.NoTitle
            };
            ApplicationItemViewModel applicationItemViewModel = new(chatFolderViewModel, item);

            OpenOpenAIChatWindowCommandExecute(applicationItemViewModel);

        }

        public static void OpenAutoGenChatWindowCommandExecute(ContentItemViewModel itemViewModel) {

            QAChatStartupProps qAChatStartupProps = CreateQAChatStartupProps(itemViewModel.ContentItem);
            AutoGenChatWindow.OpenWindow(qAChatStartupProps);
        }

        public static void OpenAutoGenChatWindowCommandExecute() {
            // チャット履歴用のItemの設定
            ApplicationFolderViewModel chatFolderViewModel = MainWindowViewModel.Instance.RootFolderViewModelContainer.ChatRootFolderViewModel;
            // チャット履歴用のItemの設定
            ApplicationItem item = new(chatFolderViewModel.Folder.Entity) {
                // タイトルを日付 + 元のタイトルにする
                Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + CommonStringResources.Instance.ChatHeader + CommonStringResources.Instance.NoTitle
            };
            ApplicationItemViewModel applicationItemViewModel = new(chatFolderViewModel, item);

            OpenAutoGenChatWindowCommandExecute(applicationItemViewModel);

        }

        // Command to open Image Chat
        public static void OpenImageChatWindowCommand(ContentItemWrapper item, System.Action action) {
            ImageChatWindow.OpenMainWindow(item, action);
        }

        // Command to Open Merge Chat
        public static void OpenMergeChatWindowCommand(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems) {
            MergeChatWindow.OpenWindow(folderViewModel, selectedItems);
        }


        // Process when "Vector DB Management" is clicked in the menu
        public static void OpenVectorDBManagementWindowCommand() {
            // Open VectorDBManagementWindow
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Edit, RootFolderViewModelContainer.FolderViewModels, (vectorDBItem) => { });
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
        public static void OpenSearchWindowCommandExecute(SearchFolderViewModel searchFolderViewModel, System.Action action) {
            SearchRule? searchConditionRule = new(
                new LibPythonAI.Data.SearchRuleEntity() {
                    SearchFolderId = searchFolderViewModel.Folder.Id,
                });
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
        public static void StartStopClipboardMonitorCommandExecute() {
            MainWindowViewModel model = MainWindowViewModel.Instance;
            model.IsClipboardMonitoringActive = !model.IsClipboardMonitoringActive;
            if (model.IsClipboardMonitoringActive) {
                ClipboardController.Instance.Start(async (applicationItem) => {
                    // Process when a clipboard item is added
                    // フォルダのルートフォルダに追加
                    await Task.Run(() => {
                        model.RootFolderViewModelContainer.RootFolderViewModel?.AddItemCommand.Execute(new ApplicationItemViewModel(model.RootFolderViewModelContainer.RootFolderViewModel, applicationItem));
                    });
                    // フォルダのルートフォルダを更新
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


        // -----------------------------------------------------------------------------------
        // プログレスインジケーター表示の処理

        // Process when Ctrl + V is pressed
        public void PasteFromClipboardCommandExecute() {
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
                SelectedFolder.PasteApplicationItemCommandExecute(
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
            clipboardFolder.ProcessApplicationItem(lastClipboardChangedEventArgs,
                async (applicationItem) => {
                    // Process when a clipboard item is added
                    await Task.Run(() => {
                        // SaveAsync to folder if saveToFolder is true
                        clipboardFolder.AddItem(applicationItem);
                        // Process after pasting
                    }).ContinueWith((obj) => {
                        UpdateIndeterminate(false);
                        UpdateView();
                    });
                });
        }

        // -----------------------------------------------------------------------------------

        public static QAChatStartupProps CreateQAChatStartupProps(ContentItemWrapper applicationItem) {

            MainWindowViewModel ActiveInstance = MainWindowViewModel.Instance;
            QAChatStartupProps props = new(applicationItem) {
                // Closeアクション
                SaveCommand = (item, saveChatHistory) => {
                    if (!saveChatHistory) {
                        return;
                    }
                    Task.Run(async () => {
                        ContentFolderWrapper chatFolder = (ContentFolderWrapper)ActiveInstance.RootFolderViewModelContainer.ChatRootFolderViewModel.Folder;
                        await ContentItemCommands.SaveChatHistoryAsync(item, chatFolder);

                    });
                },
                // ExportChatアクション
                ExportChatCommand = (chatHistory) => {

                    FolderSelectWindow.OpenFolderSelectWindow(RootFolderViewModelContainer.FolderViewModels, (folder, finished) => {
                        if (finished) {
                            ApplicationItem chatHistoryItem = new(folder.Folder.Entity);
                            // タイトルを日付 + 元のタイトルにする
                            chatHistoryItem.Description = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " Chat";
                            if (!string.IsNullOrEmpty(applicationItem.Description)) {
                                chatHistoryItem.Description += " " + applicationItem.Description;
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

    }
}
