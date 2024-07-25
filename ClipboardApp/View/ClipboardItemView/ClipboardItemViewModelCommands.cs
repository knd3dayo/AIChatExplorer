using System.Diagnostics;
using ClipboardApp.Utils;
using ClipboardApp.View.ClipboardItemFolderView;
using ClipboardApp.View.SearchView;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using QAChat.View.PromptTemplateWindow;
using QAChat.View.VectorDBWindow;
using WpfAppCommon;
using WpfAppCommon.Control.QAChat;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using QAChat.ViewModel;

namespace ClipboardApp.View.ClipboardItemView {
    public partial class ClipboardItemViewModel {
        // アイテム保存
        public SimpleDelegateCommand<bool> SaveClipboardItemCommand => new(ClipboardItem.Save);

        // Delete
        public SimpleDelegateCommand<ClipboardItemViewModel> DeleteClipboardItemCommand => new((obj) => {
            ClipboardItem.Delete();
        });

        public SimpleDelegateCommand<object> OpenFolderCommand => new((parameter) => {
            // ContentTypeがFileの場合のみフォルダを開く
            if (this.ContentType != ClipboardContentTypes.Files) {
                LogWrapper.Error("ファイル以外のコンテンツはフォルダを開けません");
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

        // コンテキストメニューの「テキストを抽出」の実行用コマンド
        public SimpleDelegateCommand<object> ExtractTextCommand => new((parameter) => {
            if (this.ContentType != ClipboardContentTypes.Files) {
                LogWrapper.Error("ファイル以外のコンテンツはテキストを抽出できません");
                return;
            }
            ClipboardItem.ExtractTextCommandExecute(this.ClipboardItem);
            // 保存
            this.SaveClipboardItemCommand.Execute(true);
        });

        // メニューの「Pythonスクリプトを実行」をクリックしたときの処理
        public SimpleDelegateCommand<ScriptItem> MenuItemRunPythonScriptCommandExecute => new(async (scriptItem) => {
            try {
                MainWindowViewModel.UpdateProgressCircleVisibility(true);
                // clipboardItemをJsonに変換
                string input_str = this.Content;
                // Pythonスクリプトを実行
                string result = input_str;
                await Task.Run(() => {
                    string result = PythonExecutor.PythonFunctions.RunScript(scriptItem.Content, input_str);
                    // 結果をClipboardItemに設定
                    this.Content = result;
                    // 保存
                    this.SaveClipboardItemCommand.Execute(true);
                });

            } catch (ClipboardAppException e) {
                LogWrapper.Error(e.Message);
            } finally {
                MainWindowViewModel.UpdateProgressCircleVisibility(false);
            }

        });

        // OpenAI Chatを開くコマンド
        public SimpleDelegateCommand<object> OpenOpenAIChatWindowCommand => new((parameter) => {

            SearchRule rule = ClipboardFolder.GlobalSearchCondition.Copy();

            QAChat.MainWindow openAIChatWindow = new();
            QAChat.ViewModel.MainWindowViewModel mainWindowViewModel = (QAChat.ViewModel.MainWindowViewModel)openAIChatWindow.DataContext;

            QAChatStartupProps qAChatStartupProps = new(FolderViewModel.ClipboardItemFolder, this.ClipboardItem, false) {

                // ベクトルDBアイテムを開くアクション
                OpenVectorDBItemAction = (vectorDBItem) => {
                    VectorDBItemViewModel vectorDBItemViewModel = new(vectorDBItem);
                    EditVectorDBWindow.OpenEditVectorDBWindow(vectorDBItemViewModel, (model) => { });
                },
                // フォルダ選択アクション
                SelectFolderAction = (vectorDBItems) => {
                    if (MainWindowViewModel.ActiveInstance == null) {
                        LogWrapper.Error("MainWindowViewModelがNullです");
                        return;
                    }
                    FolderSelectWindow.OpenFolderSelectWindow(MainWindowViewModel.ActiveInstance.RootFolderViewModel, (folderViewModel) => {
                        vectorDBItems.Add(folderViewModel.ClipboardItemFolder.GetVectorDBItem());
                    });
                },
                // 選択中のクリップボードアイテムを取得するアクション
                GetSelectedClipboardItemImageFunction = () => {
                    List<ClipboardItemImage> images = [];
                    var selectedItems = MainWindowViewModel.ActiveInstance?.SelectedItems;
                    if (selectedItems == null) {
                        return images;
                    }
                    foreach (ClipboardItemViewModel selectedItem in selectedItems) {
                        selectedItem.ClipboardItem.ClipboardItemImages.ForEach((image) => {
                            images.Add(image);
                        });
                    }
                    return images;
                }

            };

            mainWindowViewModel.Initialize(qAChatStartupProps);
            openAIChatWindow.Show();

        });

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
                LogWrapper.Error("画像以外のコンテンツはテキストを抽出できません");
                return;
            }
            // OCRが使用不可の場合はエラー
            if (!ClipboardAppConfig.AutoExtractImageWithPyOCR) {
                LogWrapper.Error("PyOCRが使用できません。設定画面でPyOCRを有効にしてください");
                return;
            }
            try {
                ClipboardItem.ExtractTextFromImageCommandExecute(ClipboardItem);
                // 保存
                ClipboardItem.Save();
            } catch (Exception ex) {
                LogWrapper.Error($"OCR処理が失敗しました。\n{ex.Message}");
            }
        });

        // コンテキストメニューの「データをマスキング」の実行用コマンド
        public SimpleDelegateCommand<object> MaskDataCommand => new((parameter) => {

            this.ClipboardItem.MaskDataCommandExecute();
            // 保存
            this.SaveClipboardItemCommand.Execute(true);

        });


        // テキストをファイルとして開くコマンド
        public SimpleDelegateCommand<object> OpenContentAsFileCommand => new((obj) => {
            try {
                // 選択中のアイテムを開く
                ClipboardAppFactory.Instance.GetClipboardProcessController().OpenClipboardItemContent(ClipboardItem);
            } catch (ClipboardAppException e) {
                LogWrapper.Error(e.Message);
            }
        });

    }

}
