using System.Text;
using System.Windows;
using AIChatExplorer.Model.Main;
using AIChatExplorer.Settings;
using AIChatExplorer.ViewModel.Main;
using LibUIPythonAI.Resource;
using LibUIPythonAI.Utils;
using PythonAILib.Common;
using PythonAILib.Model.Chat;
using PythonAILib.PythonIF;
using PythonAILib.Resources;
using WpfAppCommon.Model;
using LibPythonAI.Utils.Common;
using LibPythonAI.Utils.Python;

namespace AIChatExplorer.ViewModel.Settings {
    /// <summary>
    /// 設定画面のViewModel
    /// </summary>
    public partial class SettingUserControlViewModel : CommonViewModelBase {
        // プロパティが変更されたか否か
        private bool isPropertyChanged = false;
        // Lang

        public string Lang {
            get {
                return AIChatExplorerConfig.Instance.Lang;
            }
            set {
                AIChatExplorerConfig.Instance.Lang = value;
                OnPropertyChanged(nameof(Lang));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // SelectedLanguage
        public int SelectedLanguage {
            get {
                if (string.IsNullOrEmpty(AIChatExplorerConfig.Instance.Lang)) {
                    return 0;
                } else if (AIChatExplorerConfig.Instance.Lang == "ja-JP") {
                    return 1;
                } else if (AIChatExplorerConfig.Instance.Lang == "en-US") {
                    return 2;
                } else {
                    return 0;
                }
            }
            set {
                switch (value) {
                    case 0:
                        AIChatExplorerConfig.Instance.Lang = "";
                        break;
                    case 1:
                        AIChatExplorerConfig.Instance.Lang = "ja-JP";
                        break;
                    case 2:
                        AIChatExplorerConfig.Instance.Lang = "en-US";
                        break;
                }
                isPropertyChanged = true;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }

        // MonitorTargetAppNames

        public string MonitorTargetAppNames {
            get {
                return AIChatExplorerConfig.Instance.MonitorTargetAppNames;
            }
            set {
                AIChatExplorerConfig.Instance.MonitorTargetAppNames = value;
                OnPropertyChanged(nameof(MonitorTargetAppNames));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // PythonDLLのパス
        public string PythonDllPath {
            get {
                return AIChatExplorerConfig.Instance.PythonDllPath;
            }
            set {
                AIChatExplorerConfig.Instance.PythonDllPath = value;
                OnPropertyChanged(nameof(PythonDllPath));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // PythonVenvPath
        public string PythonVenvPath {
            get {
                return AIChatExplorerConfig.Instance.PythonVenvPath;
            }
            set {
                AIChatExplorerConfig.Instance.PythonVenvPath = value;
                OnPropertyChanged(nameof(PythonVenvPath));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // Azure OpenAIを使用するかどうか
        public bool AzureOpenAI {
            get {
                return AIChatExplorerConfig.Instance.AzureOpenAI;
            }
            set {
                AIChatExplorerConfig.Instance.AzureOpenAI = value;
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AzureOpenAIAPIVersion
        public string AzureOpenAIAPIVersion {
            get {
                return AIChatExplorerConfig.Instance.AzureOpenAIAPIVersion;
            }
            set {
                AIChatExplorerConfig.Instance.AzureOpenAIAPIVersion = value;
                OnPropertyChanged(nameof(AzureOpenAIAPIVersion));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // OpenAIのAPIキー
        public string OpenAIKey {
            get {
                return AIChatExplorerConfig.Instance.OpenAIKey;
            }
            set {
                AIChatExplorerConfig.Instance.OpenAIKey = value;
                OnPropertyChanged(nameof(OpenAIKey));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAICompletionModel
        public string OpenAICompletionModel {
            get {
                return AIChatExplorerConfig.Instance.OpenAICompletionModel;
            }
            set {
                AIChatExplorerConfig.Instance.OpenAICompletionModel = value;
                OnPropertyChanged(nameof(OpenAICompletionModel));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAIEmbeddingModel
        public string OpenAIEmbeddingModel {
            get {
                return AIChatExplorerConfig.Instance.OpenAIEmbeddingModel;
            }
            set {
                AIChatExplorerConfig.Instance.OpenAIEmbeddingModel = value;
                OnPropertyChanged(nameof(OpenAIEmbeddingModel));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // Azure OpenAIのエンドポイント
        public string AzureOpenAIEndpoint {
            get {
                return AIChatExplorerConfig.Instance.AzureOpenAIEndpoint;
            }
            set {
                AIChatExplorerConfig.Instance.AzureOpenAIEndpoint = value;
                OnPropertyChanged(nameof(AzureOpenAIEndpoint));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // OpenAIBaseURL
        public string OpenAIBaseURL {
            get {
                return AIChatExplorerConfig.Instance.OpenAIBaseURL;
            }
            set {
                AIChatExplorerConfig.Instance.OpenAIBaseURL = value;
                OnPropertyChanged(nameof(OpenAIBaseURL));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // AutoBackgroundInfo
        public bool AutoBackgroundInfo {
            get {
                return AIChatExplorerConfig.Instance.AutoBackgroundInfo;
            }
            set {
                AIChatExplorerConfig.Instance.AutoBackgroundInfo = value;
                OnPropertyChanged(nameof(AutoBackgroundInfo));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoSummary
        public bool AutoSummary {
            get {
                return AIChatExplorerConfig.Instance.AutoSummary;
            }
            set {
                AIChatExplorerConfig.Instance.AutoSummary = value;
                OnPropertyChanged(nameof(AutoSummary));

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
                return AIChatExplorerConfig.Instance.IgnoreLineCount;
            }
            set {
                AIChatExplorerConfig.Instance.IgnoreLineCount = value;
                OnPropertyChanged(nameof(IgnoreLineCount));

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
                return AIChatExplorerConfig.Instance.AutoDescriptionWithOpenAI;
            }
            set {
                AIChatExplorerConfig.Instance.AutoDescriptionWithOpenAI = value;
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
                return AIChatExplorerConfig.Instance.AutoExtractImageWithOpenAI;
            }
            set {
                AIChatExplorerConfig.Instance.AutoExtractImageWithOpenAI = value;
                OnPropertyChanged(nameof(AutoExtractImageWithOpenAI));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // AutoGenerateTasks
        public bool AutoGenerateTasks {
            get {
                return AIChatExplorerConfig.Instance.AutoGenerateTasks;
            }
            set {
                AIChatExplorerConfig.Instance.AutoGenerateTasks = value;
                OnPropertyChanged(nameof(AutoGenerateTasks));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }


        // EmbeddingWhenExtractingTextFromImage
        public bool EmbeddingWhenExtractingTextFromImage {
            get {
                return AIChatExplorerConfig.Instance.EmbeddingWhenExtractingTextFromImage;
            }
            set {
                AIChatExplorerConfig.Instance.EmbeddingWhenExtractingTextFromImage = value;
                OnPropertyChanged(nameof(EmbeddingWhenExtractingTextFromImage));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // クリップボードアイテムがファイルの場合、自動でテキスト抽出を行います
        public bool AutoFileExtract {
            get {
                return AIChatExplorerConfig.Instance.AutoFileExtract;
            }
            set {
                AIChatExplorerConfig.Instance.AutoFileExtract = value;
                OnPropertyChanged(nameof(AutoFileExtract));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // BackupGeneration
        public int BackupGeneration {
            get {
                return AIChatExplorerConfig.Instance.BackupGeneration;
            }
            set {
                AIChatExplorerConfig.Instance.BackupGeneration = value;
                OnPropertyChanged(nameof(BackupGeneration));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // ProxyURL
        public string ProxyURL {
            get {
                return AIChatExplorerConfig.Instance.ProxyURL;
            }
            set {
                AIChatExplorerConfig.Instance.ProxyURL = value;
                OnPropertyChanged(nameof(ProxyURL));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // NoProxyList
        public string NoProxyList {
            get {
                return AIChatExplorerConfig.Instance.NoProxyList;
            }
            set {
                AIChatExplorerConfig.Instance.NoProxyList = value;
                OnPropertyChanged(nameof(NoProxyList));

                // プロパティが変更されたことを設定
                isPropertyChanged = true;
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
            Log(stringBuilder, $"{CommonStringResources.Instance.PythonSettingCheck}...");

            if (string.IsNullOrEmpty(PythonDllPath)) {
                Log(stringBuilder, $"[NG]:{CommonStringResources.Instance.PythonDLLPathNotSet}");
                pythonOK = false;
            } else {
                Log(stringBuilder, $"[OK]:{CommonStringResources.Instance.PythonDLLPathSet}");
            }
            if (System.IO.File.Exists(PythonDllPath) == false) {
                Log(stringBuilder, $"[NG]:{CommonStringResources.Instance.PythonDLLNotFound}");
                pythonOK = false;
            } else {
                Log(stringBuilder, $"[OK]:{CommonStringResources.Instance.PythonDLLFileExists}");
            }
            if (pythonOK == true) {
                // TestPythonを実行
                Log(stringBuilder, $"{CommonStringResources.Instance.TestRunPythonScript}...");
                TestResult result = TestPython();
                Log(stringBuilder, result.Message);
            }

            bool openAIOK = true;
            Log(stringBuilder, $"{CommonStringResources.Instance.OpenAISettingCheck}...");
            if (string.IsNullOrEmpty(OpenAIKey)) {
                Log(stringBuilder, $"[NG]:{CommonStringResources.Instance.OpenAIKeyNotSet}");
                openAIOK = false;
            } else {
                Log(stringBuilder, $"[OK]:{CommonStringResources.Instance.OpenAIKeySet}");
            }
            if (string.IsNullOrEmpty(OpenAICompletionModel)) {
                Log(stringBuilder, $"[NG]:{CommonStringResources.Instance.OpenAICompletionModelNotSet}");
                openAIOK = false;
            } else {
                Log(stringBuilder, $"[OK]:{CommonStringResources.Instance.OpenAICompletionModelSet}");
            }
            if (string.IsNullOrEmpty(OpenAIEmbeddingModel)) {
                Log(stringBuilder, $"[NG]:{CommonStringResources.Instance.OpenAIEmbeddingModelNotSet}");
                openAIOK = false;
            } else {
                Log(stringBuilder, $"[OK]:{CommonStringResources.Instance.OpenAIEmbeddingModelSet}");
            }

            if (AzureOpenAI == true) {

                Log(stringBuilder, $"{CommonStringResources.Instance.AzureOpenAISettingCheck}...");
                if (string.IsNullOrEmpty(AzureOpenAIEndpoint)) {

                    stringBuilder.AppendLine();
                    Log(stringBuilder, $"{CommonStringResources.Instance.AzureOpenAIEndpointNotSet}");
                } else {
                    if (string.IsNullOrEmpty(OpenAIBaseURL) == false) {
                        Log(stringBuilder, $"[NG]:{CommonStringResources.Instance.CannotSetBothAzureOpenAIEndpointAndBaseURL}");
                        openAIOK = false;
                    }
                }
            }

            if (openAIOK == true) {
                // TestOpenAIを実行
                Log(stringBuilder, $"{CommonStringResources.Instance.TestRunOpenAI}...");
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
            PythonExecutor.Init(PythonAILibManager.Instance.ConfigParams);
            try {
                string result = PythonExecutor.PythonAIFunctions.HelloWorld();
                if (result != "Hello World") {
                    testResult.Message = $"[NG]:{CommonStringResources.Instance.FailedToRunPython}";
                    testResult.Result = false;

                } else {
                    testResult.Message = $"[OK]:{CommonStringResources.Instance.PythonRunIsPossible}";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = $"[NG]:{CommonStringResources.Instance.ErrorOccurredAndMessage} ex.Message  \n[{CommonStringResources.Instance.StackTrace}] {ex.StackTrace}";
                testResult.Result = false;
            }
            return testResult;
        }
        private TestResult TestOpenAI() {
            TestResult testResult = new();
            PythonExecutor.Init(PythonAILibManager.Instance.ConfigParams);

            try {
                // ChatControllerを作成
                ChatRequest chatRequest = new();
                List<ChatMessage> chatItems = [];
                // ChatItemを追加
                ChatMessage chatItem = new(ChatMessage.UserRole, "Hello");
                chatItems.Add(chatItem);
                chatRequest.ChatHistory = chatItems;

                // ChatRequestContextを作成
                ChatRequestContext chatRequestContext = new() {
                    OpenAIProperties = AIChatExplorerConfig.Instance.CreateOpenAIProperties(),
                    ChatMode = OpenAIExecutionModeEnum.Normal,
                };

                string resultString = ChatUtil.ExecuteChat(chatRequest, chatRequestContext, (message) => { })?.Output ?? "";
                if (string.IsNullOrEmpty(resultString)) {
                    testResult.Message = $"[NG]:{CommonStringResources.Instance.FailedToRunOpenAI}";
                    testResult.Result = false;
                } else {
                    testResult.Message = $"[OK]:{CommonStringResources.Instance.OpenAIRunIsPossible}";
                    testResult.Result = true;
                }
            } catch (Exception ex) {
                testResult.Message = $"[NG]:{CommonStringResources.Instance.ErrorOccurredAndMessage} ex.Message  \n[{CommonStringResources.Instance.StackTrace}] {ex.StackTrace}";
                testResult.Result = false;
            }
            return testResult;
        }

        // CheckCommand
        public SimpleDelegateCommand<object> CheckCommand => new(async (parameter) => {
            // 実行するか否かメッセージダイアログを表示する、
            string message = CommonStringResources.Instance.ConfirmRun;

            MessageBoxResult result = MessageBox.Show(message, CommonStringResources.Instance.Confirm, MessageBoxButton.YesNo);
            if (result == MessageBoxResult.No) {
                return;
            }
            try {
                CommonViewModelProperties.UpdateIndeterminate(true);
                LogWrapper.Info($"{CommonStringResources.Instance.CheckingSettings}...");
                string resultString = "";
                await Task.Run(() => {
                    resultString = CheckSetting();
                });
                CommonViewModelProperties.UpdateIndeterminate(false);
                StatusText.Instance.Init();
                // 結果をTestResultWindowで表示
                // UserControlの設定ウィンドウを開く
                TestResultUserControl.OpenTestResultWindow(resultString);

            } finally {
                CommonViewModelProperties.UpdateIndeterminate(false);
                StatusText.Instance.Init();
            }
        });

        public bool Save() {
            if (isPropertyChanged) {
                AIChatExplorerConfig.Instance.Save();

                if (CommonStringResources.Lang != AIChatExplorerConfig.Instance.ActualLang) {
                    CommonStringResources.Lang = AIChatExplorerConfig.Instance.ActualLang;
                    // PythonAILibの言語を変更
                    PythonAILibStringResources.Lang = AIChatExplorerConfig.Instance.ActualLang;
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
        // AutoDocumentReliabilityCheck
        public bool AutoDocumentReliabilityCheck {
            get {
                return AIChatExplorerConfig.Instance.AutoDocumentReliabilityCheck;
            }
            set {
                AIChatExplorerConfig.Instance.AutoDocumentReliabilityCheck = value;
                OnPropertyChanged(nameof(AutoDocumentReliabilityCheck));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }
        // APIServerURL
        public string APIServerURL {
            get {
                return AIChatExplorerConfig.Instance.APIServerURL;
            }
            set {
                AIChatExplorerConfig.Instance.APIServerURL = value;
                OnPropertyChanged(nameof(APIServerURL));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
            }
        }

        // UseInternalAPI
        public bool UseInternalAPI {
            get {
                return AIChatExplorerConfig.Instance.UseInternalAPI;
            }
            set {
                AIChatExplorerConfig.Instance.UseInternalAPI = value;
                OnPropertyChanged(nameof(UseInternalAPI));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
                // Visibilityの変更を通知
                OnPropertyChanged(nameof(UseInternalAPIVisibility));
                OnPropertyChanged(nameof(UseExternalAPIVisibility));
                OnPropertyChanged(nameof(APIServerVisibility));
                OnPropertyChanged(nameof(InternalVisibility));
            }
        }

        // UseExternalAPI
        public bool UseExternalAPI {
            get {
                return AIChatExplorerConfig.Instance.UseExternalAPI;
            }
            set {
                AIChatExplorerConfig.Instance.UseExternalAPI = value;
                OnPropertyChanged(nameof(UseExternalAPI));
                // プロパティが変更されたことを設定
                isPropertyChanged = true;
                // Visibilityの変更を通知
                OnPropertyChanged(nameof(UseInternalAPIVisibility));
                OnPropertyChanged(nameof(UseExternalAPIVisibility));
                OnPropertyChanged(nameof(APIServerVisibility));
                OnPropertyChanged(nameof(InternalVisibility));
            }
        }

        // UseInternalAPIVisibility
        public Visibility UseInternalAPIVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(UseInternalAPI);

        // UseExternalAPIVisibility
        public Visibility UseExternalAPIVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(UseExternalAPI);

        // APIServerVisibility
        public Visibility APIServerVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(UseExternalAPI || UseInternalAPI);

        // InternalVisibility
        public Visibility InternalVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(UseInternalAPI);
        #endregion

        // SaveCommand
        public SimpleDelegateCommand<Window> SaveCommand => new((window) => {
            if (Save()) {
                //追加設定.言語を変更
                AIChatExplorerFolderManager.ChangeRootFolderNames(CommonStringResources.Instance);
                LogWrapper.Info(CommonStringResources.Instance.SettingsSaved);
                // アプリケーションの再起動を促すメッセージを表示
                MessageBox.Show(CommonStringResources.Instance.RestartAppToApplyChanges, CommonStringResources.Instance.Information, MessageBoxButton.OK);

            }
            // Windowを閉じる
            window.Close();
        });

        // CancelCommand
        public SimpleDelegateCommand<Window> CancelCommand => new((window) => {
            AIChatExplorerConfig.Instance.Reload();
            LogWrapper.Info(CommonStringResources.Instance.Canceled);
            // Windowを閉じる
            window.Close();
        });
    }
}
