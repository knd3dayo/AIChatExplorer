using System.Windows;
using QAChat.View.RAGWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel.RAGWindow
{
    public class EditRAGSourceWindowViewModel : MyWindowViewModel
    {

        // 初期化
        public EditRAGSourceWindowViewModel(RAGSourceItemViewModel itemViewModel, Action<RAGSourceItemViewModel> afterUpdate)
        {
            ItemViewModel = itemViewModel;

            // test
            if (ItemViewModel != null)
            {
                ItemViewModel.SelectedVectorDBItem = itemViewModel?.SelectedVectorDBItem;
            }

            AfterUpdate = afterUpdate;
        }

        private RAGSourceItemViewModel? itemViewModel;
        public RAGSourceItemViewModel? ItemViewModel
        {
            get
            {
                return itemViewModel;
            }
            set
            {
                itemViewModel = value;
                OnPropertyChanged(nameof(ItemViewModel));
            }
        }


        private Action<RAGSourceItemViewModel> AfterUpdate { get; set; } = (promptItem) => { };

        // OKボタンのコマンド
        public SimpleDelegateCommand<Window> OKButtonCommand => new((window) =>
        {
            // TitleとContentの更新を反映
            if (ItemViewModel == null)
            {
                return;
            }
            // RAGSourceItemを更新
            ItemViewModel.Save();
            AfterUpdate(ItemViewModel);

            // ウィンドウを閉じる
            window.Close();
        });

        // キャンセルボタンのコマンド
        public SimpleDelegateCommand<Window> CancelButtonCommand => new((window) =>
        {
            // ウィンドウを閉じる
            window.Close();
        });
        // UpdateIndexButtonCommand UpdateRAGIndexWindowを開く。
        public SimpleDelegateCommand<object> UpdateIndexButtonCommand => new((parameter) =>
        {
            if (ItemViewModel == null)
            {
                return;
            }
            if (ItemViewModel.Item == null)
            {
                return;
            }
            UpdateRAGIndexWindow.OpenUpdateRAGIndexWindow(ItemViewModel, (afterUpdate) =>
            {
                // 更新
            });
        });

        // WorkingDirectoryのチェック
        public SimpleDelegateCommand<object> CheckWorkingDirCommand => new((parameter) =>
        {
            try
            {
                if (ItemViewModel == null)
                {
                    LogWrapper.Error(StringResources.ItemViewModelIsNull);
                    return;
                }
                ItemViewModel.SourceURL = "";

                ItemViewModel.CheckWorkingDirectory();

            }
            catch (Exception e)
            {
                LogWrapper.Error(e.Message);
            }
        });

    }
}
