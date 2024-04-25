using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Model;
using WpfAppCommon.Utils;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon.Factory;
using WpfAppCommon;

namespace QAChat.View.PromptTemplateWindow {
    public class ListPromptTemplateWindowViewModel : ObservableObject {

        // プロンプトテンプレートの一覧
        public ObservableCollection<PromptItemViewModel> PromptItems { get; set; } = new ObservableCollection<PromptItemViewModel>();
        // 選択中の自動処理ルール
        private static PromptItemViewModel? _selectedPromptItem;
        public static PromptItemViewModel? SelectedPromptItem {
            get => _selectedPromptItem;
            set {
                _selectedPromptItem = value;
            }
        }
        private Action<PromptItemViewModel> AfterUpdate { get; set; } = (promptItemViewModel) => { };
        // 初期化
        public void Initialize(Action<PromptItemViewModel> afterUpdate) {
            // PromptItemsを更新
            Reload();
            AfterUpdate = afterUpdate;

        }

        public void Reload() {
            // PromptItemsを更新
            PromptItems.Clear();
            IClipboardDBController clipboardDBController = ClipboardAppFactory.Instance.GetClipboardDBController();
            foreach(var item in clipboardDBController.GetAllPromptTemplates()) {
                PromptItemViewModel itemViewModel = new PromptItemViewModel(item);
                PromptItems.Add(itemViewModel);
            }
            OnPropertyChanged("PromptItems");

        }

        public  SimpleDelegateCommand EditPromptItemCommand => new (EditPromptItemCommandExecute);

        // プロンプトテンプレートを編集する処理
        public  void EditPromptItemCommandExecute(object parameter) {
            if (SelectedPromptItem == null) {
                System.Windows.MessageBox.Show("プロンプトテンプレートが選択されていません。");
                return;
            }
            EditPromptItemWindow window = new EditPromptItemWindow();
            EditPromptItemWindowViewModel editPromptItemWindowViewModel = (EditPromptItemWindowViewModel)window.DataContext;
            editPromptItemWindowViewModel.Initialize(SelectedPromptItem, (PromptItemViewModel) => {
                // PromptItemsを更新
                Reload();
            });
            window.ShowDialog();
        }

        // プロンプトテンプレート処理を追加する処理
        public SimpleDelegateCommand AddPromptItemCommand => new SimpleDelegateCommand(AddPromptItemCommandExecute);

        public void AddPromptItemCommandExecute(object parameter) {
            PromptItemViewModel itemViewModel = new PromptItemViewModel(new PromptItem());
            EditPromptItemWindow window = new EditPromptItemWindow();
            EditPromptItemWindowViewModel editPromptItemWindowViewModel = (EditPromptItemWindowViewModel)window.DataContext;
            editPromptItemWindowViewModel.Initialize(itemViewModel, (PromptItemViewModel) => { 
                // PromptItemsを更新
                Reload();
            });
            window.ShowDialog();
        }
        // プロンプトテンプレートを選択する処理
        public SimpleDelegateCommand SelectPromptItemCommand => new (SelectPromptItemCommandExecute);
        public void SelectPromptItemCommandExecute(object parameter) {
            // 選択されていない場合はメッセージを表示
            if (SelectedPromptItem == null) {
                MessageBox.Show("プロンプトテンプレートが選択されていません。");
                return;
            }
            if (parameter is PromptItemViewModel itemViewModel) {
                SelectedPromptItem = itemViewModel;
            }
            AfterUpdate(SelectedPromptItem);

            // Windowを閉じる
            if (parameter is System.Windows.Window window) {
                window.Close();
            }
        }
        // プロンプトテンプレートを削除する処理
        public SimpleDelegateCommand DeletePromptItemCommand => new(DeletePromptItemCommandExecute);
        public void DeletePromptItemCommandExecute(object parameter) {
            PromptItemViewModel? itemViewModel = SelectedPromptItem;
            if (itemViewModel == null) {
                System.Windows.MessageBox.Show("プロンプトテンプレートが選択されていません。");
                return;
            }
            PromptItem? item = SelectedPromptItem?.PromptItem;
            if (item == null) {
                System.Windows.MessageBox.Show("プロンプトテンプレートが選択されていません。");
                return;
            }
            if (System.Windows.MessageBox.Show($"プロンプトテンプレート{item.Name}を削除しますか？", "確認", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes) {
                return;
            }
            PromptItems.Remove(itemViewModel);
            // LiteDBを更新
            ClipboardAppFactory.Instance.GetClipboardDBController().DeletePromptTemplate(item);
            OnPropertyChanged("PromptItems");
        }

        // CloseCommand
        public SimpleDelegateCommand CloseCommand => new ((parameter) => {
            
            if (parameter is System.Windows.Window window) {
                window.Close();
            }
        });
    }
}
