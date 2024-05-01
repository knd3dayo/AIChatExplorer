using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfAppCommon.Utils;

namespace WpfCommonApp.Control.StatusMessage {
    public class StatusMessageWindowViewModel : ObservableObject{
        public ObservableCollection<string> Messages { get; set; } = [];

        public void Initialize() {
            List<string> messages = Tools.StatusText.Messages;
            Messages.Clear();
            foreach (string message in messages) {
                Messages.Add(message);
            }
            OnPropertyChanged(nameof(Messages));
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
