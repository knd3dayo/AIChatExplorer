using System.Text;
using System.Windows;
using PythonAILib.Model;
using PythonAILib.PythonIF;
using WpfAppCommon.Model;
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

        // Azure OpenAIを使用するかどうか
        public bool AzureOpenAI {
            get {
                return ClipboardAppConfig.AzureOpenAI;
            }
            set {
                ClipboardAppConfig.AzureOpenAI = value;
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
        // AutoBackgroundInfo
        public bool AutoBackgroundInfo {
            get {
                return ClipboardAppConfig.AutoBackgroundInfo;
            }
            set {
                ClipboardAppConfig.AutoBackgroundInfo = value;
                OnPropertyChanged(nameof(AutoBackgroundInfo));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoSummary
        public bool AutoSummary {
            get {
                return ClipboardAppConfig.AutoSummary;
            }
            set {
                ClipboardAppConfig.AutoSummary = value;
                OnPropertyChanged(nameof(AutoSummary));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // IncludeBackgroundInfoInEmbedding
        public bool IncludeBackgroundInfoInEmbedding {
            get {
                return ClipboardAppConfig.IncludeBackgroundInfoInEmbedding;
            }
            set {
                ClipboardAppConfig.IncludeBackgroundInfoInEmbedding = value;
                OnPropertyChanged(nameof(IncludeBackgroundInfoInEmbedding));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // -------------------------------------
        // その他の設定
        // -------------------------------------
        // 指定した行数以下の場合は無視する。
        public int IgnoreLineCount {
            get {
                return ClipboardAppConfig.IgnoreLineCount;
            }
            set {
                ClipboardAppConfig.IgnoreLineCount = value;
                OnPropertyChanged(nameof(IgnoreLineCount));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
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
        // AutoDescriptionNone
        public bool AutoDescriptionNone {
            get {
                return AutoDescription == false && AutoDescriptionWithOpenAI == false;
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

        // AutoExtractTextFromImageNone
        public bool AutoExtractImageNone {
            get {
                return AutoExtractImageWithOpenAI == false && AutoExtractImageWithPyOCR == false;
            }
        }

        //　AutoExtractImageWithOpenAI
        public bool AutoExtractImageWithOpenAI {
            get {
                return ClipboardAppConfig.AutoExtractImageWithOpenAI;
            }
            set {
                ClipboardAppConfig.AutoExtractImageWithOpenAI = value;
                OnPropertyChanged(nameof(AutoExtractImageWithOpenAI));

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
        // クリップボードアイテムがファイルの場合、自動でテキスト抽出を行います
        public bool AutoFileExtract {
            get {
                return ClipboardAppConfig.AutoFileExtract;
            }
            set {
                ClipboardAppConfig.AutoFileExtract = value;
                OnPropertyChanged(nameof(AutoFileExtract));

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

        // 設定をチェックする処理
        private void Log(StringBuilder stringBuilder, string message) {
            stringBuilder.AppendLine(message);
            LogWrapper.Info(message);
        }

        public string CheckSetting() {
            StringBuilder stringBuilder = new();
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
            // AutoExtractImageWithPyOCRが有効な場合
            if (AutoExtractImageWithPyOCR == true) {
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
                string result = PythonExecutor.PythonAIFunctions.HelloWorld();
                if (result != "Hello World") {
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
                string result = PythonExecutor.PythonAIFunctions.ExtractText("TestData/extract_test.txt");
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
                string result = PythonExecutor.PythonMiscFunctions.ExtractTextFromImage(
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
                // ChatControllerを作成
                ChatRequest chatController = new(ClipboardAppConfig.CreateOpenAIProperties());
                List<ChatItem> chatItems = [];
                // ChatItemを追加
                ChatItem chatItem = new(ChatItem.UserRole, "Hello");
                chatItems.Add(chatItem);
                chatController.ChatHistory = chatItems;
                chatController.ChatMode = OpenAIExecutionModeEnum.Normal;

                string resultString = chatController.ExecuteChat()?.Response ?? "";
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
            message += "OpenAIの設定チェックの際には実際にOpenAI APIを呼び出しますのでトークンを消費します.\n";
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
            LogWrapper.Info("設定をキャンセルしました");
            // Windowを閉じる
            window.Close();
        });
    }
}
