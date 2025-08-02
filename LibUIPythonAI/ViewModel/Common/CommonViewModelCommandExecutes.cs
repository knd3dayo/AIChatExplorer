using System.Collections.ObjectModel;
using System.Windows;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.ExportImport;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.AutoProcessRule;
using LibUIPythonAI.View.PromptTemplate;
using LibUIPythonAI.View.Tag;
using LibUIPythonAI.View.VectorDB;
using LibUIPythonAI.ViewModel.Folder;
using LibUIPythonAI.ViewModel.Item;
using LibUIPythonAI.ViewModel.PromptTemplate;
using LibUIPythonAI.ViewModel.VectorDB;

namespace LibUIPythonAI.ViewModel.Common {
    public abstract class CommonViewModelCommandExecutes {

        public abstract ObservableCollection<ContentItemViewModel> GetSelectedItems();

        public abstract Task PasteFromClipboardCommandExecute();

        public Action<bool> UpdateIndeterminate { get; set; } = (visible) => { };

        public Action UpdateView { get; set; } = () => { };
        // Constructor
        public CommonViewModelCommandExecutes(Action<bool> updateIndeterminate, Action updateView) {
            UpdateIndeterminate = updateIndeterminate;
            UpdateView = updateView;
        }

        // ピン留めの切り替えコマンド (複数選択可能)
        public SimpleDelegateCommand<ContentItemViewModel> ChangePinCommand => new((itemViewModel) => {
            foreach (var item in GetSelectedItems()) {
                if (item is ContentItemViewModel applicationItemViewModel) {
                    applicationItemViewModel.IsPinned = !applicationItemViewModel.IsPinned;
                    // ピン留めの時は更新日時を変更しない
                    SaveApplicationItemCommand.Execute(applicationItemViewModel);
                }
            }
        });

        // フォルダを開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenFolderInExplorerCommand => new((itemViewModel) => {
            OpenFolderInExplorerExecute(itemViewModel);
        });



        // テキストをファイルとして開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenContentAsFileCommand => new((itemViewModel) => {
            OpenFolderInExplorerExecute(itemViewModel);
        });

        // ファイルを開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenFileCommand => new((itemViewModel) => {
            OpenFolderInExplorerExecute(itemViewModel);
        });

        // ファイルを新規ファイルとして開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenFileAsNewFileCommand => new((itemViewModel) => {
            OpenFileAsNewFileExecute(itemViewModel);
        });


        public SimpleDelegateCommand<ContentItemViewModel> OpenVectorSearchWindowCommand => new(async (itemViewModel) => {
            await OpenVectorSearchWindowCommandExecute(itemViewModel);
        });
        // OpenFolderVectorSearchWindowCommand
        public SimpleDelegateCommand<ContentFolderViewModel> OpenFolderVectorSearchWindowCommand => new((folderViewModel) => {
            OpenFolderVectorSearchWindowCommandExecute(folderViewModel);
        });

        public static SimpleDelegateCommand<object> ExitCommand => new((parameter) => {
            // Display exit confirmation dialog. If Yes, exit the application
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmExit, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                Application.Current.Shutdown();
            }
        });

        // Process when "Vector DB Management" is clicked in the menu
        public static void OpenVectorDBManagementWindowCommand() {
            // Open VectorDBManagementWindow
            ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Edit, FolderViewModelManagerBase.FolderViewModels, (vectorDBItem) => { });
        }


        // アイテム保存
        public SimpleDelegateCommand<ContentItemViewModel> SaveApplicationItemCommand => new(async (itemViewModel) => {
            await itemViewModel.ContentItem.Save();
        });


        // メニューの「プロンプトテンプレートを編集」をクリックしたときの処理
        public static void OpenListPromptTemplateWindowCommandExecute() {
            // ListPromptTemplateWindowを開く
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Edit, (promptTemplateWindowViewModel, OpenAIExecutionModeEnum) => { });
        }
        // メニューの「自動処理ルールを編集」をクリックしたときの処理
        public static void OpenListAutoProcessRuleWindowCommandExecute() {
            // ListAutoProcessRuleWindowを開く
            ListAutoProcessRuleWindow.OpenListAutoProcessRuleWindow(LibUIPythonAI.ViewModel.Folder.FolderViewModelManagerBase.FolderViewModels);

        }
        // メニューの「タグ編集」をクリックしたときの処理
        public static void OpenTagWindowCommandExecute() {
            // TagWindowを開く
            TagWindow.OpenTagWindow(null, () => { });
        }



        public void DownloadWebPageCommandExecute(ObservableCollection<ContentItemViewModel>? itemViewModels) {
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
        }

        // Command to open a file as a new file
        public static void OpenFileAsNewFileExecute(ContentItemViewModel itemViewModel) {
            // Open the selected item
            ProcessUtil.OpenApplicationItemFile(itemViewModel.ContentItem, true);
        }

        // Command to open a folder
        public static void OpenFolderInExplorerExecute(ContentItemViewModel itemViewModel) {
            ContentItemCommands.OpenFolderInExplorer(itemViewModel.ContentItem);
        }

        // Command to execute a prompt template (複数選択可能)
        public static void ExecutePromptTemplateCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels, PromptItem promptItem, Action beforeAction, Action afterAction) {
            PromptItem.ExecutePromptTemplate(itemViewModels.Select(x => x.ContentItem).ToList(), promptItem, beforeAction, afterAction);
        }

        public void ExecutePromptTemplateCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels, PromptItem promptItem) {
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
                             folder.FolderCommands.LoadFolderCommand.Execute();
                         }
                         // プログレスインジケータを非表示
                         UpdateIndeterminate(false);
                         StatusText.Instance.UpdateInProgress(false);
                         UpdateView();

                     });
                 });
        }

        // Command to perform vector search
        public static async Task OpenVectorSearchWindowCommandExecute(ContentItemViewModel itemViewModel) {
            VectorSearchItem VectorSearchItem = await itemViewModel.ContentItem.GetMainVectorSearchItem();
            // Open vector search result window
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new(VectorSearchItem) {
                // Action when a vector DB item is selected
                SelectVectorDBItemAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, FolderViewModelManagerBase.FolderViewModels, (vectorDBItemBase) => {
                        vectorDBItems.Add(vectorDBItemBase);
                    });
                }
            };
            var contentItem = itemViewModel.ContentItem;
            var vectorSearchItem = await contentItem.GetMainVectorSearchItem();
            vectorSearchItem.InputText = contentItem.Content;
            vectorSearchWindowViewModel.VectorSearchItem = vectorSearchItem;

            // Execute vector search
            vectorSearchWindowViewModel.SendCommand.Execute(null);
            VectorSearchWindow.OpenVectorSearchResultWindow(vectorSearchWindowViewModel);
        }


        // Command to perform vector search
        public static async Task OpenFolderVectorSearchWindowCommandExecute(ContentFolderViewModel folderViewModel) {
            // Open vector search result window
            VectorSearchItem VectorSearchItem = await folderViewModel.Folder.GetMainVectorSearchItem();
            VectorSearchWindowViewModel vectorSearchWindowViewModel = new(VectorSearchItem) {
                // Action when a vector DB item is selected
                SelectVectorDBItemAction = (vectorDBItems) => {
                    ListVectorDBWindow.OpenListVectorDBWindow(ListVectorDBWindowViewModel.ActionModeEnum.Select, FolderViewModelManagerBase.FolderViewModels, (vectorDBItemBase) => {
                        vectorDBItems.Add(vectorDBItemBase);
                    });
                }
            };
            var folder = folderViewModel.Folder;
            vectorSearchWindowViewModel.VectorSearchItem = await folder.GetMainVectorSearchItem();
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
                folderViewModel.FolderCommands.LoadFolderCommand.Execute();
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

        public void GenerateVectorCommandExecute(ObservableCollection<ContentItemViewModel> itemViewModels) {
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

        }


    }
}
