using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model;
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

        // MultiVectorRetrieverを使用するか否か
        public bool IsUseMultiVectorRetriever {
            get {
                if (ItemViewModel == null) {
                    return false;
                }
                return ItemViewModel.IsUseMultiVectorRetriever;
            }
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.IsUseMultiVectorRetriever = value;
                OnPropertyChanged(nameof(IsUseMultiVectorRetriever));
                OnPropertyChanged(nameof(DocStoreURLVisibility));
            }
        }
        // DocStoreURLを表示するか否かのVisibility
        public Visibility DocStoreURLVisibility {
            get {
                if (ItemViewModel == null) {
                    return Visibility.Collapsed;
                }
                if (ItemViewModel.IsUseMultiVectorRetriever) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
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
                LogWrapper.Error("Faiss,Chroma(インメモリ)以外のベクトルDBタイプは現在サポートされていません");
                return;
            }

            ItemViewModel.SelectedVectorDBType = selectedVectorDBType;
        });

    }
}
