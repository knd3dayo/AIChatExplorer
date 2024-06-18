using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Factory.Default {
    /// <summary>
    /// このアプリケーションで開いたプロセスを管理するクラス
    /// ファイルを閉じたときにテキストの内容をContentに保存する
    /// </summary>
    public class DefaultClipboardProcessController : IClipboardProcessController {
        // Processとファイル名の対応を保持するハッシュテーブル
        private static readonly Hashtable processOpenedFileHashTable = [];
        // ProcessとItemの対応を保持するハッシュテーブル
        private static readonly Hashtable processOpenedItemHashTable = [];

        public void OpenClipboardItemContent(ClipboardItem item) {
            // テンポラリディレクトリにランダムな名前のファイルを作成
            string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".txt");
            File.WriteAllText(tempFileName, item.Content);

            ProcessStartInfo procInfo = new ProcessStartInfo() {
                UseShellExecute = true,
                FileName = tempFileName

            };
            if (procInfo == null) {
                return;
            }
            Process? process = Process.Start(procInfo);
            //プロセス終了時のイベントハンドラーを設定
            if (process != null) {
                process.EnableRaisingEvents = true;
                process.Exited += new EventHandler(ContentProcessExited);
                // プロセスとファイル名の対応を保持
                processOpenedFileHashTable.Add(process, tempFileName);
                // プロセスとItemの対応を保持
                processOpenedItemHashTable.Add(process, item);
            }

        }

        // 「開く」で開いたプロセス終了イベントを処理する
        private static void ContentProcessExited(object? sender, EventArgs e) {
            // System.Windows.MessageBox.Show("プロセス終了");
            if (sender == null) {
                return;
            }
            // プロセス終了時にItemに開いた内容を保存
            Process? process = (Process)sender;
            string? tempFileName = (string?)processOpenedFileHashTable[process];
            if (tempFileName == null) {
                return;
            }
            // プロセスとItemの対応を取得
            ClipboardItem? item = (ClipboardItem?)processOpenedItemHashTable[process];
            if (item == null) {
                return;
            }
            // ファイルの内容をItemに保存
            item.Content = File.ReadAllText(tempFileName);
            // itemを保存
            item.Save();

            // テンポラリファイルを削除
            File.Delete(tempFileName);
            // ハッシュテーブルから削除
            processOpenedFileHashTable.Remove(process);

            processOpenedItemHashTable.Remove(process);

        }


        public void OpenClipboardItemFile(ClipboardItem item, bool openAsNew = false) {

            foreach (var clipboardItemFile in item.ClipboardItemFiles) {
                string contentFilePath = clipboardItemFile.FilePath;

                // 新規として開く場合はテンポラリディレクトリにファイルをコピーする
                if (openAsNew) {
                    // item.Contentがディレクトリの場合はメッセージを表示して終了
                    if (Directory.Exists(contentFilePath)) {
                        throw new ThisApplicationException("ディレクトリは新規として開けません");
                    }
                    // item.Contentのファイル名を取得
                    contentFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(clipboardItemFile.FileName));
                    // テンポラリディレクトリにコピー
                    File.Copy(clipboardItemFile.FilePath, contentFilePath, true);
                }

                // ファイルを開くプロセスを実行
                ProcessStartInfo proc = new ProcessStartInfo() {
                    UseShellExecute = true,
                    FileName = contentFilePath
                };
                Process.Start(proc);
            }
        }

        public void OpenClipboardItemImage(ClipboardItem item) {

            // ClipboardItemのClipboardItemImages毎に処理を実行
            foreach (var clipboardItemImage in item.ClipboardItemImages) {

                // テンポラリディレクトリにランダムな名前のファイルを作成
                string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");
                BitmapImage? bitmapImage = clipboardItemImage.GetBitmapImage();
                if (bitmapImage == null) {
                    continue;
                }
                // テンポラリディレクトリに画像を保存
                using (FileStream stream = new(tempFileName, FileMode.Create)) {
                    PngBitmapEncoder encoder = new();
                    encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                    encoder.Save(stream);
                }

                ProcessStartInfo procInfo = new() {
                    UseShellExecute = true,
                    FileName = tempFileName

                };
                if (procInfo == null) {
                    return;
                }
                Process? process = Process.Start(procInfo);

            }


        }
    }
}
