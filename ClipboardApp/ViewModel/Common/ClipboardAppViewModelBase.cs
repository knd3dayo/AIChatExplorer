using System.Windows;
using System.Windows.Controls;
using ClipboardApp.Model;
using QAChat.Resource;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel {
    public class ClipboardAppViewModelBase : CommonViewModelBase {

        // CommonStringResources
        public CommonStringResources StringResources {
            get {
                // 文字列リソースの言語設定
                CommonStringResources.Lang = ClipboardAppConfig.Instance.ActualLang;
                return CommonStringResources.Instance;
            }
        }
    }
}
