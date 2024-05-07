using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace WpfAppCommon.Utils {
    public class TextSelector {
        public bool SingleLineSelected { get; set; } = false;
        public bool URLSelected { get; set; } = false;
        public bool AngleBracketSelected { get; set; } = false;

        public void SelectText(TextBox editor) {
            // 1行選択の場合は全選択
            if (SingleLineSelected) {
                editor.SelectAll();
                SingleLineSelected = false;
                URLSelected = false;
                return;
            }
            // 複数行選択中でない場合
            if (!editor.SelectedText.Contains('\n')) {
                int pos = editor.SelectionStart;
                // posがTextの長さを超える場合はTextの最後を指定
                if (pos >= editor.Text.Length) {
                    pos = editor.Text.Length - 1;
                }
                int lineStart = editor.Text.LastIndexOf('\n', pos) + 1;

                int lineEnd = editor.Text.IndexOf('\n', pos);
                if (lineEnd == -1) {
                    lineEnd = editor.Text.Length;
                }

                // lineEnd - lineStartが0以下の場合は何もしない
                if (lineEnd - lineStart <= 0) {
                    return;
                }
                // 選択対象文字列
                string selectedText = editor.Text[lineStart..lineEnd];
                // URLの場合はURL選択にする
                int[]? ints = Tools.GetURLPosition(selectedText);
                if (ints != null && URLSelected == false) {
                    lineStart += ints[0];
                    lineEnd = lineStart + ints[1] - ints[0];
                    editor.Select(lineStart, lineEnd - lineStart);
                    URLSelected = true;
                    return;
                }
                // AngleBracketの場合はAngleBracket選択にする
                int[] angleBracketInts = Tools.GetInAngleBracketPosition(selectedText);
                if (angleBracketInts[0] != -1 && AngleBracketSelected == false) {
                    lineStart += angleBracketInts[0];
                    lineEnd = lineStart + angleBracketInts[1] - angleBracketInts[0];
                    editor.Select(lineStart, lineEnd - lineStart);
                    AngleBracketSelected = true;
                    return;
                }
                // EditorTextSelectionを更新
                editor.Select(lineStart, lineEnd - lineStart);
                SingleLineSelected = true;
                URLSelected = false;
                AngleBracketSelected = false;

            }
        }
        // 選択中のテキストをプロセスとして実行
        public void ExecuteSelectedText(TextBox editor) {
            string selectedText = editor.SelectedText;
            if (string.IsNullOrEmpty(selectedText)) {
                return;
            }
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(selectedText) {
                UseShellExecute = true
            };
            try {
                p.Start();
            } catch (Exception ex) {
                Tools.Info("ファイルを実行できませんでした。テキストファイルとして開きます。" + ex.Message);
                OpenTextFile(selectedText);
            }
        }
        private void OpenTextFile(string text) {
            // テキストをテキストファイルに保存して、プロセスを実行
            string tempFileName = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".txt");
            File.WriteAllText(tempFileName, text);
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(tempFileName) {
                UseShellExecute = true
            };
            try {
                p.Start();
            } catch (Exception ex) {
                Tools.Error("ファイルを実行できませんでした:" + ex.Message);
            }
        }
    }
}
