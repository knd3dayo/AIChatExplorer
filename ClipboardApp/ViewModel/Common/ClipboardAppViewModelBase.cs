using System.Windows;
using ClipboardApp.Model;
using QAChat.Resource;
using WpfAppCommon.Control.Editor;

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

        // テキストを右端で折り返すかどうか
        public bool TextWrapping {
            get {
                return ClipboardAppConfig.Instance.TextWrapping == System.Windows.TextWrapping.Wrap;
            }
            set {
                if (value) {
                    ClipboardAppConfig.Instance.TextWrapping = System.Windows.TextWrapping.Wrap;
                } else {
                    ClipboardAppConfig.Instance.TextWrapping = System.Windows.TextWrapping.NoWrap;
                }
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(TextWrapping));
                if (TextWrapping) {
                    CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.WrapWithThreshold;
                } else {
                    if (value) {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
                    } else {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.NoWrap;
                    }
                }
            }
        }
        // AutoTextWrapping
        public bool AutoTextWrapping {
            get {
                return ClipboardAppConfig.Instance.AutoTextWrapping;
            }
            set {
                ClipboardAppConfig.Instance.AutoTextWrapping = value;
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(AutoTextWrapping));
                // CommonViewModelBaseのTextWrappingModeを更新
                if (value) {
                    CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.WrapWithThreshold;
                } else {
                    if (TextWrapping) {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
                    } else {
                        CommonViewModelBase.TextWrappingMode = MyTextBox.TextWrappingModeEnum.NoWrap;
                    }
                }
            }
        }
        // 開発中の機能を有効にするかどうか
        public bool EnableDevFeatures {
            get {
                return ClipboardAppConfig.Instance.EnableDevFeatures;
            }
            set {
                ClipboardAppConfig.Instance.EnableDevFeatures = value;
                // Save
                ClipboardAppConfig.Instance.Save();
                OnPropertyChanged(nameof(EnableDevFeatures));
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
            }
        }
        // 開発中機能の表示
        public Visibility EnableDevFeaturesVisibility {
            get {
                return ClipboardAppConfig.Instance.EnableDevFeatures ? Visibility.Visible : Visibility.Collapsed;
            }
        }



    }
}
