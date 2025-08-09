using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.Prompt;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using LibUIPythonAI.ViewModel.Common;

namespace LibUIPythonAI.ViewModel.Item {
    public class ContentItemViewModelCommands : ObservableObject {

        public ContentItemViewModel ItemViewModel { get; private set; } = null!;
        public CommonViewModelCommandExecutes CommandExecutes { get; set; }

        // Constructor
        public ContentItemViewModelCommands(ContentItemViewModel itemViewModel, CommonViewModelCommandExecutes commandExecutes) {
             CommandExecutes = commandExecutes;
             ItemViewModel = itemViewModel;
        }



        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> DeleteItemsCommand => new((itemViewModels) => {
            CommonViewModelCommandExecutes.DeleteItemsCommandExecute(itemViewModels,
                () => {
                    // プログレスインジケータを表示
                    CommandExecutes.UpdateIndeterminate(true);
                },
                () => {
                    CommandExecutes.UpdateIndeterminate(false);
                    CommandExecutes.UpdateView();
                    foreach (var itemViewModel in itemViewModels) {
                        CommonViewModelCommandExecutes.ReloadFolderCommandExecute(itemViewModel.FolderViewModel, () => { }, () => { });
                    }
                });
        });


        // コンテキストメニューの「テキストを抽出」の実行用コマンド (複数選択可能)
        // 処理中はプログレスインジケータを表示
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>?> ExtractTextCommand => new(async (items) => {
            if (items == null || items.Count == 0) {
                return;
            }
            CommandExecutes.UpdateIndeterminate(true);
            await CommonViewModelCommandExecutes.ExtractTextCommandExecute(items);
            LogWrapper.Info(CommonStringResources.Instance.TextExtractionCompleted);
            CommandExecutes.UpdateIndeterminate(false);
            StatusText.Instance.UpdateInProgress(false);
            CommandExecutes.UpdateView();
        });


        // ベクトル検索を実行するコマンド
        public SimpleDelegateCommand<ContentItemViewModel> VectorSearchCommand => new(async (itemViewModel) => {
            await CommonViewModelCommandExecutes.OpenVectorSearchWindowCommandExecute(itemViewModel);
        });

        // プロンプトテンプレートを実行
        public SimpleDelegateCommand<ValueTuple<ObservableCollection<ContentItemViewModel>, PromptItem>> ExecutePromptTemplateCommand => new(async (tuple) => {
            ObservableCollection<ContentItemViewModel> itemViewModels = tuple.Item1;
            PromptItem promptItem = tuple.Item2;
            await CommandExecutes.ExecutePromptTemplateCommandExecute(itemViewModels, promptItem);
        });

        // Webページをダウンロードする
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>?> DownloadWebPageCommand => new((itemViewModels) => {
            CommandExecutes.DownloadWebPageCommandExecute(itemViewModels);
        });
        // ベクトルを生成する処理 複数アイテム処理可
        public SimpleDelegateCommand<ObservableCollection<ContentItemViewModel>> GenerateVectorCommand => new(async (itemViewModels) => {
            await CommandExecutes.GenerateVectorCommandExecute(itemViewModels);
        });
        // フォルダを開くコマンド
        public SimpleDelegateCommand<ContentItemViewModel> OpenFolderInExplorerCommand => new((itemViewModel) => {
            CommonViewModelCommandExecutes.OpenFolderInExplorerExecute(itemViewModel);
        });

    }
}
