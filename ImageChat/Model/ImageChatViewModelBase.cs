using PythonAILib.Common;
using QAChat.Resource;

namespace ClipboardApp.Model
{
    public class ImageChatViewModelBase : CommonViewModelBase {

        /// <summary>
        /// Gets the common string resources.
        /// </summary>
        public CommonStringResources StringResources {
            get {
                // 言語設定
                CommonStringResources.Lang = PythonAILibManager.Instance.ConfigParams.GetLang();
                return CommonStringResources.Instance;
            }
        }
    }
}
