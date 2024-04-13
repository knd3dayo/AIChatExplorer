using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WK.Libraries.SharpClipboardNS;
using WpfApp1.Utils;
using System.IO;

namespace WpfApp1.Model {
    public class AutoProcessCommand {

        // 自動処理でテキストを抽出」を実行するコマンド
        public static ClipboardItem ExtractTextCommandExecute(ClipboardItem clipboardItem) {

            if (clipboardItem.ContentType != SharpClipboard.ContentTypes.Files) {
                throw new ThisApplicationException("ファイル以外のコンテンツはテキストを抽出できません");
            }
            string path = clipboardItem.Content;
            string text = PythonExecutor.PythonFunctions.ExtractText(clipboardItem.Content);
            clipboardItem.Content = text;
            // タイプをテキストに変更
            clipboardItem.ContentType = SharpClipboard.ContentTypes.Text;
            MainWindowViewModel.StatusText.Text = $"{path}のテキストを抽出しました";
            return clipboardItem;

        }

        // 自動処理でデータをマスキング」を実行するコマンド
        public static ClipboardItem MaskDataCommandExecute(ClipboardItem clipboardItem) {
            if (MainWindowViewModel.Instance == null) {
                return clipboardItem;
            }
            if (clipboardItem.ContentType != SharpClipboard.ContentTypes.Text) {
                throw new ThisApplicationException("テキスト以外のコンテンツはマスキングできません");
            }
            Dictionary<string, List<string>> maskPatterns = new Dictionary<string, List<string>>();
            string result = PythonExecutor.PythonFunctions.MaskData(clipboardItem.Content);
            clipboardItem.Content = result;

            MainWindowViewModel.StatusText.Text = "データをマスキングしました";
            return clipboardItem;
        }
        // 自動実行でPythonスクリプトを実行するコマンド
        public static void RunPythonScriptCommandExecute(ScriptItem scriptItem, ClipboardItem clipboardItem) {

            PythonExecutor.PythonFunctions.RunScript(scriptItem, clipboardItem);
            MainWindowViewModel.StatusText.Text = "Pythonスクリプトを実行しました";

        }
        // 自動処理でファイルパスをフォルダとファイル名に分割するコマンド
        public static void SplitFilePathCommandExecute(ClipboardItem clipboardItem) {

            if (clipboardItem.ContentType != SharpClipboard.ContentTypes.Files) {
                throw new ThisApplicationException("ファイル以外のコンテンツはファイルパスを分割できません");
            }
            string path = clipboardItem.Content;
            if (string.IsNullOrEmpty(path)) {
                // ファイルパスをフォルダ名とファイル名に分割
                string? folderPath = Path.GetDirectoryName(path);
                if (folderPath == null) {
                    throw new ThisApplicationException("フォルダパスが取得できません");
                }
                string? fileName = Path.GetFileName(path);
                clipboardItem.Content = folderPath + "\n" + fileName;
                // ContentTypeをTextに変更
                clipboardItem.ContentType = SharpClipboard.ContentTypes.Text;
                // StatusTextにメッセージを表示
                MainWindowViewModel.StatusText.Text = "ファイルパスをフォルダ名とファイル名に分割しました";
            }
        }

        public static void CreateAutoDescription(ClipboardItem item) {
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
        

    }
}
