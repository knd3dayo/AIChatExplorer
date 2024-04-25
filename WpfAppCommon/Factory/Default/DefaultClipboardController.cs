using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using WpfAppCommon.PythonIF;
using WK.Libraries.SharpClipboardNS;
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
        private Action<ActionMessage> _afterClipboardChanged = (parameter) => {};

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

        private async void ClipboardChanged(object? sender, ClipboardChangedEventArgs e) {
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
            // Taskとして実行
            await Task.Run(() => {

                // Is the content copied of text type?
                if (e.ContentType == ContentTypes.Text) {
                    // Get the cut/copied text.
                    ProcessClipboardItem(ClipboardContentTypes.Text, _clipboard.ClipboardText, e);
                }
                // Is the content copied of file type?
                else if (e.ContentType == ContentTypes.Files) {
                    // Get the cut/copied file/files.
                    for (int i = 0; i < _clipboard.ClipboardFiles.Count; i++) {
                        ProcessClipboardItem(ClipboardContentTypes.Files, _clipboard.ClipboardFiles[i], e);
                    }

                }
                // Is the content copied of image type?
                else if (e.ContentType == ContentTypes.Image) {
                    // Get the cut/copied image.
                    System.Drawing.Image img = _clipboard.ClipboardImage;
                    // UseOCRが設定されている場合はOCRを実行
                    if (WpfAppCommon.Properties.Settings.Default.UseOCR) {
                        try {
                            string text = PythonExecutor.PythonFunctions.ExtractTextFromImage(img);
                            ProcessClipboardItem(ClipboardContentTypes.Text, text, e);

                        } catch (ThisApplicationException ex) {
                            Tools.Error($"OCR処理が失敗しました。\n{ex.Message}");
                        }


                    }

                }
                // If the cut/copied content is complex, use 'Other'.
                else if (e.ContentType == ContentTypes.Other) {
                    // System.Windows.MessageBox.Show(_clipboard.ClipboardObject.ToString());
                }
            });
            // オブザーバーに通知
            _afterClipboardChanged?.Invoke(
                ActionMessage.Info("クリップボードの内容が変更されました")
            );

        }
        private void ProcessClipboardItem(ClipboardContentTypes contentTypes, string content, ClipboardChangedEventArgs e) {
            if (_clipboard == null) {
                Tools.Error("Clipboard is null");
                return;
            }
            // MonitorTargetAppNamesが空文字列ではなく、MonitorTargetAppNamesに含まれていない場合は処理しない
            if (WpfAppCommon.Properties.Settings.Default.MonitorTargetAppNames != "") {
                // 大文字同士で比較
                string upperSourceApplication = e.SourceApplication.Name.ToUpper();
                string upperMonitorTargetAppNames = WpfAppCommon.Properties.Settings.Default.MonitorTargetAppNames.ToUpper();
                if (!upperMonitorTargetAppNames.Contains(upperSourceApplication)) {
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
            if (WpfAppCommon.Properties.Settings.Default.AutoDescription) {
                try {
                    Tools.Info("自動タイトル設定処理を実行します");
                    ClipboardItem.CreateAutoDescription(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"自動タイトル設定処理が失敗しました。\n{ex.Message}");
                }
            }
            // ★TODO 自動処理ルールで処理するようにする。
            // AUTO_TAGが設定されている場合は自動でタグを設定する
            if (WpfAppCommon.Properties.Settings.Default.AutoTag) {
                try {
                    Tools.Info("自動タグ設定処理を実行します");
                    ClipboardItem.CreateAutoTags(item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"自動タグ設定処理が失敗しました。\n{ex.Message}");
                }
            }

            // ★TODO 自動処理ルールで処理するようにする。
            // AutoMergeItemsBySourceApplicationTitleが設定されている場合は自動でマージする
            if (WpfAppCommon.Properties.Settings.Default.AutoMergeItemsBySourceApplicationTitle) {
                try {
                    Tools.Info("自動マージ処理を実行します");
                    ClipboardItemFolder.MergeItemsBySourceApplicationTitleCommandExecute(ClipboardItemFolder.RootFolder, item);
                } catch (ThisApplicationException ex) {
                    Tools.Error($"自動マージ処理が失敗しました。\n{ex.Message}");
                }
            }

            // RootFolderのAddItemを呼び出す
            ClipboardItemFolder.RootFolder.AddItem(item , _afterClipboardChanged);


        }

    }
}
