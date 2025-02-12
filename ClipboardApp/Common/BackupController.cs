using System.IO;
using ClipboardApp.ViewModel.Settings;

namespace ClipboardApp.Common {
    class BackupController {
        public static void BackupNow() {
            string backupDir = Path.Combine(ClipboardAppConfig.Instance.AppDataFolder, "backup");
            int backupGenerations = Properties.Settings.Default.BackupGeneration;
            List<string> backupFiles = new List<string>();

            // バックアップ処理の初期化
            // バックアップ先のリポジトリのパスがない場合はディレクトリ作成を行い、リポジトリを初期化する
            if (!Directory.Exists(backupDir)) {
                Directory.CreateDirectory(backupDir);

            }
            // BackupFilesにバックアップ対象ファイルを追加
            backupFiles.Add("clipboard.db");
            backupFiles.Add("clipboard-log.db");

            if (backupDir == null) {
                return;
            }
            for (int i = backupGenerations; i > 0; i--) {
                // バックアップ先ディレクトリにi番目のディレクトリが存在する場合はi+1番目にリネームする
                if (Directory.Exists(Path.Combine(backupDir, $"{i}"))) {
                    // Path.Combine(BackupDir, $"{i + 1}")が存在する場合は削除
                    if (Directory.Exists(Path.Combine(backupDir, $"{i + 1}"))) {
                        Directory.Delete(Path.Combine(backupDir, $"{i + 1}"), true);
                    }
                    Directory.Move(Path.Combine(backupDir, $"{i}"), Path.Combine(backupDir, $"{i + 1}"));

                }
            }
            // 1番目のバックアップディレクトリを作成
            if (!Directory.Exists(Path.Combine(backupDir, "1"))) {
                Directory.CreateDirectory(Path.Combine(backupDir, "1"));
            }
            // バックアップ対象ファイルをコピー
            foreach (var file in backupFiles) {
                if (!File.Exists(file)) {
                    continue;
                }
                File.Copy(file, Path.Combine(backupDir, "1", file), true);
            }
            // バックアップ世代を超えるディレクトリを削除
            if (Directory.Exists(Path.Combine(backupDir, $"{backupGenerations + 1}"))) {
                Directory.Delete(Path.Combine(backupDir, $"{backupGenerations + 1}"), true);
            }

        }
    }
}
