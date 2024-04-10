using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.TagView;

namespace WpfApp1.View.ClipboardItemView
{
    class EditItemWindowViewModel : ObservableObject
    {
        private ClipboardItemViewModel? itemViewModel;
        public ClipboardItemViewModel? ItemViewModel {
            get {
                return itemViewModel;
            }
            set {
                itemViewModel = value;
                TagsString = string.Join(",", itemViewModel?.ClipboardItem?.Tags ?? new List<string>());
                Description = itemViewModel?.ClipboardItem?.Description ?? "";
                Content = itemViewModel?.ClipboardItem?.Content ?? "";

                OnPropertyChanged("ClipboardItemViewModel");
            }
        }

        private string _description = "";
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }
        private string _content = "";
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                OnPropertyChanged("Content");
            }
        }

        private string title = "";
        public string Title {
            get {
                return title;
            }
            set {
                title = value;
                OnPropertyChanged("Title");
            }
        }


        //Tagを文字列に変換したもの
        private string _tagsString = "";
        public string TagsString {
            get {
                return _tagsString;
            }
            set {
                _tagsString = value;
                OnPropertyChanged("TagsString");
            }
        }

        private Action? _afterUpdate;

        public void Initialize(ClipboardItemViewModel? itemViewModel, Action afterUpdate)
        {
            if (itemViewModel == null) {
                ClipboardItem clipboardItem = new ClipboardItem();
                clipboardItem.CollectionName = MainWindowViewModel.Instance?.SelectedFolder?.AbsoluteCollectionName;
                ItemViewModel = new ClipboardItemViewModel(clipboardItem);
                title = "新規アイテム";
            } else {
                title = "アイテム編集";
                ItemViewModel = itemViewModel;
            }
            _afterUpdate = afterUpdate;
        }

        // タグ追加ボタンのコマンド
        public SimpleDelegateCommand AddTagButtonCommand => new SimpleDelegateCommand(EditTagCommandExecute);

        /// <summary>
        /// コンテキストメニューのタグをクリックしたときの処理
        /// 更新後にフォルダ内のアイテムを再読み込みする
        /// </summary>
        /// <param name="obj"></param>
        public  void EditTagCommandExecute(object obj) {

            if (ItemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            TagWindow tagWindow = new TagWindow();
            TagWindowViewModel tagWindowViewModel = (TagWindowViewModel)tagWindow.DataContext;
            tagWindowViewModel.Initialize(ItemViewModel.ClipboardItem, () => {
                // TagsStringを更新
                TagsString = string.Join(",", ItemViewModel.ClipboardItem.Tags);
            });

            tagWindow.ShowDialog();

        }


        // OKボタンのコマンド
        public SimpleDelegateCommand OKButtonCommand => new SimpleDelegateCommand(OKButtonCommandExecute);

        private void OKButtonCommandExecute(object parameter)
        {

            // TitleとContentの更新を反映
            if (ItemViewModel == null) {
                return;
            }
            ItemViewModel.ClipboardItem.Description = Description;
            ItemViewModel.ClipboardItem.Content = Content;
            // ClipboardItemを更新
            ClipboardDatabaseController.UpsertItem(ItemViewModel.ClipboardItem);
            // 更新後の処理を実行
            _afterUpdate?.Invoke();
            // ウィンドウを閉じる
            EditItemWindow.Current?.Close();
        }
        // キャンセルボタンのコマンド
        public SimpleDelegateCommand CancelButtonCommand => new SimpleDelegateCommand(CancelButtonCommandExecute);
        private void CancelButtonCommandExecute(object parameter)
        {
            // ウィンドウを閉じる
            EditItemWindow.Current?.Close();
        }

    }
}
