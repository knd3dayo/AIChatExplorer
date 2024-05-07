using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
    public partial class ClipboardItemViewModel {
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
                    if (item is null) {
                        continue;
                    }
                    item.ClipboardItem.Delete();
                }
                // フォルダ内のアイテムを再読み込む
                clipboardItemFolder.Load();
                Tools.Info("削除しました");
            }
        }
        public static void ChangePinCommandExecute(ClipboardFolderViewModel folderViewModel, IEnumerable<ClipboardItemViewModel> itemViewModels) {
            foreach (ClipboardItemViewModel clipboardItemViewModel in itemViewModels) {
                clipboardItemViewModel.IsPinned = !clipboardItemViewModel.IsPinned;
                // ピン留めの時は更新日時を変更しない
                clipboardItemViewModel.Save(false);
            }

            // フォルダ内のアイテムを再読み込み
            folderViewModel.Load();

        }
        public static void OpenItemCommandExecute(ClipboardFolderViewModel? folderViewModel, ClipboardItemViewModel? clipboardItemViewModel) {
            if (clipboardItemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません。");
                return;
            }
            if (folderViewModel == null) {
                Tools.Error("フォルダが選択されていません。");
                return;
            }

            EditItemWindow editItemWindow = new();
            EditItemWindowViewModel editItemWindowViewModel = (EditItemWindowViewModel)editItemWindow.DataContext;
            editItemWindowViewModel.Initialize(folderViewModel, clipboardItemViewModel, () => {
                // フォルダ内のアイテムを再読み込み
                folderViewModel.Load();
                Tools.Info("更新しました");
            });

            editItemWindow.Show();
        }

        /// <summary>
        /// クリップボードアイテムを新規作成する処理
        /// 作成後にフォルダ内のアイテムを再読み込む
        /// </summary>
        /// <param name="obj"></param>
        public static void CreateItemCommandExecute(ClipboardFolderViewModel? folderViewModel) {
            if (folderViewModel == null) {
                Tools.Error("フォルダが選択されていません。");
                return;
            }
            EditItemWindow editItemWindow = new();
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

        // 選択中のアイテムを開く処理
        public static void OpenSelectedItemAsFileCommandExecute(ClipboardItemViewModel? itemViewModel) {
            if (itemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません。");
                return;
            }
            try {
                // 選択中のアイテムを開く
                itemViewModel.OpenItem();

            } catch (ClipboardAppException e) {
                Tools.Error(e.Message);
            }

        }

        // ContentTypeがFileの場合にフォルダを開く処理
        public static void OpenFolderCommandExecute(ClipboardItemViewModel? itemViewModel) {
            if (itemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません。");
                return;
            }
            // ContentTypeがFileの場合のみフォルダを開く
            if (itemViewModel.ContentType != ClipboardContentTypes.Files) {
                Tools.Error("ファイル以外のコンテンツはフォルダを開けません");
                return;
            }
            // Process.Startでフォルダを開く
            string? folderPath = itemViewModel.FolderName;
            if (folderPath != null) {
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(folderPath) {
                    UseShellExecute = true
                };
                p.Start();
            }

        }
        // ContentTypeがFileの場合にファイルを開く処理
        public static void OpenFileCommandExecute(ClipboardItemViewModel? itemViewModel) {
            if (itemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません。");
                return;
            }

            // ContentTypeがFileの場合のみファイルを開く
            if (itemViewModel.ContentType != ClipboardContentTypes.Files) {
                Tools.Error("ファイル以外のコンテンツはファイルを開けません");
                return;
            }
            // Process.Startでファイルを開く
            string? filePath = itemViewModel.FilePath;
            if (filePath != null) {
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(filePath) {
                    UseShellExecute = true
                };
                p.Start();
            }
        }
        // 一時フォルダでファイルを開く処理
        public static void OpenFileInTempFolderCommandExecute(ClipboardItemViewModel? itemViewModel) {
            if (itemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません。");
                return;
            }
            // ContentTypeがFileの場合のみファイルを開く
            if (itemViewModel.ContentType != ClipboardContentTypes.Files) {
                Tools.Error("ファイル以外のコンテンツはファイルを開けません");
                return;
            }
            string? filePath = itemViewModel.FilePath;
            if (filePath != null) {
                // テンポラリディレクトリにファイルをコピーして開く
                string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(filePath));
                // ファイルの先頭に[temp]を付ける
                tempFilePath = Path.Combine(Path.GetTempPath(), "[temp]" + Path.GetFileName(tempFilePath));
                File.Copy(filePath, tempFilePath, true);
                var p = new Process();
                p.StartInfo = new ProcessStartInfo(tempFilePath) {
                    UseShellExecute = true
                };
                p.Start();
            }
        }

        // コンテキストメニューの「テキストを抽出」の実行用コマンド
        public static SimpleDelegateCommand ExtractTextCommand => new((parameter) => {
            ClipboardItemViewModel? clipboardItemViewModel = MainWindowViewModel.SelectedItemStatic;
            if (clipboardItemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません。");
                return;
            }            // File以外の場合はエラー
            if (clipboardItemViewModel.ContentType != ClipboardContentTypes.Files) {
                Tools.Error("ファイル以外のコンテンツはテキストを抽出できません");
                return;
            }
            ClipboardItem.ExtractTextCommandExecute(clipboardItemViewModel.ClipboardItem);
            // 保存
            clipboardItemViewModel.Save();

            // フォルダ内のアイテムを再読み込み
            clipboardItemViewModel.FolderViewModel.Load();
        });

        // コンテキストメニューの「データをマスキング」の実行用コマンド
        public static SimpleDelegateCommand MaskDataCommand => new((parameter) => {
            ClipboardItemViewModel? clipboardItemViewModel = MainWindowViewModel.SelectedItemStatic;
            if (clipboardItemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません。");
                return;
            } 
            // テキスト以外の場合はエラー
            if (clipboardItemViewModel.ContentType != ClipboardContentTypes.Text) {
                // 対話処理のため、エラー時はダイアログを表示
                Tools.Error("テキスト以外のコンテンツはマスキングできません");
                return;
            }
            clipboardItemViewModel.MaskDataCommandExecute();
            // 保存
            clipboardItemViewModel.Save();
            clipboardItemViewModel.FolderViewModel.Load();

        });

        // コンテキストメニューの「画像からテキストを抽出」をクリックしたときの処理
        public static void MenuItemExtractTextFromImageCommandExecute(ClipboardItemViewModel? clipboardItemViewModel) {
            if (clipboardItemViewModel == null) {
                // 対話処理のため、エラー時はダイアログを表示
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            // 画像以外の場合はエラー
            if (clipboardItemViewModel.ContentType != ClipboardContentTypes.Image) {
                // 対話処理のため、エラー時はダイアログを表示
                Tools.Error("画像以外のコンテンツはテキストを抽出できません");
                return;
            }
            // OCRが使用不可の場合はエラー
            if (!ClipboardAppConfig.UseOCR) {
                Tools.Error("PyOCRが使用できません。設定画面でPyOCRを有効にしてください");
                return;
            }
            try {
                ClipboardItemViewModel.ExtractTextFromImage(clipboardItemViewModel);
            } catch (Exception ex) {
                Tools.Error($"OCR処理が失敗しました。\n{ex.Message}");
            }

        }

        // メニューの「Pythonスクリプトを実行」をクリックしたときの処理
        public async static void MenuItemRunPythonScriptCommandExecute(ScriptItem scriptItem, ClipboardItemViewModel itemViewModel) {
            try {
                MainWindowViewModel.UpdateProgressCircleVisibility(true);
                // clipboardItemをJsonに変換
                string input_str = itemViewModel.Content;
                // Pythonスクリプトを実行
                string result = input_str;
                await Task.Run(() => {
                    string result = PythonExecutor.PythonFunctions.RunScript(scriptItem.Content, input_str);
                    // 結果をClipboardItemに設定
                    itemViewModel.Content = result;
                    // 保存
                    itemViewModel.Save();
                    // フォルダ内のアイテムを再読み込み
                    itemViewModel.FolderViewModel.Load();
                });

            } catch (ClipboardAppException e) {
                Tools.Error(e.Message);
            } finally {
                MainWindowViewModel.UpdateProgressCircleVisibility(false);
            }

        }
        // OpenAI Chatを開くコマンド
        public static void OpenOpenAIChatWindowExecute(ClipboardItemViewModel? itemViewModel) {

            QAChat.MainWindow openAIChatWindow = new();
            QAChat.MainWindowViewModel mainWindowViewModel = (QAChat.MainWindowViewModel)openAIChatWindow.DataContext;
            // 外部プロジェクトとして設定
            mainWindowViewModel.IsStartFromInternalApp = false;
            // InputTextに選択中のアイテムのContentを設定
            if (itemViewModel != null) {
                mainWindowViewModel.InputText = itemViewModel.Content;
            }
            openAIChatWindow.Show();
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
            promptTemplateWindowViewModel.Initialize(
                ListPromptTemplateWindowViewModel.ActionModeEum.Exec,
                (PromptItemViewModel promptItemViewModel, OpenAIExecutionModeEnum mode) => {
                    // itemViewModelのClipboardItemのContentの先頭にプロンプトテンプレートのPromptに設定
                    itemViewModel.Content = promptItemViewModel.PromptItem.Prompt + "\n----------\n" + itemViewModel.Content;

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

                List<ChatItem> chatItems = [];
                ChatResult result = new();
                // modeがRAGの場合はLangChainChatを実行
                if (mode == OpenAIExecutionModeEnum.RAG) {
                    // LangChainChatを実行
                    await Task.Run(() => {
                        result = PythonExecutor.PythonFunctions.LangChainChat(itemViewModel.Content, chatItems, VectorDBItem.GetEnabledItems());
                    });
                }
                // modeがNormalの場合はOpenAIChatを実行
                else if (mode == OpenAIExecutionModeEnum.Normal) {
                    // OpenAIChatを実行
                    await Task.Run(() => {
                        result = PythonExecutor.PythonFunctions.OpenAIChat(itemViewModel.Content, chatItems);
                    });

                } else {
                    return;
                }
                // レスポンスをClipboardItemに設定
                itemViewModel.Content = result.Response;
                itemViewModel.Save();
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
