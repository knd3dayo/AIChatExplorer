using System.Windows;

namespace AIChatExplorer.ViewModel.Settings {
    public partial class SettingUserControlViewModel {


        public bool EnableDevFeatures {
            get {
                return AIChatExplorerConfig.Instance.EnableDevFeatures;
            }
            set {
                AIChatExplorerConfig.Instance.EnableDevFeatures = value;
                OnPropertyChanged(nameof(EnableDevFeatures));
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        public Visibility EnableDevFeaturesVisibility {
            get {
                if (EnableDevFeatures) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }

        // IsAutoTagEnabled
        public bool AutoTag {
            get {
                return AIChatExplorerConfig.Instance.AutoTag;
            }
            set {
                AIChatExplorerConfig.Instance.AutoTag = value;
                OnPropertyChanged(nameof(AutoTag));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoDescription
        public bool AutoDescription {
            get {
                return AIChatExplorerConfig.Instance.AutoDescription;
            }
            set {
                AIChatExplorerConfig.Instance.AutoDescription = value;
                OnPropertyChanged(nameof(AutoDescription));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoExtractImageWithPyOCR
        public bool AutoExtractImageWithPyOCR {
            get {
                return AIChatExplorerConfig.Instance.AutoExtractImageWithPyOCR;
            }
            set {
                AIChatExplorerConfig.Instance.AutoExtractImageWithPyOCR = value;
                OnPropertyChanged(nameof(AutoExtractImageWithPyOCR));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
    }
}
