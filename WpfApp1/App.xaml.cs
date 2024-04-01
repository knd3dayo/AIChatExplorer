using System.Configuration;
using System.Data;
using System.Windows;
using Python.Runtime;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                base.OnStartup(e);
                // here you take control

            }catch(System.Exception ex)
            {
                // error.logにエラーログを出力
                System.IO.File.AppendAllText("error.log", ex.Message + "\n" + ex.StackTrace + "\n");
            }

        }
    }

}
