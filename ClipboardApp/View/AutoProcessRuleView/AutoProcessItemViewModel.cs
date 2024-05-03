using System.Collections.ObjectModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.AutoProcessRuleView {
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

        // コンストラクタ
        public AutoProcessItemViewModel(SystemAutoProcessItem autoProcessItem) {
            this.autoProcessItem = autoProcessItem;
        }

        public static ObservableCollection<AutoProcessItemViewModel> SystemAutoProcesses {
            get {
                ObservableCollection<AutoProcessItemViewModel> autoProcesses = [];
                foreach (SystemAutoProcessItem item in AutoProcessItemSystemActions.SystemAutoProcesses) {
                    autoProcesses.Add(new AutoProcessItemViewModel(item));
                }
                return autoProcesses;
            }
        }
        public bool IsCopyOrMoveOrMergeAction() {
            return autoProcessItem.IsCopyOrMoveOrMergeAction();
        }

        // 編集コマンド
        public static SimpleDelegateCommand EditAutoProcessRuleCommand => new(ListAutoProcessRuleWindowViewModel.EditAutoProcessRuleCommandExecute);

    }
}
