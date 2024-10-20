using QAChat.Resource;
using WpfAppCommon.Model;

namespace QAChat.Model {
    public class QAChatViewModelBase : CommonViewModelBase {

        // CommonStringResources
        public CommonStringResources StringResources {
            get {
                // 文字列リソースの言語設定
                PythonAILibManager? libManager = PythonAILibManager.Instance;
                if (libManager == null) {
                    return CommonStringResources.Instance;
                }
                CommonStringResources.Lang = libManager.ConfigParams.GetLang();
                return CommonStringResources.Instance;
            }
        }

    }
}
