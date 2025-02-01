using QAChat.Model;
using QAChat.ViewModel;
using WpfAppCommon.Utils;

namespace MergeChat.ViewModel {
    public class MergeChatWindowViewModel : QAChatViewModelBase {

        //初期化
        public MergeChatWindowViewModel(QAChatStartupProps props) {
            // PythonAILibのLogWrapperのログ出力設定
            PythonAILib.Utils.Common.LogWrapper.SetActions(LogWrapper.Info, LogWrapper.Warn, LogWrapper.Error);
            
            MergeTargetDataGridViewControlViewModel mergeTargetDataGridViewControlViewModel = new() {
                UpdateIndeterminateAction = UpdateIndeterminate,
            };

            MergeTargetTreeViewControlViewModel mergeTargetTreeViewControlViewModel = new() {
                UpdateIndeterminateAction = UpdateIndeterminate,
                SelectedFolderChangedAction = (folder) => {
                    mergeTargetDataGridViewControlViewModel.SelectedFolder = folder;
                }
            };

            MergeTargetPanelViewModel mergeTargetPanelViewModel = new(mergeTargetDataGridViewControlViewModel, mergeTargetTreeViewControlViewModel);

            // QAChatControlViewModelを生成
            MergeChatControlViewModel = new(props, mergeTargetPanelViewModel);

        }
        // QAChatControlのViewModel
        public MergeChatControlViewModel MergeChatControlViewModel { get; set; }

        // 
        public static Action<bool> UpdateProgressCircleVisibility { get; set; } = (visible) => { };

        public void UpdateIndeterminate(bool visible) {
            IsIndeterminate = visible;
            OnPropertyChanged(nameof(IsIndeterminate));
        }

    }
}
