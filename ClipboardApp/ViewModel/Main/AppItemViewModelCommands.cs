using System.Collections.ObjectModel;
using System.Windows;
using ClipboardApp.Model.Folders.Clipboard;
using ClipboardApp.Model.Folders.Search;
using ClipboardApp.Model.Item;
using ClipboardApp.Model.Main;
using ClipboardApp.View.Settings;
using ClipboardApp.ViewModel.Content;
using ClipboardApp.ViewModel.Folders.Clipboard;
using ClipboardApp.ViewModel.Folders.Search;
using LibGit2Sharp;
using LibPythonAI.Utils.Common;
using LibUIImageChat.View;
using LibUIMergeChat.View;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.ChatMain;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.View.Item;
using LibUIPythonAI.View.RAG;
using LibUIPythonAI.View.Search;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.ViewModel.VectorDB;
using NetOffice.OutlookApi;
using PythonAILib.Model.Content;
using PythonAILib.Model.Folder;
using PythonAILib.Model.Search;
using PythonAILib.Resources;
using PythonAILibUI.ViewModel.Item;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.ViewModel.Main {
    public class AppItemViewModelCommands(Action<bool> updateIndeterminate) : ContentItemViewModelCommands(updateIndeterminate) {


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
        public SimpleDelegateCommand<ContentItemViewModel> SaveClipboardItemCommand => new((itemViewModel) => {
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


        public void OpenItemCommandExecute(ContentItemViewModel? itemViewModel) {
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

            AppTabContainer container = new(itemViewModel.ContentItem.Description, editItemControl);

            // UserControlをクローズする場合の処理を設定
            editItemControl.SetCloseUserControl(() => {
                MainWindowViewModel.Instance.RemoveTabItem(container);
            });

            MainWindowViewModel.Instance.AddTabItem(container);
        }

        // Command to open OpenAI Chat
        public void OpenOpenAIChatWindowCommandExecute(ContentItemViewModel itemViewModel) {

            QAChatStartupProps qAChatStartupProps = CreateQAChatStartupProps(itemViewModel.ContentItem);
            QAChatMainWindow.OpenOpenAIChatWindow(qAChatStartupProps);
        }

        // Command to open Image Chat
        public void OpenImageChatWindowCommand(ContentItemWrapper item, System.Action action) {
            ImageChatMainWindow.OpenMainWindow(item, action);
        }

        // Command to Open Merge Chat
        public void OpenMergeChatWindowCommand(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems) {
            MergeChatMainWindow.OpenWindow(folderViewModel, selectedItems);
        }

        // Process when "RAG Management" is clicked in the menu
        public void OpenRAGManagementWindowCommand() {
            // Open RARManagementWindow
            ListRAGSourceWindow.OpenRagManagementWindow();
        }

        // Process when "Vector DB Management" is clicked in the menu
        public void OpenVectorDBManagementWindowCommand() {
            // Open VectorDBManagementWindow
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Edit, RootFolderViewModelContainer.FolderViewModels, (vectorDBItem) => { });
        }

        // Process when "Settings" is clicked in the menu
        public void SettingCommandExecute() {
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
        public void OpenSearchWindowCommand(SearchFolderViewModel searchFolderViewModel, System.Action action) {
            SearchRule? searchConditionRule = new(
                new LibPythonAI.Data.SearchRuleEntity() {
                    SearchFolder = searchFolderViewModel.Folder.Entity,
                });
            SearchWindow.OpenSearchWindow(searchConditionRule, searchFolderViewModel.Folder, action);

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

        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> CutItemCommand => new((itemViewModels) => {
            CutItemCommandExecute(itemViewModels);
        });

        // Process when Ctrl + X is pressed on clipboard items; multiple items can be processed
        public void CutItemCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels) {
            // Do not process if no items are selected
            if (itemViewModels.Count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            // Set Cut Flag
            ClipboardController.Instance.CutFlag = ClipboardController.CutFlagEnum.Item;
            // Set the selected items to CopiedItems
            ClipboardController.Instance.CopiedObjects.Clear();
            foreach (ClipboardItemViewModel item in itemViewModels) {
                ClipboardController.Instance.CopiedObjects.Add(item);
            }
            LogWrapper.Info(CommonStringResources.Instance.Cut);
        }

        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> CopyItemCommand => new((itemViewModels) => {
            CopyToClipboardCommandExecute(itemViewModels);
        });

        // Process when Ctrl + C is pressed
        public void CopyToClipboardCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels) {

            // Do not process if no items are selected
            if (itemViewModels.Count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            // Reset Cut flag
            ClipboardController.Instance.CutFlag = ClipboardController.CutFlagEnum.None;
            // Set the selected items to CopiedItems
            ClipboardController.Instance.CopiedObjects.Clear();
            foreach (ClipboardItemViewModel item in itemViewModels) {
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


        // ピン留めの切り替えコマンド (複数選択可能)
        public SimpleDelegateCommand<ClipboardItemViewModel> ChangePinCommand => new((itemViewModel) => {
            foreach (var item in MainWindowViewModel.Instance.MainPanelDataGridViewControlViewModel.SelectedItems) {
                if (item is ClipboardItemViewModel clipboardItemViewModel) {
                    clipboardItemViewModel.IsPinned = !clipboardItemViewModel.IsPinned;
                    // ピン留めの時は更新日時を変更しない
                    SaveClipboardItemCommand.Execute(clipboardItemViewModel);
                }
            }
        });

        // Command to start/stop clipboard monitoring
        public void StartStopClipboardMonitorCommand() {
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


        // -----------------------------------------------------------------------------------
        #region プログレスインジケーター表示の処理

        // Process when Ctrl + V is pressed
        public void PasteFromClipboardCommandExecute() {
            MainWindowViewModel windowViewModel = MainWindowViewModel.Instance;
            ContentFolderViewModel? folder = windowViewModel.MainPanelTreeViewControlViewModel?.SelectedFolder;
            if (folder is not ClipboardFolderViewModel SelectedFolder) {
                LogWrapper.Error(CommonStringResources.Instance.NoPasteFolder);
                return;
            }
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
                ProcessClipboardItem(clipboardFolder, ClipboardController.LastClipboardChangedEventArgs);
            }
        }

        private void ProcessClipboardItem(ClipboardFolder clipboardFolder, ClipboardChangedEventArgs lastClipboardChangedEventArgs) {
            UpdateIndeterminate(true);
            clipboardFolder.ProcessClipboardItem(lastClipboardChangedEventArgs,
                async (clipboardItem) => {
                    // Process when a clipboard item is added
                    await Task.Run(() => {
                        // Save to folder if saveToFolder is true
                        clipboardFolder.AddItem(clipboardItem);
                        // Process after pasting
                    }).ContinueWith((obj) => {
                        MainUITask.Run(() => {
                            MainWindowViewModel.Instance.MainPanelTreeViewControlViewModel.SelectedFolder?.LoadFolderCommand.Execute();
                        });
                        UpdateIndeterminate(false);
                    });
                });
        }

        #endregion
        // -----------------------------------------------------------------------------------

        public static QAChatStartupProps CreateQAChatStartupProps(ContentItemWrapper clipboardItem) {

            MainWindowViewModel ActiveInstance = MainWindowViewModel.Instance;
            QAChatStartupProps props = new(clipboardItem) {
                // Closeアクション
                CloseCommand = (item, saveChatHistory) => {
                    bool flag = clipboardItem.Entity.Folder?.FolderTypeString != FolderTypeEnum.Chat.ToString();
                    clipboardItem.Save();

                    if (saveChatHistory && flag) {
                        // チャット履歴用のItemの設定
                        ClipboardFolder chatFolder = (ClipboardFolder)ActiveInstance.RootFolderViewModelContainer.ChatRootFolderViewModel.Folder;
                        ContentItemWrapper chatHistoryItem = clipboardItem.Copy(); // new();
                        chatHistoryItem.Folder = chatFolder;

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

                    FolderSelectWindow.OpenFolderSelectWindow(RootFolderViewModelContainer.FolderViewModels, (folder, finished) => {
                        if (finished) {
                            ClipboardItem chatHistoryItem = new(folder.Folder.Entity);
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

    }
}
