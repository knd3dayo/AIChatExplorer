using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Properties;

namespace WpfApp1
{
    internal class BackupController
    {
        public static int BackupGenerations { get; set; }
        public static string? BackupDir { get; set; }

        public static List<string> BackupFiles { get; set; } = new List<string>();
        public static void Init()
        {
            string backupDir = "backup";
            int backupGenerations = Properties.Settings.Default.BACKUP_GENERATIONS;

            // バックアップ処理の初期化
            // バックアップ先のリポジトリのパスがない場合はディレクトリ作成を行い、リポジトリを初期化する
            if (!System.IO.Directory.Exists(backupDir))
            {
                System.IO.Directory.CreateDirectory(backupDir);

            }
            BackupGenerations = backupGenerations;
            BackupDir = backupDir;
            // BackupFilesにバックアップ対象ファイルを追加
            BackupFiles.Add("clipboard.db");
            BackupFiles.Add("clipboard-log.db");

            // バックアップ処理を開始
            BackupNow();

        }
        public static void BackupNow()
        {
            if (BackupDir == null)
            {
                return;
            }
            for (int i = BackupGenerations; i > 0; i--)
            {
                // バックアップ先ディレクトリにi番目のディレクトリが存在する場合はi+1番目にリネームする
                if (Directory.Exists(Path.Combine(BackupDir, $"{i}")))
                {
                    // Path.Combine(BackupDir, $"{i + 1}")が存在する場合は削除
                    if (Directory.Exists(Path.Combine(BackupDir, $"{i + 1}")))
                    {
                        Directory.Delete(Path.Combine(BackupDir, $"{i + 1}"), true);
                    }
                    Directory.Move(Path.Combine(BackupDir, $"{i}"), Path.Combine(BackupDir, $"{i + 1}"));

                }
            }
            // 1番目のバックアップディレクトリを作成
            if (!Directory.Exists(Path.Combine(BackupDir, "1")))
            { 
                Directory.CreateDirectory(Path.Combine(BackupDir, "1"));
            }
            // バックアップ対象ファイルをコピー
            foreach (var file in BackupFiles)
            {
                if (!File.Exists(file))
                {
                    continue;
                }
                File.Copy(file, Path.Combine(BackupDir, "1", file), true);
            }
            // バックアップ世代を超えるディレクトリを削除
            if (Directory.Exists(Path.Combine(BackupDir, $"{BackupGenerations + 1}")))
            {
                Directory.Delete(Path.Combine(BackupDir, $"{BackupGenerations + 1}"), true);
            }

        }
    }
}
