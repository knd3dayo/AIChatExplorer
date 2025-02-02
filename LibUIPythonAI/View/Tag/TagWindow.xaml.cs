using System.Windows;
using PythonAILib.Model.Content;
using LibUIPythonAI.ViewModel.Tag;

namespace LibUIPythonAI.View.Tag
{
    /// <summary>
    /// Tag.xaml の相互作用ロジック
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
