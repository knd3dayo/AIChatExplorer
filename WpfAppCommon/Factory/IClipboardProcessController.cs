using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppCommon.Model;

namespace WpfAppCommon.Factory {
    public interface IClipboardProcessController {
        public void OpenClipboardItemContent(ClipboardItem item);

        public void OpenClipboardItemFile(ClipboardItem item, bool openAsNew = false);

        // 画像を開く
        public void OpenClipboardItemImage(ClipboardItem item);
    }
}
