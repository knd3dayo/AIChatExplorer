using System.Windows.Documents;
using PythonAILib.Model.Chat;
using LibUIPythonAI.Utils;
using System.Windows;
using System.ComponentModel;
using LibUIPythonAI.Resource;

namespace LibUIPythonAI.ViewModel.Chat {
    public class EditChatItemWindowViewModel : CommonViewModelBase {

        public ChatMessage ChatItem { get; set; }

        public EditChatItemWindowViewModel(ChatMessage chatItem) {
            ChatItem = chatItem;
            OnPropertyChanged(nameof(ChatItem));

            CommonViewModelProperties.PropertyChanged += OnPropertyChanged;
        }


        // OnPropertyChanged
        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CommonViewModelProperties.MarkdownView)) {
                OnPropertyChanged(nameof(ChatItem));
                OnPropertyChanged(nameof(MarkdownVisibility));
                OnPropertyChanged(nameof(TextVisibility));

            }
        }
        public Visibility MarkdownVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView);

        public Visibility TextVisibility => LibUIPythonAI.Utils.Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView == false);



        // FlowDocument
        public FlowDocument ContentFlowDocument => LibPythonAI.Utils.Common.Tools.CreateFlowDocument(ChatItem.Content);

        public FlowDocument SourcesFlowDocument => LibPythonAI.Utils.Common.Tools.CreateFlowDocument(ChatItem.SourcesString);

        // CloseButtonを押した時の処理
        public override SimpleDelegateCommand<object> CloseCommand => new((parameter) => {
            CommonViewModelProperties.PropertyChanged -= OnPropertyChanged;
            base.CloseCommand.Execute(parameter);
        });
    }
}
