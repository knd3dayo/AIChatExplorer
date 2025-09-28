using System.Collections.ObjectModel;
using LibMain.Model.AutoProcess;
using LibUIMain.Resource;

namespace LibUIMain.ViewModel.AutoProcess {
    public class AutoProcessItemViewModel(AutoProcessItem autoProcessItem) : CommonViewModelBase {
        public AutoProcessItem AutoProcessItem { get; set; } = autoProcessItem;

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
                foreach (AutoProcessItem item in AutoProcessItem.GetSystemDefinedItems()) {
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
