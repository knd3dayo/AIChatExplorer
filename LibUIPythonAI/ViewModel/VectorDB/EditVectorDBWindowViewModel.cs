using System.Windows;
using System.Windows.Controls;
using PythonAILib.Model.VectorDB;
using WpfAppCommon.Utils;

namespace LibUIPythonAI.ViewModel.VectorDB {
    public class EditVectorDBWindowViewModel : ChatViewModelBase {

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
        // VectorDBURLのヒントテキスト
        public string VectorDBURLHintText {
            get {
                if (ItemViewModel == null) {
                    return "";
                }
                if (ItemViewModel.SelectedVectorDBType == VectorDBTypeEnum.Chroma) {
                    return StringResources.ExampleVectorDBLocationChroma;
                }
                if (ItemViewModel.SelectedVectorDBType == VectorDBTypeEnum.PGVector) {
                    return StringResources.ExampleVectorDBLocationPostgres;
                }
                return "";
            }
        }


        private Action<VectorDBItemViewModel> AfterUpdate { get; set; } = (promptItem) => { };
        // 初期化
        public EditVectorDBWindowViewModel(VectorDBItemViewModel itemViewModel, Action<VectorDBItemViewModel> afterUpdate) {
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
            ItemViewModel.Item.Save();

            AfterUpdate(ItemViewModel);

            // ウィンドウを閉じる
            window.Close();
        });

        // VectorDBTypeSelectionChangedCommand
        public SimpleDelegateCommand<RoutedEventArgs> VectorDBTypeSelectionChangedCommand => new((routedEventArgs) => {
            if (ItemViewModel == null) {
                return;
            }

            ComboBox comboBox = (ComboBox)routedEventArgs.OriginalSource;
            // 選択中のアイテムを取得
            VectorDBTypeEnum selectedItem = (VectorDBTypeEnum)comboBox.SelectedItem;
            // 現在はChroma(インメモリ)とPGVectorのみ対応
            if (selectedItem == VectorDBTypeEnum.Chroma) {
                ItemViewModel.VectorDBURL = "";
                OnPropertyChanged(nameof(VectorDBURLHintText));
                OnPropertyChanged(nameof(ItemViewModel));
                return;
            }
            if (selectedItem == VectorDBTypeEnum.PGVector) {
                ItemViewModel.VectorDBURL = "";
                OnPropertyChanged(nameof(VectorDBURLHintText));
                OnPropertyChanged(nameof(ItemViewModel));
                return;
            }

            LogWrapper.Error(StringResources.OnlyChromaInMemoryVectorDBTypeIsCurrentlySupported);

        });

    }
}
