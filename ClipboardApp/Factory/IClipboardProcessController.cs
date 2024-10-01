
using ClipboardApp.Model;

namespace ClipboardApp.Factory {
    public interface IClipboardProcessController {
        public void OpenClipboardItemContent(ClipboardItem item);

        public void OpenClipboardItemFile(ClipboardItem item, bool openAsNew = false);

    }
}
