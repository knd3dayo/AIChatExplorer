using System.Text;
using PythonAILib.PythonIF;

namespace ClipboardApp.Settings {
    public partial class SettingUserControlViewModel {


        #region 開発中機能関連の設定
        // TesseractExePath
        public string TesseractExePath {
            get {
                return ClipboardAppConfig.Instance.TesseractExePath;
            }
            set {
                ClipboardAppConfig.Instance.TesseractExePath = value;
                OnPropertyChanged(nameof(TesseractExePath));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // UseSpacy
        public bool UseSpacy {
            get {
                return ClipboardAppConfig.Instance.UseSpacy;
            }
            set {
                ClipboardAppConfig.Instance.UseSpacy = value;
                OnPropertyChanged(nameof(UseSpacy));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // SpacyModel
        public string SpacyModel {
            get {
                return ClipboardAppConfig.Instance.SpacyModel;
            }
            set {
                ClipboardAppConfig.Instance.SpacyModel = value;
                OnPropertyChanged(nameof(SpacyModel));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // UserMaskedDataInOpenAI
        public bool UserMaskedDataInOpenAI {
            get {
                return ClipboardAppConfig.Instance.UserMaskedDataInOpenAI;
            }
            set {
                ClipboardAppConfig.Instance.UserMaskedDataInOpenAI = value;
                OnPropertyChanged(nameof(UserMaskedDataInOpenAI));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoTag
        public bool AutoTag {
            get {
                return ClipboardAppConfig.Instance.AutoTag;
            }
            set {
                ClipboardAppConfig.Instance.AutoTag = value;
                OnPropertyChanged(nameof(AutoTag));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoDescription
        public bool AutoDescription {
            get {
                return ClipboardAppConfig.Instance.AutoDescription;
            }
            set {
                ClipboardAppConfig.Instance.AutoDescription = value;
                OnPropertyChanged(nameof(AutoDescription));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoExtractImageWithPyOCR
        public bool AutoExtractImageWithPyOCR {
            get {
                return ClipboardAppConfig.Instance.AutoExtractImageWithPyOCR;
            }
            set {
                ClipboardAppConfig.Instance.AutoExtractImageWithPyOCR = value;
                OnPropertyChanged(nameof(AutoExtractImageWithPyOCR));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        #endregion
        private TestResult TestSpacy() {
            TestResult testResult = new();
            PythonExecutor.Init(PythonDllPath, PythonVenvPath, ClipboardAppConfig.Instance.AppDataFolder,
                ClipboardAppConfig.Instance.ProxyURL, ClipboardAppConfig.Instance.NoProxyList);
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
