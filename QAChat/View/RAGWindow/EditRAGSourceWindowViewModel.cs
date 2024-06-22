using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.View.RAGWindow {
    public class EditRAGSourceWindowViewModel : MyWindowViewModel {

        private RAGSourceItemViewModel? itemViewModel;
        public RAGSourceItemViewModel? ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                OnPropertyChanged(nameof(ItemViewModel));
            }
        }

        // Windowのタイトル　ItemViewModelがnullの場合は新規作成、それ以外は編集
        public string WindowTitle {
            get {
                return itemViewModel == null ? "新規作成" : "編集"; ;
            }
        }

        private Action<RAGSourceItemViewModel> AfterUpdate { get; set; } = (promptItem) => { };
        // 初期化
        public void Initialize(RAGSourceItemViewModel itemViewModel, Action<RAGSourceItemViewModel> afterUpdate) {
            ItemViewModel = itemViewModel;

            // Windowのタイトルを更新
            OnPropertyChanged(nameof(WindowTitle));

            // test
            if (ItemViewModel != null) {
                ItemViewModel.SelectedVectorDBItem = itemViewModel?.SelectedVectorDBItem;
            }

            AfterUpdate = afterUpdate;
        }
        // OKボタンのコマンド
        public SimpleDelegateCommand<Window> OKButtonCommand => new((window) => {
            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            // RAGSourceItemを更新
            ItemViewModel.Save();
            AfterUpdate(ItemViewModel);

            // ウィンドウを閉じる
            window.Close();
        });

        // キャンセルボタンのコマンド
        public SimpleDelegateCommand<Window> CancelButtonCommand => new((window) => {
            // ウィンドウを閉じる
            window.Close();
        });
        // UpdateIndexButtonCommand UpdateRAGIndexWindowを開く。
        public SimpleDelegateCommand<object> UpdateIndexButtonCommand => new((parameter) => {
            if (ItemViewModel == null) {
                return;
            }
            if (ItemViewModel.Item == null) {
                return;
            }
            UpdateRAGIndexWindow.OpenUpdateRAGIndexWindow(ItemViewModel, (afterUpdate) => {
                // 更新
            });
        });

        // WorkingDirectoryのチェック
        public SimpleDelegateCommand<object> CheckWorkingDirCommand => new((parameter) => {
            try {
                if (ItemViewModel == null) {
                    LogWrapper.Error("ItemViewModelがnullです");
                    return;
                }
                ItemViewModel.SourceURL = "";

                ItemViewModel.CheckWorkingDirectory();

            } catch (Exception e) {
                LogWrapper.Error(e.Message);
            }
        });

    }
}
