using CommunityToolkit.Mvvm.ComponentModel;
using WK.Libraries.SharpClipboardNS;

namespace WpfApp1
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
        public ClipboardItemFolder? clipboardItemFolder { get; set; }

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
        // OKボタンのコマンド
        public SimpleDelegateCommand OKButtonCommand => new SimpleDelegateCommand(OKButtonCommandExecute);
        private void OKButtonCommandExecute(object parameter)
        {
            // 新規アイテムを作成
            var newItem = new ClipboardItem();
            newItem.Description = Description;
            newItem.Content = Content;
            newItem.ContentType = SharpClipboard.ContentTypes.Text;
            newItem.CollectionName = clipboardItemFolder?.AbsoluteCollectionName ?? "";
            // ClipboardItemFolderに追加
            clipboardItemFolder?.UpsertItem(newItem);

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
