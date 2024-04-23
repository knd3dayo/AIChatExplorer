using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using QAChat.Model;

namespace QAChat.View.PromptTemplateWindow {
    public class PromptItemViewModel : ObservableObject{
        public PromptItem PromptItem { get; set; }
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
        public PromptItemViewModel(PromptItem promptItem) {
            PromptItem = promptItem;
            if (promptItem != null) {
                Content = promptItem.Prompt;
                Description = promptItem.Description;
            }
        }
    }
}
