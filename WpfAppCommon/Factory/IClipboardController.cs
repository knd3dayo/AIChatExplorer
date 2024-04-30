using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Factory {
    public interface IClipboardController {

        public void Start(Action<ActionMessage> afterClipboardChanged);

        public void Stop();

        public void SetDataObject(ClipboardItem item);
    }
}
