using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppCommon.Model;

namespace WpfAppCommon.Factory {
    public interface IClipboardProcessController {
        public void OpenItem(ClipboardItem item, bool openAsNew = false);

    }
}
