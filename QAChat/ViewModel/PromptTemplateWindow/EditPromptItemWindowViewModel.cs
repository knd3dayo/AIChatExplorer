using System.Windows;
using PythonAILib.Model.Abstract;
using WpfAppCommon.Utils;
using QAChat.Model;

namespace QAChat.ViewModel.PromptTemplateWindow {
    public class EditPromptItemWindowViewModel : QAChatViewModelBase {

        // 初期化
        public EditPromptItemWindowViewModel(PromptItemViewModel itemViewModel, Action<PromptItemViewModel> afterUpdate) {
            ItemViewModel = itemViewModel;
            Name = ItemViewModel.PromptItem.Name ?? "";
            Description = ItemViewModel.Description ?? "";
            Prompt = ItemViewModel.Content ?? "";
            AfterUpdate = afterUpdate;
        }

        private PromptItemViewModel? itemViewModel;
        public PromptItemViewModel? ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                OnPropertyChanged(nameof(ItemViewModel));
            }
        }
        // 名前
        public string Name {
            get => ItemViewModel?.PromptItem?.Name ?? "";
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.PromptItem.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

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
        public string Prompt {
            get => ItemViewModel?.Content ?? "";
            set {
                if (ItemViewModel == null) {
                    return;
                }
                ItemViewModel.Content = value;
                OnPropertyChanged(nameof(Prompt));
            }
        }

        private Action<PromptItemViewModel> AfterUpdate { get; set; } = (promtItem) => { };
        public TextWrapping TextWrapping {
            get {
                if (QAChatManager.Instance == null) {
                    return TextWrapping.NoWrap;
                }
                return QAChatManager.Instance.ConfigParams.GetTextWrapping();
            }
        }



        // OKボタンのコマンド
        public SimpleDelegateCommand<Window> OKButtonCommand => new((window) => {
            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            if (ItemViewModel.PromptItem == null) {
                return;
            }
            PromptItemBase promptItem = ItemViewModel.PromptItem;
            promptItem.Description = Description;
            promptItem.Prompt = Prompt;
            promptItem.Name = Name;
            // Nameが空の場合はエラーメッセージを表示
            if (string.IsNullOrEmpty(Name)) {
                LogWrapper.Error(StringResources.EnterName);
                return;
            }
            // PromptItemBaseを更新
            promptItem.Save();

            AfterUpdate(ItemViewModel);

            // ウィンドウを閉じる
            window.Close();
        });

    }
}
