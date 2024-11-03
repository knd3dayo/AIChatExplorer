using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAChat.Resource {
    public partial class CommonStringResourcesEn : CommonStringResources  {

        #region QAChatControl
        // Export all chat contents
        public override string ExportAllChatContents { get; } = "Export All Chat Contents";
        // Copy selected chat contents
        public override string CopySelectedChatContents { get; } = "Copy Selected Chat Contents";
        // Copy all chat contents
        public override string CopyAllChatContents { get; } = "Copy All Chat Contents";

        #endregion

    }
}
