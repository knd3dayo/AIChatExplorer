using System.Windows;
using AIChatExplorer.ViewModel.Settings;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Control.Editor;
using LibUIPythonAI.Utils;

namespace AIChatExplorer.ViewModel.Main {
    public class AppViewModelBase : CommonViewModelBase {

        // CommonStringResources
        public static CommonStringResources StringResources {
            get {
                // 文字列リソースの言語設定
                CommonStringResources.Lang = AIChatExplorerConfig.Instance.ActualLang;
                return CommonStringResources.Instance;
            }
        }

        // テキストを右端で折り返すかどうか
        public bool TextWrapping {
            get {
                return AIChatExplorerConfig.Instance.TextWrapping == System.Windows.TextWrapping.Wrap;
            }
            set {
                if (value) {
                    AIChatExplorerConfig.Instance.TextWrapping = System.Windows.TextWrapping.Wrap;
                } else {
                    AIChatExplorerConfig.Instance.TextWrapping = System.Windows.TextWrapping.NoWrap;
                }
                // Save
                AIChatExplorerConfig.Instance.Save();
                OnPropertyChanged(nameof(TextWrapping));
                if (TextWrapping) {
                    TextWrappingMode = MyTextBox.TextWrappingModeEnum.WrapWithThreshold;
                } else {
                    if (value) {
                        TextWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
                    } else {
                        TextWrappingMode = MyTextBox.TextWrappingModeEnum.NoWrap;
                    }
                }
            }
        }
        // AutoTextWrapping
        public bool AutoTextWrapping {
            get {
                return AIChatExplorerConfig.Instance.AutoTextWrapping;
            }
            set {
                AIChatExplorerConfig.Instance.AutoTextWrapping = value;
                // Save
                AIChatExplorerConfig.Instance.Save();
                OnPropertyChanged(nameof(AutoTextWrapping));
                // CommonViewModelBaseのTextWrappingModeを更新
                if (value) {
                    TextWrappingMode = MyTextBox.TextWrappingModeEnum.WrapWithThreshold;
                } else {
                    if (TextWrapping) {
                        TextWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
                    } else {
                        TextWrappingMode = MyTextBox.TextWrappingModeEnum.NoWrap;
                    }
                }
            }
        }
        // 開発中の機能を有効にするかどうか
        public bool EnableDevFeatures {
            get {
                return AIChatExplorerConfig.Instance.EnableDevFeatures;
            }
            set {
                AIChatExplorerConfig.Instance.EnableDevFeatures = value;
                // Save
                AIChatExplorerConfig.Instance.Save();
                OnPropertyChanged(nameof(EnableDevFeatures));
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
            }
        }
        // 開発中機能の表示
        public Visibility EnableDevFeaturesVisibility => Tools.BoolToVisibility(AIChatExplorerConfig.Instance.EnableDevFeatures);
    }
}
