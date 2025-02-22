using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.AutoGen;

namespace LibUIPythonAI.ViewModel.AutoGen {
    public class AutoGenAgentViewModel : ObservableObject {
        public AutoGenAgentViewModel(AutoGenAgent autoGenAgent) {
            AutoGenAgent = autoGenAgent;
        }

        public AutoGenAgent AutoGenAgent { get; set; }

        // Name
        public string Name {
            get => AutoGenAgent.Name;
        }

        private bool _isChecked;
        public bool IsChecked {
            get => _isChecked;
            set {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        public override bool Equals(object? other)  {
            if (other is AutoGenAgentViewModel otherAgent) {
                return AutoGenAgent.Name == otherAgent.AutoGenAgent.Name;
            }
            return false;
        }
        public override int GetHashCode() {
            return AutoGenAgent.Name.GetHashCode();
        }

    }
}
