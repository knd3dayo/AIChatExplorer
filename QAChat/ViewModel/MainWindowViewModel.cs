using System.Collections.ObjectModel;
using System.Windows;
using PythonAILib.Model;
using QAChat.Control;
using QAChat.View.PromptTemplateWindow;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.ViewModel {
    public  class MainWindowViewModel : MyWindowViewModel {


        //初期化
        public MainWindowViewModel(QAChatStartupProps props) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
            // QAChatControlViewModelを生成
            QAChatControlViewModel = new(props, PromptTemplateCommandExecute);
        }
        // QAChatControlのViewModel
        public QAChatControlViewModel QAChatControlViewModel { get; set; }


        // 選択中のフォルダの全てのClipboardItem
        public ObservableCollection<ContentItemBase> ClipboardItems {
            get {
                return QAChatControlViewModel.ClipboardItems;
            }
            set {
                QAChatControlViewModel.ClipboardItems = value;
                OnPropertyChanged(nameof(ClipboardItems));
            }
        }

        private void PromptTemplateCommandExecute(object parameter) {
            ListPromptTemplateWindow.OpenListPromptTemplateWindow(ListPromptTemplateWindowViewModel.ActionModeEum.Select, (promptTemplateWindowViewModel, Mode) => {
                QAChatControlViewModel.PromptText = promptTemplateWindowViewModel.PromptItem.Prompt;
            });
        }
    }
}
