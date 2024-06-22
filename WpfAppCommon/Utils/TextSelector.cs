using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace WpfAppCommon.Utils {
    public class TextSelector {
        public bool SingleLineSelected { get; set; } = false;
        public bool URLSelected { get; set; } = false;
        public bool AngleBracketSelected { get; set; } = false;

        // 最後に選択したテキスト
        public string LastSelectedText { get; set; } = "";

        public void SelectText(TextBox editor) {
            // 最後に選択したテキストと異なる場合は初期状態にする。
            if (editor.SelectedText != LastSelectedText) {
                SingleLineSelected = false;
                URLSelected = false;
                AngleBracketSelected = false;
            }
            // 1行選択状態または複数行選択状態の場合は全選択
            if (SingleLineSelected || editor.SelectedText.Contains('\n')) {
                editor.SelectAll();
                SingleLineSelected = false;
                URLSelected = false;
                // 最後に選択したテキストを更新
                LastSelectedText = editor.SelectedText;
                return;
            } else {
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
                    // 最後に選択したテキストを更新
                    LastSelectedText = editor.SelectedText;
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
                    // 最後に選択したテキストを更新
                    LastSelectedText = editor.SelectedText;
                    return;
                }
                // AngleBracketの場合はAngleBracket選択にする
                int[] angleBracketInts = Tools.GetInAngleBracketPosition(selectedText);
                if (angleBracketInts[0] != -1 && AngleBracketSelected == false) {
                    lineStart += angleBracketInts[0];
                    lineEnd = lineStart + angleBracketInts[1] - angleBracketInts[0];
                    editor.Select(lineStart, lineEnd - lineStart);
                    AngleBracketSelected = true;
                    // 最後に選択したテキストを更新
                    LastSelectedText = editor.SelectedText;
                    return;
                }
                // EditorTextSelectionを更新
                editor.Select(lineStart, lineEnd - lineStart);
                SingleLineSelected = true;
                URLSelected = false;
                AngleBracketSelected = false;
                // 最後に選択したテキストを更新
                LastSelectedText = editor.SelectedText;

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
                LogWrapper.Info("ファイルを実行できませんでした。テキストファイルとして開きます。" + ex.Message);
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
                LogWrapper.Error("ファイルを実行できませんでした:" + ex.Message);
            }
        }
    }
}
