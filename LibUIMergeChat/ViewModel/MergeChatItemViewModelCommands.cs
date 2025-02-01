using LibUIMergeChat.ViewModel;
using PythonAILib.Model.Content;
using PythonAILib.Model.Prompt;
using PythonAILibUI.ViewModel.Item;
using QAChat.ViewModel.Item;
using WpfAppCommon.Utils;

namespace ClipboardApp.ViewModel.Main {
    public class MergeChatItemViewModelCommands : ContentItemViewModelCommands {

        public override SimpleDelegateCommand<ContentItemViewModel> OpenItemCommand => new((itemViewModel) => {
            // 何もしない
        });


        // ピン留めの切り替えコマンド (複数選択可能)
        public SimpleDelegateCommand<MergeChatItemViewModel> ChangeMergeTargetCommand => new((itemViewModel) => {
        });

        // Command to open a folder
        public override void OpenFolder(ContentItem contentItem) {
            // 何もしない
        }

        // Command to open a file
        public override void OpenFile(ContentItem contentItem) {
            // 何もしない
        }

        // Command to open a file as a new file
        public override void OpenFileAsNewFile(ContentItem contentItem) {
            // 何もしない
        }

        // Command to open text content as a file
        public override void OpenContentAsFile(ContentItem contentItem) {
            // 何もしない
        }


        // -----------------------------------------------------------------------------------
        #region プログレスインジケーター表示の処理



        public override void OpenOpenAIChatWindowCommand(ContentItem? item) {
            throw new NotImplementedException();
        }

        #endregion
        // -----------------------------------------------------------------------------------



    }
}
