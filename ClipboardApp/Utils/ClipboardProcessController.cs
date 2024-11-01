using ClipboardApp.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.Utils {
    /// <summary>
    /// このアプリケーションで開いたプロセスを管理するクラス
    /// ファイルを閉じたときにテキストの内容をContentに保存する
    /// </summary>
    public class ClipboardProcessController {

        public static void OpenClipboardItemContent(ClipboardItem item) {

            ProcessUtil.OpenTempTextFile(item.Content, (process) => { },
            (content) => {
                // プロセス終了時にItemに開いた内容を保存
                item.Content = content;
                item.Save();
            });

        }

        public static void OpenClipboardItemFile(ClipboardItem item, bool openAsNew = false) {

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
