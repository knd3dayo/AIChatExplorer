using System.Collections.ObjectModel;
using ClipboardApp.Model.AutoProcess;

namespace ClipboardApp.ViewModel.AutoProcess {
    public class AutoProcessItemViewModel(SystemAutoProcessItem autoProcessItem) : ClipboardAppViewModelBase {
        public SystemAutoProcessItem AutoProcessItem { get; set; } = autoProcessItem;

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
                foreach (SystemAutoProcessItem item in SystemAutoProcessItem.SystemAutoProcesses) {
                    autoProcesses.Add(new AutoProcessItemViewModel(item));
                }
                return autoProcesses;
            }
        }
        // ScriptAutoProcessItemを取得
        public static ObservableCollection<AutoProcessItemViewModel> ScriptAutoProcesses {
            get {
                ObservableCollection<AutoProcessItemViewModel> autoProcesses = [];
                foreach (ScriptAutoProcessItem item in ScriptAutoProcessItem.GetScriptAutoProcessItems()) {
                    autoProcesses.Add(new AutoProcessItemViewModel(item));
                }
                return autoProcesses;
            }
        }

        public bool IsCopyOrMoveOrMergeAction() {
            return AutoProcessItem.IsCopyOrMoveOrMergeAction();
        }

    }
}
