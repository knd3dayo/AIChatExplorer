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

        #region システムのクリップボードへ貼り付けられたアイテムに関連する処理
        public static void ProcessClipboardItem(ClipboardFolder clipboardFolder, ClipboardChangedEventArgs e, Action<ClipboardItem> _afterClipboardChanged) {

            // Is the content copied of text type?
            if (e.ContentType == SharpClipboard.ContentTypes.Text) {
                string? text = e.Content.ToString();
                if (text == null) {
                    return;
                }
                // Get the cut/copied text.
                ClipboardFolderViewModel.ProcessClipboardItem(clipboardFolder, ClipboardContentTypes.Text, text, null, e, _afterClipboardChanged);
            }
            // Is the content copied of file type?
            else if (e.ContentType == SharpClipboard.ContentTypes.Files) {
                string[] files = (string[])e.Content;

                // Get the cut/copied file/files.
                for (int i = 0; i < files.Length; i++) {
                    ClipboardFolderViewModel.ProcessClipboardItem(clipboardFolder, ClipboardContentTypes.Files, files[i], null, e, _afterClipboardChanged);
                }

            }
            // Is the content copied of image type?
            else if (e.ContentType == SharpClipboard.ContentTypes.Image) {
                // Get the cut/copied image.
                System.Drawing.Image img = (System.Drawing.Image)e.Content;
                ClipboardFolderViewModel.ProcessClipboardItem(clipboardFolder, ClipboardContentTypes.Image, "", img, e, _afterClipboardChanged);

            }
            // If the cut/copied content is complex, use 'Other'.
            else if (e.ContentType == SharpClipboard.ContentTypes.Other) {
                // Do nothing
                // System.Windows.MessageBox.Show(_clipboard.ClipboardObject.ToString());
            }


        }

        /// <summary>
        /// Process clipboard item
        /// </summary>
        /// <param name="contentTypes"></param>
        /// <param name="content"></param>
        /// <param name="image"></param>
        /// <param name="e"></param>
        public static void ProcessClipboardItem(
            ClipboardFolder clipboardFolder,
            ClipboardContentTypes contentTypes, string content, System.Drawing.Image? image, ClipboardChangedEventArgs e, Action<ClipboardItem> _afterClipboardChanged) {

            ClipboardItem item = CreateClipboardItem(clipboardFolder, contentTypes, content, image, e);

            // Execute in a separate thread
            Task.Run(() => {
                string oldReadyText = Tools.StatusText.ReadyText;
                Application.Current.Dispatcher.Invoke(() => {
                    Tools.StatusText.ReadyText = StringResources.Instance.AutoProcessing;
                });
                try {
                    // Apply automatic processing
                    ClipboardItem? updatedItem = ApplyAutoAction(item, image);
                    if (updatedItem == null) {
                        // If the item is ignored, return
                        return;
                    }
                    // Notify the completion of processing
                    _afterClipboardChanged(updatedItem);

                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.Instance.AddItemFailed}\n{ex.Message}");
                } finally {
                    Application.Current.Dispatcher.Invoke(() => {
                        Tools.StatusText.ReadyText = oldReadyText;
                    });
                }
            });
        }


        /// Create ClipboardItem
        private static ClipboardItem CreateClipboardItem(
            ClipboardFolder clipboardFolder, ClipboardContentTypes contentTypes, string content, System.Drawing.Image? image, ClipboardChangedEventArgs e) {
            ClipboardItem item = new(clipboardFolder.Id) {
                ContentType = contentTypes
            };
            SetApplicationInfo(item, e);
            item.Content = content;

            // If ContentType is Image, set image data
            if (contentTypes == ClipboardContentTypes.Image && image != null) {
                ClipboardItemImage imageItem = ClipboardItemImage.Create(item, image);
                imageItem.SetImage(image);
                item.ClipboardItemImage = imageItem;
            }
            // If ContentType is Files, set file data
            else if (contentTypes == ClipboardContentTypes.Files) {
                ClipboardItemFile clipboardItemFile = ClipboardItemFile.Create(item, content);
                item.ClipboardItemFile = clipboardItemFile;
            }
            return item;

        }

        /// <summary>
        /// Set application information from ClipboardChangedEventArgs
        /// </summary>
        /// <param name="item"></param>
        /// <param name="sender"></param>
        private static void SetApplicationInfo(ClipboardItem item, ClipboardChangedEventArgs sender) {
            item.SourceApplicationName = sender.SourceApplication.Name;
            item.SourceApplicationTitle = sender.SourceApplication.Title;
            item.SourceApplicationID = sender.SourceApplication.ID;
            item.SourceApplicationPath = sender.SourceApplication.Path;
        }

        /// <summary>
        /// Apply automatic processing
        /// </summary>
        /// <param name="item"></param>
        /// <param name="image"></param>
        private static ClipboardItem? ApplyAutoAction(ClipboardItem item, System.Drawing.Image? image) {
            // ★TODO Implement processing based on automatic processing rules.
            // 指定した行数未満のテキストアイテムは無視
            int lineCount = item.Content.Split('\n').Length;
            if (item.ContentType == ClipboardContentTypes.Text && lineCount < ClipboardAppConfig.IgnoreLineCount) {
                return null;
            }
            // If AUTO_DESCRIPTION is set, automatically set the Description
            if (ClipboardAppConfig.AutoDescription) {
                try {
                    Tools.Info(StringResources.Instance.AutoSetTitle);
                    ClipboardItem.CreateAutoTitle(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.Instance.AutoSetTitle}\n{ex.Message}");
                }
            } else if (ClipboardAppConfig.AutoDescriptionWithOpenAI) {

                try {
                    Tools.Info(StringResources.Instance.AutoSetTitle);
                    ClipboardItem.CreateAutoTitleWithOpenAI(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.Instance.AutoSetTitle}\n{ex.Message}");
                }
            }
            // ★TODO Implement processing based on automatic processing rules.
            // If AUTO_TAG is set, automatically set the tags
            if (ClipboardAppConfig.AutoTag) {
                Tools.Info(StringResources.Instance.AutoSetTag);
                try {
                    ClipboardItem.CreateAutoTags(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.Instance.SetTagFailed}\n{ex.Message}");
                }
            }

            // ★TODO Implement processing based on automatic processing rules.
            // If AutoMergeItemsBySourceApplicationTitle is set, automatically merge items
            if (ClipboardAppConfig.AutoMergeItemsBySourceApplicationTitle) {
                Tools.Info(StringResources.Instance.AutoMerge);
                try {
                    ClipboardFolder.RootFolder.MergeItemsBySourceApplicationTitleCommandExecute(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.Instance.MergeFailed}\n{ex.Message}");
                }
            }
            // ★TODO Implement processing based on automatic processing rules.
            // If UseOCR is set, perform OCR
            if (ClipboardAppConfig.UseOCR && image != null) {
                Tools.Info(StringResources.Instance.OCR);
                try {
                    string text = PythonExecutor.PythonFunctions.ExtractTextFromImage(image, ClipboardAppConfig.TesseractExePath);
                    item.Content = text;
                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.Instance.OCRFailed}\n{ex.Message}");
                }
            }
            // If AutoFileExtract is set, extract files
            if (ClipboardAppConfig.AutoFileExtract && item.ContentType == ClipboardContentTypes.Files && item.ClipboardItemFile != null) {
                Tools.Info(StringResources.Instance.ExecuteAutoFileExtract);
                try {
                    string text = PythonExecutor.PythonFunctions.ExtractText(item.ClipboardItemFile.FilePath);
                    item.Content = text;

                } catch (ThisApplicationException ex) {
                    Tools.Error($"{StringResources.Instance.AutoFileExtractFailed}\n{ex.Message}");
                }
            }
            return item;
        }

        #endregion
    }

}
