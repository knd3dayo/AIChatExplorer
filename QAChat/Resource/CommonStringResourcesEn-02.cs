using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QAChat.Resource {
    public partial class CommonStringResourcesEn : CommonStringResources  {


        #region ClipboardItem関連
        // 新規アイテム
        public override string NewItem { get; } = "New Item";

        #endregion

        #region FolderView related
        // Add my folder to target vector DB
        public override string AddMyFolderToTargetVectorDB { get; } = "Add my folder to target vector DB";


        // Input description of this folder here
        public override string InputDescriptionOfThisFolder { get; } = "Input description of this folder here";

        #endregion
    }
}
