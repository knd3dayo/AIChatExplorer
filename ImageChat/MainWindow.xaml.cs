using System.Windows;

namespace ImageChat {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(new PythonAILib.Model.Content.ContentItem(), () => { });
        }
    }
}