using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using LibUIPythonAI.ViewModel.Item;
using WpfAppCommon.Utils;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Folder;
using System.Collections.ObjectModel;
using LibUIPythonAI.Resource;
using WpfAppCommon.Model;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.VectorDB;
using System.Diagnostics;
using PythonAILib.Model.File;
using System.Windows;
using LibPythonAI.Utils.Common;

namespace PythonAILibUI.ViewModel.Item {
    public abstract class ContentItemViewModelCommands {

        // Command to open a file
        public  void OpenFileExecute(ContentItemViewModel itemViewModel) {
            ContentItemWrapper contentItem = itemViewModel.ContentItem;
            // Open the selected item
            ProcessUtil.OpenClipboardItemFile(contentItem, false);
        }

        // Command to open a file as a new file
        public  void OpenFileAsNewFileExecute(ContentItemViewModel itemViewModel) {
            // Open the selected item
            ProcessUtil.OpenClipboardItemFile(itemViewModel.ContentItem, true);
        }

        // Command to open text content as a file
        public  void OpenContentAsFileExecute(ContentItemViewModel itemViewModel) {
            try {
                // Open the selected item
                ProcessUtil.OpenClipboardItemContent(itemViewModel.ContentItem);
            } catch (Exception e) {
                LogWrapper.Error(e.Message);
            }
        }


        public abstract void OpenItemCommandExecute(ContentItemViewModel itemViewModel);

        public abstract void OpenOpenAIChatWindowCommandExecute(ContentItemViewModel itemViewModel);

        // Command to open a folder
        public void OpenFolderExecute(ContentItemViewModel itemViewModel) {
            ContentItemWrapper contentItem = itemViewModel.ContentItem;
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

        // Command to execute a prompt template (複数選択可能)
        public void ExecutePromptTemplateCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels, PromptItem promptItem, Action beforeAction, Action afterAction) {

            // promptNameからDescriptionを取得
            string description = promptItem.Description;

            LogWrapper.Info(CommonStringResources.Instance.PromptTemplateExecute(description));
            int count = itemViewModels.Count;
            Task.Run(() => {
                beforeAction();
                object lockObject = new();
                int start_count = 0;
                var items = itemViewModels.Select(x => x.ContentItem).ToList();
                ParallelOptions parallelOptions = new() {
                    MaxDegreeOfParallelism = 8
                };
                Parallel.For(0, count, parallelOptions, (i) => {
                    lock (lockObject) {
                        start_count++;
                    }
                    int index = i; // Store the current index in a separate variable to avoid closure issues
                    string message = $"{CommonStringResources.Instance.PromptTemplateInProgress(description)} ({start_count}/{count})";
                    StatusText.Instance.UpdateInProgress(true, message);
                    ContentItemWrapper item = items[index];

                    ContentItemCommands.CreateChatResult(item, promptItem.Name);
                    // Save
                    item.Save(false);
                });
                // Execute if obj is an Action
                afterAction();
                LogWrapper.Info(CommonStringResources.Instance.PromptTemplateExecuted(description));
            });

        }

        // Command to perform vector search
        public  void OpenVectorSearchWindowCommandExecute(ContentItemViewModel itemViewModel) {
            // Open vector search result window
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new();
            // Action when a vector DB item is selected
            vectorSearchWindowViewModel.SelectVectorDBItemAction = (vectorDBItems) => {
                ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                    vectorDBItems.Add(vectorDBItemBase);
                });
            };
            var contentItem = itemViewModel.ContentItem;
            vectorSearchWindowViewModel.VectorSearchProperty = contentItem.GetMainVectorSearchProperty();
            vectorSearchWindowViewModel.InputText = contentItem.Content;
            // Execute vector search
            vectorSearchWindowViewModel.SendCommand.Execute(null);
            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }


        // Command to perform vector search
        public void OpenFolderVectorSearchWindowCommandExecute(ContentFolderViewModel folderViewModel) {
            // Open vector search result window
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new();
            // Action when a vector DB item is selected
            vectorSearchWindowViewModel.SelectVectorDBItemAction = (vectorDBItems) => {
                ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                    vectorDBItems.Add(vectorDBItemBase);
                });
            };
            var folder = folderViewModel.Folder;
            vectorSearchWindowViewModel.VectorSearchProperty = folder.GetMainVectorSearchProperty();
            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public void DeleteItemsCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels, Action beforeAction, Action afterAction) {
            // 選択中のアイテムがない場合は処理をしない
            if (itemViewModels.Count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteSelectedItems, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                beforeAction();
                ContentItemViewModel.DeleteItems([.. itemViewModels]).ContinueWith((task) => {
                    LogWrapper.Info(CommonStringResources.Instance.Deleted);
                    afterAction();
                });
            }
        }
        // Command to reload the folder
        public void ReloadFolderCommandExecute(ContentFolderViewModel? folderViewModel, Action beforeAction, Action afterAction) {
            if (folderViewModel == null) {
                return;
            }
            Task.Run(() => {
                // Display ProgressIndicator until processing is complete
                beforeAction();
                folderViewModel.LoadFolderCommand.Execute();
                LogWrapper.Info(CommonStringResources.Instance.Reloaded);
                afterAction();
            });
        }

        public void ExtractTextCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels, Action beforeAction, Action afterAction) {

            int count = itemViewModels.Count;
            if (count == 0) {
                LogWrapper.Error(CommonStringResources.Instance.NoItemSelected);
                return;
            }
            beforeAction();

            Task.Run(() => {
                object lockObject = new();
                int start_count = 0;
                var items = itemViewModels.ToList();
                ParallelOptions parallelOptions = new() {
                    // 20並列
                    MaxDegreeOfParallelism = 8
                };
                Parallel.For(0, count, parallelOptions, (i) => {
                    int index = i; // Store the current index in a separate variable to avoid closure issues
                    lock (lockObject) {
                        start_count++;
                    }
                    string message = $"{CommonStringResources.Instance.TextExtractionInProgress} ({start_count}/{count})";
                    StatusText.Instance.UpdateInProgress(true, message);
                    var itemViewModel = items[index];

                    ContentItemWrapper item = itemViewModel.ContentItem;
                    if (item.ContentType == ContentTypes.ContentItemTypes.Text) {
                        LogWrapper.Error(CommonStringResources.Instance.CannotExtractTextForNonFileContent);
                        return;
                    }
                    ContentItemCommands.ExtractTextCommandExecute(item);
                    // Save the item
                    item.Save(false);
                });
                afterAction();
            });
        }
        // Command to generate vectors
        public void GenerateVectorCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels, Action beforeAction, Action afterAction) {

            Task.Run(() => {
                beforeAction();
                var items = itemViewModels.Select(x => x.ContentItem).ToList();
                ContentItemCommands.UpdateEmbeddings(items);
                // Save
                foreach (var item in items) {
                    item.Save(false);
                }
                LogWrapper.Info(CommonStringResources.Instance.GenerateVectorCompleted);
                // Execute if obj is an Action
                afterAction();
            });
        }



    }
}
