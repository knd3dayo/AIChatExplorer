using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using QAChat.ViewModel;
using WpfAppCommon.Model;
using WpfAppCommon.Utils;

namespace QAChat.Control {
    public class AdditionalItemViewModel {


        public QAChatControlViewModel QAChatControlViewModel { get; set; }
        public ClipboardItem ClipboardItem { get; set; }

        public AdditionalItemViewModel(QAChatControlViewModel qaChatControlViewModel, ClipboardItem clipboardItem) {
            QAChatControlViewModel = qaChatControlViewModel;
            ClipboardItem = clipboardItem;
        }

        // RemoveSelectedItemCommand
        public SimpleDelegateCommand<object> RemoveSelectedItemCommand => new((parameter) => {
            QAChatControlViewModel.AdditionalItems.Remove(this);
        });

        // OpenSelectedItemCommand
        public SimpleDelegateCommand<object> OpenSelectedItemCommand => new((parameter) => {
            QAChatControlViewModel.QAChatStartupProps?.OpenSelectedItemCommand(ClipboardItem);
        });
    }
}
