using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using ClipboardApp.Utils;
using WpfAppCommon.Utils;
using WpfAppCommon.Control;

namespace ClipboardApp.View.StatusMessageView {
    public class StatusMessageWindowViewModel : ObservableObject{
        public ObservableCollection<string> Messages { get; set; } = [];

        public void Initialize() {
            List<string> messages = Tools.StatusText.Messages;
            Messages.Clear();
            foreach (string message in messages) {
                Messages.Add(message);
            }
            OnPropertyChanged("Messages");
        }

        public SimpleDelegateCommand CloseCommand => new ((parameter) => {
            // ウィンドウを閉じる
            if (parameter is not System.Windows.Window window) {
                return;
            }
            window.Close();

        });
    }
}
