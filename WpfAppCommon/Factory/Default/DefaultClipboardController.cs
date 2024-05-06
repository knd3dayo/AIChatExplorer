using System.Windows;
using WK.Libraries.SharpClipboardNS;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace WpfAppCommon.Factory.Default {
    /// <summary>
    /// クリップボード監視機能用のクラス
    /// </summary>
    class DefaultClipboardController : IClipboardController {
        //--------------------------------------------------------------------------------

        // 起動時フラグ(起動時のクリップボードを読み捨てるため)
        private bool IsStartup { get; set; } = true;
        // クリップボード監視有効無効フラグ
        public bool IsClipboardMonitorEnabled { get; set; } = false;
        private SharpClipboard? _clipboard = null;
        private Action<ActionMessage> _afterClipboardChanged = (parameter) => { };

        // クリップボード監視を開始する
        public void Start(Action<ActionMessage> afterClipboardChanged) {
            _afterClipboardChanged = afterClipboardChanged;
            if (_clipboard == null) {
                // Clipboardの監視開始
                _clipboard = new SharpClipboard();
                _clipboard.ClipboardChanged += ClipboardChanged;
                // クリップボードの監視を有効にする
            }
            IsClipboardMonitorEnabled = true;
        }

        // クリップボード監視を停止する
        public void Stop() {
            if (_clipboard != null) {
                _clipboard.Dispose();
            }
            IsClipboardMonitorEnabled = false;
        }
        // ClipboardItemの内容をクリップボードにコピーする 
        // Ctrl+Cで実行するコマンド
        public void SetDataObject(ClipboardItem item) {
            // System.Windows.MessageBox.Show(item.Content);

            IsClipboardMonitorEnabled = false;
            // ContentTypeがTextの場合はクリップボードにコピー
            if (item.ContentType == ClipboardContentTypes.Text) {
                if (item.Content == null) {
                    return;
                }
                System.Windows.Clipboard.SetDataObject(item.Content);
            }
            // ContentTypeがFilesの場合はクリップボードにファイルをコピー
            else if (item.ContentType == ClipboardContentTypes.Files) {
                System.Windows.Clipboard.SetFileDropList(new System.Collections.Specialized.StringCollection { item.Content });
            }
            IsClipboardMonitorEnabled = true;
        }

        private void ClipboardChanged(object? sender, ClipboardChangedEventArgs e) {
            // 起動時のクリップボードを読み捨てる
            if (IsStartup) {
                IsStartup = false;
                return;
            }

            if (IsClipboardMonitorEnabled == false) {
                // System.Windows.MessageBox.Show("Clipboard monitor disabled");
                return;
            }
            if (_clipboard == null) {
                return;
            }
            // Is the content copied of text type?
            if (e.ContentType == SharpClipboard.ContentTypes.Text) {
                // Get the cut/copied text.
                ProcessClipboardItem(ClipboardContentTypes.Text, _clipboard.ClipboardText, null, e);
            }
            // Is the content copied of file type?
            else if (e.ContentType == SharpClipboard.ContentTypes.Files) {
                // Get the cut/copied file/files.
                for (int i = 0; i < _clipboard.ClipboardFiles.Count; i++) {
                    ProcessClipboardItem(ClipboardContentTypes.Files, _clipboard.ClipboardFiles[i], null, e);
                }

            }
            // Is the content copied of image type?
            else if (e.ContentType == SharpClipboard.ContentTypes.Image) {
                // Get the cut/copied image.
                System.Drawing.Image img = _clipboard.ClipboardImage;
                ProcessClipboardItem(ClipboardContentTypes.Image, "", img, e);

            }
            // If the cut/copied content is complex, use 'Other'.
            else if (e.ContentType == SharpClipboard.ContentTypes.Other) {
                // System.Windows.MessageBox.Show(_clipboard.ClipboardObject.ToString());
            }
            // オブザーバーに通知
            _afterClipboardChanged?.Invoke(
                ActionMessage.Info("クリップボードの内容が変更されました")
            );

        }
        private void ProcessClipboardItem(ClipboardContentTypes contentTypes, string content, System.Drawing.Image? image, ClipboardChangedEventArgs e) {
            if (_clipboard == null) {
                Tools.Error("Clipboard is null");
                return;
            }
            // 監視対象アプリケーションかどうかを判定
            if (!IsMonitorTargetApp(e)) {
                return;
            }

            ClipboardItem item = CreateClipboardItem(contentTypes, content, image, e);

            // 別スレッドで実行
            Task.Run(() => {
                string oldReadyText = Tools.StatusText.ReadyText;
                Application.Current.Dispatcher.Invoke(() => {
                    Tools.StatusText.ReadyText = "自動処理を実行中";
                });
                try {
                    // 自動処理を適用
                    ApplyAutoAction(item, image);
                    // RootFolderにClipboardItemを追加
                    ClipboardFolder.RootFolder.AddItem(item, _afterClipboardChanged);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"クリップボードアイテムの追加処理が失敗しました。\n{ex.Message}");
                } finally {
                    Application.Current.Dispatcher.Invoke(() => {
                        Tools.StatusText.ReadyText = oldReadyText;
                    });
                }
            });
        }

        // クリップボードアイテムが監視対象かどうかを判定
        public bool IsMonitorTargetApp(ClipboardChangedEventArgs e) {
            // MonitorTargetAppNamesが空文字列ではなく、MonitorTargetAppNamesに含まれていない場合は処理しない
            if (ClipboardAppConfig.MonitorTargetAppNames != "") {
                // 大文字同士で比較
                string upperSourceApplication = e.SourceApplication.Name.ToUpper();
                string upperMonitorTargetAppNames = ClipboardAppConfig.MonitorTargetAppNames.ToUpper();
                if (!upperMonitorTargetAppNames.Contains(upperSourceApplication)) {
                    return false;
                }
            }
            return true;
        }

        // ClipboardItemを作成
        // ★TODO ClipboardITemに移動
        public ClipboardItem CreateClipboardItem(ClipboardContentTypes contentTypes, string content, System.Drawing.Image? image, ClipboardChangedEventArgs e) {
            ClipboardItem item = new() {
                ContentType = contentTypes
            };
            item.SetApplicationInfo(e);
            item.Content = content;
            item.CollectionName = ClipboardFolder.RootFolder.AbsoluteCollectionName;

            // ContentTypeがImageの場合は画像データを設定
            if (contentTypes == ClipboardContentTypes.Image && image != null) {
                ClipboardItemImage imageItem = new();
                imageItem.SetImage(image);
                item.ClipboardItemImage = imageItem;
            }
            // ContentTypeがFilesの場合はファイルデータを設定
            else if (contentTypes == ClipboardContentTypes.Files) {
                ClipboardItemFile clipboardItemFile = new(content);
                item.ClipboardItemFile = clipboardItemFile;
            }
            return item;

        }

        // 自動処理を適用
        public void ApplyAutoAction(ClipboardItem item, System.Drawing.Image? image) {
            // ★TODO 自動処理ルールで処理するようにする。

            // AUTO_DESCRIPTIONが設定されている場合は自動でDescriptionを設定する
            if (ClipboardAppConfig.AutoDescription) {
                try {
                    Tools.Info("自動タイトル設定処理を実行します");
                    ClipboardItem.CreateAutoDescription(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"自動タイトル設定処理が失敗しました。\n{ex.Message}");
                }
            }
            // ★TODO 自動処理ルールで処理するようにする。
            // AUTO_TAGが設定されている場合は自動でタグを設定する
            if (ClipboardAppConfig.AutoTag) {
                Tools.Info("自動タグ設定処理を実行します");
                try {
                    ClipboardItem.CreateAutoTags(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"自動タグ設定処理が失敗しました。\n{ex.Message}");
                }
            }

            // ★TODO 自動処理ルールで処理するようにする。
            // AutoMergeItemsBySourceApplicationTitleが設定されている場合は自動でマージする
            if (ClipboardAppConfig.AutoMergeItemsBySourceApplicationTitle) {
                Tools.Info("自動マージ処理を実行します");
                try {
                    ClipboardFolder.RootFolder.MergeItemsBySourceApplicationTitleCommandExecute(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"自動マージ処理が失敗しました。\n{ex.Message}");
                }
            }
            // ★TODO 自動処理ルールで処理するようにする。
            // UseOCRが設定されている場合はOCRを実行
            if (ClipboardAppConfig.UseOCR && image != null) {
                Tools.Info("OCR処理を実行します");
                try {
                    string text = PythonExecutor.PythonFunctions.ExtractTextFromImage(image, ClipboardAppConfig.TesseractExePath);
                    item.Content = text;
                } catch (ThisApplicationException ex) {
                    Tools.Error($"OCR処理が失敗しました。\n{ex.Message}");
                }
            }
        }

    }
}
