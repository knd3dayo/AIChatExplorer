using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfAppCommon.Utils;

namespace ClipboardApp.View.AutoProcessRuleView {
    public class AutoProcessItemViewModel {


        // 編集コマンド
        public static SimpleDelegateCommand EditAutoProcessRuleCommand => new SimpleDelegateCommand(ListAutoProcessRuleWindowViewModel.EditAutoProcessRuleCommandExecute);

    }
}
