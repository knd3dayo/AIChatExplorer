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

        #region 開発中機能関連の設定
        // TesseractExePath
        public string TesseractExePath {
            get {
                return AIChatExplorerConfig.Instance.TesseractExePath;
            }
            set {
                AIChatExplorerConfig.Instance.TesseractExePath = value;
                OnPropertyChanged(nameof(TesseractExePath));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // UseSpacy
        public bool UseSpacy {
            get {
                return AIChatExplorerConfig.Instance.UseSpacy;
            }
            set {
                AIChatExplorerConfig.Instance.UseSpacy = value;
                OnPropertyChanged(nameof(UseSpacy));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // SpacyModel
        public string SpacyModel {
            get {
                return AIChatExplorerConfig.Instance.SpacyModel;
            }
            set {
                AIChatExplorerConfig.Instance.SpacyModel = value;
                OnPropertyChanged(nameof(SpacyModel));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // UserMaskedDataInOpenAI
        public bool UserMaskedDataInOpenAI {
            get {
                return AIChatExplorerConfig.Instance.UserMaskedDataInOpenAI;
            }
            set {
                AIChatExplorerConfig.Instance.UserMaskedDataInOpenAI = value;
                OnPropertyChanged(nameof(UserMaskedDataInOpenAI));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoTag
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

        #endregion





    }
}
