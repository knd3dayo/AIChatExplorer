using System.Windows.Documents;
using LibUIMain.Utils;
using System.Windows;
using System.ComponentModel;
using LibUIMain.Resource;
using LibMain.Model.Chat;

namespace LibUIMain.ViewModel.Chat {
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
        public Visibility MarkdownVisibility => LibUIMain.Utils.Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView);

        public Visibility TextVisibility => LibUIMain.Utils.Tools.BoolToVisibility(CommonViewModelProperties.MarkdownView == false);


        // EditorFontSize
        public double EditorFontSize => CommonViewModelProperties.EditorFontSize;

        // FlowDocument
        public FlowDocument ContentFlowDocument => LibMain.Utils.Common.Tools.CreateFlowDocument(ChatItem.Content);

        public FlowDocument SourcesFlowDocument => LibMain.Utils.Common.Tools.CreateFlowDocument(ChatItem.SourcesString);

        // CloseButtonを押した時の処理
        public override SimpleDelegateCommand<object> CloseCommand => new((parameter) => {
            CommonViewModelProperties.PropertyChanged -= OnPropertyChanged;
            base.CloseCommand.Execute(parameter);
        });
    }
}
