using System.Collections;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WK.Libraries.SharpClipboardNS;
using WpfApp1.Model;
using WpfApp1.Utils;
using static WK.Libraries.SharpClipboardNS.SharpClipboard;

namespace WpfApp1 {
    public class ClipboardProcessController {
        // Processとファイル名の対応を保持するハッシュテーブル
        private static Hashtable processOpenedFileHashtable = new Hashtable();
        // ProcessとItemの対応を保持するハッシュテーブル
        private static Hashtable processOpenedItemHashtable = new Hashtable();

        public static void OpenItem(ClipboardItem item, bool openAsNew = false) {
            if (item == null) {
                return;
            }
            // System.Windows.MessageBox.Show(item.Content);

            if (item.ContentType == SharpClipboard.ContentTypes.Files) {
                string contentFileName = item.Content;
                // 新規として開く場合はテンポラリディレクトリにファイルをコピーする
                if (openAsNew) {
                    // item.Contentがディレクトリの場合はメッセージを表示して終了
                    if (System.IO.Directory.Exists(item.Content)) {
                        throw new ThisApplicationException("ディレクトリは新規として開けません");
                    }
                    // item.Contentのファイル名を取得
                    contentFileName = Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetFileName(item.Content));

                    // テンポラリディレクトリにコピー
                    System.IO.File.Copy(item.Content, contentFileName, true);

                }

                // ファイルを開くプロセスを実行
                ProcessStartInfo proc = new ProcessStartInfo() {
                    UseShellExecute = true,
                    FileName = contentFileName
                };
                Process.Start(proc);
            } else if (item.ContentType == SharpClipboard.ContentTypes.Text) {
                // テンポラリディレクトリにランダムな名前のファイルを作成
                string tempFileName = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName() + ".txt");

                System.IO.File.WriteAllText(tempFileName, item.Content);

                ProcessStartInfo procInfo = new ProcessStartInfo() {
                    UseShellExecute = true,
                    FileName = tempFileName

                };
                if (procInfo == null) {
                    return;
                }
                Process? process = Process.Start(procInfo);
                // 新規として開く場合はイベントハンドラーを設定しない
                if (openAsNew) {
                    return;
                }
                //プロセス終了時のイベントハンドラーを設定
                if (process != null) {
                    process.EnableRaisingEvents = true;
                    process.Exited += new EventHandler(ProcessExited);
                    // プロセスとファイル名の対応を保持
                    processOpenedFileHashtable.Add(process, tempFileName);
                    // プロセスとitemの対応を保持
                    processOpenedItemHashtable.Add(process, item);
                }

            }
        }
        // 「開く」で開いたプロセス終了イベントを処理する
        private static void ProcessExited(object? sender, EventArgs e) {
            // System.Windows.MessageBox.Show("プロセス終了");
            if (sender == null) {
                return;
            }
            // プロセス終了時にitemに開いた内容を保存
            Process? process = (Process)sender;
            string? tempFileName = (string?)processOpenedFileHashtable[process];
            if (tempFileName == null) {
                return;
            }
            // プロセスとitemの対応を取得
            ClipboardItem? item = (ClipboardItem?)processOpenedItemHashtable[process];
            if (item == null) {
                return;
            }
            // 検索条件がある場合はそのまま保持してLoadする。
            Application.Current.Dispatcher.Invoke(() => {
                // ファイルの内容をitemに保存
                item.Content = System.IO.File.ReadAllText(tempFileName);
                // itemをDBに保存
                ClipboardDatabaseController.UpsertItem(item);

            });
            // テンポラリファイルを削除
            System.IO.File.Delete(tempFileName);
            // ハッシュテーブルから削除
            processOpenedFileHashtable.Remove(process);

            processOpenedItemHashtable.Remove(process);

        }



    }
    public class ClipboardController {
        //--------------------------------------------------------------------------------


        public static string CreateChatSessionCollectionName() {
            // 現在のエポック秒を取得
            long epoch = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
            return "chat_" + epoch.ToString();

        }
        public static ObservableCollection<ScriptItem> GetScriptItems() {
            var collection = ClipboardDatabaseController.GetClipboardDatabase().GetCollection<ScriptItem>(ClipboardDatabaseController.SCRIPT_COLLECTION_NAME);
            var items = collection.FindAll();
            ObservableCollection<ScriptItem> result = new ObservableCollection<ScriptItem>();
            foreach (var i in items) {
                result.Add(i);
            }
            return result;
        }

        // 起動時フラグ(起動時のクリップボードを読み捨てるため)
        public static bool IsStartup { get; set; } = true;
        // クリップボード監視有効無効フラグ
        public static bool IsClipboardMonitorEnabled { get; set; } = false;
        private static SharpClipboard? clipboard = null;

        public static MainWindowViewModel? MainWindowViewModel { get; set; }
        public static void Init(MainWindowViewModel mainWindowViewModel) {

            MainWindowViewModel = mainWindowViewModel;


            // Clipboardの監視開始
            clipboard = new SharpClipboard();
            clipboard.ClipboardChanged += ClipboardChanged;
            // クリップボードの監視を有効にする
            IsClipboardMonitorEnabled = true;

            // PropertiesにUSE_PYTHONが設定されている場合はPythonスクリプトを実行する準備を行う。
            if (Properties.Settings.Default.USE_PYTHON) {
                PythonExecutor.Init();
            }

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

            // RootFolderにClipboardItemを追加

            // ★TODO 関数化
            ClipboardItem item = new ClipboardItem();
            item.ContentType = contentTypes;
            item.SetApplicationInfo(e);
            item.Content = content;
            item.CollectionName = ClipboardItemFolder.RootFolder.AbsoluteCollectionName;

            // AUTO_DESCRIPTIONが設定されている場合は自動でDescriptionを設定する
            if (Properties.Settings.Default.AUTO_DESCRIPTION) {
                string updatedAtString = item.UpdatedAt.ToString("yyyy/MM/dd HH:mm:ss");
                // Textの場合
                if (item.ContentType == SharpClipboard.ContentTypes.Text) {
                    item.Description = $"{updatedAtString} {item.SourceApplicationName} {item.SourceApplicationTitle}";
                }
                // Fileの場合
                else if (item.ContentType == SharpClipboard.ContentTypes.Files) {
                    item.Description = $"{updatedAtString} {item.SourceApplicationName} {item.SourceApplicationTitle}";
                    // Contentのサイズが50文字以上の場合は先頭20文字 + ... + 最後の30文字をDescriptionに設定
                    if (item.Content.Length > 20) {
                        item.Description += " ファイル：" + item.Content.Substring(0, 20) + "..." + item.Content.Substring(item.Content.Length - 30);
                    } else {
                        item.Description += " ファイル：" + item.Content;
                    }
                }

            }
            // test
            ClipboardItemAppClient client = new ClipboardItemAppClient();
            client.Post(item);
            
            // RootFolderのAddItemを呼び出す
            ClipboardItemFolder.RootFolder.AddItem(item);

        }

    }
}
