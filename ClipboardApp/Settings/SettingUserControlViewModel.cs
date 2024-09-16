using System.Text;
using System.Windows;
using ClipboardApp.Model;
using PythonAILib.Model;
using PythonAILib.Model.Chat;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.Settings
{
    /// <summary>
    /// 設定画面のViewModel
    /// </summary>
    public partial class SettingUserControlViewModel : MyWindowViewModel {
        // プロパティが変更されたか否か
        private bool isPropertyChanged = false;
        // Lang

        public string Lang {
            get {
                return ClipboardAppConfig.Lang;
            }
            set {
                ClipboardAppConfig.Lang = value;
                OnPropertyChanged(nameof(Lang));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // SelectedLanguage
        public int SelectedLanguage {
            get {
                if (string.IsNullOrEmpty(ClipboardAppConfig.Lang)) {
                    return 0;
                } else if (ClipboardAppConfig.Lang == "ja-JP") {
                    return 1;
                } else if (ClipboardAppConfig.Lang == "en-US") {
                    return 2;
                } else {
                    return 0;
                }
            }
            set {
                switch (value) {
                    case 0:
                        ClipboardAppConfig.Lang = "";
                        break;
                    case 1:
                        ClipboardAppConfig.Lang = "ja-JP";
                        break;
                    case 2:
                        ClipboardAppConfig.Lang = "en-US";
                        break;
                }
                isPropertyChanged = true;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }

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
        // PythonVenvPath
        public string PythonVenvPath {
            get {
                return ClipboardAppConfig.PythonVenvPath;
            }
            set {
                ClipboardAppConfig.PythonVenvPath = value;
                OnPropertyChanged(nameof(PythonVenvPath));
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
        // AnalyzeJapaneseSentence
        public bool AnalyzeJapaneseSentence {
            get {
                return ClipboardAppConfig.AnalyzeJapaneseSentence;
            }
            set {
                ClipboardAppConfig.AnalyzeJapaneseSentence = value;
                OnPropertyChanged(nameof(AnalyzeJapaneseSentence));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoGenerateQA
        public bool AutoGenerateQA {
            get {
                return ClipboardAppConfig.AutoGenerateQA;
            }
            set {
                ClipboardAppConfig.AutoGenerateQA = value;
                OnPropertyChanged(nameof(AutoGenerateQA));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoGenerateIssues
        public bool AutoGenerateIssues {
            get {
                return ClipboardAppConfig.AutoGenerateIssues;
            }
            set {
                ClipboardAppConfig.AutoGenerateIssues = value;
                OnPropertyChanged(nameof(AutoGenerateIssues));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }


        // EmbeddingWhenExtractingTextFromImage
        public bool EmbeddingWhenExtractingTextFromImage {
            get {
                return ClipboardAppConfig.EmbeddingWhenExtractingTextFromImage;
            }
            set {
                ClipboardAppConfig.EmbeddingWhenExtractingTextFromImage = value;
                OnPropertyChanged(nameof(EmbeddingWhenExtractingTextFromImage));

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
            Log(stringBuilder, $"{StringResources.PythonSettingCheck}...");

            if (string.IsNullOrEmpty(PythonDllPath)) {
                Log(stringBuilder, $"[NG]:{StringResources.PythonDLLPathNotSet}");
                pythonOK = false;
            } else {
                Log(stringBuilder, $"[OK]:{StringResources.PythonDLLPathSet}");
            }
            if (System.IO.File.Exists(PythonDllPath) == false) {
                Log(stringBuilder, $"[NG]:{StringResources.PythonDLLNotFound}");
                pythonOK = false;
            } else {
                Log(stringBuilder, $"[OK]:{StringResources.PythonDLLFileExists}");
            }
            if (pythonOK == true) {
                // TestPythonを実行
                Log(stringBuilder, $"{StringResources.TestRunPythonScript}...");
                TestResult result = TestPython();
                Log(stringBuilder, result.Message);
            }

            bool openAIOK = true;
            Log(stringBuilder, $"{StringResources.OpenAISettingCheck}...");
            if (string.IsNullOrEmpty(OpenAIKey)) {
                Log(stringBuilder, $"[NG]:{StringResources.OpenAIKeyNotSet}");
                openAIOK = false;
            } else {
                Log(stringBuilder, $"[OK]:{StringResources.OpenAIKeySet}");
            }
            if (string.IsNullOrEmpty(OpenAICompletionModel)) {
                Log(stringBuilder, $"[NG]:{StringResources.OpenAICompletionModelNotSet}");
                openAIOK = false;
            } else {
                Log(stringBuilder, $"[OK]:{StringResources.OpenAICompletionModelSet}");
            }
            if (string.IsNullOrEmpty(OpenAIEmbeddingModel)) {
                Log(stringBuilder, $"[NG]:{StringResources.OpenAIEmbeddingModelNotSet}");
                openAIOK = false;
            } else {
                Log(stringBuilder, $"[OK]:{StringResources.OpenAIEmbeddingModelSet}");
            }

            if (AzureOpenAI == true) {

                Log(stringBuilder, $"{StringResources.AzureOpenAISettingCheck}...");
                if (string.IsNullOrEmpty(AzureOpenAIEndpoint)) {

                    stringBuilder.AppendLine();
                    Log(stringBuilder, $"{StringResources.AzureOpenAIEndpointNotSet}");
                    if (string.IsNullOrEmpty(OpenAICompletionBaseURL) || string.IsNullOrEmpty(OpenAIEmbeddingBaseURL)) {
                        Log(stringBuilder, $"[NG]:{StringResources.SetAzureOpenAIEndpointOrBaseURL}");
                        openAIOK = false;
                    }
                } else {
                    if (string.IsNullOrEmpty(OpenAICompletionBaseURL) == false || string.IsNullOrEmpty(OpenAIEmbeddingBaseURL) == false) {
                        Log(stringBuilder, $"[NG]:{StringResources.CannotSetBothAzureOpenAIEndpointAndBaseURL}");
                        openAIOK = false;
                    }
                }
            }

            if (openAIOK == true) {
                // TestOpenAIを実行
                Log(stringBuilder, $"{StringResources.TestRunOpenAI}...");
                TestResult result = TestOpenAI();
                Log(stringBuilder, result.Message);
            }

            return stringBuilder.ToString();
        }

        private class TestResult {
            public bool Result { get; set; } = false;
            public string Message { get; set; } = "";
        }

        private TestResult TestPython() {
            TestResult testResult = new();
            PythonExecutor.Init(PythonDllPath, ClipboardAppConfig.PythonVenvPath);
            try {
                string result = PythonExecutor.PythonAIFunctions.HelloWorld();
                if (result != "Hello World") {
                    testResult.Message = $"[NG]:{StringResources.FailedToRunPython}";
                    testResult.Result = false;

                } else {
                    testResult.Message = $"[OK]:{StringResources.PythonRunIsPossible}";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = $"[NG]:{StringResources.ErrorOccurredAndMessage} ex.Message  \n[{StringResources.StackTrace}] {ex.StackTrace}";
                testResult.Result = false;
            }
            return testResult;
        }
        private TestResult TestOpenAI() {
            TestResult testResult = new();
            PythonExecutor.Init(PythonDllPath, ClipboardAppConfig.PythonVenvPath);
            try {
                // ChatControllerを作成
                Chat chatController = new(ClipboardAppConfig.CreateOpenAIProperties());
                List<ChatIHistorytem> chatItems = [];
                // ChatItemを追加
                ChatIHistorytem chatItem = new(ChatIHistorytem.UserRole, "Hello");
                chatItems.Add(chatItem);
                chatController.ChatHistory = chatItems;
                chatController.ChatMode = OpenAIExecutionModeEnum.Normal;

                string resultString = chatController.ExecuteChat()?.Response ?? "";
                if (string.IsNullOrEmpty(resultString)) {
                    testResult.Message = $"[NG]:{StringResources.FailedToRunOpenAI}";
                    testResult.Result = false;
                } else {
                    testResult.Message = $"[OK]:{StringResources.OpenAIRunIsPossible}";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = $"[NG]:{StringResources.ErrorOccurredAndMessage} ex.Message  \n[{StringResources.StackTrace}] {ex.StackTrace}";
                testResult.Result = false;
            }
            return testResult;
        }

        // TestLangChain
        private void TestLangChain() {
            try {
                Chat chatController = new(ClipboardAppConfig.CreateOpenAIProperties());
                List<ChatIHistorytem> chatItems = [new ChatIHistorytem(ChatIHistorytem.UserRole, "Hello")];
                chatController.ChatHistory = chatItems;
                chatController.ChatMode = OpenAIExecutionModeEnum.LangChain;
                ChatResult? result = chatController.ExecuteChat();
                if (string.IsNullOrEmpty(result?.Response)) {
                    LogWrapper.Error($"[NG]:{StringResources.FailedToRunLangChain}");
                } else {
                    string Message = $"[OK]:{StringResources.LangChainRunIsPossible}";
                    LogWrapper.Info(Message);
                }
            } catch (Exception ex) {
                string Message = $"[NG]:{StringResources.ErrorOccurredAndMessage} ex.Message  \n[{StringResources.StackTrace}] {ex.StackTrace}";
                LogWrapper.Error(Message);
            }
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
            string message = StringResources.ConfirmRun;

            MessageBoxResult result = MessageBox.Show(message, StringResources.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) {
                return;
            }
            try {
                IsIndeterminate = true;
                LogWrapper.Info($"{StringResources.CheckingSettings}...");
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

                if (CommonStringResources.Lang != ClipboardAppConfig.ActualLang) {
                    CommonStringResources.Lang = ClipboardAppConfig.ActualLang;
                    // PythonAILibの言語を変更
                    PythonAILibStringResources.Lang = ClipboardAppConfig.ActualLang;
                }

                isPropertyChanged = false;
                return true;
            }
            return false;

        }
        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {

            if (Save()) {
                //追加設定.言語を変更
                ClipboardFolder.ChangeRootFolderNames(CommonStringResources.Instance);

                LogWrapper.Info(StringResources.SettingsSaved);
                // アプリケーションの再起動を促すメッセージを表示
                MessageBox.Show(StringResources.RestartAppToApplyChanges, StringResources.Information, MessageBoxButton.OK);

            }
            // Windowを閉じる
            window.Close();
        });

        // CancelCommand
        public SimpleDelegateCommand<Window> CancelCommand => new((window) => {
            ClipboardAppConfig.Reload();
            LogWrapper.Info(StringResources.Canceled);
            // Windowを閉じる
            window.Close();
        });
    }
}
