using System.IO;
using System.Windows;
using AIChatExplorer.View.Main;
using AIChatExplorer.ViewModel.Main;
using AIChatExplorer.ViewModel.Settings;
using LibPythonAI.Common;
using LibPythonAI.Data;
using LibPythonAI.PythonIF;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;
using LibUIPythonAI.Resource;

namespace AIChatExplorer.AppStartup {

    public class StartupWindowViewModel {
        public static void Startup() {

            AIChatExplorerPythonAILibConfigParams configParams = new();
            // 言語設定
            SetupLanguage();

            // ログ設定
            SetupLog();

            // Python環境のチェック
            CheckEnvironment(configParams);

            // PythonAILibManagerの初期化
            PythonAILibManager.Init(configParams);

            // DBの初期化
            PythonAILibDBContext.Init();

            // DataContextにViewModelを設定
            MainWindowViewModel mainWindowViewModel = new();
            // MainWindowを表示
            MainWindow mainWindow = new() {
                DataContext = mainWindowViewModel
            };
            mainWindow.Show();

        }

        private static void SetupLanguage() {
            // 言語設定
            CommonStringResources.Lang = AIChatExplorerConfig.Instance.ActualLang;
            // MergeChatの言語設定
            LibUIMergeChat.Resources.MergeChatStringResources.Lang = AIChatExplorerConfig.Instance.ActualLang;
            // ImageChatの言語設定
            // LibUIImageChat.Resources.ImageChatStringResources.Lang = AIChatExplorerConfig.Instance.ActualLang;
            // AutogetnChatの言語設定
            // LibUIAutoGenChat.Resources.AutoGenChatStringResources.Lang = AIChatExplorerConfig.Instance.ActualLang;

            // NormalChatの言語設定
            LibUINormalChat.Resources.NormalChatStringResources.Lang = AIChatExplorerConfig.Instance.ActualLang;

        }

        private static void SetupLog() {
            // LogWrapperの初期化
            string logDirPath = Path.Combine(AIChatExplorerConfig.Instance.AppDataPath, "log");
            LogWrapper.SetLogFolder(logDirPath);
            // LogWrapperのログ出力設定
            LogWrapper.SetActions(AIChatExplorerConfig.Instance.GetLogWrapperAction());
        }


        private static void CheckEnvironment(IPythonAILibConfigParams configParams) {

            PythonExecutor.PythonEnvironmentCheckResult result = PythonExecutor.CheckPythonEnvironment(configParams);
            if (result == PythonExecutor.PythonEnvironmentCheckResult.PythonNotFound) {
                // 環境チェックに失敗した場合、エラーメッセージを表示
                string errorMessage = PythonAILibStringResources.Instance.PythonNotFound;
                MessageBox.Show(errorMessage);

            } else if (result == PythonExecutor.PythonEnvironmentCheckResult.UvNotFound) {
                // uvが見つからない場合、エラーメッセージを表示
                string errorMessage = PythonAILibStringResources.Instance.UvNotFound;
                MessageBox.Show(errorMessage);

            } else if (result == PythonExecutor.PythonEnvironmentCheckResult.PythonVenvPathNotFound) {
                // Python仮想環境が見つからない場合、エラーメッセージを表示

            } else if (result == PythonExecutor.PythonEnvironmentCheckResult.PythonVenvNotFound) {
                // Python仮想環境が見つからない場合はVenvを作成するか確認
                string confirmMessage = PythonAILibStringResources.Instance.ConfirmPythonVenvCreate;
                MessageBoxResult resultMessageBox = MessageBox.Show(confirmMessage, CommonStringResources.Instance.CreateItem, MessageBoxButton.YesNo);
                if (resultMessageBox == MessageBoxResult.Yes) {
                    // Venvを作成
                    PythonExecutor.CreatePythonVenvAndInstallLibs(configParams);
                    // Venv作成後のメッセージを表示
                    MessageBox.Show(PythonAILibStringResources.Instance.PythonVenvCreationSuccess);

                } else {
                    // Venvを作成しない場合は、作成手順を表示
                    string message = PythonAILibStringResources.Instance.PythonVenvMaualCreateMessage(configParams);
                    MessageBox.Show(message);
                }

            }
            // 再チェック
            result = PythonExecutor.CheckPythonEnvironment(configParams);

            if (result == PythonExecutor.PythonEnvironmentCheckResult.OpenAIKeyNotSet) {
                // OpenAIのAPIキーが設定されていない場合、エラーメッセージを表示
                string errorMessage = PythonAILibStringResources.Instance.OpenAIKeyNotSet;
                MessageBox.Show(errorMessage);
            }

        }

    }
}