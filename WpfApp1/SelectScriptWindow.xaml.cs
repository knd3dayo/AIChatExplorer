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

namespace WpfApp1
{
    /// <summary>
    /// SelectScriptWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SelectScriptWindow : Window
    {
        public static SelectScriptWindow? Current { get; set; }
        public SelectScriptWindow()
        {
            InitializeComponent();
            Current = this;
        }
    }
}
