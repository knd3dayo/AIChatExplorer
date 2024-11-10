using PythonAILib.Model.Content;
using QAChat.ViewModel.ImageChat;
using PythonAILib.Common;

namespace ImageChat
{
    public class MainWindowViewModel : ImageChatMainWindowViewModel {


        public MainWindowViewModel(ContentItem contentItem, Action afterUpdate) : base(contentItem, afterUpdate) {
            Init();
            ImageChatMainWindowViewModel = new ImageChatMainWindowViewModel(contentItem, afterUpdate);
        }

        public void Init() {

            ImageChatPythonAILibConfigParams configParams = new();
            PythonAILibManager.Init(configParams);

        }

        public ImageChatMainWindowViewModel ImageChatMainWindowViewModel { get; set; }

    }
}
