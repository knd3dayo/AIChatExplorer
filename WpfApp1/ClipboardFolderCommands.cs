using System.Windows;


namespace WpfApp1
{
    public class ClipboardFolderCommands
    {
        // 選択中のフォルダを表示する処理
        public static void OpenFolderCommandExecute(object parameter)
        {
            MainWindowViewModel? Instance = MainWindowViewModel.Instance;
            StatusText? StatusText = MainWindowViewModel.StatusText;

            if (Instance == null)
            {
                return;
            }
            if (parameter is ClipboardItemFolder folder)
            {
                Instance.SelectedFolder = folder;
                folder.IsSelected = true;
                // フォルダ内のアイテムを読み込む
                folder.Load();


            }
            if (StatusText != null)
            {
                string message = $"フォルダ[{Instance.SelectedFolder?.DisplayName}]";
                // AutoProcessRuleが設定されている場合
                if (ClipboardDatabaseController.GetAutoProcessRules(Instance.SelectedFolder).Count() > 0)
                {
                    message += " 自動処理が設定されています[";
                    foreach (AutoProcessRule item in ClipboardDatabaseController.GetAutoProcessRules(Instance.SelectedFolder))
                    {
                        message += item.RuleName + " ";
                    }
                    message += "]";
                }

                // SearchConditionがEmptyではなく、AlwaysApplySearchConditionがTrueの場合
                if (Instance.SelectedFolder?.AlwaysApplySearchCondition == true)
                {
                    message += " 検索条件を常時適用[有効]";
                }
                // IsApplyingSearchConditionがTrueの場合
                else if (Instance.SelectedFolder?.IsApplyingSearchCondition == true)
                {
                    message += " 検索条件を適用中";
                }
                // SearchConditionがNullでなく、 Emptyでもない場合
                if (Instance.SelectedFolder?.SearchCondition != null && !Instance.SelectedFolder.SearchCondition.IsEmpty())
                {
                    message += " 検索条件[";
                    message += Instance.SelectedFolder.SearchCondition.ToStringSearchCondition();
                    message += "]";
                }
                Instance.UpdateStatusText(message);
                StatusText.InitText = message;
            }
        }
        //フォルダを再読み込みする処理
        public static void ReloadCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance?.SelectedFolder == null)
            {
                return;
            }
            ClipboardItemFolder clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
            clipboardItemFolder.SearchCondition.Clear();
            ClipboardController.RootFolder.Load();
            MainWindowViewModel.Instance.UpdateStatusText("リロードしました");

        }
        // 選択中のフォルダ内のアイテムを削除する処理
        public static void DeleteDisplayedItemCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance?.SelectedFolder == null)
            {
                return;
            }
            //　削除確認ボタン
            MessageBoxResult result = System.Windows.MessageBox.Show("表示中のアイテムを削除しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ClipboardItemFolder clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
                clipboardItemFolder.DeleteItems();
                MainWindowViewModel.Instance.UpdateStatusText("削除しました");
            }
        }
        // 新規フォルダ作成コマンド
        public static void CreateFolderCommandExecute(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            FolderEditWindow FolderEditWindow = new FolderEditWindow();
            FolderEditWindowViewModel FolderEditWindowViewModel = ((FolderEditWindowViewModel)FolderEditWindow.DataContext);
            FolderEditWindowViewModel.Init((ClipboardItemFolder)parameter, FolderEditWindowViewModel.Mode.CreateChild);

            FolderEditWindow.ShowDialog();

        }
        // フォルダ編集コマンド
        public static void EditFolderCommandExecute(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            FolderEditWindow FolderEditWindow = new FolderEditWindow();
            FolderEditWindowViewModel FolderEditWindowViewModel = ((FolderEditWindowViewModel)FolderEditWindow.DataContext);
            FolderEditWindowViewModel.Init((ClipboardItemFolder)parameter, FolderEditWindowViewModel.Mode.Edit);

            FolderEditWindow.ShowDialog();

        }

        // 検索ウィンドウを表示する処理
        public static void SearchCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance == null)
            {
                return;
            }
            if (MainWindowViewModel.Instance.SelectedFolder == null)
            {
                return;
            }
            SearchWindow searchWindow = new SearchWindow();
            SearchWindowViewModel searchWindowViewModel = ((SearchWindowViewModel)searchWindow.DataContext);
            searchWindowViewModel.ClipboardItemFolder = MainWindowViewModel.Instance?.SelectedFolder;

            searchWindow.ShowDialog();

        }
        // フォルダ削除コマンド
        public static void DeleteFolderCommandExecute(object parameter)
        {
            if (parameter is not ClipboardItemFolder)
            {
                return;
            }
            ClipboardItemFolder folder = (ClipboardItemFolder)parameter;

            // フォルダ削除するかどうか確認
            if (System.Windows.MessageBox.Show("フォルダを削除しますか？", "確認", System.Windows.MessageBoxButton.YesNo) != System.Windows.MessageBoxResult.Yes)
            {
                return;
            }
            ClipboardDatabaseController.DeleteFolder(folder);

            // RootFolderをリロード
            ClipboardController.RootFolder.Load();

            MainWindowViewModel.Instance?.UpdateStatusText("フォルダを削除しました");
        }
    }

}
