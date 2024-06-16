using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using ClipboardApp.View.ClipboardItemView;
using Microsoft.WindowsAPICodePack.Dialogs;
using WK.Libraries.SharpClipboardNS;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp.View.ClipboardItemFolderView {
    public partial class ClipboardFolderViewModel {



        //--------------------------------------------------------------------------------
        //--コマンド
        //--------------------------------------------------------------------------------

        // 新規フォルダ作成コマンド
        public static SimpleDelegateCommand<ClipboardFolderViewModel> CreateFolderCommand => new((folderViewModel) => {

            CreateFolderCommandExecute(folderViewModel, () => {
                // 親フォルダを保存
                folderViewModel.Save();
                folderViewModel.Load();
            });
        });
        // フォルダ編集コマンド
        public SimpleDelegateCommand<ClipboardFolderViewModel> EditFolderCommand => new((parameter) => {

            EditFolderCommandExecute(parameter, () => {
                Load();
                Tools.Info("フォルダを編集しました");
            });
        });


        // FolderSelectWindowでFolderSelectWindowSelectFolderCommandが実行されたときの処理
        public static SimpleDelegateCommand<object> FolderSelectWindowSelectFolderCommand => new(FolderSelectWindowViewModel.FolderSelectWindowSelectFolderCommandExecute);

        // フォルダ内のアイテムをJSON形式でエクスポートする処理
        public SimpleDelegateCommand<object> ExportItemsFromFolderCommand => new(
            (parameter) => {
                ExportItemsFromFolderCommandExecute(this);
            });

        // フォルダ内のアイテムをJSON形式でインポートする処理
        public SimpleDelegateCommand<object> ImportItemsToFolderCommand => new((parameter) => {
            ImportItemsToFolderCommandExecute(this);
        });


        /// <summary>
        /// Ctrl + V が押された時の処理
        /// コピー中のアイテムを選択中のフォルダにコピー/移動する
        /// 貼り付け後にフォルダ内のアイテムを再読み込む
        /// 
        /// </summary>
        /// <param name="Instance"></param>
        /// <param name="item"></param>
        /// <param name="fromFolder"></param>
        /// <param name="toFolder"></param>
        /// <returns></returns>

        public static void PasteClipboardItemCommandExecute(bool CutFlag,
            IEnumerable<ClipboardItemViewModel> items, ClipboardFolderViewModel fromFolder, ClipboardFolderViewModel toFolder) {
            foreach (var item in items) {
                ClipboardItemViewModel newItem = item.Copy();
                toFolder.AddItem(newItem);
                // Cutフラグが立っている場合はコピー元のアイテムを削除する
                if (CutFlag) {

                    fromFolder.DeleteItem(item);
                }
            }
            // フォルダ内のアイテムを再読み込み
            toFolder.Load();
            Tools.Info("貼り付けました");
        }


        public static void MergeItemCommandExecute(
            ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems, bool mergeWithHeader) {

            if (selectedItems.Count < 2) {
                Tools.Error("マージするアイテムを2つ選択してください");
                return;
            }
            // マージ先のアイテム。SelectedItems[0]がマージ先
            if (selectedItems[0] is not ClipboardItemViewModel toItemViewModel) {
                Tools.Error("マージ先のアイテムが選択されていません");
                return;
            }
            List<ClipboardItemViewModel> fromItemsViewModel = [];
            try {
                // toItemにSelectedItems[1]からCount - 1までのアイテムをマージする
                for (int i = 1; i < selectedItems.Count; i++) {
                    if (selectedItems[i] is not ClipboardItemViewModel fromItemModelView) {
                        Tools.Error("マージ元のアイテムが選択されていません");
                        return;
                    }
                    fromItemsViewModel.Add(fromItemModelView);
                }
                toItemViewModel.MergeItems(fromItemsViewModel, mergeWithHeader, Tools.DefaultAction);

                // ClipboardItemをLiteDBに保存
                toItemViewModel.Save();
                // コピー元のアイテムを削除
                foreach (var fromItem in fromItemsViewModel) {
                    fromItem.Delete();
                }

                // フォルダ内のアイテムを再読み込み
                folderViewModel.Load();
                Tools.Info("マージしました");

            } catch (Exception e) {
                string message = $"エラーが発生しました。\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}";
                Tools.Error(message);
            }

        }

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
            // 子フォルダを作成する
            ClipboardFolderViewModel childFolderViewModel = folderViewModel.CreateChild("");
            // folderViewModelが検索フォルダの場合は、子フォルダも検索フォルダにする
            if (folderViewModel.ClipboardItemFolder.Id == ClipboardFolder.SearchRootFolder.Id
                || folderViewModel.IsSearchFolder) {
                childFolderViewModel.IsSearchFolder = true;
            }
            FolderEditWindow.OpenFolderEditWindow(childFolderViewModel, afterUpdate);

        }

        /// <summary>
        ///  フォルダ編集コマンド
        ///  フォルダ編集ウィンドウを表示する処理
        ///  フォルダ編集後に実行するコマンドが設定されている場合は、実行する.
        /// </summary>
        /// <param name="parameter"></param>
        public static void EditFolderCommandExecute(ClipboardFolderViewModel folderViewModel, Action afterUpdate) {

            FolderEditWindow.OpenFolderEditWindow(folderViewModel, afterUpdate);

        }

        // フォルダーのアイテムをエクスポートする処理
        public static void ExportItemsFromFolderCommandExecute(ClipboardFolderViewModel clipboardItemFolder) {
            DirectoryInfo directoryInfo = new("export");
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
                clipboardItemFolder.ClipboardItemFolder.ExportItemsToJson(folderPath);
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
                clipboardItemFolder.ClipboardItemFolder.ImportItemsFromJson(filaPath, (actionMessage) => {
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
        public SimpleDelegateCommand<object> DeleteFolderCommand => new((parameter) => {

            if (parameter is not ClipboardFolderViewModel folderViewModel) {
                Tools.Error("フォルダが選択されていません");
                return;
            }

            if (folderViewModel.ClipboardItemFolder.Id == ClipboardFolder.RootFolder.Id
                || folderViewModel.FolderPath == ClipboardFolder.SEARCH_ROOT_FOLDER_NAME) {
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
        });

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
