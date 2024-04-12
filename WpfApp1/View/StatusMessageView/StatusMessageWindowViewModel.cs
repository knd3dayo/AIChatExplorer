using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using WpfApp1.Utils;

namespace WpfApp1.View.StatusMessageView {
    public class StatusMessageWindowViewModel : ObservableObject{
        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public void Initialize() {
            List<string> messages = MainWindowViewModel.StatusText.Messages;
            Messages.Clear();
            foreach (string message in messages) {
                Messages.Add(message);
            }
            OnPropertyChanged("Messages");
        }

        public SimpleDelegateCommand CloseCommand => new SimpleDelegateCommand((parameter) => {
            StatusMessageWindow.Current?.Close();
        });
    }
}
