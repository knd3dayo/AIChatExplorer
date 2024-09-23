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
using ClipboardApp.Model;
using ClipboardApp.ViewModel;

namespace ClipboardApp.View.AutoProcessRuleView
{
    /// <summary>
    /// EditAutoProcessRuleWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class EditAutoProcessRuleWindow : Window
    {
        public EditAutoProcessRuleWindow()
        {
            InitializeComponent();
        }
        public static void OpenEditAutoProcessRuleWindow(EditAutoProcessRuleWindowViewModel.Mode mode, MainWindowViewModel? mainWindowViewModel, AutoProcessRule? autoProcessRule, Action<AutoProcessRule> afterUpdate) {
            EditAutoProcessRuleWindow editAutoProcessRuleWindow = new() {
                DataContext = new EditAutoProcessRuleWindowViewModel(mode, mainWindowViewModel, autoProcessRule, afterUpdate)
            };
            editAutoProcessRuleWindow.ShowDialog();
        }
    }
}
