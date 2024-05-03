using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.AutoProcessRuleView {
    public class AutoProcessItemViewModel {

        private readonly SystemAutoProcessItem autoProcessItem;

        public  SystemAutoProcessItem AutoProcessItem => autoProcessItem;

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
            return  autoProcessItem.IsCopyOrMoveOrMergeAction();
        }

        // 編集コマンド
        public static SimpleDelegateCommand EditAutoProcessRuleCommand => new (ListAutoProcessRuleWindowViewModel.EditAutoProcessRuleCommandExecute);

    }
}
