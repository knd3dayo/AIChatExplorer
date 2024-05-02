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
        public SimpleDelegateCommand OKButtonCommand => new((parameter) => {
            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            // RAGSourceItemを更新
            ItemViewModel.Save();
            AfterUpdate(ItemViewModel);

            if (parameter is not Window window) {
                return;
            }
            // ウィンドウを閉じる
            window.Close();
        });

        // キャンセルボタンのコマンド
        public SimpleDelegateCommand CancelButtonCommand => new((parameter) => {
            if (parameter is not Window window) {
                return;
            }
            // ウィンドウを閉じる
            window.Close();
        });
        // UpdateIndexButtonCommand UpdateRAGIndexWindowを開く。
        public SimpleDelegateCommand UpdateIndexButtonCommand => new((parameter) => {
            if (ItemViewModel == null) {
                return;
            }
            if (ItemViewModel.Item == null) {
                return;
            }
            UpdateRAGIndexWindow updateRAGIndexWindow = new();
            UpdateRAGIndexWindowViewModel viewModel = (UpdateRAGIndexWindowViewModel)updateRAGIndexWindow.DataContext;
            viewModel.Initialize(ItemViewModel, (afterUpdate) => {
                // 更新
            });
            updateRAGIndexWindow.ShowDialog();
        });

        // WorkingDirectoryのチェック
        public SimpleDelegateCommand CheckWorkingDirCommand => new((parameter) => {
            try {
                if (ItemViewModel == null) {
                    Tools.Error("ItemViewModelがnullです");
                    return;
                }
                ItemViewModel.SourceURL = "";

                ItemViewModel.CheckWorkingDirectory();

            } catch (Exception e) {
                Tools.Error(e.Message);
            }
        });

    }
}
