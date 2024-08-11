using PythonAILib.PythonIF;
using System.Text;
using System.Windows;
using WpfAppCommon.Model;

namespace WpfAppCommon.Control.Settings {
    public partial class SettingUserControlViewModel {


        #region 開発中機能関連の設定
        public Visibility EnableDevFeaturesVisibility {
            get {
                if (ClipboardAppConfig.EnableDevFeatures) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
            set {
                if (value == Visibility.Visible) {
                    ClipboardAppConfig.EnableDevFeatures = true;
                } else {
                    ClipboardAppConfig.EnableDevFeatures = false;
                }
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // EnableDevFeatures
        public bool EnableDevFeatures {
            get {
                return ClipboardAppConfig.EnableDevFeatures;
            }
            set {
                ClipboardAppConfig.EnableDevFeatures = value;
                OnPropertyChanged(nameof(EnableDevFeatures));
                OnPropertyChanged(nameof(EnableDevFeaturesVisibility));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // TesseractExePath
        public string TesseractExePath {
            get {
                return ClipboardAppConfig.TesseractExePath;
            }
            set {
                ClipboardAppConfig.TesseractExePath = value;
                OnPropertyChanged(nameof(TesseractExePath));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // UseSpacy
        public bool UseSpacy {
            get {
                return ClipboardAppConfig.UseSpacy;
            }
            set {
                ClipboardAppConfig.UseSpacy = value;
                OnPropertyChanged(nameof(UseSpacy));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // SpacyModel
        public string SpacyModel {
            get {
                return ClipboardAppConfig.SpacyModel;
            }
            set {
                ClipboardAppConfig.SpacyModel = value;
                OnPropertyChanged(nameof(SpacyModel));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // UserMaskedDataInOpenAI
        public bool UserMaskedDataInOpenAI {
            get {
                return ClipboardAppConfig.UserMaskedDataInOpenAI;
            }
            set {
                ClipboardAppConfig.UserMaskedDataInOpenAI = value;
                OnPropertyChanged(nameof(UserMaskedDataInOpenAI));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoTag
        public bool AutoTag {
            get {
                return ClipboardAppConfig.AutoTag;
            }
            set {
                ClipboardAppConfig.AutoTag = value;
                OnPropertyChanged(nameof(AutoTag));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoDescription
        public bool AutoDescription {
            get {
                return ClipboardAppConfig.AutoDescription;
            }
            set {
                ClipboardAppConfig.AutoDescription = value;
                OnPropertyChanged(nameof(AutoDescription));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoExtractImageWithPyOCR
        public bool AutoExtractImageWithPyOCR {
            get {
                return ClipboardAppConfig.AutoExtractImageWithPyOCR;
            }
            set {
                ClipboardAppConfig.AutoExtractImageWithPyOCR = value;
                OnPropertyChanged(nameof(AutoExtractImageWithPyOCR));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        #endregion
        private TestResult TestSpacy() {
            TestResult testResult = new();
            PythonExecutor.Init(PythonDllPath);
            StringBuilder stringBuilder = new();
            stringBuilder.AppendLine("def execute(input_str):");
            stringBuilder.AppendLine("    import spacy");
            stringBuilder.AppendLine("    nlp = spacy.load(\"" + SpacyModel + "\")");
            stringBuilder.AppendLine("    doc = nlp(input_str)");
            stringBuilder.AppendLine("    return doc.text");


            try {
                string resultString = PythonExecutor.PythonMiscFunctions.RunScript(stringBuilder.ToString(), "Hello World!");
                if (string.IsNullOrEmpty(resultString)) {
                    testResult.Message = "[NG]:Spacyの実行に失敗しました。";
                    testResult.Result = false;
                } else {
                    testResult.Message = "[OK]:Spacyの実行が可能です。";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = "[NG]:Spacyの実行に失敗しました。\n[メッセージ]" + ex.Message + "\n[スタックトレース]" + ex.StackTrace;
                testResult.Result = false;
            }
            return testResult;
        }




    }
}
