using CommunityToolkit.Mvvm.ComponentModel;
using PythonAILib.Model.AutoGen;

namespace QAChat.ViewModel.AutoGen {
    public class AutoGenAgentViewModel : ObservableObject {


        public AutoGenAgentViewModel(AutoGenAgent autoGenAgent) {
            AutoGenAgent = autoGenAgent;
        }

        public AutoGenAgent AutoGenAgent { get; set; }

        private bool _isChecked;
        public bool IsChecked {
            get => _isChecked;
            set {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }
    }
}
