using System.Collections.ObjectModel;
using System.Windows;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.TagView;
using QAChat.Model;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.ClipboardItemView {
    public class ClipboardItemCommands {
        /// <summary>
        /// 選択中のアイテムを削除する処理
        /// 削除後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>        
        public static void DeleteSelectedItemCommandExecute(
            ClipboardFolderViewModel clipboardItemFolder, IEnumerable<ClipboardItemViewModel> itemViewModels) {

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
        public static void ChangePinCommandExecute(ClipboardFolderViewModel folderViewModel, IEnumerable<ClipboardItemViewModel> itemViewModels) {
            foreach (ClipboardItemViewModel clipboardItemViewModel in itemViewModels) {
                clipboardItemViewModel.ClipboardItem.IsPinned = !clipboardItemViewModel.ClipboardItem.IsPinned;
                // ピン留めの時は更新日時を変更しない
                clipboardItemViewModel.ClipboardItem.Save(false);
            }

            // フォルダ内のアイテムを再読み込み
            folderViewModel.Load();

        }
        public static void OpenItemCommandExecute(ClipboardFolderViewModel folderViewModel, ClipboardItemViewModel clipboardItemViewModel) {

            EditItemWindow editItemWindow = new EditItemWindow();
            EditItemWindowViewModel editItemWindowViewModel = (EditItemWindowViewModel)editItemWindow.DataContext;
            editItemWindowViewModel.Initialize(folderViewModel, clipboardItemViewModel, () => {
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

            if (obj is not ClipboardItemViewModel clipboardItemViewModel) {
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            TagWindow tagWindow = new TagWindow();
            TagWindowViewModel tagWindowViewModel = (TagWindowViewModel)tagWindow.DataContext;
            tagWindowViewModel.Initialize(clipboardItemViewModel.ClipboardItem, () => {
                Tools.Info("更新しました");
            });

            tagWindow.ShowDialog();

        }
        /// <summary>
        /// クリップボードアイテムを新規作成する処理
        /// 作成後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public static void CreateItemCommandExecute(ClipboardFolderViewModel folderViewModel) {
            EditItemWindow editItemWindow = new EditItemWindow();
            EditItemWindowViewModel editItemWindowViewModel = (EditItemWindowViewModel)editItemWindow.DataContext;
            editItemWindowViewModel.Initialize(folderViewModel, null, () => {
                // フォルダ内のアイテムを再読み込み
                folderViewModel.Load();
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
            IEnumerable<ClipboardItemViewModel> items, ClipboardFolderViewModel fromFolder, ClipboardFolderViewModel toFolder) {
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
            ClipboardFolderViewModel folderViewModel, Collection<ClipboardItemViewModel> selectedItems, bool mergeWithHeader) {

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
            if (obj is not ClipboardItemViewModel clipboardItemViewModel) {
                // 対話処理のため、エラー時はダイアログを表示
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            // File以外の場合はエラー
            if (clipboardItemViewModel.ClipboardItem.ContentType != ClipboardContentTypes.Files) {
                Tools.Error("ファイル以外のコンテンツはテキストを抽出できません");
                return;
            }
            ClipboardItem.ExtractTextCommandExecute(clipboardItemViewModel.ClipboardItem);
            // 保存
            clipboardItemViewModel.ClipboardItem.Save();

            // フォルダ内のアイテムを再読み込み
            clipboardItemViewModel.FolderViewModel.Load();

        }


        // コンテキストメニューの「データをマスキング」をクリックしたときの処理
        public static void MenuItemMaskDataCommandExecute(object obj) {
            if (obj is not ClipboardItemViewModel clipboardItemViewModel) {
                // 対話処理のため、エラー時はダイアログを表示
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            // テキスト以外の場合はエラー
            if (clipboardItemViewModel.ClipboardItem.ContentType != ClipboardContentTypes.Text) {
                // 対話処理のため、エラー時はダイアログを表示
                Tools.Error("テキスト以外のコンテンツはマスキングできません");
                return;
            }
            ClipboardItem.MaskDataCommandExecute(clipboardItemViewModel.ClipboardItem);
            // 保存
            clipboardItemViewModel.ClipboardItem.Save();
            clipboardItemViewModel.FolderViewModel.Load();

        }

        // メニューの「Pythonスクリプトを実行」をクリックしたときの処理
        public static void MenuItemRunPythonScriptCommandExecute(ScriptItem scriptItem, ClipboardItemViewModel itemViewModel) {
            try {
                // clipboardItemをJsonに変換
                string clipboardItemJson = ClipboardItem.ToJson(itemViewModel.ClipboardItem);
                // Pythonスクリプトを実行
                string result = PythonExecutor.PythonFunctions.RunScript(scriptItem.Content, clipboardItemJson);
                // JsonからClipboardItemに変換
                ClipboardItem? resultItem = ClipboardItem.FromJson(result, Tools.DefaultAction);

                // 保存
                itemViewModel.ClipboardItem.Save();

                // フォルダ内のアイテムを再読み込み
                itemViewModel.FolderViewModel.Load();

            } catch (ClipboardAppException e) {
                Tools.Error(e.Message);
            }

        }
        // OpenAI Chatを開くコマンド
        public static void OpenOpenAIChatWindowExecute(ClipboardItemViewModel? itemViewModel) {

            QAChat.MainWindow openAIChatWindow = new();
            QAChat.MainWindowViewModel mainWindowViewModel = (QAChat.MainWindowViewModel)openAIChatWindow.DataContext;
            // 外部プロジェクトとして設定
            mainWindowViewModel.IsInternalProject = false;
            // InputTextに選択中のアイテムのContentを設定
            if (itemViewModel != null) {
                mainWindowViewModel.InputText = itemViewModel.ClipboardItem.Content;
            }
            openAIChatWindow.ShowDialog();
        }

        // プロンプトテンプレート一覧を開いて選択したプロンプトテンプレートを実行するコマンド
        public static void OpenAIChatCommandExecute(ClipboardItemViewModel? itemViewModel) {
            // itemViewModelがnullの場合はエラー
            if (itemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            ListPromptTemplateWindow promptTemplateWindow = new();
            ListPromptTemplateWindowViewModel promptTemplateWindowViewModel = (ListPromptTemplateWindowViewModel)promptTemplateWindow.DataContext;
            promptTemplateWindowViewModel.InitializeExec((PromptItemViewModel promptItemViewModel, OpenAIExecutionModeEnum mode) => {
                // OpenAIChatを実行
                OpenAIChatCommandExecute(mode, itemViewModel);
            });
            promptTemplateWindow.ShowDialog();
        }
        // OpenAI Chatを実行してその結果をClipboardItemに設定するコマンド
        public static async void OpenAIChatCommandExecute(OpenAIExecutionModeEnum mode, ClipboardItemViewModel itemViewModel) {
            try {
                // プログレスインジケーターを表示
                MainWindowViewModel.UpdateProgressCircleVisibility(true);

                List<ChatItem> chatItems = new();
                ChatResult result = new();
                // modeがRAGの場合はLangChainChatを実行
                if (mode == OpenAIExecutionModeEnum.RAG) {
                    // LangChainChatを実行
                    await Task.Run(() => {
                        result = PythonExecutor.PythonFunctions.LangChainChat(itemViewModel.ClipboardItem.Content, chatItems);
                    });
                }
                // modeがNormalの場合はOpenAIChatを実行
                else if (mode == OpenAIExecutionModeEnum.Normal) {
                    // OpenAIChatを実行
                    await Task.Run(() => {
                        result = PythonExecutor.PythonFunctions.OpenAIChat(itemViewModel.ClipboardItem.Content, chatItems);
                    });

                } else {
                    return;
                }
                // レスポンスをClipboardItemに設定
                itemViewModel.ClipboardItem.Content = result.Response;
                itemViewModel.ClipboardItem.Save();
                // フォルダ内のアイテムを再読み込み
                itemViewModel.FolderViewModel.Load();

            } catch (Exception e) {
                Tools.Error($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            }finally {
                // プログレスインジケーターを非表示
                MainWindowViewModel.UpdateProgressCircleVisibility(false);
            }
        }
    }

}
