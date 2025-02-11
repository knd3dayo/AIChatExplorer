using System.Windows;
using LibUIPythonAI.Resource;
using PythonAILib.Common;

namespace LibUIPythonAI.ViewModel {
    public class ChatViewModelBase : CommonViewModelBase {

        // CommonStringResources
        public static CommonStringResources StringResources {
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
                PythonAILibManager? libManager = PythonAILibManager.Instance;
                if (libManager == null) {
                    return TextWrapping.NoWrap;
                }
                return libManager.ConfigParams.GetTextWrapping();
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
        public void UpdateIndeterminate(bool visible) {
            IsIndeterminate = visible;
            // OnPropertyChanged(nameof(IsIndeterminate));
        }
    }
}
