using System.Windows;
using PythonAILib.Common;
using QAChat.Resource;

namespace QAChat.Model
{
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
        public TextWrapping TextWrapping {
            get {
                if (QAChatManager.Instance == null) {
                    return TextWrapping.NoWrap;
                }
                return QAChatManager.Instance.ConfigParams.GetTextWrapping();
            }
        }

        // Progress Indicatorの表示状態
        private bool _IsIndeterminate = false;
        public bool IsIndeterminate {
            get {
                return _IsIndeterminate;
            }
            set {
                _IsIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }

    }
}
