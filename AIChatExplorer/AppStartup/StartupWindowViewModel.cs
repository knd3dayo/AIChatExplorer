using System.IO;
using System.Windows;
using AIChatExplorer.View.Main;
using AIChatExplorer.View.Settings;
using AIChatExplorer.ViewModel.Main;
using AIChatExplorer.ViewModel.Settings;
using LibMain.Common;
using LibMain.PythonIF;
using LibMain.Resources;
using LibMain.Utils.Common;
using LibUIMain.Resource;

namespace AIChatExplorer.AppStartup {

    public class StartupWindowViewModel {
        public static async Task StartupAsync() {
            AIChatExplorerPythonAILibConfigParams configParams = new();
            // 言語設定
            SetupLanguage();

            // MaterialDesignのダークテーマ設定
            AIChatExplorerConfig.Instance.UpdateMaterialDesignDarkTheme();
            // ログ設定
            SetupLog();

            // Python環境のチェック ( Python.exe, uvの存在確認 )
            CheckEnvironment1(configParams);

            // Python仮想環境のチェック ( OpenAIのAPIキー、Venvのパス設定、AppDataパスの設定 )
            CheckEnvironment2(configParams);

            // Python仮想環境のチェック ( Venvの存在確認、Venvの作成 )
            CheckEnvironment3(configParams);


            // PythonAILibManagerの初期化（UIスレッドをブロックしないようバックグラウンドで待機）
            await Task.Run(() => PythonAILibManager.Init(configParams));

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


        private static void CheckEnvironment1(IPythonAILibConfigParams configParams) {

            PythonExecutor.PythonEnvironmentCheckResult result = PythonExecutor.CheckPythonEnvironment(configParams);
            if (result == PythonExecutor.PythonEnvironmentCheckResult.PythonNotFound) {
                // 環境チェックに失敗した場合、エラーメッセージを表示
                string errorMessage = PythonAILibStringResources.Instance.PythonNotFound;
                MessageBox.Show(errorMessage);

            } else if (result == PythonExecutor.PythonEnvironmentCheckResult.UvNotFound) {
                // uvが見つからない場合、エラーメッセージを表示
                string errorMessage = PythonAILibStringResources.Instance.UvNotFound;
                MessageBox.Show(errorMessage);
            }
        }
        private static void CheckEnvironment2(IPythonAILibConfigParams configParams) {
            string errorMessage = string.Empty;
            PythonExecutor.PythonEnvironmentCheckResult result = PythonExecutor.CheckPythonEnvironment(configParams);

            if (result == PythonExecutor.PythonEnvironmentCheckResult.PythonVenvPathNotSet) {
                // Python仮想環境が見つからない場合、エラーメッセージを表示
                errorMessage = PythonAILibStringResources.Instance.PythonVenvPathNotSet;
            } else if (result == PythonExecutor.PythonEnvironmentCheckResult.OpenAIKeyNotSet) {
                // OpenAIのAPIキーが設定されていない場合、エラーメッセージを表示
                errorMessage = PythonAILibStringResources.Instance.OpenAIKeyNotSet;
            } else if (result == PythonExecutor.PythonEnvironmentCheckResult.AppDataPathNotSet) {
                // AppDataパスが設定されていない場合、エラーメッセージを表示
                errorMessage = PythonAILibStringResources.Instance.AppDataPathNotSet;
            }
            if (!string.IsNullOrEmpty(errorMessage)) {
                // エラーメッセージを表示
                MessageBox.Show(errorMessage);
                // 設置画面を表示
                SettingUserControlViewModel settingViewModel = new();
                SettingWindow settingWindow = new() {
                    DataContext = settingViewModel
                };
                settingWindow.ShowDialog();
            }
        }

        private static void CheckEnvironment3(IPythonAILibConfigParams configParams) {

            PythonExecutor.PythonEnvironmentCheckResult result = PythonExecutor.CheckPythonEnvironment(configParams);

            if (result == PythonExecutor.PythonEnvironmentCheckResult.PythonVenvNotFound) {
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
                    // アプリケーションを終了
                    Application.Current.Shutdown();
                }
            }

            if (result == PythonExecutor.PythonEnvironmentCheckResult.OpenAIKeyNotSet) {
                // OpenAIのAPIキーが設定されていない場合、エラーメッセージを表示
                string errorMessage = PythonAILibStringResources.Instance.OpenAIKeyNotSet;
                MessageBox.Show(errorMessage);
            }
        }

    }
}