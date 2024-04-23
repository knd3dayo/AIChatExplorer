using System.IO;
using ClipboardApp.Properties;

namespace ClipboardApp.Factory.Default {
    class DefaultBackupController : IBackupController {
        public int BackupGenerations { get; set; }
        public string? BackupDir { get; set; }

        public List<string> BackupFiles { get; set; } = new List<string>();
        public DefaultBackupController() {

        }
        public void BackupNow() {
            string backupDir = "backup";
            int backupGenerations = Settings.Default.BackupGeneration;

            // バックアップ処理の初期化
            // バックアップ先のリポジトリのパスがない場合はディレクトリ作成を行い、リポジトリを初期化する
            if (!Directory.Exists(backupDir)) {
                Directory.CreateDirectory(backupDir);

            }
            BackupGenerations = backupGenerations;
            BackupDir = backupDir;
            // BackupFilesにバックアップ対象ファイルを追加
            BackupFiles.Add("clipboard.db");
            BackupFiles.Add("clipboard-log.db");

            if (BackupDir == null) {
                return;
            }
            for (int i = BackupGenerations; i > 0; i--) {
                // バックアップ先ディレクトリにi番目のディレクトリが存在する場合はi+1番目にリネームする
                if (Directory.Exists(Path.Combine(BackupDir, $"{i}"))) {
                    // Path.Combine(BackupDir, $"{i + 1}")が存在する場合は削除
                    if (Directory.Exists(Path.Combine(BackupDir, $"{i + 1}"))) {
                        Directory.Delete(Path.Combine(BackupDir, $"{i + 1}"), true);
                    }
                    Directory.Move(Path.Combine(BackupDir, $"{i}"), Path.Combine(BackupDir, $"{i + 1}"));

                }
            }
            // 1番目のバックアップディレクトリを作成
            if (!Directory.Exists(Path.Combine(BackupDir, "1"))) {
                Directory.CreateDirectory(Path.Combine(BackupDir, "1"));
            }
            // バックアップ対象ファイルをコピー
            foreach (var file in BackupFiles) {
                if (!File.Exists(file)) {
                    continue;
                }
                File.Copy(file, Path.Combine(BackupDir, "1", file), true);
            }
            // バックアップ世代を超えるディレクトリを削除
            if (Directory.Exists(Path.Combine(BackupDir, $"{BackupGenerations + 1}"))) {
                Directory.Delete(Path.Combine(BackupDir, $"{BackupGenerations + 1}"), true);
            }

        }
    }
}
