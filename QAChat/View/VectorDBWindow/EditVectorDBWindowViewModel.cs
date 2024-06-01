using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.View.VectorDBWindow {
    public class EditVectorDBWindowViewModel : MyWindowViewModel {

        private VectorDBItemViewModel? itemViewModel;
        public VectorDBItemViewModel? ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                OnPropertyChanged(nameof(ItemViewModel));
            }
        }
        // Name
        public string Name {
            get => ItemViewModel?.Name ?? "";
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        // Description
        public string Description {
            get => ItemViewModel?.Description ?? "";
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
        // IsEnabled
        public bool IsEnabled {
            get => ItemViewModel?.IsEnabled ?? false;
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.IsEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public string VectorDBURL {
            get => ItemViewModel?.VectorDBURL ?? "";
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.VectorDBURL = value;
                OnPropertyChanged(nameof(VectorDBURL));
            }
        }

        private Action<VectorDBItemViewModel> AfterUpdate { get; set; } = (promptItem) => { };
        // 初期化
        public void Initialize(VectorDBItemViewModel itemViewModel, Action<VectorDBItemViewModel> afterUpdate) {
            ItemViewModel = itemViewModel;

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

        // VectorDBTypeSelectionChangedCommand
        public SimpleDelegateCommand<VectorDBTypeEnum> VectorDBTypeSelectionChangedCommand => new((selectedVectorDBType) => {

            if (ItemViewModel == null) {
                return;
            }
            // 現在はFaiss,Chroma(インメモリ)のみ
            if (selectedVectorDBType != VectorDBTypeEnum.Faiss && selectedVectorDBType != VectorDBTypeEnum.Chroma) {
                Tools.Error("Faiss,Chroma(インメモリ)以外のベクトルDBタイプは現在サポートされていません");
                return;
            }

            ItemViewModel.SelectedVectorDBType = selectedVectorDBType;
        });

    }
}
