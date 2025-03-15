using CommunityToolkit.Mvvm.ComponentModel;
using LibPythonAI.Model.AutoGen;

namespace LibUIPythonAI.ViewModel.AutoGen {
    public class AutoGenToolViewModel : ObservableObject{

        // AutoGenToolViewModel
        public AutoGenToolViewModel(AutoGenTool autoGenTool) {
            AutoGenTool = autoGenTool;
        }

        public AutoGenTool AutoGenTool { get; set; }

        // ToolsForExecution IsChecked
        private bool _toolIsChecked = false;
        public bool ToolIsChecked {
            get { return _toolIsChecked; }
            set {
                _toolIsChecked = value;
                OnPropertyChanged(nameof(ToolIsChecked));
            }
        }

    }
}
