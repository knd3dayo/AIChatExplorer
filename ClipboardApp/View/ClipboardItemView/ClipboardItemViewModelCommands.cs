using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.SearchView;
using ClipboardApp.View.TagView;
using ClipboardApp.Views.ClipboardItemView;
using MS.WindowsAPICodePack.Internal;
using QAChat.Model;
using QAChat.View.PromptTemplateWindow;
using QAChat.View.VectorDBWindow;
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
        // ピン留めの切り替え処理 複数アイテム処理可能
        public static SimpleDelegateCommand<object> ChangePinCommand => new((parameter) => {
            ChangePinCommandExecute();
        });

        // ピン留めの切り替え処理 複数アイテム処理可能
        public static void ChangePinCommandExecute() {
            MainWindowViewModel? windowViewModel = MainWindowViewModel.ActiveInstance;
            if (windowViewModel == null) {
                Tools.Error("MainWindowViewModelが取得できませんでした");
                return;
            }

            ClipboardFolderViewModel? SelectedFolder = windowViewModel.SelectedFolder;
            ObservableCollection<ClipboardItemViewModel> SelectedItems = windowViewModel.SelectedItems;

            // 選択中のアイテムがない場合は処理をしない
            if (SelectedItems == null || SelectedItems.Count == 0) {
                Tools.Error("選択中のアイテムがない");
                return;
            }
            // 選択中のフォルダがない場合は処理をしない
            if (SelectedFolder == null) {
                Tools.Error("選択中のフォルダがない");
                return;
            }

            foreach (ClipboardItemViewModel clipboardItemViewModel in SelectedItems) {
                clipboardItemViewModel.IsPinned = !clipboardItemViewModel.IsPinned;
                // ピン留めの時は更新日時を変更しない
                clipboardItemViewModel.Save(false);
            }

            // フォルダ内のアイテムを再読み込み
            SelectedFolder.Load();

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
            EditItemWindow.OpenEditItemWindow(folderViewModel, clipboardItemViewModel, () => {
                // フォルダ内のアイテムを再読み込み
                folderViewModel.Load();
                Tools.Info("更新しました");
            });
        }




        // ContentTypeがFileの場合にフォルダを開く処理
        public SimpleDelegateCommand<object> OpenFolderCommand => new((parameter) => {
            // ContentTypeがFileの場合のみフォルダを開く
            if (this.ContentType != ClipboardContentTypes.Files) {
                Tools.Error("ファイル以外のコンテンツはフォルダを開けません");
                return;
            }
            // Process.Startでフォルダを開く
            foreach (var item in this.ClipboardItem.ClipboardItemFiles) {
                string? folderPath = item.FolderName;
                if (folderPath != null) {
                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo(folderPath) {
                        UseShellExecute = true
                    };
                    p.Start();
                }
            }
        });

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
            foreach (var item in itemViewModel.ClipboardItem.ClipboardItemFiles) {
                string? filePath = item.FilePath;
                if (filePath != null) {
                    var p = new Process();
                    p.StartInfo = new ProcessStartInfo(filePath) {
                        UseShellExecute = true
                    };
                    p.Start();
                }
            }
        }

        // コンテキストメニューの「テキストを抽出」の実行用コマンド
        public SimpleDelegateCommand<object> ExtractTextCommand => new((parameter) => {
            if (this.ContentType != ClipboardContentTypes.Files) {
                Tools.Error("ファイル以外のコンテンツはテキストを抽出できません");
                return;
            }
            ClipboardItem.ExtractTextCommandExecute(this.ClipboardItem);
            // 保存
            this.Save();
        });

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
                });

            } catch (ClipboardAppException e) {
                Tools.Error(e.Message);
            } finally {
                MainWindowViewModel.UpdateProgressCircleVisibility(false);
            }

        }
        // OpenAI Chatを開くコマンド
        public static void OpenOpenAIChatWindowExecute(ClipboardFolderViewModel? folderViewModel, ClipboardItemViewModel? itemViewModel) {

            SearchRule rule = ClipboardFolder.GlobalSearchCondition.Copy();

            QAChat.MainWindow openAIChatWindow = new();
            QAChat.MainWindowViewModel mainWindowViewModel = (QAChat.MainWindowViewModel)openAIChatWindow.DataContext;
            // 外部プロジェクトとして設定
            mainWindowViewModel.IsStartFromInternalApp = false;
            mainWindowViewModel.Initialize(folderViewModel?.ClipboardItemFolder, itemViewModel?.ClipboardItem);
            mainWindowViewModel.ShowSearchWindowAction = (afterSelect) =>{
                SearchWindow.OpenSearchWindow(rule, null, () => {
                    // QAChatのContextを更新
                    List<ClipboardItem> clipboardItems = rule.SearchItems();
                    afterSelect(clipboardItems);

                });
            };
            mainWindowViewModel.SetContentTextFromClipboardItemsAction = (afterSelect) => {
                List<ClipboardItem> items = [];
                var clipboardItemViews = MainWindowViewModel.ActiveInstance?.SelectedFolder?.Items;
                if (clipboardItemViews != null) {
                    foreach (var item in clipboardItemViews) {
                        items.Add(item.ClipboardItem);
                    }
                }
                afterSelect(items);
            };
            // クリップボード編集画面を開くアクション
            mainWindowViewModel.OpenClipboardItemAction = (clipboardItem) => {
                ClipboardItemViewModel clipboardItemViewModel = new(clipboardItem);
                OpenItemCommandExecute(folderViewModel, clipboardItemViewModel);
            };
            // ベクトルDBアイテムを開くアクション
            mainWindowViewModel.OpenVectorDBItemAction = (vectorDBItem) => {
                VectorDBItemViewModel vectorDBItemViewModel = new(vectorDBItem);
                EditVectorDBWindow.OpenEditVectorDBWindow(vectorDBItemViewModel, (model) => { });
            };

            openAIChatWindow.Show();

        }
        // プロンプトテンプレート一覧を開いて選択したプロンプトテンプレートを実行するコマンド
        public static void OpenAIChatCommandExecute(ClipboardItemViewModel? itemViewModel) {
            // itemViewModelがnullの場合はエラー
            if (itemViewModel == null) {
                Tools.Error("クリップボードアイテムが選択されていません");
                return;
            }
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(
                ListPromptTemplateWindowViewModel.ActionModeEum.Exec,
                (PromptItemViewModel promptItemViewModel, OpenAIExecutionModeEnum mode) => {
                    // OpenAIChatを実行
                    OpenAIChatCommandExecute(mode, itemViewModel, promptItemViewModel);
            });
        }


        // OpenAI Chatを実行してその結果をClipboardItemに設定するコマンド
        public static async void OpenAIChatCommandExecute(OpenAIExecutionModeEnum mode, ClipboardItemViewModel itemViewModel, PromptItemViewModel promptItemViewModel) {

            try {

                // プログレスインジケーターを表示
                MainWindowViewModel.UpdateProgressCircleVisibility(true);
                ChatResult? result = new();
                ChatController chatController = new();
                chatController.ChatMode = mode;

                await Task.Run(() => {
                    result = itemViewModel.ClipboardItem.OpenAIChat(mode, promptItemViewModel.PromptItem.Prompt);
                });
                if (result == null) {
                    Tools.Error("OpenAI Chatの実行に失敗しました");
                    return;
                }
                itemViewModel.Content = result.Response;
                itemViewModel.Save();

            } catch (Exception e) {
                Tools.Error($"エラーが発生ました:\nメッセージ:\n{e.Message}\nスタックトレース:\n{e.StackTrace}");
            }finally {
                // プログレスインジケーターを非表示
                MainWindowViewModel.UpdateProgressCircleVisibility(false);
            }
        }

        public void UpdateTagList(ObservableCollection<TagItemViewModel> tagList) {
            // TagListのチェックを反映
            foreach (var item in tagList) {
                if (item.IsChecked) {
                    Tags.Add(item.Tag);
                } else {
                    Tags.Remove(item.Tag);
                }
            }
            // DBに反映
            Save();
        }

        // ファイルを開くコマンド
        public SimpleDelegateCommand<object> OpenFileCommand => new((obj) => {
            // 選択中のアイテムを開く
            ClipboardAppFactory.Instance.GetClipboardProcessController().OpenClipboardItemFile(ClipboardItem, false);
        });

        // ファイルを新規ファイルとして開くコマンド
        public SimpleDelegateCommand<object> OpenFileAsNewFileCommand => new((obj) => {
            // 選択中のアイテムを開く
            ClipboardAppFactory.Instance.GetClipboardProcessController().OpenClipboardItemFile(ClipboardItem, true);
        });

        // 画像を開くコマンド
        public SimpleDelegateCommand<object> OpenImageCommand => new((obj) => {
            // 選択中のアイテムを開く
            ClipboardAppFactory.Instance.GetClipboardProcessController().OpenClipboardItemImage(ClipboardItem);
        });

        // 画像からテキストを抽出するコマンド
        public SimpleDelegateCommand<object> MenuItemExtractTextFromImageCommand => new((parameter) => {
            // 画像以外の場合はエラー
            if (ContentType != ClipboardContentTypes.Image) {
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
                ExtractTextFromImage(this);
            } catch (Exception ex) {
                Tools.Error($"OCR処理が失敗しました。\n{ex.Message}");
            }
        });



        // コンテキストメニュー
        public ClipboardItemFolderContextMenuItems ContextMenuItems {
            get {
                return new ClipboardItemFolderContextMenuItems(this);
            }
        }
        // コンテキストメニューの「データをマスキング」の実行用コマンド
        public SimpleDelegateCommand<object> MaskDataCommand => new((parameter) => {

            this.ClipboardItem.MaskDataCommandExecute();
            // 保存
            this.Save();

        });

    }

}
