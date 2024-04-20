using System.Data;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.SearchView;

namespace ClipboardApp.View.ClipboardItemFolderView
{
    public class ClipboardFolderCommands
    {
        // 選択中のフォルダを表示する処理
        public static void OpenFolderCommandExecute(object parameter)
        {
            MainWindowViewModel? Instance = MainWindowViewModel.Instance;

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
            // ClipboardItemFolderのLoadメソッドを呼び出す
            // Children.Item,SearchCondition,IsApplyingSearchCondition,AlwaysApplySearchConditionを更新
            folderViewModel.Load();
            UpdateStatusText(folderViewModel);
        }
        private static void UpdateStatusText(ClipboardItemFolderViewModel folderViewModel) {
            StatusText? StatusText = MainWindowViewModel.StatusText;
            if (StatusText == null) {
                return;
            }
                string message = $"フォルダ[{folderViewModel.DisplayName}]";
                // AutoProcessRuleが設定されている場合
                if (ClipboardDatabaseController.GetAutoProcessRules(folderViewModel.ClipboardItemFolder).Count() > 0)
                {
                    message += " 自動処理が設定されています[";
                    foreach (AutoProcessRule item in ClipboardDatabaseController.GetAutoProcessRules(folderViewModel.ClipboardItemFolder))
                    {
                        message += item.RuleName + " ";
                    }
                    message += "]";
                }

                // folderが検索フォルダの場合
                SearchConditionRule? searchConditionRule = ClipboardItemFolder.GlobalSearchCondition;
                if (folderViewModel.ClipboardItemFolder.IsSearchFolder) {
                    searchConditionRule = ClipboardDatabaseController.GetSearchConditionRuleByCollectionName(folderViewModel.ClipboardItemFolder.AbsoluteCollectionName);
                }
                SearchCondition? searchCondition = searchConditionRule?.SearchCondition;
                // SearchConditionがNullでなく、 Emptyでもない場合
                if (searchCondition != null)
                {
                    message += " 検索条件[";
                    message += searchCondition.ToStringSearchCondition();
                    message += "]";
                }
                StatusText.Text = message;
                StatusText.InitText = message;

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
                string absoluteCollectionName = folderViewModel.ClipboardItemFolder.AbsoluteCollectionName;
                SearchConditionRule? searchConditionRule = ClipboardDatabaseController.GetSearchConditionRuleByCollectionName(absoluteCollectionName);
                if (searchConditionRule == null) {
                    searchConditionRule = new SearchConditionRule();
                    searchConditionRule.Type = SearchConditionRule.SearchType.SearchFolder;
                    searchConditionRule.SearchFolder = folderViewModel.ClipboardItemFolder;

                }
                searchWindowViewModel.Initialize(searchConditionRule, MainWindowViewModel.Instance.SelectedFolder, () =>
                {
                    folderViewModel.Load();
                });
            } else {
                searchWindowViewModel.Initialize(ClipboardItemFolder.GlobalSearchCondition, () =>
                {
                    folderViewModel.Load();
                });
            }

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
            FolderEditWindowViewModel.Initialize(folderViewModel, FolderEditWindowViewModel.Mode.CreateChild, AfterUpdate);

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
            FolderEditWindowViewModel.Initialize(folderViewModel, FolderEditWindowViewModel.Mode.Edit, AfterUpdate);

            FolderEditWindow.ShowDialog();

        }

        // フォルダーのアイテムをエクスポートする処理
        public static void ExportItemsFromFolderCommandExecute(object obj) {
            if (MainWindowViewModel.Instance?.SelectedFolder == null) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardItemFolderViewModel clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
            ClipboardItemFolder folder = clipboardItemFolder.ClipboardItemFolder;
            DirectoryInfo directoryInfo = new DirectoryInfo("export");
            // exportフォルダが存在しない場合は作成
            if (!System.IO.Directory.Exists("export")) {
                directoryInfo =System.IO.Directory.CreateDirectory("export");
            }
            //ファイルダイアログを表示
            using var dialog = new CommonOpenFileDialog() {
                Title = "フォルダを選択してください",
                InitialDirectory = directoryInfo.FullName,
                // フォルダ選択モードにする
                IsFolderPicker = true,
            };
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            } else {
                string folderPath = dialog.FileName;
                folder.ExportItemsToJson(folderPath);
                // フォルダ内のアイテムを読み込む
                Tools.Info("フォルダをエクスポートしました");
            }
        }

        //フォルダーのアイテムをインポートする処理
        public static void ImportItemsToFolderCommandExecute(object obj) {
            if (MainWindowViewModel.Instance?.SelectedFolder == null) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardItemFolderViewModel clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
            ClipboardItemFolder folder = clipboardItemFolder.ClipboardItemFolder;

            //ファイルダイアログを表示
            using var dialog = new CommonOpenFileDialog() {
                Title = "フォルダを選択してください",
                InitialDirectory = @".",
                // フォルダ選択モードにする
                IsFolderPicker = true,
            };
            if (dialog.ShowDialog() != CommonFileDialogResult.Ok) {
                return;
            } else { 
                string filapath = dialog.FileName;
                folder.ImportItemsFromJson(filapath);
                // フォルダ内のアイテムを読み込む
                clipboardItemFolder.Load();
                Tools.Info("フォルダをインポートしました");
            }
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
            MessageBoxResult result = MessageBox.Show("ピン留めされたアイテム以外の表示中のアイテムを削除しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                ClipboardItemFolderViewModel clipboardItemFolder = MainWindowViewModel.Instance.SelectedFolder;
                foreach (ClipboardItemViewModel item in clipboardItemFolder.Items) {
                    if (item.ClipboardItem.IsPinned) {
                        continue;
                    }
                    ClipboardDatabaseController.DeleteItem(item.ClipboardItem);
                }

                // フォルダ内のアイテムを読み込む
                clipboardItemFolder.Load();
                Tools.Info("ピン留めされたアイテム以外の表示中のアイテムを削除しました");
            }
        }
    }

}
