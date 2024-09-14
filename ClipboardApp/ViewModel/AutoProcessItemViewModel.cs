using System.Collections.ObjectModel;
using ClipboardApp.Model;
using WpfAppCommon.Model;

namespace ClipboardApp.ViewModel
{
    public class AutoProcessItemViewModel : MyWindowViewModel {

        private readonly SystemAutoProcessItem autoProcessItem;

        public SystemAutoProcessItem AutoProcessItem => autoProcessItem;

        public string Name {
            get {
                return autoProcessItem.Name;
            }
            set {
                autoProcessItem.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string DisplayName {
            get {
                return autoProcessItem.DisplayName;
            }
            set {
                autoProcessItem.DisplayName = value;
                OnPropertyChanged(nameof(DisplayName));
            }
        }

        // コンストラクタ
        public AutoProcessItemViewModel(SystemAutoProcessItem autoProcessItem) {
            this.autoProcessItem = autoProcessItem;
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
            return autoProcessItem.IsCopyOrMoveOrMergeAction();
        }

    }
}
