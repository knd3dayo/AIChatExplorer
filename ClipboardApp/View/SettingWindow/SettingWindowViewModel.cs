
using System.Text;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Model;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.SettingWindow {
    public class SettingWindowViewModel : ObservableObject {
        public enum PythonExecModeEnum {
            None,
            PythonNet,
        }

        // MonitorTargetAppNames
        public string MonitorTargetAppNames {
            get {
                return WpfAppCommon.Properties.Settings.Default.MonitorTargetAppNames;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.MonitorTargetAppNames = value;
                OnPropertyChanged(nameof(MonitorTargetAppNames));
            }
        }
        // PythonDLLのパス
        public string PythonDllPath {
            get {
                return WpfAppCommon.Properties.Settings.Default.PythonDllPath;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.PythonDllPath = value;
                OnPropertyChanged(nameof(PythonDllPath));
            }
        }
        public bool PythonExecMode {
            get {
                int mode = WpfAppCommon.Properties.Settings.Default.PythonExecution;
                if (mode == 0) {
                    return false;
                } else {
                    return true;
                }
            }
            set {
                int mode = value ? 1 : 0;
                WpfAppCommon.Properties.Settings.Default.PythonExecution = mode;
                OnPropertyChanged(nameof(PythonExecMode));
                // 関連項目の表示/非表示を更新
                OnPropertyChanged(nameof(UsePythonVisibility));
                OnPropertyChanged(nameof(UseOpenAIVisibility));
                OnPropertyChanged(nameof(AzureOpenAIVisibility));
                OnPropertyChanged(nameof(UseSpacyVisibility));


            }
        }
        // OpenAIを使用するかどうか
        public bool UseOpenAI {
            get {
                return WpfAppCommon.Properties.Settings.Default.UseOpenAI;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.UseOpenAI = value;
                OnPropertyChanged(nameof(UseOpenAI));
                // 関連項目の表示/非表示を更新
                OnPropertyChanged(nameof(UseOpenAIVisibility));
                OnPropertyChanged(nameof(AzureOpenAIVisibility));
                OnPropertyChanged(nameof(UseSpacyVisibility));
            }
        }
        // Azure OpenAIを使用するかどうか
        public bool AzureOpenAI {
            get {
                return WpfAppCommon.Properties.Settings.Default.AzureOpenAI;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.AzureOpenAI = value;
                OnPropertyChanged(nameof(AzureOpenAI));
                // 関連項目の表示/非表示を更新
                OnPropertyChanged(nameof(AzureOpenAIVisibility));
            }
        }
        // OpenAIのAPIキー
        public string OpenAIKey {
            get {
                return WpfAppCommon.Properties.Settings.Default.OpenAIKey;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.OpenAIKey = value;
                OnPropertyChanged(nameof(OpenAIKey));
            }
        }
        // OpenAICompletionModel
        public string OpenAICompletionModel {
            get {
                return WpfAppCommon.Properties.Settings.Default.OpenAICompletionModel;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.OpenAICompletionModel = value;
                OnPropertyChanged(nameof(OpenAICompletionModel));
            }
        }
        // OpenAIEmbeddingModel
        public string OpenAIEmbeddingModel {
            get {
                return WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingModel;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingModel = value;
                OnPropertyChanged(nameof(OpenAIEmbeddingModel));
            }
        }

        // Azure OpenAIのエンドポイント
        public string AzureOpenAIEndpoint {
            get {
                return WpfAppCommon.Properties.Settings.Default.AzureOpenAIEndpoint;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.AzureOpenAIEndpoint = value;
                OnPropertyChanged(nameof(AzureOpenAIEndpoint));
            }
        }
        // OpenAICompletionBaseURL
        public string OpenAICompletionBaseURL {
            get {
                return WpfAppCommon.Properties.Settings.Default.OpenAICompletionBaseURL;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.OpenAICompletionBaseURL = value;
                OnPropertyChanged(nameof(OpenAICompletionBaseURL));
            }
        }
        // OpenAIEmbeddingBaseURL
        public string OpenAIEmbeddingBaseURL {
            get {
                return WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingBaseURL;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.OpenAIEmbeddingBaseURL = value;
                OnPropertyChanged(nameof(OpenAIEmbeddingBaseURL));
            }
        }
        // UseOCR
        public bool UseOCR {
            get {
                return WpfAppCommon.Properties.Settings.Default.UseOCR;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.UseOCR = value;
                OnPropertyChanged(nameof(UseOCR));
                // 関連項目の表示/非表示を更新
                OnPropertyChanged(nameof(UseOCRVisibility));
            }
        }
        // TesseractExePath
        public string TesseractExePath {
            get {
                return WpfAppCommon.Properties.Settings.Default.TesseractExePath;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.TesseractExePath = value;
                OnPropertyChanged(nameof(TesseractExePath));
            }
        }

        // UseSpacy
        public bool UseSpacy {
            get {
                return WpfAppCommon.Properties.Settings.Default.UseSpacy;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.UseSpacy = value;
                OnPropertyChanged(nameof(UseSpacy));
                // 関連項目の表示/非表示を更新
                OnPropertyChanged(nameof(UseSpacyVisibility));

            }
        }
        // SpacyModel

        public string SpacyModel {
            get {
                return WpfAppCommon.Properties.Settings.Default.SpacyModel;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.SpacyModel = value;
                OnPropertyChanged(nameof(SpacyModel));
            }
        }

        // AutoMergeItemsBySourceApplicationTitle
        public bool AutoMergeItemsBySourceApplicationTitle {
            get {
                return WpfAppCommon.Properties.Settings.Default.AutoMergeItemsBySourceApplicationTitle;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.AutoMergeItemsBySourceApplicationTitle = value;
                OnPropertyChanged(nameof(AutoMergeItemsBySourceApplicationTitle));
            }
        }

        // AutoTag
        public bool AutoTag {
            get {
                return WpfAppCommon.Properties.Settings.Default.AutoTag;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.AutoTag = value;
                OnPropertyChanged(nameof(AutoTag));
            }
        }

        // AutoDescription
        public bool AutoDescription {
            get {
                return WpfAppCommon.Properties.Settings.Default.AutoDescription;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.AutoDescription = value;
                OnPropertyChanged(nameof(AutoDescription));
            }
        }
        // UserMaskedDataInOpenAI
        public bool UserMaskedDataInOpenAI {
            get {
                return WpfAppCommon.Properties.Settings.Default.UserMaskedDataInOpenAI;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.UserMaskedDataInOpenAI = value;
                OnPropertyChanged(nameof(UserMaskedDataInOpenAI));
            }
        }
        // VectorDBURL
        public string VectorDBURL {
            get {
                return WpfAppCommon.Properties.Settings.Default.VectorDBURL;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.VectorDBURL = value;
                OnPropertyChanged(nameof(VectorDBURL));
            }
        }
        // SourceDocumentURL
        public string SourceDocumentURL {
            get {
                return WpfAppCommon.Properties.Settings.Default.SourceDocumentURL;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.SourceDocumentURL = value;
                OnPropertyChanged(nameof(SourceDocumentURL));
            }
        }
        // BackupGeneration
        public int BackupGeneration {
            get {
                return WpfAppCommon.Properties.Settings.Default.BackupGeneration;
            }
            set {
                WpfAppCommon.Properties.Settings.Default.BackupGeneration = value;
                OnPropertyChanged(nameof(BackupGeneration));
            }
        }
        // 表示/非表示の制御
        // 
        public Visibility UsePythonVisibility {
            get {
                if (PythonExecMode == true) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        // OpenAIが有効かどうか
        public Visibility UseOpenAIVisibility {
            get {
                if (UseOpenAI == true && PythonExecMode == true) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        // Azure OpenAIが有効かどうか
        public Visibility AzureOpenAIVisibility {
            get {
                if (AzureOpenAI == true && UseOpenAI == true && PythonExecMode == true) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        // Spacyが有効かどうか
        public Visibility UseSpacyVisibility {
            get {
                if (UseSpacy == true && PythonExecMode == true) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        // UseOCRが有効かどうか
        public Visibility UseOCRVisibility {
            get {
                if (UseOCR == true && PythonExecMode == true) {
                    return Visibility.Visible;
                } else {
                    return Visibility.Collapsed;
                }
            }
        }
        // 設定をチェックする処理
        public string CheckSetting() {
            StringBuilder stringBuilder = new StringBuilder();
            if (PythonExecMode == true) {
                bool pythonOK = true;
                stringBuilder.AppendLine("Pythonの設定チェック...");

                if (string.IsNullOrEmpty(PythonDllPath)) {
                    stringBuilder.AppendLine("[NG]:PythonDLLのパスが設定されていません");
                    pythonOK = false;
                } else {
                    stringBuilder.AppendLine("[OK]:PythonDLLのパスが設定されています");
                    if (System.IO.File.Exists(PythonDllPath) == false) {
                        stringBuilder.AppendLine("[NG]:PythonDLLのパスが存在しません");
                        pythonOK = false;
                    } else {
                        stringBuilder.AppendLine("[OK]:PythonDLLのパスが存在します");

                    }
                }
                if (pythonOK == true) {
                    // TestPythonを実行
                    stringBuilder.AppendLine("Pythonスクリプトをテスト実行...");
                    TestResult result = TestPython();
                    stringBuilder.AppendLine(result.Message);
                    // TestExtractTextを実行
                    stringBuilder.AppendLine("テキスト抽出のテスト実行...");
                    result = TestExtractText();
                    stringBuilder.AppendLine(result.Message);

                }

            }
            if (UseOpenAI == true) {
                bool openAIOK = true;
                stringBuilder.AppendLine("OpenAIの設定チェック...");
                if (string.IsNullOrEmpty(OpenAIKey)) {
                    stringBuilder.AppendLine("[NG]:OpenAIのAPIキーが設定されていません");
                    openAIOK = false;
                } else {
                    stringBuilder.AppendLine("[OK]:OpenAIのAPIキーが設定されています");
                }
                if (string.IsNullOrEmpty(OpenAICompletionModel)) {
                    stringBuilder.AppendLine("[NG]:OpenAIのCompletionModelが設定されていません");
                    openAIOK = false;
                } else {
                    stringBuilder.AppendLine("[OK]:OpenAIのCompletionModelが設定されています");
                }
                if (string.IsNullOrEmpty(OpenAIEmbeddingModel)) {
                    stringBuilder.AppendLine("[NG]:OpenAIのEmbeddingModelが設定されていません");
                    openAIOK = false;
                } else {
                    stringBuilder.AppendLine("[OK]:OpenAIのEmbeddingModelが設定されています");
                }
                if (string.IsNullOrEmpty(VectorDBURL)) {
                    stringBuilder.AppendLine("[NG]:ベクトルDBの格納先が設定されていません");
                    openAIOK = false;
                } else {
                    stringBuilder.AppendLine("[OK]:ベクトルDBの格納先が設定されています");
                }   
                if (AzureOpenAI == true) {
                    stringBuilder.AppendLine("Azure OpenAIの設定チェック...");
                    if (string.IsNullOrEmpty(AzureOpenAIEndpoint)) {
                        stringBuilder.AppendLine("Azure OpenAIのエンドポイントが設定されていないためBaseURL設定をチェック");
                        if (string.IsNullOrEmpty(OpenAICompletionBaseURL) || string.IsNullOrEmpty(OpenAIEmbeddingBaseURL)) {
                            stringBuilder.AppendLine("[NG]:Azure OpenAIのエンドポイント、BaseURLのいずれかを設定してください");
                            openAIOK = false;
                        }
                    } else {
                        if (string.IsNullOrEmpty(OpenAICompletionBaseURL) == false || string.IsNullOrEmpty(OpenAIEmbeddingBaseURL) == false) {
                            stringBuilder.AppendLine("[NG]:Azure OpenAIのエンドポイント、BaseURLのどちらかのみ設定してください");
                            openAIOK = false;
                        }
                    }
                }

                if (openAIOK == true) {
                    // TestOpenAIを実行
                    stringBuilder.AppendLine("OpenAIのテスト実行...");
                    TestResult result = TestOpenAI();
                    stringBuilder.AppendLine(result.Message);
                    // TestLangChainを実行
                    stringBuilder.AppendLine("LangChainのテスト実行...");
                    result = TestLangChain();
                    stringBuilder.AppendLine(result.Message);

                }
            }
            if (UseSpacy == true) {
                bool spacyOK = true;
                stringBuilder.AppendLine("Spacyが使用可能かチェック...");
                if (string.IsNullOrEmpty(SpacyModel)) {
                    stringBuilder.AppendLine("[NG]:Spacyのモデルが設定されていません");
                    spacyOK = false;
                }
                if (spacyOK == true) {
                    stringBuilder.AppendLine("[OK]:設定されています");
                    // TestSpacyを実行
                    stringBuilder.AppendLine("Spacyのテスト実行...");
                    TestResult result = TestSpacy();
                    stringBuilder.AppendLine(result.Message);

                }
                
            }
            // UseOCRが有効な場合
            if (UseOCR == true) {
                bool ocrOK = true;
                stringBuilder.AppendLine("OCRの設定チェック...");
                if (string.IsNullOrEmpty(TesseractExePath)) {
                    stringBuilder.AppendLine("[NG]:Tesseractのパスが設定されていません");
                    ocrOK = false;
                } else {
                    stringBuilder.AppendLine("[OK]:Tesseractのパスが設定されています");
                    if (System.IO.File.Exists(TesseractExePath) == false) {
                        stringBuilder.AppendLine("[NG]:Tesseractのパスが存在しません");
                        ocrOK = false;
                    } else {
                        stringBuilder.AppendLine("[OK]:Tesseractのパスが存在します");

                    }
                }
                if (ocrOK == true) {
                    // TestOCRを実行
                    stringBuilder.AppendLine("OCRのテスト実行...");
                    TestResult result = TestOCR();
                    stringBuilder.AppendLine(result.Message);
                }
            }
            return stringBuilder.ToString();
        }
        private class TestResult {
            public bool Result { get; set; } = false;
            public string Message { get; set; } = "";
        }

        private TestResult TestPython() {
            TestResult testResult = new TestResult();
            PythonExecutor.Init(PythonDllPath);
            try {
                string result = PythonExecutor.PythonFunctions.HelloWorld();
                if ( result != "Hello World") {
                    testResult.Message = "[NG]:Pythonの実行に失敗しました。";
                    testResult.Result = false;

                } else {
                    testResult.Message = "[OK]:Pythonの実行が可能です。";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = "[NG]:Pythonの実行に失敗しました。" + ex.Message;
                testResult.Result = false;
            }
            return testResult;
        }
        private TestResult TestExtractText() {
            TestResult testResult = new TestResult();
            PythonExecutor.Init(PythonDllPath);
            try {
                string result = PythonExecutor.PythonFunctions.ExtractText("TestData/extract_test.txt");
                if (result != "Hello World!") {
                    testResult.Message = "[NG]:テキストファイルからのテキスト抽出に失敗しました。";
                    testResult.Result = false;

                } else {
                    testResult.Message = "[OK]:テキストファイルからのテキスト抽出が可能です。";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = "[NG]:テキストファイルからのテキスト抽出に失敗しました。" + ex.Message;
                testResult.Result = false;
            }
            return testResult;

        }
        private TestResult TestOCR() {
            TestResult testResult = new TestResult();
            PythonExecutor.Init(PythonDllPath);
            try {
                string result = PythonExecutor.PythonFunctions.ExtractTextFromImage(
                    new System.Drawing.Bitmap("TestData/extract_test.png"), TesseractExePath);
                if (result != "Hello World!") {
                    testResult.Message = "[NG]:画像ファイルからのテキスト抽出に失敗しました。";
                    testResult.Result = false;

                } else {
                    testResult.Message = "[OK]:画像ファイルからのテキスト抽出が可能です。";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = "[NG]:画像ファイルからのテキスト抽出に失敗しました。" + ex.Message;
                testResult.Result = false;
            }
            return testResult;
        }
        private TestResult TestOpenAI() {
            TestResult testResult = new TestResult();
            PythonExecutor.Init(PythonDllPath);
            try {
                string resultString = PythonExecutor.PythonFunctions.OpenAIChat("Hello", []).Response;
                if (string.IsNullOrEmpty(resultString)) {
                    testResult.Message = "[NG]:OpenAIの実行に失敗しました。";
                    testResult.Result = false;
                } else {
                    testResult.Message = "[OK]:OpenAIの実行が可能です。";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = "[NG]:OpenAIの実行に失敗しました。" + ex.Message;
                testResult.Result = false;
            }
            return testResult;
        }
        // TestLangChain
        private TestResult TestLangChain() {
            TestResult testResult = new TestResult();
            PythonExecutor.Init(PythonDllPath);
            try {

                ChatResult result = PythonExecutor.PythonFunctions.LangChainChat("Hello", []);
                if (string.IsNullOrEmpty(result.Response)) {
                    testResult.Message = "[NG]:LangChainの実行に失敗しました。";
                    testResult.Result = false;
                } else {
                    testResult.Message = "[OK]:LangChainの実行が可能です。";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = "[NG]:LangChainの実行に失敗しました。" + ex.StackTrace;
                testResult.Result = false;
            }
            return testResult;
        }

        private TestResult TestSpacy() {
            TestResult testResult = new TestResult();
            PythonExecutor.Init(PythonDllPath);
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("def execute(input_str):");
            stringBuilder.AppendLine("    import spacy");
            stringBuilder.AppendLine("    nlp = spacy.load(\"" + SpacyModel  + "\")");
            stringBuilder.AppendLine("    doc = nlp(input_str)");
            stringBuilder.AppendLine("    return doc.text" );


            try {
                string resultString = PythonExecutor.PythonFunctions.RunScript(stringBuilder.ToString(), "Hello World!");
                if (string.IsNullOrEmpty(resultString)) {
                    testResult.Message = "[NG]:Spacyの実行に失敗しました。";
                    testResult.Result = false;
                } else {
                    testResult.Message = "[OK]:Spacyの実行が可能です。";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = "[NG]:Spacyの実行に失敗しました。" + ex.Message;
                testResult.Result = false;
            }
            return testResult;
        }

        // プログレスインジケーターを表示するかどうか
        private bool isIndeterminate = false;
        public bool IsIndeterminate {
            get {
                return isIndeterminate;
            }
            set {
                isIndeterminate = value;
                OnPropertyChanged(nameof(IsIndeterminate));
            }
        }
        // CheckCommand
        public SimpleDelegateCommand CheckCommand => new(async (parameter) => {
            // 実行するか否かメッセージダイアログを表示する、
            string message = "[OK]をクリックすると設定チェックを行います。\n";
            if (UseOpenAI) {
                message += "OpenAIの設定チェックの際には実際にOpenAI APIを呼び出しますのでトークンを消費します.\n";
            }
            message += "実行しますか？";

            MessageBoxResult result = MessageBox.Show(message, "確認", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) {
                return;
            }
            try {
                IsIndeterminate = true;
                string resultString = "";
                await Task.Run(() => {
                    resultString = CheckSetting();
                });
                IsIndeterminate = false;
                // 結果をTestResultWindowで表示
                TestResultWindow testResultWindow = new();
                TestResultWindowViewModel testResultWindowViewModel = (TestResultWindowViewModel)testResultWindow.DataContext;
                testResultWindowViewModel.LogText = resultString;
                testResultWindow.ShowDialog();

            } finally {
                IsIndeterminate = false;
            }
        });
        // SaveCommand
        public SimpleDelegateCommand SaveCommand => new((parameter) => {
            WpfAppCommon.Properties.Settings.Default.Save();
            MessageBox.Show("設定を保存しました。アプリケーションを再起動してください");
            // Windowを閉じる
            if (parameter is Window window) {
                window.Close();
            }
        });

        // CancelCommand
        public SimpleDelegateCommand CancelCommand => new((parameter) => {
            WpfAppCommon.Properties.Settings.Default.Reload();
            Tools.Info("設定をキャンセルしました");
            // Windowを閉じる
            if (parameter is Window window) {
                window.Close();
            }
        });
    }
}
