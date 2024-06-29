using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using ClipboardApp.View.ClipboardItemView;
using ClipboardApp.View.SearchView;
using Microsoft.WindowsAPICodePack.Dialogs;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemFolderView {
    public partial class ClipboardFolderViewModel {
        //--------------------------------------------------------------------------------
        //--コマンド
        //--------------------------------------------------------------------------------

        // フォルダー保存コマンド
        public SimpleDelegateCommand<ClipboardFolderViewModel> SaveFolderCommand => new((folderViewModel) => {
            this.ClipboardItemFolder.Save();
        });

        // アイテム削除コマンド
        public SimpleDelegateCommand<ClipboardItemViewModel> DeleteItemCommand => new((ClipboardItemViewModel item) => {
            item.DeleteClipboardItemCommand.Execute();
            Items.Remove(item);

        });
        // アイテム保存コマンド
        public SimpleDelegateCommand<ClipboardItemViewModel> AddItemCommand => new((item) => {
            ClipboardItemFolder.AddItem(item.ClipboardItem);
        });


        // 新規フォルダ作成コマンド
        public SimpleDelegateCommand<ClipboardFolderViewModel> CreateFolderCommand => new((folderViewModel) => {

            CreateFolderCommandExecute(folderViewModel, () => {
                // 親フォルダを保存
                folderViewModel.ClipboardItemFolder.Save();
                folderViewModel.LoadFolderCommand.Execute();

            });
        });
        // フォルダ編集コマンド
        public SimpleDelegateCommand<ClipboardFolderViewModel> EditFolderCommand => new((folderViewModel) => {

            EditFolderCommandExecute(folderViewModel, () => {
                LoadFolderCommand.Execute(); 
                LogWrapper.Info("フォルダを編集しました");
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



        //フォルダを再読み込みする処理
        public static void ReloadCommandExecute(ClipboardFolderViewModel clipboardItemFolder) {
            clipboardItemFolder.LoadFolderCommand.Execute();
            LogWrapper.Info("リロードしました");
        }


        // --------------------------------------------------------------
        // 2024/04/07 以下の処理はフォルダ更新後の再読み込み対応済み
        // --------------------------------------------------------------


        public SimpleDelegateCommand<object> LoadFolderCommand => new((parameter) => {
            MainWindowViewModel.IsIndeterminate = true;
            try {

                LoadChildren();
                LoadItems();

                UpdateStatusText();
            } finally {
                MainWindowViewModel.IsIndeterminate = false;
            }
        });




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
                LogWrapper.Info("フォルダをエクスポートしました");
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
                        LogWrapper.Error(actionMessage.Message);
                    } else {
                        LogWrapper.Info(actionMessage.Message);
                    }
                });
                // フォルダ内のアイテムを読み込む
                clipboardItemFolder.LoadFolderCommand.Execute();
                LogWrapper.Info("フォルダをインポートしました");
            }
        }

        /// <summary>
        /// フォルダ削除コマンド
        /// フォルダを削除した後に、RootFolderをリロードする処理を行う。
        /// </summary>
        /// <param name="parameter"></param>        
        public SimpleDelegateCommand<ClipboardFolderViewModel> DeleteFolderCommand => new((folderViewModel) => {


            if (folderViewModel.ClipboardItemFolder.Id == ClipboardFolder.RootFolder.Id
                || folderViewModel.FolderPath == ClipboardFolder.SEARCH_ROOT_FOLDER_NAME) {
                LogWrapper.Error("ルートフォルダは削除できません");
                return;
            }

            // フォルダ削除するかどうか確認
            if (MessageBox.Show("フォルダを削除しますか？", "確認", MessageBoxButton.YesNo) != MessageBoxResult.Yes) {
                return;
            }
            folderViewModel.ClipboardItemFolder.Delete();

            // ルートフォルダを再読み込み
            MainWindowViewModel.ReLoadRootFolders();


            LogWrapper.Info("フォルダを削除しました");
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
                    item.DeleteClipboardItemCommand.Execute();
                }

                // フォルダ内のアイテムを読み込む
                folderViewModel.LoadFolderCommand.Execute(null);
                LogWrapper.Info("ピン留めされたアイテム以外の表示中のアイテムを削除しました");
            }
        }

        //クリップボードアイテムを開く
        public SimpleDelegateCommand<ClipboardItemViewModel> OpenItemCommand => new((itemViewModel) => {

            OpenItemCommandExecute(itemViewModel);
        });


    }

}
