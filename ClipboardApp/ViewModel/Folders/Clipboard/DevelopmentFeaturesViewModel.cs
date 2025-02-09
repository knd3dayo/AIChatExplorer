using System.Windows;
using ClipboardApp.Model.Item;
using ClipboardApp.Settings;
using ClipboardApp.ViewModel.Folders.Clipboard;
using LibUIPythonAI.Utils;
using PythonAILib.PythonIF;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Content {

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

        // コンテキストメニューの「データをマスキング」の実行用コマンド
        public SimpleDelegateCommand<object> MaskDataCommand => new((parameter) => {
            if (ContentItem is not ClipboardItem clipboardItem) {
                return;
            }
            clipboardItem.MaskDataCommandExecute();
            // 保存
            ClipboardItemViewModelCommands command = new();
            command.SaveClipboardItemCommand.Execute(true);

        });





    }
}
