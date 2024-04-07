using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Model;

namespace WpfApp1.Utils
{
    public class Tools
    {
        public static StatusText? StatusText;
        public static void ShowMessage(string message)
        {
            System.Windows.MessageBox.Show(message);
        }
        public static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Debug(string message)
        {
            Logger.Debug(message);
            // 開発中はメッセージボックスを表示する
            System.Windows.MessageBox.Show(message);
        }

        public static void Info(string message)
        {
            Logger.Info(message);
            if (StatusText != null)
            {
                StatusText.Text = message;
            }
        }
        public static void Warn(string message)
        {
            Logger.Warn(message);
            if (StatusText != null)
            {
                StatusText.Text = message;
            }
            // 開発中はメッセージボックスを表示する
            System.Windows.MessageBox.Show(message);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
            if (StatusText != null)
            {
                StatusText.Text = message;
            }
            System.Windows.MessageBox.Show(message);
        }
        // Listの要素を要素 > 要素 ... の形式にして返す.最後の要素の後には>はつかない
        // Listの要素がNullの場合はNull > と返す
        public static string ListToString(List<string> list)
        {
            if (list == null)
            {
                return "Null > ";
            }
            string result = "";
            for (int i = 0; i < list.Count; i++)
            {
                result += list[i];
                if (i < list.Count - 1)
                {
                    result += " > ";
                }
            }
            return result;
        }

    }
}
