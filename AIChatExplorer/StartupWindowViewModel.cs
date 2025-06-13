using System.Windows;
using AIChatExplorer.View.Main;
using AIChatExplorer.ViewModel.Main;
using AIChatExplorer.ViewModel.Settings;
using LibPythonAI.PythonIF;
using LibUIPythonAI.Resource;

namespace AIChatExplorer {

    internal class StartupWindowViewModel {


        public  void Startup(Window window) {
            // 言語設定
            CommonStringResources.Lang = AIChatExplorerConfig.Instance.ActualLang;
            // チェック処理と初期化処理
            CheckEnvironment();
            // DataContextにViewModelを設定
            MainWindowViewModel mainWindowViewModel = new();
            // MainWindowを表示
            MainWindow mainWindow = new() {
                DataContext = mainWindowViewModel
            };
            // mainWindowViewModel.Init();
            mainWindow.Show();

            // このウィンドウを閉じる
            window.Close();
        }

        private static bool CheckEnvironment() {
            // 環境チェックの実装
            // ここでは、Pythonのパスや必要なライブラリの存在を確認する処理を実装する。
            // 例えば、Pythonのバージョンや必要なモジュールがインストールされているかどうかを確認する。
            // 環境チェック
            return PythonExecutor.CheckPythonEnvironment();
        }

        private static void InitPythonEnvironment() {
            // Python環境の初期化処理
            // ここでは、Pythonの仮想環境の作成や必要なライブラリのインストールを行う。
            // 例えば、Pythonのパスを設定し、必要なモジュールをインストールする処理を実装する。
        }

    }
}