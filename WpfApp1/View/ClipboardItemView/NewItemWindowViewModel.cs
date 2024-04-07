using CommunityToolkit.Mvvm.ComponentModel;
using WK.Libraries.SharpClipboardNS;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.ClipboardItemFolderView;

namespace WpfApp1.View.ClipboardItemView
{
    class NewItemWindowViewModel : ObservableObject
    {
        private ClipboardItem? clipboardItem;
        public ClipboardItem? ClipboardItem
        {
            get
            {
                return clipboardItem;
            }
            set
            {
                clipboardItem = value;
                Description = clipboardItem?.Description ?? "";
                Content = clipboardItem?.Content ?? "";

                OnPropertyChanged("ClipboardItem");
            }
        }
        public ClipboardItemFolderViewModel? clipboardItemFolder { get; set; }

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
        private Action? _afterUpdate;
        public void Initialize(ClipboardItemFolderViewModel clipboardItemFolder, Action afterUpdate)
        {
            this.clipboardItemFolder = clipboardItemFolder;
            _afterUpdate = afterUpdate;
        }
        // OKボタンのコマンド
        public SimpleDelegateCommand OKButtonCommand => new SimpleDelegateCommand(OKButtonCommandExecute);
        private void OKButtonCommandExecute(object parameter)
        {
            // 新規アイテムを作成
            var newItem = new ClipboardItem();
            newItem.Description = Description;
            newItem.Content = Content;
            newItem.ContentType = SharpClipboard.ContentTypes.Text;
            newItem.CollectionName = clipboardItemFolder?.ClipboardItemFolder.AbsoluteCollectionName ?? "";
            // ClipboardItemFolderに追加
            clipboardItemFolder?.ClipboardItemFolder.UpsertItem(newItem);
            // 追加後の処理を実行
            _afterUpdate?.Invoke();

            // ウィンドウを閉じる
            NewItemWindow.Current?.Close();
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
