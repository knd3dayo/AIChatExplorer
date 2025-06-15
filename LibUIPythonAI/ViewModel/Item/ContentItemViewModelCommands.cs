using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.ExportImport;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.VectorDB;
using WpfAppCommon.Model;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Resources;

namespace LibUIPythonAI.ViewModel.Item {
    public class ContentItemViewModelCommands {

        public Action<bool> UpdateIndeterminate { get; set; } = (visible) => { };

        public Action UpdateView { get; set; } = () => { };
        // Constructor
        public ContentItemViewModelCommands(Action<bool> updateIndeterminate, Action updateView) {
            UpdateIndeterminate = updateIndeterminate;
            UpdateView = updateView;
        }




        // コンテキストメニューの「テキストを抽出」の実行用コマンド (複数選択可能)
        // 処理中はプログレスインジケータを表示
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>?> ExtractTextCommand => new((items) => {
            if (items == null || items.Count == 0) {
                return;
            }
            ExtractTextCommandExecute(items, () => {
                UpdateIndeterminate(true);
            }, () => {
                LogWrapper.Info(CommonStringResources.Instance.TextExtractionCompleted);
                UpdateIndeterminate(false);
                StatusText.Instance.UpdateInProgress(false);
                UpdateView();
            });
        });

        // Webページをダウンロードする
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>?> DownloadWebPageCommand => new((itemViewModels) => {
            if (itemViewModels == null || itemViewModels.Count == 0) {
                return;
            }
            UpdateIndeterminate(true);
            Task.Run(() => {
                ImportExportUtil.ImportFromURLList(itemViewModels.Select(x => x.ContentItem).ToList(), (item) => {
                    UpdateIndeterminate(false);
                    StatusText.Instance.UpdateInProgress(false);
                    UpdateView();
                });
            });


        });

        // ベクトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> GenerateVectorCommand => new((itemViewModels) => {
            GenerateVectorCommandExecute(itemViewModels,
                () => {
                    // Display ProgressIndicator
                    UpdateIndeterminate(true);
                },
                () => {

                    // Hide ProgressIndicator
                    UpdateIndeterminate(false);
                    StatusText.Instance.UpdateInProgress(false);
                    UpdateView();
                });
        });

        // プロンプトテンプレートを実行
        public SimpleDelegateCommand<ValueTuple<ObservableCollection<ContentItemViewModel>, PromptItem>> ExecutePromptTemplateCommand => new((tuple) => {
            ObservableCollection<ContentItemViewModel> itemViewModels = tuple.Item1;
            PromptItem promptItem = tuple.Item2;
            ExecutePromptTemplateCommandExecute(itemViewModels, promptItem,
                () => {
                    // プログレスインジケータを表示
                    UpdateIndeterminate(true);
                },
                () => {
                    // フォルダ内のアイテムを再読み込み
                    MainUITask.Run(() => {
                        var folders = itemViewModels.Select(x => x.FolderViewModel).DistinctBy(x => x.Folder.Id);
                        foreach (var folder in folders) {
                            folder.LoadFolderCommand.Execute();
                        }
                        // プログレスインジケータを非表示
                        UpdateIndeterminate(false);
                        StatusText.Instance.UpdateInProgress(false);
                        UpdateView();

                    });
                });
        });

        // タイトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> GenerateTitleCommand => new((itemViewModels) => {
            string promptName = SystemDefinedPromptNames.TitleGeneration.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand.Execute((itemViewModels, promptItem));
        });

        // タグを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> GenerateTagsCommand => new((itemViewModels) => {
            string promptName = SystemDefinedPromptNames.TagGeneration.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand.Execute((itemViewModels, promptItem));
        });


        // 背景情報を生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> GenerateBackgroundInfoCommand => new((itemViewModels) => {
            string promptName = SystemDefinedPromptNames.BackgroundInformationGeneration.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand.Execute((itemViewModels, promptItem));
        });

        // サマリーを生成する処理　複数アイテム処理可
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> GenerateSummaryCommand => new((itemViewModels) => {
            string promptName = SystemDefinedPromptNames.SummaryGeneration.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand.Execute((itemViewModels, promptItem));
        });

        // 課題リストを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> GenerateTasksCommand => new((itemViewModels) => {
            string promptName = SystemDefinedPromptNames.TasksGeneration.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand.Execute((itemViewModels, promptItem));
        });

        // 文書の信頼度を判定する処理 複数アイテム処理可
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> CheckDocumentReliabilityCommand => new((itemViewModels) => {
            string promptName = SystemDefinedPromptNames.DocumentReliabilityCheck.ToString();
            PromptItem? promptItem = PromptItem.GetPromptItemByName(promptName);
            if (promptItem == null) {
                LogWrapper.Error(CommonStringResources.Instance.PromptTemplateNotFound);
                return;
            }
            ExecutePromptTemplateCommand.Execute((itemViewModels, promptItem));
        });


        // ベクトル検索を実行するコマンド
        public SimpleDelegateCommand<ContentItemViewModel> VectorSearchCommand => new((itemViewModel) => {
            OpenVectorSearchWindowCommandExecute(itemViewModel);
        });

        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> DeleteItemsCommand => new((itemViewModels) => {
            DeleteItemsCommandExecute(itemViewModels,
                () => {
                    // プログレスインジケータを表示
                    UpdateIndeterminate(true);
                },
                () => {
                    UpdateIndeterminate(false);
                    UpdateView();
                });
        });


        // Command to open a file
        public static void OpenFileExecute(ContentItemViewModel itemViewModel) {
            ContentItemWrapper contentItem = itemViewModel.ContentItem;
            // Open the selected item
            ProcessUtil.OpenApplicationItemFile(contentItem, false);
        }

        // Command to open a file as a new file
        public static void OpenFileAsNewFileExecute(ContentItemViewModel itemViewModel) {
            // Open the selected item
            ProcessUtil.OpenApplicationItemFile(itemViewModel.ContentItem, true);
        }

        // Command to open text content as a file
        public void OpenContentAsFileExecute(ContentItemViewModel itemViewModel) {
            try {
                // Open the selected item
                ProcessUtil.OpenApplicationItemContent(itemViewModel.ContentItem);
            } catch (Exception e) {
                LogWrapper.Error(e.Message);
            }
        }

        // Command to open a folder
        public static void OpenFolderExecute(ContentItemViewModel itemViewModel) {
            ContentItemCommands.OpenFolder(itemViewModel.ContentItem);
        }

        // Command to execute a prompt template (複数選択可能)
        public static void ExecutePromptTemplateCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels, PromptItem promptItem, Action beforeAction, Action afterAction) {
            PromptItem.ExecutePromptTemplate(itemViewModels.Select(x => x.ContentItem).ToList(), promptItem, beforeAction, afterAction);
        }

        // Command to perform vector search
        public static void OpenVectorSearchWindowCommandExecute(ContentItemViewModel itemViewModel) {
            VectorSearchItem VectorSearchItem = itemViewModel.ContentItem.GetMainVectorSearchItem();
            // Open vector search result window
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new(VectorSearchItem) {
                // Action when a vector DB item is selected
                SelectVectorDBItemAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                        vectorDBItems.Add(vectorDBItemBase);
                    });
                }
            };
            var contentItem = itemViewModel.ContentItem;
            vectorSearchWindowViewModel.VectorSearchItem = contentItem.GetMainVectorSearchItem();
            vectorSearchWindowViewModel.VectorSearchItem.InputText = contentItem.Content;
            // Execute vector search
            vectorSearchWindowViewModel.SendCommand.Execute(null);
            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }


        // Command to perform vector search
        public static void OpenFolderVectorSearchWindowCommandExecute(ContentFolderViewModel folderViewModel) {
            // Open vector search result window
            VectorSearchItem VectorSearchItem = folderViewModel.Folder.GetMainVectorSearchItem();
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new(VectorSearchItem) {
                // Action when a vector DB item is selected
                SelectVectorDBItemAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, RootFolderViewModelContainer.FolderViewModels, (vectorDBItemBase) => {
                        vectorDBItems.Add(vectorDBItemBase);
                    });
                }
            };
            var folder = folderViewModel.Folder;
            vectorSearchWindowViewModel.VectorSearchItem = folder.GetMainVectorSearchItem();
            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }

        // Deleteが押された時の処理 選択中のアイテムを削除する処理
        public static void DeleteItemsCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels, Action beforeAction, Action afterAction) {
            // 選択中のアイテムがない場合は処理をしない
            if (itemViewModels.Count == 0) {
                LogWrapper.Error(PythonAILibStringResources.Instance.NoItemSelected);
                return;
            }
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteSelectedItems, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result != MessageBoxResult.Yes) {
                return;
            }
            beforeAction();
            ContentItemViewModel.DeleteItems([.. itemViewModels]).ContinueWith((task) => {
                LogWrapper.Info(CommonStringResources.Instance.Deleted);
                afterAction();
            });
        }

        // Command to reload the folder
        public static void ReloadFolderCommandExecute(ContentFolderViewModel? folderViewModel, Action beforeAction, Action afterAction) {
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

        public static void ExtractTextCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels, Action beforeAction, Action afterAction) {

            ContentItemCommands.ExtractTexts(itemViewModels.Select(x => x.ContentItem).ToList(), beforeAction, afterAction);
        }

        // Command to generate vectors
        public static void GenerateVectorCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels, Action beforeAction, Action afterAction) {
            ContentItemCommands.UpdateEmbeddings(itemViewModels.Select(x => x.ContentItem).ToList(), beforeAction, afterAction);
        }


    }
}
