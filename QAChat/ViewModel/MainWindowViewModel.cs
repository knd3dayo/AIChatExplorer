using System.Collections.ObjectModel;
using System.Windows;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon.Control.QAChat;
using WpfAppCommon.Control.Settings;
using WpfAppCommon.Model;
using WpfAppCommon.Model.ClipboardApp;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel {
    public partial class MainWindowViewModel : MyWindowViewModel {

        // OnActivatedAction
        public override void OnActivatedAction() {
            if (ClipboardFolder == null) {
                return;
            }
            // StatusText.Readyにフォルダ名を設定
            Tools.StatusText.ReadyText = $"{StringResources.Folder}:[{ClipboardFolder?.FolderName}]";
            // StatusText.Textにフォルダ名を設定
            Tools.StatusText.Text = $"{StringResources.Folder}:[{ClipboardFolder?.FolderName}]";
        }
        private ClipboardFolder? _ClipboardFolder;
        public ClipboardFolder? ClipboardFolder {
            get {
                return _ClipboardFolder;
            }
            set {
                _ClipboardFolder = value;
                OnPropertyChanged(nameof(ClipboardFolder));
            }
        }

        //初期化
        public void Initialize(QAChatStartupProps props) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
            QAChatControlViewModel.Initialize(props, PromptTemplateCommandExecute);
        }

        // 選択中のフォルダの全てのClipboardItem
        public ObservableCollection<ClipboardItem> ClipboardItems {
            get {
                return QAChatControlViewModel.ClipboardItems;
            }
            set {
                QAChatControlViewModel.ClipboardItems = value;
                OnPropertyChanged(nameof(ClipboardItems));
            }
        }

        // QAChatControlのViewModel
        public QAChatControlViewModel QAChatControlViewModel { get; set; } = new();


        // 設定画面を開くコマンド
        public SimpleDelegateCommand<object> SettingCommand => new((parameter) => {
            // SettingUserControlを生成してWindowを表示する。
            SettingsUserControl settingsControl = new();
            Window window = new() {
                SizeToContent = SizeToContent.Height,
                Title = CommonStringResources.Instance.SettingWindowTitle,
                Content = settingsControl
            };
            window.ShowDialog();
        });
        private void PromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                QAChatControlViewModel.PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;
            },
            // PromptItemBaseを生成する関数
            () => {
                return new PromptItem();
            }

            );
        }
    }
}
