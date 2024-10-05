using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;
using WpfAppCommon.ViewModel;

namespace QAChat.Model {
    public class QAChatViewModelBase : CommonViewModelBase {

        // CommonStringResources
        public override CommonStringResources StringResources {
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
