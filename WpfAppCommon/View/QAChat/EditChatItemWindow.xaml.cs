using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using QAChat.Model;

namespace WpfAppCommon.View.QAChat {
    /// <summary>
    /// EditChatItemWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditChatItemWindow : Window {
        public EditChatItemWindow() {
            InitializeComponent();
        }

        public static void OpenEditChatItemWindow(ChatItem chatItem) {
            var window = new EditChatItemWindow();
            EditChatItemWindowViewModel model = (EditChatItemWindowViewModel)window.DataContext;
            model.Initialize(chatItem);
            window.Show();
        }

    }

}
