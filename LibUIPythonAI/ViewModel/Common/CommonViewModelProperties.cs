using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using LibUIPythonAI.Control.Editor;
using PythonAILib.Common;

namespace LibUIPythonAI.ViewModel.Common {
    public class CommonViewModelProperties : ObservableObject {


        public static CommonViewModelProperties Instance { get; set; } = new CommonViewModelProperties();

        public bool MarkdownView {
            get {
                return PythonAILibManager.Instance.ConfigParams.IsMarkdownView();
            }
            set {
                PythonAILibManager.Instance.ConfigParams.UpdateMarkdownView(value);
                OnPropertyChanged(nameof(MarkdownView));
            }
        }
        private static MyTextBox.TextWrappingModeEnum _textWrappingMode = MyTextBox.TextWrappingModeEnum.Wrap;
        public static MyTextBox.TextWrappingModeEnum TextWrappingMode {
            get { return _textWrappingMode; }
            set { _textWrappingMode = value; }
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


        // テキストを右端で折り返すかどうか
        public bool TextWrapping {
            get {
                PythonAILibManager libManager = PythonAILibManager.Instance;
                return libManager.ConfigParams.GetTextWrapping() == System.Windows.TextWrapping.Wrap;
            }
            set {
                PythonAILibManager.Instance.ConfigParams.UpdateTextWrapping(value ? System.Windows.TextWrapping.Wrap : System.Windows.TextWrapping.NoWrap);
                OnPropertyChanged(nameof(TextWrapping));
                if (AutoTextWrapping) {
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
        // AutoTextWrapping
        public bool AutoTextWrapping {
            get {
                return PythonAILibManager.Instance.ConfigParams.IsAutoTextWrapping();
            }
            set {
                PythonAILibManager.Instance.ConfigParams.UpdateAutoTextWrapping(value);
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
                return PythonAILibManager.Instance.ConfigParams.IsAutoTextWrapping();
            }
            set {
                PythonAILibManager.Instance.ConfigParams.UpdateDevFeaturesEnabled(value);
                // Save
                OnPropertyChanged(nameof(EnableDevFeatures));
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
            }
        }
        // 開発中機能の表示
        public Visibility EnableDevFeaturesVisibility => Utils.Tools.BoolToVisibility(PythonAILibManager.Instance.ConfigParams.IsAutoTextWrapping());

    }
}
