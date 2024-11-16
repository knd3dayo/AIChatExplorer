using System.Windows;
using ClipboardApp.ViewModel;
using PythonAILib.Model.Content;
using QAChat.ViewModel.TagWindow;

namespace QAChat.View.Tag
{
    /// <summary>
    /// TagWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class TagWindow : Window {
        public TagWindow() {
            InitializeComponent();
        }
        public static void OpenTagWindow(ContentItem? contentItem, Action action) {
            TagWindow tagWindow = new();
            TagWindowViewModel tagWindowViewModel =new(contentItem, action);
            tagWindow.DataContext = tagWindowViewModel;
            tagWindow.ShowDialog();
        }
    }
}
