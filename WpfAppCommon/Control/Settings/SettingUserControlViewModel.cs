
using System.Configuration;
using System.Text;
using System.Windows;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace WpfAppCommon.Control.Settings {
    /// <summary>
    /// 設定画面のViewModel
    /// </summary>
    public partial class SettingUserControlViewModel : MyWindowViewModel {
        // プロパティが変更されたか否か
        private bool isPropertyChanged = false;
        // MonitorTargetAppNames
        public string MonitorTargetAppNames {
            get {
                return ClipboardAppConfig.MonitorTargetAppNames;
            }
            set {
                ClipboardAppConfig.MonitorTargetAppNames = value;
                OnPropertyChanged(nameof(MonitorTargetAppNames));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // PythonDLLのパス
        public string PythonDllPath {
            get {
                return ClipboardAppConfig.PythonDllPath;
            }
            set {
                ClipboardAppConfig.PythonDllPath = value;
                OnPropertyChanged(nameof(PythonDllPath));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        public bool PythonExecMode {
            get {
                int mode = ClipboardAppConfig.PythonExecute;
                if (mode == 0) {
                    return false;
                } else {
                    return true;
                }
            }
            set {
                int mode = value ? 1 : 0;
                ClipboardAppConfig.PythonExecute = mode;
                OnPropertyChanged(nameof(PythonExecMode));
                // 関連項目の表示/非表示を更新
                OnPropertyChanged(nameof(UsePythonVisibility));
                OnPropertyChanged(nameof(UseOpenAIVisibility));
                OnPropertyChanged(nameof(AzureOpenAIVisibility));
                OnPropertyChanged(nameof(UseSpacyVisibility));
                OnPropertyChanged(nameof(UseOCRVisibility));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAIを使用するかどうか
        public bool UseOpenAI {
            get {
                return ClipboardAppConfig.UseOpenAI;
            }
            set {
                ClipboardAppConfig.UseOpenAI = value;
                OnPropertyChanged(nameof(UseOpenAI));
                // 関連項目の表示/非表示を更新
                OnPropertyChanged(nameof(UseOpenAIVisibility));
                OnPropertyChanged(nameof(AzureOpenAIVisibility));
                OnPropertyChanged(nameof(UseSpacyVisibility));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // Azure OpenAIを使用するかどうか
        public bool AzureOpenAI {
            get {
                return ClipboardAppConfig.AzureOpenAI;
            }
            set {
                ClipboardAppConfig.AzureOpenAI = value;
                OnPropertyChanged(nameof(AzureOpenAI));
                // 関連項目の表示/非表示を更新
                OnPropertyChanged(nameof(AzureOpenAIVisibility));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAIのAPIキー
        public string OpenAIKey {
            get {
                return ClipboardAppConfig.OpenAIKey;
            }
            set {
                ClipboardAppConfig.OpenAIKey = value;
                OnPropertyChanged(nameof(OpenAIKey));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAICompletionModel
        public string OpenAICompletionModel {
            get {
                return ClipboardAppConfig.OpenAICompletionModel;
            }
            set {
                ClipboardAppConfig.OpenAICompletionModel = value;
                OnPropertyChanged(nameof(OpenAICompletionModel));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAIEmbeddingModel
        public string OpenAIEmbeddingModel {
            get {
                return ClipboardAppConfig.OpenAIEmbeddingModel;
            }
            set {
                ClipboardAppConfig.OpenAIEmbeddingModel = value;
                OnPropertyChanged(nameof(OpenAIEmbeddingModel));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // Azure OpenAIのエンドポイント
        public string AzureOpenAIEndpoint {
            get {
                return ClipboardAppConfig.AzureOpenAIEndpoint;
            }
            set {
                ClipboardAppConfig.AzureOpenAIEndpoint = value;
                OnPropertyChanged(nameof(AzureOpenAIEndpoint));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAICompletionBaseURL
        public string OpenAICompletionBaseURL {
            get {
                return ClipboardAppConfig.OpenAICompletionBaseURL;
            }
            set {
                ClipboardAppConfig.OpenAICompletionBaseURL = value;
                OnPropertyChanged(nameof(OpenAICompletionBaseURL));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAIEmbeddingBaseURL
        public string OpenAIEmbeddingBaseURL {
            get {
                return ClipboardAppConfig.OpenAIEmbeddingBaseURL;
            }
            set {
                ClipboardAppConfig.OpenAIEmbeddingBaseURL = value;
                OnPropertyChanged(nameof(OpenAIEmbeddingBaseURL));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // UseOCR
        public bool UseOCR {
            get {
                return ClipboardAppConfig.UseOCR;
            }
            set {
                ClipboardAppConfig.UseOCR = value;
                OnPropertyChanged(nameof(UseOCR));
                // 関連項目の表示/非表示を更新
                OnPropertyChanged(nameof(UseOCRVisibility));

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
                // 関連項目の表示/非表示を更新
                OnPropertyChanged(nameof(UseSpacyVisibility));

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

        // AutoMergeItemsBySourceApplicationTitle
        public bool AutoMergeItemsBySourceApplicationTitle {
            get {
                return ClipboardAppConfig.AutoMergeItemsBySourceApplicationTitle;
            }
            set {
                ClipboardAppConfig.AutoMergeItemsBySourceApplicationTitle = value;
                OnPropertyChanged(nameof(AutoMergeItemsBySourceApplicationTitle));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // -------------------------------------
        // その他の設定
        // -------------------------------------
        // クリップボードアイテムとOS上のフォルダを同期するかどうか
        public bool SyncClipboardItemAndOSFolder {
            get {
                return ClipboardAppConfig.SyncClipboardItemAndOSFolder;
            }
            set {
                ClipboardAppConfig.SyncClipboardItemAndOSFolder = value;
                OnPropertyChanged(nameof(SyncClipboardItemAndOSFolder));
                OnPropertyChanged(nameof(SyncClipboardItemAndOSFolderVisibility));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // ファイル更新時に自動的にコミットするかどうか
        public bool AutoCommit {
            get {
                return ClipboardAppConfig.AutoCommit;
            }
            set {
                ClipboardAppConfig.AutoCommit = value;
                OnPropertyChanged(nameof(AutoCommit));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // SyncFolderName
        public string SyncFolderName {
            get {
                return ClipboardAppConfig.SyncFolderName;
            }
            set {
                ClipboardAppConfig.SyncFolderName = value;
                OnPropertyChanged(nameof(SyncFolderName));

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
        // AutoDescriptionNone
        public bool AutoDescriptionNone {
            get {
                return AutoDescription == false && AutoDescriptionWithOpenAI == false;
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
        // AutoDescriptionWithOpenAI
        public bool AutoDescriptionWithOpenAI {
            get {
                return ClipboardAppConfig.AutoDescriptionWithOpenAI;
            }
            set {
                ClipboardAppConfig.AutoDescriptionWithOpenAI = value;
                OnPropertyChanged(nameof(AutoDescriptionWithOpenAI));

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
        // クリップボードアイテム保存時に自動的にEmbeddingを行うかどうか
        public bool AutoEmbedding {
            get {
                return ClipboardAppConfig.AutoEmbedding;
            }
            set {
                ClipboardAppConfig.AutoEmbedding = value;
                OnPropertyChanged(nameof(AutoEmbedding));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }


        // BackupGeneration
        public int BackupGeneration {
            get {
                return ClipboardAppConfig.BackupGeneration;
            }
            set {
                ClipboardAppConfig.BackupGeneration = value;
                OnPropertyChanged(nameof(BackupGeneration));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
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
        // SyncClipboardItemAndOSFolderが有効かどうか
        public Visibility SyncClipboardItemAndOSFolderVisibility {
            get {
                if (SyncClipboardItemAndOSFolder == true) {
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
        private void Log(StringBuilder stringBuilder, string message) {
            stringBuilder.AppendLine(message);
            Tools.Info(message);
        }
        public string CheckSetting() {
            StringBuilder stringBuilder = new();
            if (PythonExecMode == true) {
                bool pythonOK = true;
                Log(stringBuilder, "Pythonの設定チェック...");

                if (string.IsNullOrEmpty(PythonDllPath)) {
                    Log(stringBuilder, "[NG]:PythonDLLのパスが設定されていません");
                    pythonOK = false;
                } else {
                    Log(stringBuilder, "[OK]:PythonDLLのパスが設定されています");
                    if (System.IO.File.Exists(PythonDllPath) == false) {
                        Log(stringBuilder, "[NG]:PythonDLLのパスが存在しません");
                        pythonOK = false;
                    } else {
                        Log(stringBuilder, "[OK]:PythonDLLのパスが存在します");
                    }
                }
                if (pythonOK == true) {
                    // TestPythonを実行
                    Log(stringBuilder, "Pythonスクリプトをテスト実行...");
                    TestResult result = TestPython();
                    Log(stringBuilder, result.Message);
                    // TestExtractTextを実行
                    Log(stringBuilder, "テキスト抽出のテスト実行...");
                    result = TestExtractText();
                    Log(stringBuilder, result.Message);

                }

            }
            if (UseOpenAI == true) {
                bool openAIOK = true;
                Log(stringBuilder, "OpenAIの設定チェック...");
                if (string.IsNullOrEmpty(OpenAIKey)) {
                    Log(stringBuilder, "[NG]:OpenAIのAPIキーが設定されていません");
                    openAIOK = false;
                } else {
                    Log(stringBuilder, "[OK]:OpenAIのAPIキーが設定されています");
                }
                if (string.IsNullOrEmpty(OpenAICompletionModel)) {
                    Log(stringBuilder, "[NG]:OpenAIのCompletionModelが設定されていません");
                    openAIOK = false;
                } else {
                    Log(stringBuilder, "[OK]:OpenAIのCompletionModelが設定されています");
                }
                if (string.IsNullOrEmpty(OpenAIEmbeddingModel)) {
                    Log(stringBuilder, "[NG]:OpenAIのEmbeddingModelが設定されていません");
                    openAIOK = false;
                } else {
                    Log(stringBuilder, "[OK]:OpenAIのEmbeddingModelが設定されています");
                }

                if (AzureOpenAI == true) {

                    Log(stringBuilder, "Azure OpenAIの設定チェック...");
                    if (string.IsNullOrEmpty(AzureOpenAIEndpoint)) {

                        stringBuilder.AppendLine();
                        Log(stringBuilder, "Azure OpenAIのエンドポイントが設定されていないためBaseURL設定をチェック");
                        if (string.IsNullOrEmpty(OpenAICompletionBaseURL) || string.IsNullOrEmpty(OpenAIEmbeddingBaseURL)) {
                            Log(stringBuilder, "[NG]:Azure OpenAIのエンドポイント、BaseURLのいずれかを設定してください");
                            openAIOK = false;
                        }
                    } else {
                        if (string.IsNullOrEmpty(OpenAICompletionBaseURL) == false || string.IsNullOrEmpty(OpenAIEmbeddingBaseURL) == false) {
                            Log(stringBuilder, "[NG]:Azure OpenAIのエンドポイントとBaseURLの両方を設定することはできません");
                            openAIOK = false;
                        }
                    }
                }

                if (openAIOK == true) {
                    // TestOpenAIを実行
                    Log(stringBuilder, "OpenAIのテスト実行...");
                    TestResult result = TestOpenAI();
                    Log(stringBuilder, result.Message);

                }
            }
            if (UseSpacy == true) {
                bool spacyOK = true;
                Log(stringBuilder, "Spacyが使用可能かチェック...");
                if (string.IsNullOrEmpty(SpacyModel)) {
                    Log(stringBuilder, "[NG]:Spacyのモデルが設定されていません");
                    spacyOK = false;
                }
                if (spacyOK == true) {
                    Log(stringBuilder, "[OK]:Spacyのモデルが設定されています");
                    // TestSpacyを実行
                    Log(stringBuilder, "Spacyのテスト実行...");
                    TestResult result = TestSpacy();
                    Log(stringBuilder, result.Message);

                }
                
            }
            // UseOCRが有効な場合
            if (UseOCR == true) {
                bool ocrOK = true;
                Log(stringBuilder, "OCRの設定チェック...");
                if (string.IsNullOrEmpty(TesseractExePath)) {
                    Log(stringBuilder, "[NG]:Tesseractのパスが設定されていません");
                    ocrOK = false;
                } else {
                    Log(stringBuilder, "[OK]:Tesseractのパスが設定されています");
                    if (System.IO.File.Exists(TesseractExePath) == false) {
                        Log(stringBuilder, "[NG]:Tesseractのパスが存在しません");
                        ocrOK = false;
                    } else {
                        Log(stringBuilder, "[OK]:Tesseractのパスが存在します");

                    }
                }
                if (ocrOK == true) {
                    // TestOCRを実行
                    Log(stringBuilder, "OCRのテスト実行...");
                    TestResult result = TestOCR();
                    Log(stringBuilder, result.Message);
                }
            }
            return stringBuilder.ToString();
        }
        private class TestResult {
            public bool Result { get; set; } = false;
            public string Message { get; set; } = "";
        }

        private TestResult TestPython() {
            TestResult testResult = new();
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
                testResult.Message = "[NG]:Pythonの実行に失敗しました。\n[メッセージ]" + ex.Message + "\n[スタックトレース]" + ex.StackTrace;
                testResult.Result = false;
            }
            return testResult;
        }
        private TestResult TestExtractText() {
            TestResult testResult = new();
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
                testResult.Message = "[NG]:テキストファイルからのテキスト抽出に失敗しました。\n[メッセージ]" + ex.Message + "\n[スタックトレース]" + ex.StackTrace;
                testResult.Result = false;
            }
            return testResult;

        }
        private TestResult TestOCR() {
            TestResult testResult = new();
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
                testResult.Message = "[NG]:画像ファイルからのテキスト抽出に失敗しました。\n[メッセージ]" + ex.Message + "\n[スタックトレース]" + ex.StackTrace;
                testResult.Result = false;
            }
            return testResult;
        }
        private TestResult TestOpenAI() {
            TestResult testResult = new();
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
                testResult.Message = "[NG]:OpenAIの実行に失敗しました。\n[メッセージ]" + ex.Message + "\n[スタックトレース]" + ex.StackTrace;
                testResult.Result = false;
            }
            return testResult;
        }

        private TestResult TestSpacy() {
            TestResult testResult = new();
            PythonExecutor.Init(PythonDllPath);
            StringBuilder stringBuilder = new();
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
                testResult.Message = "[NG]:Spacyの実行に失敗しました。\n[メッセージ]" + ex.Message + "\n[スタックトレース]" + ex.StackTrace;
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
        public SimpleDelegateCommand<object> CheckCommand => new(async (parameter) => {
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
                Tools.StatusText.ReadyText = "設定チェック中...";
                string resultString = "";
                await Task.Run(() => {
                    resultString = CheckSetting();
                });
                IsIndeterminate = false;
                Tools.StatusText.Init();
                // 結果をTestResultWindowで表示
                // UserControlの設定ウィンドウを開く
                TestResultUserControl.OpenTestResultWindow(resultString);

            } finally {
                IsIndeterminate = false;
                Tools.StatusText.Init();
            }
        });

        public bool Save() {
            if (isPropertyChanged) {
                ClipboardAppConfig.Save();

                isPropertyChanged = false;
                return true;
            }
            return false;

        }
        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {

            if (Save()) {
                MessageBox.Show("設定を保存しました。");
            }
            // Windowを閉じる
            window.Close();
        });

       

        // CancelCommand
        public SimpleDelegateCommand<Window> CancelCommand => new((window) => {
            ClipboardAppConfig.Reload();
            Tools.Info("設定をキャンセルしました");
            // Windowを閉じる
            window.Close();
        });
    }
}
