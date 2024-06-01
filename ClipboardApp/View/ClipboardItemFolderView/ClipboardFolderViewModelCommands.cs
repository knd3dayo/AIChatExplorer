using System.IO;
using System.Windows;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.SearchView;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfAppCommon;
using WpfAppCommon.Factory.Default;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemFolderView
{
    public partial class ClipboardFolderViewModel {

        //フォルダを再読み込みする処理
        public static void ReloadCommandExecute(ClipboardFolderViewModel clipboardItemFolder) {
            clipboardItemFolder.Load();
            Tools.Info("リロードしました");
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
        public static void CreateFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {

            FolderEditWindow FolderEditWindow = new ();
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
        public static void EditFolderCommandExecute(object parameter, Action afterUpdate) {
            if (parameter is not ClipboardFolderViewModel folderViewModel) {
                Tools.Error("フォルダが選択されていません");
                return;
            }
            FolderEditWindow FolderEditWindow = new ();
            FolderEditWindowViewModel FolderEditWindowViewModel = (FolderEditWindowViewModel)FolderEditWindow.DataContext;
            FolderEditWindowViewModel.Initialize(folderViewModel, FolderEditWindowViewModel.Mode.Edit, afterUpdate);

            FolderEditWindow.ShowDialog();

        }

        // フォルダーのアイテムをエクスポートする処理
        public static void ExportItemsFromFolderCommandExecute(ClipboardFolderViewModel clipboardItemFolder) {
            DirectoryInfo directoryInfo = new ("export");
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
                clipboardItemFolder.ExportItemsToJson(folderPath);
                // フォルダ内のアイテムを読み込む
                Tools.Info("フォルダをエクスポートしました");
            }
        }

        //フォルダーのアイテムをインポートする処理
        public static void ImportItemsToFolderCommandExecute(ClipboardFolderViewModel clipboardItemFolder) {

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
                clipboardItemFolder.ImportItemsFromJson(filaPath, (actionMessage) => {
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

            if (parameter is not ClipboardFolderViewModel folderViewModel) {
                Tools.Error("フォルダが選択されていません");
                return;
            }

            if (folderViewModel.CollectionName == DefaultClipboardDBController.CLIPBOARD_ROOT_FOLDER_NAME
                || folderViewModel.CollectionName == DefaultClipboardDBController.SEARCH_ROOT_FOLDER_NAME) {
                Tools.Error("ルートフォルダは削除できません");
                return;
            }

            // フォルダ削除するかどうか確認
            if (MessageBox.Show("フォルダを削除しますか？", "確認", MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            folderViewModel.Delete();
            // フォルダ内のアイテムを読み込む
            folderViewModel.Load();

            Tools.Info("フォルダを削除しました");
        }
        /// <summary>
        /// フォルダ内の表示中のアイテムを削除する処理
        /// 削除後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public static void DeleteDisplayedItemCommandExecute(ClipboardFolderViewModel folderViewModel) {
            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show("ピン留めされたアイテム以外の表示中のアイテムを削除しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                foreach (ClipboardItemViewModel item in folderViewModel.Items) {
                    if (item.IsPinned) {
                        continue;
                    }
                    // item.ClipboardItemを削除
                    item.Delete();
                }

                // フォルダ内のアイテムを読み込む
                folderViewModel.Load();
                Tools.Info("ピン留めされたアイテム以外の表示中のアイテムを削除しました");
            }
        }
    }

}
