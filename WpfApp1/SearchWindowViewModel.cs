namespace WpfApp1
{
    class SearchWindowViewModel
    {
        private ClipboardItemFolder? clipboardItemFolder;
        public ClipboardItemFolder? ClipboardItemFolder
        { 
            get { return clipboardItemFolder; }
            set { 
                this.clipboardItemFolder = value;
                if (clipboardItemFolder != null)
                {
                    this.SearchCondition.CopyFrom(clipboardItemFolder.SearchCondition);
                }
            } 
        }
        public SearchCondition SearchCondition { get; set; } = new SearchCondition();

        public SimpleDelegateCommand ClearCommand => new SimpleDelegateCommand(ClearCommandExecute);

        private void ClearCommandExecute(object parameter)
        {
            SearchCondition.Clear();
        }

        public SimpleDelegateCommand applyCommand => new SimpleDelegateCommand(ApplyCommandExecute);

        private void ApplyCommandExecute(object parameter)
        {
            if (ClipboardItemFolder == null)
            {
                Tools.ShowMessage("フォルダが指定されていません");
                return;
            }
            ClipboardItemFolder.Filter(SearchCondition);
            if (SearchCondition == null || SearchCondition.IsEmpty())
            {
                ClipboardItemFolder.SearchCondition.Clear();
                // Update LiteDB
                ClipboardDatabaseController.UpsertFolder(ClipboardItemFolder);
                ClipboardController.RootFolder.Load();

            }
            else
            {
                // ClipboardItemFolderに検索条件を設定
                ClipboardItemFolder.SearchCondition = SearchCondition;
                // 現在検索条件適用中フラグを立てる
                ClipboardItemFolder.IsApplyingSearchCondition = true;
                // Update LiteDB
                ClipboardDatabaseController.UpsertFolder(ClipboardItemFolder);
            }
            // Close the window
            SearchWindow.Current?.Close();
        }

    }
}
