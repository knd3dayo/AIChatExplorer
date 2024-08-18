using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using WpfAppCommon.Model;

namespace WpfAppCommon.Utils {
    public class ProcessUtil {

        // Processとファイル名の対応を保持するハッシュテーブル
        private static readonly Hashtable processOpenedFileHashTable = [];
        // Processとクローズ後の処理を保持するハッシュテーブル
        private static readonly Hashtable processAfterCloseHashTable = [];


        public static void OpenTempTextFile(string content, Action<Process> afterOpen, Action<string> afterClose) {
            // テンポラリディレクトリにランダムな名前のファイルを作成
            string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".txt");
            File.WriteAllText(tempFileName, content);

            ProcessStartInfo procInfo = new() {
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
                // プロセスとクローズ後の処理を保持
                processAfterCloseHashTable.Add(process, afterClose);
                // 事後処理を実行
                afterOpen(process);

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
            // ファイルの内容を取得
            string content = File.ReadAllText(tempFileName);

            // テンポラリファイルを削除
            File.Delete(tempFileName);
            // ハッシュテーブルから削除
            processOpenedFileHashTable.Remove(process);

            // 事後処理を実行
            ((Action<string>?)processAfterCloseHashTable[process])?.Invoke(content);

        }
        public static void OpenFile(string contentFilePath, bool openAsNew = false) {

            // 新規として開く場合はテンポラリディレクトリにファイルをコピーする
            if (openAsNew) {
                // item.Contentがディレクトリの場合はメッセージを表示して終了
                if (Directory.Exists(contentFilePath)) {
                    throw new Exception(CommonStringResources.Instance.CannotOpenDirectoryAsNewFile);
                }
                // item.Contentのファイル名を取得
                string tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetFileName(contentFilePath));
                // テンポラリディレクトリにコピー
                File.Copy(contentFilePath, tempFilePath, true);
                contentFilePath = tempFilePath;
            }

            // ファイルを開くプロセスを実行
            ProcessStartInfo proc = new() {
                UseShellExecute = true,
                FileName = contentFilePath
            };
            Process.Start(proc);
        }

        public static void OpenBitmapImage(BitmapImage? bitmapImage) {


            // テンポラリディレクトリにランダムな名前のファイルを作成
            string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".png");
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
