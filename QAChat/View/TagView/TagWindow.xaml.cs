using System.Windows;
using ClipboardApp.ViewModel;
using PythonAILib.Model;
using QAChat.ViewModel.TagWindow;

namespace QAChat.View.TagView {
    /// <summary>
    /// TagWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TagWindow : Window {
        public TagWindow() {
            InitializeComponent();
        }
        public static void OpenTagWindow(ContentItemBase? contentItem, Action action) {
            TagWindow tagWindow = new();
            TagWindowViewModel tagWindowViewModel =new(contentItem, action);
            tagWindow.DataContext = tagWindowViewModel;
            tagWindow.ShowDialog();
        }
    }
}
