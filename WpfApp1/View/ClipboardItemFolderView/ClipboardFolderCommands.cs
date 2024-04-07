using System.Windows;
using WpfApp1.Model;
using WpfApp1.Utils;
using WpfApp1.View.SearchView;

namespace WpfApp1.View.ClipboardItemFolderView
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
            if (parameter == null)
            {
                return;
            }
            if (parameter is not ClipboardItemFolderViewModel)
            {
                return;
            }

            ClipboardItemFolderViewModel folderViewModel = (ClipboardItemFolderViewModel)parameter;
            Instance.SelectedFolder = folderViewModel;
            folderViewModel.IsSelected = true;
            // フォルダ内のアイテムを読み込む
            folderViewModel.Load();

            if (StatusText != null)
            {
                string message = $"フォルダ[{folderViewModel.DisplayName}]";
                // AutoProcessRuleが設定されている場合
                if (ClipboardDatabaseController.GetAutoProcessRules(Instance.SelectedFolder?.ClipboardItemFolder).Count() > 0)
                {
                    message += " 自動処理が設定されています[";
                    foreach (AutoProcessRule item in ClipboardDatabaseController.GetAutoProcessRules(folderViewModel.ClipboardItemFolder))
                    {
                        message += item.RuleName + " ";
                    }
                    message += "]";
                }

                // SearchConditionがEmptyではなく、AlwaysApplySearchConditionがTrueの場合
                /***
                if (folderViewModel.ClipboardItemFolder.AlwaysApplySearchCondition == true)
                {
                    message += " 検索条件を常時適用[有効]";
                }
                ***/
                // IsApplyingSearchConditionがTrueの場合
                if (folderViewModel.ClipboardItemFolder.IsApplyingSearchCondition)
                {
                    message += " 検索条件を適用中";
                }
                // SearchConditionがNullでなく、 Emptyでもない場合
                if (ClipboardItemFolder.GlobalSearchCondition != null && !ClipboardItemFolder.GlobalSearchCondition.IsEmpty())
                {
                    message += " 検索条件[";
                    message += ClipboardItemFolder.GlobalSearchCondition.ToStringSearchCondition();
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
            ClipboardItemFolderViewModel clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
            clipboardItemFolder.Load();
            MainWindowViewModel.Instance.UpdateStatusText("リロードしました");

        }
 
        // 検索ウィンドウを表示する処理
        public static void SearchCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance == null)
            {
                Tools.Error("MainWindowViewModelが取得できません");
                return;
            }
            if (MainWindowViewModel.Instance.SelectedFolder == null)
            {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardItemFolderViewModel folderViewModel = MainWindowViewModel.Instance.SelectedFolder;

            SearchWindow searchWindow = new SearchWindow();
            SearchWindowViewModel searchWindowViewModel = (SearchWindowViewModel)searchWindow.DataContext;
            // 選択されたフォルダが検索フォルダの場合
            if (folderViewModel.ClipboardItemFolder.IsSearchFolder) { 
                SearchCondition searchCondition = folderViewModel.ClipboardItemFolder.SearchCondition;
                
                searchWindowViewModel.Initialize(searchCondition, MainWindowViewModel.Instance.SelectedFolder, () =>
                {
                    folderViewModel.Load();
                });
            }

            searchWindowViewModel.Initialize(ClipboardItemFolder.GlobalSearchCondition, () =>
            {
                folderViewModel.Load();
            });

            searchWindow.ShowDialog();

        }

        // --------------------------------------------------------------
        // 2024/04/07 以下の処理はフォルダ更新後の再読み込み対応済み
        // --------------------------------------------------------------

        /// <summary>
        /// フォルダ作成コマンド
        /// フォルダ作成ウィンドウを表示する処理
        /// 新規フォルダが作成された場合は、リロード処理を行う.
        /// </summary>
        /// <param name="parameter"></param>
        public static void CreateFolderCommandExecute(object parameter)
        {
            if (parameter is not ClipboardItemFolderViewModel)
            {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            // フォルダ作成後に実行するコマンド
            void AfterUpdate()
            {
                // フォルダ構成を更新
                MainWindowViewModel.Instance?.ReloadFolder();
                Tools.Info("フォルダを作成しました");
            }
            ClipboardItemFolderViewModel folderViewModel = (ClipboardItemFolderViewModel)parameter;
            FolderEditWindow FolderEditWindow = new FolderEditWindow();
            FolderEditWindowViewModel FolderEditWindowViewModel = (FolderEditWindowViewModel)FolderEditWindow.DataContext;
            FolderEditWindowViewModel.Init(folderViewModel.ClipboardItemFolder, FolderEditWindowViewModel.Mode.CreateChild, AfterUpdate);

            FolderEditWindow.ShowDialog();

        }

        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public static void EditFolderCommandExecute(object parameter)
        {
            if (parameter is not ClipboardItemFolderViewModel)
            {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            // フォルダ編集ウィンドウを表示する処理
            void AfterUpdate()
            {
                // フォルダ構成を更新
                MainWindowViewModel.Instance?.ReloadFolder();

                Tools.Info("フォルダを編集しました");

            }
            ClipboardItemFolderViewModel folderViewModel = (ClipboardItemFolderViewModel)parameter;
            FolderEditWindow FolderEditWindow = new FolderEditWindow();
            FolderEditWindowViewModel FolderEditWindowViewModel = (FolderEditWindowViewModel)FolderEditWindow.DataContext;
            FolderEditWindowViewModel.Init(folderViewModel.ClipboardItemFolder, FolderEditWindowViewModel.Mode.Edit, AfterUpdate);

            FolderEditWindow.ShowDialog();

        }

        /// <summary>
        /// フォルダ削除コマンド
        /// フォルダを削除した後に、RootFolderをリロードする処理を行う。
        /// </summary>
        /// <param name="parameter"></param>        
        public static void DeleteFolderCommandExecute(object parameter)
        {

            if (parameter is not ClipboardItemFolderViewModel)
            {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardItemFolderViewModel folderViewModel = (ClipboardItemFolderViewModel)parameter;
            ClipboardItemFolder folder = folderViewModel.ClipboardItemFolder;

            if (folder.AbsoluteCollectionName == ClipboardDatabaseController.CLIPBOARD_ROOT_FOLDER_NAME
                || folder.AbsoluteCollectionName == ClipboardDatabaseController.SEARCH_ROOT_FOLDER_NAME )
            {
                Tools.Error("ルートフォルダは削除できません");
                return;
            }

            // フォルダ削除するかどうか確認
            if (MessageBox.Show("フォルダを削除しますか？", "確認", MessageBoxButton.YesNo) != MessageBoxResult.Yes)
            {
                return;
            }
            ClipboardDatabaseController.DeleteFolder(folder);

            // RootFolderをリロード
            MainWindowViewModel.Instance?.ReloadFolder();
            Tools.Info("フォルダを削除しました");
        }
        /// <summary>
        /// フォルダ内の表示中のアイテムを削除する処理
        /// 削除後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public static void DeleteDisplayedItemCommandExecute(object obj)
        {
            if (MainWindowViewModel.Instance?.SelectedFolder == null)
            {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show("表示中のアイテムを削除しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ClipboardItemFolderViewModel clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
                clipboardItemFolder.ClipboardItemFolder.DeleteItems();
                // フォルダ内のアイテムを読み込む
                clipboardItemFolder.Load();
                Tools.Info("表示中のアイテムを削除しました");
            }
        }
    }

}
