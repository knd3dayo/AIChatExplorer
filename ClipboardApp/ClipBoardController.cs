using System.Collections.ObjectModel;
using WK.Libraries.SharpClipboardNS;
using ClipboardApp.Model;
using ClipboardApp.PythonIF;
using ClipboardApp.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace ClipboardApp {
    /// <summary>
    /// クリップボード監視機能用のクラス
    /// </summary>
    public class ClipboardController {
        //--------------------------------------------------------------------------------

        // 起動時フラグ(起動時のクリップボードを読み捨てるため)
        public static bool IsStartup { get; set; } = true;
        // クリップボード監視有効無効フラグ
        public static bool IsClipboardMonitorEnabled { get; set; } = false;
        private static SharpClipboard? clipboard = null;

        public static MainWindowViewModel? MainWindowViewModel { get; set; }
        public static void Init(MainWindowViewModel mainWindowViewModel) {

            MainWindowViewModel = mainWindowViewModel;

        }
        // クリップボード監視を開始する
        public static void Start() {
            if (clipboard == null) {
                // Clipboardの監視開始
                clipboard = new SharpClipboard();
                clipboard.ClipboardChanged += ClipboardChanged;
                // クリップボードの監視を有効にする
            }
            IsClipboardMonitorEnabled = true;
        }

        // クリップボード監視を停止する
        public static void Stop() {
            if (clipboard != null) {
                clipboard.Dispose();
            }
            IsClipboardMonitorEnabled = false;
        }
        // ClipboardItemの内容をクリップボードにコピーする 
        // Ctrl+Cで実行するコマンド
        public static void CopyToClipboard(ClipboardItem item) {
            // System.Windows.MessageBox.Show(item.Content);

            IsClipboardMonitorEnabled = false;
            // ContentTypeがTextの場合はクリップボードにコピー
            if (item.ContentType == SharpClipboard.ContentTypes.Text) {
                if (item.Content == null) {
                    return;
                }
                System.Windows.Clipboard.SetDataObject(item.Content);
            }
            // ContentTypeがFilesの場合はクリップボードにファイルをコピー
            else if (item.ContentType == SharpClipboard.ContentTypes.Files) {
                System.Windows.Clipboard.SetFileDropList(new System.Collections.Specialized.StringCollection { item.Content });
            }
            IsClipboardMonitorEnabled = true;
        }

        private static void ClipboardChanged(Object? sender, ClipboardChangedEventArgs e) {
            // 起動時のクリップボードを読み捨てる
            if (IsStartup) {
                IsStartup = false;
                return;
            }

            if (IsClipboardMonitorEnabled == false) {
                // System.Windows.MessageBox.Show("Clipboard monitor disabled");
                return;
            }
            if (clipboard == null) {
                return;
            }

            // Is the content copied of text type?
            if (e.ContentType == SharpClipboard.ContentTypes.Text) {
                // Get the cut/copied text.
                ProcessClipboardItem(e.ContentType, clipboard.ClipboardText, e);
            }
            // Is the content copied of file type?
            else if (e.ContentType == SharpClipboard.ContentTypes.Files) {
                // Get the cut/copied file/files.
                for (int i = 0; i < clipboard.ClipboardFiles.Count; i++) {
                    ProcessClipboardItem(e.ContentType, clipboard.ClipboardFiles[i], e);
                }

            }
            // Is the content copied of image type?
            else if (e.ContentType == SharpClipboard.ContentTypes.Image) {
                // Get the cut/copied image.
                System.Drawing.Image img = clipboard.ClipboardImage;
                // UseOCRが設定されている場合はOCRを実行
                if (Properties.Settings.Default.UseOCR) {
                    try {
                        string text = PythonExecutor.PythonFunctions.ExtractTextFromImage(img);
                        ProcessClipboardItem(SharpClipboard.ContentTypes.Text, text, e);

                    } catch (ClipboardAppException ex) {
                        Tools.Error($"OCR処理が失敗しました。\n{ex.Message}");
                    }


                }

            }
            // If the cut/copied content is complex, use 'Other'.
            else if (e.ContentType == SharpClipboard.ContentTypes.Other) {
                // System.Windows.MessageBox.Show(clipboard.ClipboardObject.ToString());
            }
        }
        private static void ProcessClipboardItem(SharpClipboard.ContentTypes contentTypes, string content, ClipboardChangedEventArgs e) {
            if (clipboard == null) {
                Tools.Error("Clipboard is null");
                return;
            }
            // MonitorTargetAppNamesが空文字列ではなく、MonitorTargetAppNamesに含まれていない場合は処理しない
            if (Properties.Settings.Default.MonitorTargetAppNames != "") {
                // 大文字同士で比較
                string upperSourceApplication = e.SourceApplication.Name.ToUpper();
                string upperMonitorTargetAppNames = Properties.Settings.Default.MonitorTargetAppNames.ToUpper();
                if (! upperMonitorTargetAppNames.Contains(upperSourceApplication)) {
                    return;
                }

            }

            // RootFolderにClipboardItemを追加

            // ★TODO 関数化
            ClipboardItem item = new ClipboardItem();
            item.ContentType = contentTypes;
            item.SetApplicationInfo(e);
            item.Content = content;
            item.CollectionName = ClipboardItemFolder.RootFolder.AbsoluteCollectionName;

            // ★TODO 自動処理ルールで処理するようにする。
            // AUTO_DESCRIPTIONが設定されている場合は自動でDescriptionを設定する
            if (Properties.Settings.Default.AutoDescription) {
                try {
                    Tools.Info("自動タイトル設定処理を実行します");
                    AutoProcessCommand.CreateAutoDescription(item);
                } catch (ClipboardAppException ex) {
                    Tools.Error($"自動タイトル設定処理が失敗しました。\n{ex.Message}");
                }
            }
            // ★TODO 自動処理ルールで処理するようにする。
            // AUTO_TAGが設定されている場合は自動でタグを設定する
            if (Properties.Settings.Default.AutoTag) {
                try {
                    Tools.Info("自動タグ設定処理を実行します");
                    AutoProcessCommand.CreateAutoTags(item);
                } catch (ClipboardAppException ex) {
                    Tools.Error($"自動タグ設定処理が失敗しました。\n{ex.Message}");
                }
            }

            // ★TODO 自動処理ルールで処理するようにする。
            // AutoMergeItemsBySourceApplicationTitleが設定されている場合は自動でマージする
            if (Properties.Settings.Default.AutoMergeItemsBySourceApplicationTitle) {
                try {
                    Tools.Info("自動マージ処理を実行します");
                    AutoProcessCommand.MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItemFolder.RootFolder, item);
                } catch (ClipboardAppException ex) {
                    Tools.Error($"自動マージ処理が失敗しました。\n{ex.Message}");
                }
            }

            // RootFolderのAddItemを呼び出す

            ClipboardItemFolder.RootFolder.AddItem(item);
            // 現在選択中のClipboardItemFolderに通知
            MainWindowViewModel?.ReloadClipboardItems();


        }

    }
}
