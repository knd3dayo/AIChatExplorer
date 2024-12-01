using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.AutoGen;

namespace QAChat.ViewModel.AutoGen {
    public class AutoGenToolViewModel : ObservableObject{

        // AutoGenToolViewModel
        public AutoGenToolViewModel(AutoGenTool autoGenTool) {
            AutoGenTool = autoGenTool;
        }

        public AutoGenTool AutoGenTool { get; set; }

        // ToolsForExecution IsChecked
        private bool _toolsForExecutionIsChecked = false;
        public bool ToolsForExecutionIsChecked {
            get { return _toolsForExecutionIsChecked; }
            set {
                _toolsForExecutionIsChecked = value;
                OnPropertyChanged(nameof(ToolsForExecutionIsChecked));
            }
        }

        // ToolsForLLM IsChecked
        private bool _toolsForLLMIsChecked = false;
        public bool ToolsForLLMIsChecked {
            get { return _toolsForLLMIsChecked; }
            set {
                _toolsForLLMIsChecked = value;
                OnPropertyChanged(nameof(ToolsForLLMIsChecked));
            }
        }

    }
}
