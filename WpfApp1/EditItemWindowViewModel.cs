using CommunityToolkit.Mvvm.ComponentModel;

namespace WpfApp1
{
    class EditItemWindowViewModel:ObservableObject
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

        private string _description ="";
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }
        private string _content ="";    
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
            if (ClipboardItem != null)
            {
                // TitleとContentの更新を反映
                ClipboardItem.Description = Description;
                ClipboardItem.Content = Content;
                // ClipboardItemを更新
                ClipboardDatabaseController.UpsertItem(ClipboardItem);

            }
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
