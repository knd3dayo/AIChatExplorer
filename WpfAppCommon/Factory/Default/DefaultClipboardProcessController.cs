using System.Diagnostics;
using System.IO;
using System.Windows.Media.Imaging;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Factory.Default {
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
                ProcessUtil.OpenFile(clipboardItemFile.FilePath);
            }
        }

        public void OpenClipboardItemImage(ClipboardItem item) {

            // ClipboardItemのClipboardItemImages毎に処理を実行
            foreach (var clipboardItemImage in item.ClipboardItemImages) {

                BitmapImage? bitmapImage = clipboardItemImage.BitmapImage;
                if (bitmapImage == null) {
                    continue;
                }
                // 画像を表示
                ProcessUtil.OpenBitmapImage(bitmapImage);
            }
        }
    }
}
