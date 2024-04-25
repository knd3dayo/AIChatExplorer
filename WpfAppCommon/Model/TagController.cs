using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfAppCommon.Model {
    public class TagController {

        public static IEnumerable<TagItem> GetTagList() {
            return ClipboardAppFactory.Instance.GetClipboardDBController().GetTagList();
        }


    }
}
