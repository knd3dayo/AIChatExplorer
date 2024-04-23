using System.IO;
using System.Windows;
using ClipboardApp.Factory.Default;
using ClipboardApp.Model;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.SearchView;
using Microsoft.WindowsAPICodePack.Dialogs;
using WK.Libraries.SharpClipboardNS;

namespace ClipboardApp.View.ClipboardItemFolderView
{
    public class ClipboardFolderCommands {

        //フォルダを再読み込みする処理
        public static void ReloadCommandExecute(ClipboardItemFolderViewModel clipboardItemFolder) {
            clipboardItemFolder.Load();
            Tools.Info("リロードしました");
        }

        // 検索ウィンドウを表示する処理
        public static void SearchCommandExecute(ClipboardItemFolderViewModel? folderViewModel) {
            
            SearchWindow searchWindow = new SearchWindow();
            SearchWindowViewModel searchWindowViewModel = (SearchWindowViewModel)searchWindow.DataContext;
            // 選択されたフォルダが検索フォルダの場合
            if (folderViewModel != null && folderViewModel.ClipboardItemFolder.IsSearchFolder) {
                string absoluteCollectionName = folderViewModel.ClipboardItemFolder.AbsoluteCollectionName;
                SearchRule? searchConditionRule = SearchRuleController.GetSearchRuleByFolderName(absoluteCollectionName);
                if (searchConditionRule == null) {
                    searchConditionRule = new SearchRule();
                    searchConditionRule.Type = SearchRule.SearchType.SearchFolder;
                    searchConditionRule.SearchFolder = folderViewModel.ClipboardItemFolder;

                }
                searchWindowViewModel.Initialize(searchConditionRule, folderViewModel, () => {
                    folderViewModel.Load();
                });
            } else {
                searchWindowViewModel.Initialize(ClipboardItemFolder.GlobalSearchCondition, () => {
                    folderViewModel?.Load();
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
        public static void CreateFolderCommandExecute(ClipboardItemFolderViewModel folderViewModel, Action afterUpdate) {

            FolderEditWindow FolderEditWindow = new FolderEditWindow();
            FolderEditWindowViewModel FolderEditWindowViewModel = (FolderEditWindowViewModel)FolderEditWindow.DataContext;
            FolderEditWindowViewModel.Initialize(folderViewModel, FolderEditWindowViewModel.Mode.CreateChild, afterUpdate);

            FolderEditWindow.ShowDialog();

        }

        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public static void EditFolderCommandExecute(object parameter) {
            if (parameter is not ClipboardItemFolderViewModel) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            // フォルダ編集ウィンドウを表示する処理
            void AfterUpdate() {
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
                directoryInfo = System.IO.Directory.CreateDirectory("export");
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
                string filaPath = dialog.FileName;
                folder.ImportItemsFromJson(filaPath, (actionMessage) => {
                    if (actionMessage.MessageType == ActionMessage.MessageTypes.Error) {
                        Tools.Error(actionMessage.Message);
                    } else {
                        Tools.Info(actionMessage.Message);
                    }
                });
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
        public static void DeleteFolderCommandExecute(object parameter) {

            if (parameter is not ClipboardItemFolderViewModel) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            ClipboardItemFolderViewModel folderViewModel = (ClipboardItemFolderViewModel)parameter;
            ClipboardItemFolder folder = folderViewModel.ClipboardItemFolder;

            if (folder.AbsoluteCollectionName == DefaultClipboardDBController.CLIPBOARD_ROOT_FOLDER_NAME
                || folder.AbsoluteCollectionName == DefaultClipboardDBController.SEARCH_ROOT_FOLDER_NAME) {
                Tools.Error("ルートフォルダは削除できません");
                return;
            }

            // フォルダ削除するかどうか確認
            if (MessageBox.Show("フォルダを削除しますか？", "確認", MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            ClipboardAppFactory.Instance.GetClipboardDBController().DeleteFolder(folder);

            // RootFolderをリロード
            MainWindowViewModel.Instance?.ReloadFolder();
            Tools.Info("フォルダを削除しました");
        }
        /// <summary>
        /// フォルダ内の表示中のアイテムを削除する処理
        /// 削除後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public static void DeleteDisplayedItemCommandExecute(ClipboardItemFolderViewModel folderViewModel) {
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show("ピン留めされたアイテム以外の表示中のアイテムを削除しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                foreach (ClipboardItemViewModel item in folderViewModel.Items) {
                    if (item.ClipboardItem.IsPinned) {
                        continue;
                    }
                    // item.ClipboardItemを削除
                    item.ClipboardItem.Delete();
                }

                // フォルダ内のアイテムを読み込む
                folderViewModel.Load();
                Tools.Info("ピン留めされたアイテム以外の表示中のアイテムを削除しました");
            }
        }
        // 指定されたフォルダの全アイテムをマージするコマンド
        public static void MergeItemsCommandExecute(ClipboardItemFolder folder, ClipboardItem item) {
            if (folder.Items.Count == 0) {
                return;
            }

            // マージ元のアイテム
            List<ClipboardItem> mergedFromItems = new List<ClipboardItem>();
            for (int i = folder.Items.Count - 1; i > 0; i--) {
                // TypeがTextのアイテムのみマージ
                if (folder.Items[i].ContentType == ClipboardContentTypes.Text) {
                    mergedFromItems.Add(folder.Items[i]);
                }
            }
            // 先頭に引数のアイテムを追加
            mergedFromItems.Insert(0, item);
            // mergeToItemを取得(更新時間が一番古いアイテム)
            ClipboardItem mergeToItem = mergedFromItems.Last();
            // mergedFromItemsからmergeToItemを削除
            mergedFromItems.RemoveAt(mergedFromItems.Count - 1);

            // マージ元のアイテムをマージ先のアイテムにマージ
            mergeToItem.MergeItems(mergedFromItems, false, Tools.DefaultAction);

            // マージ先アイテムをnewItemにコピー
            mergeToItem.CopyTo(item);

            // マージしたアイテムを削除
            foreach (var mergedItem in mergedFromItems) {
                folder.DeleteItem(mergedItem);
            }
            // マージ先アイテムを削除
            folder.DeleteItem(mergeToItem);

        }
        // 指定されたフォルダの中のSourceApplicationTitleが一致するアイテムをマージするコマンド
        public static void MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItemFolder folder, ClipboardItem newItem) {
            // NewItemのSourceApplicationTitleが空の場合は何もしない
            if (string.IsNullOrEmpty(newItem.SourceApplicationTitle)) {
                return;
            }
            if (folder.Items.Count == 0) {
                return;
            }
            List<ClipboardItem> sameTitleItems = new List<ClipboardItem>();
            // マージ先のアイテムのうち、SourceApplicationTitleが一致するアイテムを取得
            foreach (var item in folder.Items) {
                if (newItem.SourceApplicationTitle == item.SourceApplicationTitle) {
                    // TypeがTextのアイテムのみマージ
                    if (item.ContentType == ClipboardContentTypes.Text) {
                        sameTitleItems.Add(item);
                    }
                }
            }
            // mergeFromItemsが空の場合はnewItemをそのまま返す。
            if (sameTitleItems.Count == 0) {
                return;
            }
            // マージ元のアイテムをマージ先(更新時間が一番古いもの)のアイテムにマージ
            ClipboardItem mergeToItem = folder.Items.Last();
            // sameTitleItemsの1から最後までをマージ元のアイテムとする
            sameTitleItems.RemoveAt(sameTitleItems.Count - 1);

            // sameTitleItemsにnewItemを追加
            sameTitleItems.Insert(0, newItem);
            // マージ元のアイテムをマージ先のアイテムにマージ

            mergeToItem.MergeItems(sameTitleItems, false, Tools.DefaultAction);
            // newItemにマージしたアイテムをコピー
            mergeToItem.CopyTo(newItem);
            // マージしたアイテムを削除
            foreach (var mergedItem in sameTitleItems) {
                folder.DeleteItem(mergedItem);
            }
            // mergedItemを削除
            folder.DeleteItem(mergeToItem);
        }

    }

}
