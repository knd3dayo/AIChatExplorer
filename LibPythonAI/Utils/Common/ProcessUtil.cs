using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media.Imaging;
using LibGit2Sharp;
using PythonAILib.Model.Content;

namespace LibPythonAI.Utils.Common {
    public class ProcessUtil {

        // 実行プロセスのリスト
        private static readonly List<Process> processList = [];

        // Processとファイル名の対応を保持するハッシュテーブル
        private static readonly Hashtable processOpenedFileHashTable = [];
        // Processとクローズ後の処理を保持するハッシュテーブル
        private static readonly Hashtable contentWriterProcessAfterCloseHashTable = [];


        public static Process? StartBackgroundProcess(string fileName, string arguments, Dictionary<string, string> environmentVariables, bool showConsole, Action<Process> afterOpen,
                        DataReceivedEventHandler? OutputDataReceived = null, DataReceivedEventHandler? ErrorDataReceived = null, EventHandler? Exited = null) {

            ProcessStartInfo procInfo = new() {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = !showConsole,
                FileName = fileName,
                Arguments = arguments
            };

            if (procInfo == null) {
                return null;
            }
            // 環境変数を設定
            foreach (KeyValuePair<string, string> pair in environmentVariables) {
                if (procInfo.EnvironmentVariables.ContainsKey(pair.Key)) {
                    procInfo.EnvironmentVariables[pair.Key] = pair.Value;
                } else {
                    procInfo.EnvironmentVariables.Add(pair.Key, pair.Value);
                }
            }

            Process process = new() {
                StartInfo = procInfo
            };

            process.EnableRaisingEvents = true;
            // 標準出力イベントハンドラーを設定
            if (OutputDataReceived != null) {
                process.OutputDataReceived += OutputDataReceived;
            }
            // 標準エラー出力イベントハンドラーを設定
            if (ErrorDataReceived != null) {
                process.ErrorDataReceived += ErrorDataReceived;
            }
            // プロセス終了イベントハンドラーを設定
            if (Exited != null) {
                process.Exited += Exited;
            }
            // デフォルトのプロセス終了イベントハンドラーを設定
            process.Exited += new EventHandler(ProcessExited);

            process.Start();

            // 非同期出力読出し開始
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            // プロセスリストにプロセスを追加
            processList.Add(process);

            // 事後処理を実行
            afterOpen(process);

            return process;
        }


        public static void StopProcess(Process process) {
            // プロセスとサブプロセスを終了
            process.Kill(true);

            // プロセス終了時の事後処理を取得
            Action<string>? processAfterClose = (Action<string>?)contentWriterProcessAfterCloseHashTable[process];
            if (processAfterClose == null) {
                return;
            }
            // 事後処理を実行
            processAfterClose("");

        }

        // プロセスリストのプロセスを全て終了
        public static void StopAllProcess() {
            foreach (Process process in processList.ToList()) {
                // processがnullでなく、HasExitedがFalseの場合
                if (process != null && !process.HasExited) {
                    StopProcess(process);
                }
            }
        }


        public static Process? StartWindowsBackgroundCommandLine(List<string> commands, Dictionary<string, string> environmentVariables, bool showConsole, Action<Process> afterOpen,
            DataReceivedEventHandler? OutputDataReceived = null, DataReceivedEventHandler? ErrorDataReceived = null, EventHandler? Exited = null) {

            string cmd = "cmd";

            ProcessStartInfo procInfo = new() {
                RedirectStandardOutput = true,
                RedirectStandardInput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = !showConsole,
                FileName = cmd,
            };


            // 環境変数を設定
            foreach (KeyValuePair<string, string> pair in environmentVariables) {
                if (procInfo.EnvironmentVariables.ContainsKey(pair.Key)) {
                    procInfo.EnvironmentVariables[pair.Key] = pair.Value;
                } else {
                    procInfo.EnvironmentVariables.Add(pair.Key, pair.Value);
                }
            }

            Process process = new() {
                StartInfo = procInfo
            };

            process.EnableRaisingEvents = true;
            // 標準出力イベントハンドラーを設定
            if (OutputDataReceived != null) {
                process.OutputDataReceived += OutputDataReceived;
            }
            // 標準エラー出力イベントハンドラーを設定
            if (ErrorDataReceived != null) {
                process.ErrorDataReceived += ErrorDataReceived;
            }
            // プロセス終了イベントハンドラーを設定
            if (Exited != null) {
                process.Exited += Exited;
            }
            // デフォルトのプロセス終了イベントハンドラーを設定
            process.Exited += new EventHandler(ProcessExited);

            process.Start();
            var sw = process.StandardInput;
            if (sw.BaseStream.CanWrite) {
                foreach (var command in commands) {
                    sw.WriteLine(command);
                }

            }
            // 非同期出力読出し開始
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();

            // プロセスリストにプロセスを追加
            processList.Add(process);

            // 事後処理を実行
            afterOpen(process);

            return process;
        }

        public static Process? StartWindowsCommandLine(List<string> commands, string workingDirectory, Action<Process> afterOpen) {

            string cmd = "cmd";
            // テンポラリファイルにコマンドを書き込む.拡張値は.bat
            string tempFileName = Path.GetTempFileName();
            tempFileName = Path.ChangeExtension(tempFileName, ".bat");
            File.WriteAllLines(tempFileName, commands);
            string arguments = $"/c {tempFileName}";
            ProcessStartInfo procInfo = new(cmd, arguments) {
                UseShellExecute = true,
            };
            if (string.IsNullOrEmpty(workingDirectory)) {
                procInfo.WorkingDirectory = workingDirectory;
            }

            Process process = new() {
                StartInfo = procInfo
            };

            process.EnableRaisingEvents = true;

            // デフォルトのプロセス終了イベントハンドラーを設定
            process.Exited += new EventHandler(ProcessExited);

            process.Start();
            if (process == null) {
                return null;
            }
            // プロセスとファイル名の対応を保持
            processOpenedFileHashTable.Add(process, tempFileName);

            // プロセスリストにプロセスを追加
            processList.Add(process);

            // 事後処理を実行
            afterOpen(process);

            return process;
        }


        // プロセス終了イベントを処理する
        private static void ProcessExited(object? sender, EventArgs e) {
            // System.Windows.MessageBox.Show("プロセス終了");
            if (sender == null) {
                return;
            }
            // プロセス終了時にItemに開いた内容を保存
            Process? process = (Process)sender;

            // ファイル名を取得
            string? tempFileName = (string?)processOpenedFileHashTable[process];
            if (tempFileName != null) {
                // テンポラリファイルを削除
                File.Delete(tempFileName);
                // ハッシュテーブルから削除
                processOpenedFileHashTable.Remove(process);
            }
            // プロセスリストのプロセスを削除
            processList.Remove(process);
        }


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
            Process process = new() {
                StartInfo = procInfo
            };
            //プロセス終了時のイベントハンドラーを設定
            process.EnableRaisingEvents = true;
            process.Exited += new EventHandler(ContentProcessExited);

            process.Start();
            if (process != null) {
                // プロセスとファイル名の対応を保持
                processOpenedFileHashTable.Add(process, tempFileName);
                // プロセスとクローズ後の処理を保持
                contentWriterProcessAfterCloseHashTable.Add(process, afterClose);
                // プロセスリストにプロセスを追加
                processList.Add(process);
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
            ((Action<string>?)contentWriterProcessAfterCloseHashTable[process])?.Invoke(content);

        }

        public static void OpenFile(string contentFilePath, bool openAsNew = false) {

            // 新規として開く場合はテンポラリディレクトリにファイルをコピーする
            if (openAsNew) {
                // item.Contentがディレクトリの場合はメッセージを表示して終了
                if (Directory.Exists(contentFilePath)) {
                    throw new Exception("Cannot Open Directory As NewFile");
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
            Process? process = Process.Start(proc);
            if (process == null) {
                return;
            }
            // プロセスリストにプロセスを追加
            processList.Add(process);
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
            if (process == null) {
                return;
            }
            // プロセスリストにプロセスを追加
            processList.Add(process);
        }


        public static void OpenClipboardItemContent(ContentItemWrapper item) {

            ProcessUtil.OpenTempTextFile(item.Content, (process) => { },
            (content) => {
                // プロセス終了時にItemに開いた内容を保存
                item.Content = content;
                item.Save();
            });

        }

        public static void OpenClipboardItemFile(ContentItemWrapper item, bool openAsNew = false) {
            // FilePathが存在しない場合かつBase64Stringが存在する場合はByte配列を取得
            if (string.IsNullOrEmpty(item.FilePath)) {
                // BitmapImageがNullでない場合はファイルを開く
                if (item.BitmapImage != null) {
                    ProcessUtil.OpenBitmapImage(item.BitmapImage);
                }
            } else {
                ProcessUtil.OpenFile(item.FilePath, openAsNew);
            }
        }
    }
}
