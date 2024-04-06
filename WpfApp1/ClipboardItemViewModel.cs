using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1
{
    public class ClipboardItemViewModel(ClipboardItem clipboardItem)
    {
        // ClipboardItem
        public ClipboardItem ClipboardItem { get; set; } = clipboardItem;
    }
}
