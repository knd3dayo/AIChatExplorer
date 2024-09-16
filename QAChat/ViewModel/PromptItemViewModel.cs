using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.Abstract;

namespace QAChat.ViewModel
{
    public class PromptItemViewModel : ObservableObject {
        public PromptItemBase PromptItem { get; set; }
        public string Content {
            get => PromptItem.Prompt;
            set {
                PromptItem.Prompt = value;
                OnPropertyChanged(nameof(Content));
            }
        }
        public string Description {
            get => PromptItem.Description;
            set {
                PromptItem.Description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Name {
            get => PromptItem.Name;
            set {
                PromptItem.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public PromptItemViewModel(PromptItemBase promptItem) {
            PromptItem = promptItem;
            if (promptItem != null) {
                Content = promptItem.Prompt;
                Description = promptItem.Description;
            }
        }
    }
}
