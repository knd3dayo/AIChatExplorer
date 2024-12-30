using System.Windows;
using PythonAILib.PythonIF;
using WpfAppCommon.Utils;
using PythonAILib.Model.Script;
using ClipboardApp.Model.Item;
using ClipboardApp.Settings;

namespace ClipboardApp.ViewModel.Content
{

    public partial class ClipboardItemViewModel {

        // EnableDevelopmentFeaturesがTrueの場合のみ有効
        public bool EnableDevelopmentFeatures {
            get { return ClipboardAppConfig.Instance.EnableDevFeatures; }
        }

        // EnableDevelopmentFeaturesがTrueの場合はVisible
        public Visibility DevFeaturesVisibility {
            get {
                return EnableDevelopmentFeatures ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        // メニューの「Pythonスクリプトを実行」をクリックしたときの処理
        public SimpleDelegateCommand<ScriptItem> MenuItemRunPythonScriptCommandExecute => new(async (scriptItem) => {
            try {
                MainWindowViewModel.UpdateProgressCircleVisibility(true);
                // clipboardItemをJsonに変換
                string input_str = Content;
                // Pythonスクリプトを実行
                string result = input_str;
                await Task.Run(() => {
                    string result = PythonExecutor.PythonMiscFunctions.RunScript(scriptItem.Content, input_str);
                    // 結果をClipboardItemに設定
                    Content = result;
                    // 保存
                    Commands.SaveClipboardItemCommand.Execute(true);
                });

            } catch (Exception e) {
                LogWrapper.Error(e.Message);
            } finally {
                MainWindowViewModel.UpdateProgressCircleVisibility(false);
            }

        });

        // コンテキストメニューの「データをマスキング」の実行用コマンド
        public SimpleDelegateCommand<object> MaskDataCommand => new((parameter) => {
            if (ContentItem is not ClipboardItem clipboardItem) {
                return;
            }
            clipboardItem.MaskDataCommandExecute();
            // 保存
            Commands.SaveClipboardItemCommand.Execute(true);

        });





    }
}
