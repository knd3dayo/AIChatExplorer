using System.Text;
using System.Windows;
using ClipboardApp.Model;
using ClipboardApp.Model.Folder;
using ClipboardApp.ViewModel;
using PythonAILib.Model;
using PythonAILib.Model.Chat;
using PythonAILib.PythonIF;
using PythonAILib.Resource;
using WpfAppCommon.Utils;
using QAChat.Resource;

namespace ClipboardApp.Settings {
    /// <summary>
    /// 設定画面のViewModel
    /// </summary>
    public partial class SettingUserControlViewModel : ClipboardAppViewModelBase {
        // プロパティが変更されたか否か
        private bool isPropertyChanged = false;
        // Lang

        public string Lang {
            get {
                return ClipboardAppConfig.Instance.Lang;
            }
            set {
                ClipboardAppConfig.Instance.Lang = value;
                OnPropertyChanged(nameof(Lang));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // SelectedLanguage
        public int SelectedLanguage {
            get {
                if (string.IsNullOrEmpty(ClipboardAppConfig.Instance.Lang)) {
                    return 0;
                } else if (ClipboardAppConfig.Instance.Lang == "ja-JP") {
                    return 1;
                } else if (ClipboardAppConfig.Instance.Lang == "en-US") {
                    return 2;
                } else {
                    return 0;
                }
            }
            set {
                switch (value) {
                    case 0:
                        ClipboardAppConfig.Instance.Lang = "";
                        break;
                    case 1:
                        ClipboardAppConfig.Instance.Lang = "ja-JP";
                        break;
                    case 2:
                        ClipboardAppConfig.Instance.Lang = "en-US";
                        break;
                }
                isPropertyChanged = true;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }

        // MonitorTargetAppNames

        public string MonitorTargetAppNames {
            get {
                return ClipboardAppConfig.Instance.MonitorTargetAppNames;
            }
            set {
                ClipboardAppConfig.Instance.MonitorTargetAppNames = value;
                OnPropertyChanged(nameof(MonitorTargetAppNames));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // PythonDLLのパス
        public string PythonDllPath {
            get {
                return ClipboardAppConfig.Instance.PythonDllPath;
            }
            set {
                ClipboardAppConfig.Instance.PythonDllPath = value;
                OnPropertyChanged(nameof(PythonDllPath));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // PythonVenvPath
        public string PythonVenvPath {
            get {
                return ClipboardAppConfig.Instance.PythonVenvPath;
            }
            set {
                ClipboardAppConfig.Instance.PythonVenvPath = value;
                OnPropertyChanged(nameof(PythonVenvPath));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // Azure OpenAIを使用するかどうか
        public bool AzureOpenAI {
            get {
                return ClipboardAppConfig.Instance.AzureOpenAI;
            }
            set {
                ClipboardAppConfig.Instance.AzureOpenAI = value;
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAIのAPIキー
        public string OpenAIKey {
            get {
                return ClipboardAppConfig.Instance.OpenAIKey;
            }
            set {
                ClipboardAppConfig.Instance.OpenAIKey = value;
                OnPropertyChanged(nameof(OpenAIKey));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAICompletionModel
        public string OpenAICompletionModel {
            get {
                return ClipboardAppConfig.Instance.OpenAICompletionModel;
            }
            set {
                ClipboardAppConfig.Instance.OpenAICompletionModel = value;
                OnPropertyChanged(nameof(OpenAICompletionModel));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAIEmbeddingModel
        public string OpenAIEmbeddingModel {
            get {
                return ClipboardAppConfig.Instance.OpenAIEmbeddingModel;
            }
            set {
                ClipboardAppConfig.Instance.OpenAIEmbeddingModel = value;
                OnPropertyChanged(nameof(OpenAIEmbeddingModel));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // Azure OpenAIのエンドポイント
        public string AzureOpenAIEndpoint {
            get {
                return ClipboardAppConfig.Instance.AzureOpenAIEndpoint;
            }
            set {
                ClipboardAppConfig.Instance.AzureOpenAIEndpoint = value;
                OnPropertyChanged(nameof(AzureOpenAIEndpoint));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAICompletionBaseURL
        public string OpenAICompletionBaseURL {
            get {
                return ClipboardAppConfig.Instance.OpenAICompletionBaseURL;
            }
            set {
                ClipboardAppConfig.Instance.OpenAICompletionBaseURL = value;
                OnPropertyChanged(nameof(OpenAICompletionBaseURL));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAIEmbeddingBaseURL
        public string OpenAIEmbeddingBaseURL {
            get {
                return ClipboardAppConfig.Instance.OpenAIEmbeddingBaseURL;
            }
            set {
                ClipboardAppConfig.Instance.OpenAIEmbeddingBaseURL = value;
                OnPropertyChanged(nameof(OpenAIEmbeddingBaseURL));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // AutoMergeItemsBySourceApplicationTitle
        public bool AutoMergeItemsBySourceApplicationTitle {
            get {
                return ClipboardAppConfig.Instance.AutoMergeItemsBySourceApplicationTitle;
            }
            set {
                ClipboardAppConfig.Instance.AutoMergeItemsBySourceApplicationTitle = value;
                OnPropertyChanged(nameof(AutoMergeItemsBySourceApplicationTitle));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoBackgroundInfo
        public bool AutoBackgroundInfo {
            get {
                return ClipboardAppConfig.Instance.AutoBackgroundInfo;
            }
            set {
                ClipboardAppConfig.Instance.AutoBackgroundInfo = value;
                OnPropertyChanged(nameof(AutoBackgroundInfo));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoSummary
        public bool AutoSummary {
            get {
                return ClipboardAppConfig.Instance.AutoSummary;
            }
            set {
                ClipboardAppConfig.Instance.AutoSummary = value;
                OnPropertyChanged(nameof(AutoSummary));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // IncludeBackgroundInfoInEmbedding
        public bool IncludeBackgroundInfoInEmbedding {
            get {
                return ClipboardAppConfig.Instance.IncludeBackgroundInfoInEmbedding;
            }
            set {
                ClipboardAppConfig.Instance.IncludeBackgroundInfoInEmbedding = value;
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
                return ClipboardAppConfig.Instance.IgnoreLineCount;
            }
            set {
                ClipboardAppConfig.Instance.IgnoreLineCount = value;
                OnPropertyChanged(nameof(IgnoreLineCount));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // クリップボードアイテムとOS上のフォルダを同期するかどうか
        public bool SyncClipboardItemAndOSFolder {
            get {
                return ClipboardAppConfig.Instance.SyncClipboardItemAndOSFolder;
            }
            set {
                ClipboardAppConfig.Instance.SyncClipboardItemAndOSFolder = value;
                OnPropertyChanged(nameof(SyncClipboardItemAndOSFolder));
                OnPropertyChanged(nameof(SyncClipboardItemAndOSFolderVisibility));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // ファイル更新時に自動的にコミットするかどうか
        public bool AutoCommit {
            get {
                return ClipboardAppConfig.Instance.AutoCommit;
            }
            set {
                ClipboardAppConfig.Instance.AutoCommit = value;
                OnPropertyChanged(nameof(AutoCommit));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // SyncFolderName
        public string SyncFolderName {
            get {
                return ClipboardAppConfig.Instance.SyncFolderName;
            }
            set {
                ClipboardAppConfig.Instance.SyncFolderName = value;
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
                return ClipboardAppConfig.Instance.AutoDescriptionWithOpenAI;
            }
            set {
                ClipboardAppConfig.Instance.AutoDescriptionWithOpenAI = value;
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
                return ClipboardAppConfig.Instance.AutoExtractImageWithOpenAI;
            }
            set {
                ClipboardAppConfig.Instance.AutoExtractImageWithOpenAI = value;
                OnPropertyChanged(nameof(AutoExtractImageWithOpenAI));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoGenerateTasks
        public bool AutoGenerateTasks {
            get {
                return ClipboardAppConfig.Instance.AutoGenerateTasks;
            }
            set {
                ClipboardAppConfig.Instance.AutoGenerateTasks = value;
                OnPropertyChanged(nameof(AutoGenerateTasks));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }


        // EmbeddingWhenExtractingTextFromImage
        public bool EmbeddingWhenExtractingTextFromImage {
            get {
                return ClipboardAppConfig.Instance.EmbeddingWhenExtractingTextFromImage;
            }
            set {
                ClipboardAppConfig.Instance.EmbeddingWhenExtractingTextFromImage = value;
                OnPropertyChanged(nameof(EmbeddingWhenExtractingTextFromImage));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // クリップボードアイテムがファイルの場合、自動でテキスト抽出を行います
        public bool AutoFileExtract {
            get {
                return ClipboardAppConfig.Instance.AutoFileExtract;
            }
            set {
                ClipboardAppConfig.Instance.AutoFileExtract = value;
                OnPropertyChanged(nameof(AutoFileExtract));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // BackupGeneration
        public int BackupGeneration {
            get {
                return ClipboardAppConfig.Instance.BackupGeneration;
            }
            set {
                ClipboardAppConfig.Instance.BackupGeneration = value;
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
            PythonExecutor.Init(PythonDllPath, ClipboardAppConfig.Instance.PythonVenvPath);
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
            PythonExecutor.Init(PythonDllPath, ClipboardAppConfig.Instance.PythonVenvPath);
            try {
                // ChatControllerを作成
                Chat chatController = new();
                List<ChatHistoryItem> chatItems = [];
                // ChatItemを追加
                ChatHistoryItem chatItem = new(ChatHistoryItem.UserRole, "Hello");
                chatItems.Add(chatItem);
                chatController.ChatHistory = chatItems;
                chatController.ChatMode = OpenAIExecutionModeEnum.Normal;

                string resultString = chatController.ExecuteChat(ClipboardAppConfig.Instance.CreateOpenAIProperties())?.Response ?? "";
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
                Chat chatController = new();
                List<ChatHistoryItem> chatItems = [new ChatHistoryItem(ChatHistoryItem.UserRole, "Hello")];
                chatController.ChatHistory = chatItems;
                chatController.ChatMode = OpenAIExecutionModeEnum.LangChain;
                ChatResult? result = chatController.ExecuteChat(ClipboardAppConfig.Instance.CreateOpenAIProperties());
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
                ClipboardAppConfig.Instance.Save();

                if (CommonStringResources.Lang != ClipboardAppConfig.Instance.ActualLang) {
                    CommonStringResources.Lang = ClipboardAppConfig.Instance.ActualLang;
                    // PythonAILibの言語を変更
                    PythonAILibStringResources.Lang = ClipboardAppConfig.Instance.ActualLang;
                }

                isPropertyChanged = false;
                return true;
            }
            return false;

        }
        #region listAutoProcessRuleから移動した処理

        // IgnoreLineCountChecked
        public bool IgnoreLineCountChecked {
            get {
                // IgnoreLineCountが-1の場合はFalseを返す
                if (IgnoreLineCount == -1) {
                    return false;
                }
                return true;
            }
            set {
                // Falseの場合はIgnoreLineCountを-1にする
                if (!value) {
                    IgnoreLineCountText = "";
                }
                OnPropertyChanged(nameof(IgnoreLineCountChecked));
            }
        }
        // IgnoreLineCountText
        public string IgnoreLineCountText {
            get {
                // IgnoreLineCountが-1の場合は空文字を返す
                if (IgnoreLineCount == -1) {
                    return "";
                }
                return IgnoreLineCount.ToString();
            }
            set {
                // 空文字の場合は-1にする
                if (value == "") {
                        IgnoreLineCount = -1;
                } else {
                    IgnoreLineCount = int.Parse(value);
                }
                OnPropertyChanged(nameof(IgnoreLineCountText));
            }
        }

        #endregion


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
            ClipboardAppConfig.Instance.Reload();
            LogWrapper.Info(StringResources.Canceled);
            // Windowを閉じる
            window.Close();
        });
    }
}
