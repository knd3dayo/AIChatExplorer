﻿using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WK.Libraries.SharpClipboardNS;
using WpfApp1.Model;
using WpfApp1.Utils;

namespace WpfApp1 {
    /// <summary>
    /// このアプリケーションで開いたプロセスを管理するクラス
    /// </summary>
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
}
