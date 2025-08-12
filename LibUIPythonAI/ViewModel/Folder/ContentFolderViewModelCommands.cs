using System.Collections.ObjectModel;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.Content;
using LibPythonAI.Model.VectorDB;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.View.Folder;
using LibUIPythonAI.ViewModel.Common;
using LibUIPythonAI.ViewModel.Item;

namespace LibUIPythonAI.ViewModel.Folder {
    public class ContentFolderViewModelCommands : ObservableObject {

        public CommonViewModelCommandExecutes CommandExecutes { get; set; }

        public Action UpdateView { get; set; } = () => { };

        public ContentFolderViewModel FolderViewModel { get; private set; } = null!;
        // Constructor
        public ContentFolderViewModelCommands(ContentFolderViewModel folderViewModel, CommonViewModelCommandExecutes commandExecutes) {
            FolderViewModel = folderViewModel;
            CommandExecutes = commandExecutes;
        }

        // アイテム保存コマンド
        public virtual SimpleDelegateCommand<ContentItemViewModel> AddItemCommand => new(async (item) => {
            await FolderViewModel.Folder.AddItemAsync(item.ContentItem);
        });

        // フォルダー保存コマンド
        public virtual SimpleDelegateCommand<ContentFolderViewModel> SaveFolderCommand => new(async (folderViewModel) => {
            try {
                await FolderViewModel.Folder.SaveAsync();
            } catch (Exception ex) {
                LogWrapper.Error($"SaveFolderCommand failed: {ex.Message}");
            }
        });

        // 新規フォルダ作成コマンド
        public SimpleDelegateCommand<object> CreateFolderCommand => new((parameter) => {
            FolderViewModel.CreateFolderCommandExecute(FolderViewModel, async () => {
                try {
                    await FolderViewModel.Folder.SaveAsync();
                } catch (Exception ex) {
                    LogWrapper.Error($"CreateFolderCommand failed: {ex.Message}");
                }
                FolderViewModel.FolderCommands.LoadFolderCommand.Execute();
            });
        });

        public virtual SimpleDelegateCommand<object> LoadFolderCommand => new(async (parameter) => {
            CommandExecutes.UpdateIndeterminate(true);
            await FolderViewModel.LoadFolderExecuteAsync();
            MainUITask.Run(() => {
                CommandExecutes.UpdateIndeterminate(false);
                FolderViewModel.UpdateStatusText();
            });
        });


        // フォルダ編集コマンド
        public SimpleDelegateCommand<object> EditFolderCommand => new((parameter) => {
            FolderViewModel.EditFolderCommandExecute(async () => {
                try {
                    await FolderViewModel.Folder.SaveAsync();
                } catch (Exception ex) {
                    LogWrapper.Error($"EditFolderCommand failed: {ex.Message}");
                }
                LoadFolderCommand.Execute();
                LogWrapper.Info(CommonStringResources.Instance.FolderEdited);
            });
        });


        public async Task DeleteDisplayedItemCommandExecuteAsync(Action beforeAction, Action afterAction) {
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteItems, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                beforeAction();
                await ContentItemViewModel.DeleteItems([.. FolderViewModel.Items]);
                // 全ての削除処理が終了した後、後続処理を実行
                afterAction();
            }
        }

        // Ctrl + DeleteAsync が押された時の処理 選択中のフォルダのアイテムを削除する
        public SimpleDelegateCommand<object> DeleteDisplayedItemCommand => new(async (parameter) => {
            await DeleteDisplayedItemCommandExecuteAsync(() => {
                CommandExecutes.UpdateIndeterminate(true);
            }, () => {
                // 全ての削除処理が終了した後、後続処理を実行
                // フォルダ内のアイテムを再読み込む
                MainUITask.Run(() => {
                    LoadFolderCommand.Execute();
                });
                LogWrapper.Info(CommonStringResources.Instance.Deleted);
                CommandExecutes.UpdateIndeterminate(false);
            });
        });

        public SimpleDelegateCommand<object> DeleteFolderCommand => new(async (parameter) => {
            // フォルダ削除するかどうか確認
            if (MessageBox.Show(CommonStringResources.Instance.ConfirmDeleteFolder, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            // 親フォルダを取得
            ContentFolderViewModel? parentFolderViewModel = FolderViewModel.ParentFolderViewModel;

            try {
                await FolderViewModel.Folder.DeleteAsync();
            } catch (Exception ex) {
                LogWrapper.Error($"DeleteFolderCommand failed: {ex.Message}");
            }

            // 親フォルダが存在する場合は、親フォルダを再読み込み
            if (parentFolderViewModel != null) {
                parentFolderViewModel.FolderCommands.LoadFolderCommand.Execute();
            }

            LogWrapper.Info(CommonStringResources.Instance.FolderDeleted);
        });
        // ベクトルのリフレッシュ
        public SimpleDelegateCommand<object> RefreshVectorDBCollectionCommand => new(async (parameter) => {
            // MainWindowViewModelのIsIndeterminateをTrueに設定
            var item = await FolderViewModel.Folder.GetMainVectorSearchItem();
            string? vectorDBItemName = item.VectorDBItemName;
            if (vectorDBItemName == null) {
                return;
            }
            CommandExecutes.UpdateIndeterminate(true);
            var contentFolderPath = await FolderViewModel.Folder.GetContentFolderPath();
            await Task.Run(() => VectorEmbeddingItem.DeleteEmbeddingsByFolderAsync(vectorDBItemName, contentFolderPath));
            var contentItems = await FolderViewModel.Folder.GetItemsAsync<ContentItemWrapper>(isSync: false);
            await Task.Run(() => ContentItemCommands.UpdateEmbeddingsAsync(contentItems, () => { }, async () => {
                var items = await FolderViewModel.Folder.GetItemsAsync<ContentItemWrapper>(isSync: false);
                foreach (var contentItem in items) {
                    await contentItem.SaveAsync();
                }
                CommandExecutes.UpdateIndeterminate(false);
            }));
        });
        // ExportImportFolderCommand
        public SimpleDelegateCommand<object> ExportImportFolderCommand => new((parameter) => {
            // ExportImportFolderWindowを開く
            ExportImportWindow.OpenExportImportFolderWindow(FolderViewModel, () => {
                // ファイルを再読み込み
                FolderViewModel.FolderCommands.LoadFolderCommand.Execute();
            });
        });


        // ExtractTextCommand
        public SimpleDelegateCommand<object> ExtractTextCommand => new((parameter) => {
            // ContentTypes.Files, ContentTypes.Imageのアイテムを取得
            var itemViewModels = FolderViewModel.Items.Where(x => x.ContentItem.ContentType == ContentItemTypes.ContentItemTypeEnum.Files || x.ContentItem.ContentType == ContentItemTypes.ContentItemTypeEnum.Files);
            ExtractTextCommand.Execute(FolderViewModel.GetSelectedItems());

        });

        // Webページをダウンロードする
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>?> DownloadWebPageCommand => new((itemViewModels) => {
            CommandExecutes.DownloadWebPageCommandExecute(itemViewModels);
        });


        // ベクトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> GenerateVectorCommand => new(async (itemViewModels) => {
            await CommandExecutes.GenerateVectorCommandExecute(itemViewModels);
        });

    }
}
