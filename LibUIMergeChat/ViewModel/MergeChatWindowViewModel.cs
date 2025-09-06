using System.Collections.ObjectModel;
using LibPythonAI.Utils.Common;
using LibUIMain.Resource;
using LibUIMain.Utils;
using LibUIMain.ViewModel;
using LibUIMain.ViewModel.Folder;
using LibUIMain.ViewModel.Item;

namespace LibUIMergeChat.ViewModel {
    public class MergeChatWindowViewModel : CommonViewModelBase {

        //初期化
        public MergeChatWindowViewModel(ContentFolderViewModel folderViewModel, ObservableCollection<ContentItemViewModel> selectedItems) {
            // PythonAILibのLogWrapperのログ出力設定
            LogWrapper.SetActions(new LogWrapperAction());

            MergeTargetPanelViewModel mergeTargetPanelViewModel = new(folderViewModel, selectedItems, CommonViewModelProperties.UpdateIndeterminate);

            // ChatControlViewModelを生成
            MergeChatControlViewModel = new(mergeTargetPanelViewModel);

        }

        public MergeChatControlViewModel MergeChatControlViewModel { get; set; }

    }
}
