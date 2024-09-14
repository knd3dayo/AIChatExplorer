using ClipboardApp.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.Factory.Default {
    /// <summary>
    /// このアプリケーションで開いたプロセスを管理するクラス
    /// ファイルを閉じたときにテキストの内容をContentに保存する
    /// </summary>
    public class DefaultClipboardProcessController : IClipboardProcessController {

        public void OpenClipboardItemContent(ClipboardItem item) {

            ProcessUtil.OpenTempTextFile(item.Content, (process) => { },
            (content) => {
                // プロセス終了時にItemに開いた内容を保存
                item.Content = content;
                item.Save();
            });

        }

        public void OpenClipboardItemFile(ClipboardItem item, bool openAsNew = false) {

            foreach (var clipboardItemFile in item.ClipboardItemFiles) {
                // FilePathが存在しない場合かつBase64Stringが存在する場合はByte配列を取得
                if (string.IsNullOrEmpty(clipboardItemFile.FilePath)) {
                    // BitmapImageがNullでない場合はファイルを開く
                    if (clipboardItemFile.BitmapImage != null) {
                        ProcessUtil.OpenBitmapImage(clipboardItemFile.BitmapImage);
                    }
                }
                ProcessUtil.OpenFile(clipboardItemFile.FilePath);
            }
        }
    }
}
