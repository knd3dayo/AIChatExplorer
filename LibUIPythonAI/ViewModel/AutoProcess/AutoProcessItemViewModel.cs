using System.Collections.ObjectModel;
using LibPythonAI.Model.AutoProcess;
using PythonAILib.Model.AutoProcess;

namespace LibUIPythonAI.ViewModel.AutoProcess {
    public class AutoProcessItemViewModel(AutoProcessItem autoProcessItem) : ChatViewModelBase {
        public AutoProcessItem AutoProcessItem { get; set; } = autoProcessItem;

        public string Name {
            get {
                return AutoProcessItem.Name;
            }
            set {
                AutoProcessItem.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string DisplayName {
            get {
                return AutoProcessItem.DisplayName;
            }
            set {
                AutoProcessItem.DisplayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        // システムデフォルトのAutoProcessItemを取得
        public static ObservableCollection<AutoProcessItemViewModel> SystemAutoProcesses {
            get {
                ObservableCollection<AutoProcessItemViewModel> autoProcesses = [];
                foreach (AutoProcessItem item in AutoProcessItem.SystemAutoProcesses) {
                    autoProcesses.Add(new AutoProcessItemViewModel(item));
                }
                return autoProcesses;
            }
        }

        public bool IsCopyOrMoveAction() {
            return AutoProcessItem.IsCopyOrMoveAction();
        }

    }
}
