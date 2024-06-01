using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using QAChat.Model;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon.Control.QAChat;
using WpfAppCommon.Control.Settings;
using WpfAppCommon.Model;
using WpfAppCommon.PythonIF;
using WpfAppCommon.Utils;

namespace QAChat {
    public partial class MainWindowViewModel : MyWindowViewModel {

        //初期化
        public void Initialize(ClipboardItem? clipboardItem) {

            QAChatControlViewModel.Initialize(clipboardItem, PromptTemplateCommandExecute);

        }
        public Action ShowSearchWindowAction {
            get {
                return QAChatControlViewModel.ShowSearchWindowAction;
            }
            set {
                QAChatControlViewModel.ShowSearchWindowAction = value;
                OnPropertyChanged(nameof(ShowSearchWindowAction));
            }
        }
        // ClipboardItemを選択するアクション
        public Action SetContentTextFromClipboardItemsAction {
            get {
                return QAChatControlViewModel.SetContentTextFromClipboardItemsAction;
            }
            set {
                QAChatControlViewModel.SetContentTextFromClipboardItemsAction = value;
                OnPropertyChanged(nameof(SetContentTextFromClipboardItemsAction));
            }
        }

        public string ContextText {
            get {
                return QAChatControlViewModel.ContextText;
            }
            set {
                QAChatControlViewModel.ContextText = value;
                OnPropertyChanged(nameof(ContextText));
            }
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

        // 内部から起動されたか否か
        private bool isStartFromInternalApp = true;
        public bool IsStartFromInternalApp {
            get {
                return isStartFromInternalApp;
            }
            set {
                isStartFromInternalApp = value;
                OnPropertyChanged(nameof(IsStartFromInternalApp));
            }
        }

        // 設定画面を開くコマンド
        public SimpleDelegateCommand<object> SettingCommand => new((parameter) => {
            // SettingUserControlを生成してWindowを表示する。
            SettingsUserControl settingsControl = new();
            Window window = new() {
                Title = StringResources.Instance.SettingWindowTitle,
                Content = settingsControl
            };
            window.ShowDialog();
        }

        );

        private void PromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                QAChatControlViewModel.PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;

            });
        }

    }
}
