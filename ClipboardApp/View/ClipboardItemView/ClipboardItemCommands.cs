using System.Windows;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.TagView;
using System.IO;
using System.Collections.ObjectModel;
using WpfAppCommon.Utils;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Properties;
using WpfAppCommon.Model;
using WpfAppCommon;

namespace ClipboardApp.View.ClipboardItemView
{
    public class ClipboardItemCommands {
        /// <summary>
        /// 選択中のアイテムを削除する処理
        /// 削除後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>        
        public static void DeleteSelectedItemCommandExecute(
            ClipboardItemFolderViewModel clipboardItemFolder, IEnumerable<ClipboardItemViewModel> itemViewModels) {

            //　削除確認ボタン
            MessageBoxResult result = MessageBox.Show("選択中のアイテムを削除しますか?", "Confirmation", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes) {
                // 選択中のアイテムを削除
                foreach (var item in itemViewModels) {
                    if (item is not ClipboardItemViewModel) {
                        continue;
                    }
                    ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)item;
                    clipboardItemFolder.ClipboardItemFolder.DeleteItem(clipboardItemViewModel.ClipboardItem);
                }
                // フォルダ内のアイテムを再読み込む
                clipboardItemFolder.Load();
                Tools.Info("削除しました");
            }
        }
        public static void ChangePinCommandExecute(ClipboardItemFolderViewModel folderViewModel, IEnumerable<ClipboardItemViewModel> itemViewModels) { 
            foreach (ClipboardItemViewModel clipboardItemViewModel in itemViewModels) {
                clipboardItemViewModel.ClipboardItem.IsPinned = !clipboardItemViewModel.ClipboardItem.IsPinned;
                clipboardItemViewModel.ClipboardItem.Save();
            }

            // フォルダ内のアイテムを再読み込み
            folderViewModel.Load();

        }
        public static void OpenItemCommandExecute(ClipboardItemFolderViewModel folderViewModel ,ClipboardItemViewModel clipboardItemViewModel) {

            EditItemWindow editItemWindow = new EditItemWindow();
            EditItemWindowViewModel editItemWindowViewModel = (EditItemWindowViewModel)editItemWindow.DataContext;
            editItemWindowViewModel.Initialize(clipboardItemViewModel, () => {
                // フォルダ内のアイテムを再読み込み
                folderViewModel.Load();
                Tools.Info("更新しました");
            });

            editItemWindow.Show();
        }

        /// <summary>
        /// コンテキストメニューのタグをクリックしたときの処理
        /// 更新後にフォルダ内のアイテムを再読み込みする
        /// </summary>
        /// <param name="obj"></param>
        public static void EditTagCommandExecute(object obj) {

            if (obj is not ClipboardItemViewModel) {
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            TagWindow tagWindow = new TagWindow();
            TagWindowViewModel tagWindowViewModel = (TagWindowViewModel)tagWindow.DataContext;
            ClipboardItemViewModel clipboardItemViewModel = (ClipboardItemViewModel)obj;
            tagWindowViewModel.Initialize(clipboardItemViewModel.ClipboardItem, () => {
                // フォルダ内のアイテムを再読み込み
                MainWindowViewModel.Instance?.SelectedFolder?.Load();
                Tools.Info("更新しました");
            });

            tagWindow.ShowDialog();

        }
        /// <summary>
        /// クリップボードアイテムを新規作成する処理
        /// 作成後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public static void CreateItemCommandExecute(ClipboardItemFolderViewModel folderViewModel) {
            EditItemWindow editItemWindow = new EditItemWindow();
            EditItemWindowViewModel editItemWindowViewModel = (EditItemWindowViewModel)editItemWindow.DataContext;
            editItemWindowViewModel.Initialize(null, () => {
                // フォルダ内のアイテムを再読み込み
                folderViewModel?.Load();
                Tools.Info("追加しました");
            });

            editItemWindow.Show();
        }


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
            IEnumerable<ClipboardItemViewModel> items, ClipboardItemFolderViewModel fromFolder, ClipboardItemFolderViewModel toFolder) {
            foreach (var item in items) {
                ClipboardItem newItem = item.ClipboardItem.Copy();
                toFolder.ClipboardItemFolder.AddItem(newItem, (actionMessage) => { });
                // Cutフラグが立っている場合はコピー元のアイテムを削除する
                if (CutFlag) {

                    fromFolder.ClipboardItemFolder.DeleteItem(item.ClipboardItem);
                }
            }
            // フォルダ内のアイテムを再読み込み
            toFolder.Load();
            Tools.Info("貼り付けました");
        }

        public static void MergeItemCommandExecute(
            ClipboardItemFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems,  bool mergeWithHeader) {

            if (selectedItems.Count < 2) {
                Tools.Error("マージするアイテムを2つ選択してください");
                return;
            }
            // マージ先のアイテム。SelectedItems[0]がマージ先
            if (selectedItems[0] is not ClipboardItemViewModel toItemModelView) {
                Tools.Error("マージ先のアイテムが選択されていません");
                return;
            }
            ClipboardItem toItem = toItemModelView.ClipboardItem;
            List<ClipboardItem> fromItems = new List<ClipboardItem>();
            try {
                // toItemにSelectedItems[1]からCount - 1までのアイテムをマージする
                for (int i = 1; i < selectedItems.Count; i++) {
                    if (selectedItems[i] is not ClipboardItemViewModel fromItemModelView) {
                        Tools.Error("マージ元のアイテムが選択されていません");
                        return;
                    }
                    fromItems.Add(fromItemModelView.ClipboardItem);
                }
                toItem.MergeItems(fromItems, mergeWithHeader, Tools.DefaultAction);

                // ClipboardItemをLiteDBに保存
                toItem.Save();
                // コピー元のアイテムを削除
                foreach (var fromItem in fromItems) {
                    fromItem.Delete();
                }

                // フォルダ内のアイテムを再読み込み
                folderViewModel.Load();
                Tools.Info("マージしました");

            } catch (Exception e) {
                string message = string.Format("エラーが発生しました。\nメッセージ:\n{0]\nスタックトレース:\n[1]", e.Message, e.StackTrace);
                Tools.Error(message);
            }

        }

        // 選択中のアイテムを開く処理
        public static void OpenSelectedItemAsFileCommandExecute(ClipboardItemViewModel itemViewModel) {
            try {
                // 選択中のアイテムを開く
                ClipboardAppFactory.Instance.GetClipboardProcessController().OpenItem(itemViewModel.ClipboardItem);

            } catch (ClipboardAppException e) {
                Tools.Error(e.Message);
            }

        }
        // 選択中のアイテムを新規として開く処理
        public static void OpenSelectedItemAsNewFileCommandExecute(ClipboardItemViewModel itemViewModel) {
            try {
                // 選択中のアイテムを新規として開く
                ClipboardAppFactory.Instance.GetClipboardProcessController().OpenItem(itemViewModel.ClipboardItem, true);
            } catch (ClipboardAppException e) {
                Tools.Error(e.Message);
            }

        }
        // コンテキストメニューの「テキストを抽出」をクリックしたときの処理
        public static void MenuItemExtractTextCommandExecute(object obj) {
            // 対話処理のため、エラー時はダイアログを表示
            if (MainWindowViewModel.Instance == null) {
                Tools.Error("MainWindowViewModelがありません");
                return;
            }
            if (MainWindowViewModel.Instance.SelectedItem == null) {
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            // File以外の場合はエラー
            if (MainWindowViewModel.Instance.SelectedItem.ClipboardItem.ContentType != ClipboardContentTypes.Files) {
                Tools.Error("ファイル以外のコンテンツはテキストを抽出できません");
                return;
            }
            ClipboardItem clipboardItem = MainWindowViewModel.Instance.SelectedItem.ClipboardItem;
            ClipboardItem.ExtractTextCommandExecute(clipboardItem);
            // 保存
            clipboardItem.Save();

            // フォルダ内のアイテムを再読み込み
            MainWindowViewModel.Instance?.SelectedFolder?.Load();

        }


        // コンテキストメニューの「データをマスキング」をクリックしたときの処理
        public static void MenuItemMaskDataCommandExecute(object obj) {
            if (MainWindowViewModel.Instance == null) {
                // 対話処理のため、エラー時はダイアログを表示
                Tools.Error("MainWindowViewModelがありません");
                return;
            }
            if (MainWindowViewModel.Instance.SelectedItem == null) {
                // 対話処理のため、エラー時はダイアログを表示
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            // テキスト以外の場合はエラー
            if (MainWindowViewModel.Instance.SelectedItem.ClipboardItem.ContentType != ClipboardContentTypes.Text) {
                // 対話処理のため、エラー時はダイアログを表示
                Tools.Error("テキスト以外のコンテンツはマスキングできません");
                return;
            }
            ClipboardItem clipboardItem = MainWindowViewModel.Instance.SelectedItem.ClipboardItem;
            ClipboardItem.MaskDataCommandExecute(clipboardItem);
            // 保存
            clipboardItem.Save();

            // フォルダ内のアイテムを再読み込み
            MainWindowViewModel.Instance?.SelectedFolder?.Load();

        }

        // メニューの「Pythonスクリプトを実行」をクリックしたときの処理
        public static void MenuItemRunPythonScriptCommandExecute(object obj) {
            MainWindowViewModel? Instance = MainWindowViewModel.Instance;

            if (Instance == null) {
                throw new ClipboardAppException("MainWindowViewModelがありません");
            }
            if (Instance.SelectedItem == null) {
                throw new ClipboardAppException("SelectedItemがありません");
            }

            if (obj is not ScriptItem) {
                throw new ClipboardAppException("ScriptItemがありません");
            }
            ScriptItem scriptItem = (ScriptItem)obj;
            ClipboardItem clipboardItem = Instance.SelectedItem.ClipboardItem;
            try {
                // clipboardItemをJsonに変換
                string clipboardItemJson = ClipboardItem.ToJson(clipboardItem);
                // Pythonスクリプトを実行
                string result = PythonExecutor.PythonFunctions.RunScript(scriptItem.Content, clipboardItemJson);
                // JsonからClipboardItemに変換
                ClipboardItem? resultItem = ClipboardItem.FromJson(result, Tools.DefaultAction);
                
                // 保存
                clipboardItem.Save();

                // フォルダ内のアイテムを再読み込み
                MainWindowViewModel.Instance?.SelectedFolder?.Load();

            } catch (ClipboardAppException e) {
                Tools.Error(e.Message);
            }

        }
        // OpenAI Chatを開くコマンド
        public static void OpenAIChatCommandExecute(ClipboardItemViewModel? itemViewModel) {

            QAChat.MainWindow openAIChatWindow = new QAChat.MainWindow();
            QAChat.MainWindowViewModel mainWindowViewModel = (QAChat.MainWindowViewModel)openAIChatWindow.DataContext;
            // 外部プロジェクトとして設定
            mainWindowViewModel.IsInternalProject = false;
            // InputTextに選択中のアイテムのContentを設定
            if (itemViewModel != null) {
                mainWindowViewModel.InputText = itemViewModel.ClipboardItem.Content;
            }
            openAIChatWindow.ShowDialog();
        }

    }

}
