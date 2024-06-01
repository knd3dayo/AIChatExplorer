using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Model;
using WpfAppCommon;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.View.PromptTemplateWindow {
    public class EditPromptItemWindowViewModel : MyWindowViewModel {
            
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
        // Windowのタイトル　ItemViewModelがnullの場合は新規作成、それ以外は編集
        public string WindowTitle {
            get {
                return itemViewModel == null ? "新規作成" : "編集"; ;
            }
        }
            
        private Action<PromptItemViewModel> AfterUpdate { get; set; } = (promtItem) => { };
        // 初期化
        public void Initialize(PromptItemViewModel? itemViewModel, Action<PromptItemViewModel> afterUpdate) {
            if (itemViewModel != null) {
                ItemViewModel = itemViewModel;
                Name = ItemViewModel.PromptItem.Name ?? "";
                Description = ItemViewModel.Description ?? "";
                Prompt = ItemViewModel.Content ?? "";
                // Windowのタイトルを更新
                OnPropertyChanged(nameof(WindowTitle));
            } else {
                ItemViewModel = new PromptItemViewModel(new PromptItem());

            }
            AfterUpdate = afterUpdate;
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
            PromptItem promptItem = ItemViewModel.PromptItem;
            promptItem.Description = Description;
            promptItem.Prompt = Prompt;
            promptItem.Name = Name;
            // Nameが空の場合はエラーメッセージを表示
            if (string.IsNullOrEmpty(Name)) {
                MessageBox.Show("名前を入力してください。");
                return;
            }
            // ClipboardItemを更新
            ClipboardAppFactory.Instance.GetClipboardDBController().UpsertPromptTemplate(promptItem);

            AfterUpdate(ItemViewModel);

            // ウィンドウを閉じる
            window.Close();
        });
        // キャンセルボタンのコマンド
        public SimpleDelegateCommand<Window> CancelButtonCommand => new((window) => {
            // ウィンドウを閉じる
            window.Close();
        });

    }
}
