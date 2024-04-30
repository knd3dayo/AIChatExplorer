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
        public string SourceURL {
            get => ItemViewModel?.Item.SourceURL ?? "";
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.Item.SourceURL = value;
                OnPropertyChanged(nameof(SourceURL));
            }
        }
        // 最後にインデックス化したコミットの情報
        public string LastIndexedCommitInfo {
            get {
                return ItemViewModel?.LastIndexedCommitInfo ?? "";
            }
        }

        public string WorkingDirectory {
            get => ItemViewModel?.Item.WorkingDirectory ?? "";
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.Item.WorkingDirectory = value;
                OnPropertyChanged(nameof(WorkingDirectory));
                OnPropertyChanged(nameof(SourceURL));
                OnPropertyChanged(nameof(LastIndexedCommitInfo));
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
            AfterUpdate = afterUpdate;
            SourceURL = itemViewModel.Item.SourceURL;

            // Windowのタイトルを更新
            OnPropertyChanged(nameof(WindowTitle));
            // WorkingDirectoryが指定されている場合
            if (!string.IsNullOrEmpty(itemViewModel.Item.WorkingDirectory)) {
                WorkingDirectory = itemViewModel.Item.WorkingDirectory;
            }

            AfterUpdate = afterUpdate;
        }
        // OKボタンのコマンド
        public SimpleDelegateCommand OKButtonCommand => new((parameter) => {
            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            if (ItemViewModel.Item == null) {
                return;
            }
            // RAGSourceItemを更新
            ItemViewModel.Item.Save();


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
                // SourceURLをクリア
                SourceURL = "";

                ItemViewModel.Item.CheckWorkingDirectory(WorkingDirectory);
                // SourceURLを更新
                OnPropertyChanged(nameof(SourceURL));
                // LastIndexedCommitInfoを更新
                OnPropertyChanged(nameof(LastIndexedCommitInfo));

            } catch (Exception e) {
                Tools.Error(e.Message);
            }
        });

    }
}
