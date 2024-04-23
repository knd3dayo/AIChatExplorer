using System.Text.RegularExpressions;
using System.Windows;
using ClipboardApp.Model;

namespace ClipboardApp.Utils {
    public class Tools {
        public static StatusText? StatusText { get; set; }

        public static NLog.Logger Logger { get; } = NLog.LogManager.GetCurrentClassLogger();

        public static void Debug(string message) {
            Logger.Debug(message);
            // 開発中はメッセージボックスを表示する
            System.Windows.MessageBox.Show(message);
        }

        public static void Info(string message) {
            Logger.Info(message);
            if (StatusText != null) {
                StatusText.Text = message;
            }
        }
        public static void Warn(string message) {
            Logger.Warn(message);
            if (StatusText != null) {
                StatusText.Text = message;
            }
            // 開発中はメッセージボックスを表示する
            System.Windows.MessageBox.Show(message);
        }

        public static void Error(string message) {
            Logger.Error(message);
            if (StatusText != null) {
                StatusText.Text = message;
            }
            System.Windows.MessageBox.Show(message);
        }
        // Listの要素を要素 > 要素 ... の形式にして返す.最後の要素の後には>はつかない
        // Listの要素がNullの場合はNull > と返す
        public static string ListToString(List<string> list) {
            return list == null ? "Null" : string.Join(" > ", list);
        }
        
        public static int[] GetInAngleBracketPosition(string text) {
            // int[0] = start、int[1] = end
            // < > で囲まれた文字列のStartとEndを返す。< >は含まない。
            Regex regex = new Regex(@"<[^>]+>");
            Match match = regex.Match(text);
            if (match.Success) {
                return new int[] { match.Index + 1, match.Index + match.Length - 1 };
            }
            return new int[] { -1, -1 };
        }
        public static int[]? GetURLPosition(string text) {
            // int[0] = start、int[1] = end
            // 正規表現でURLにマッチした場合にStartとEndを返す.URLには日本語が含まれることがある。
            Regex regex = new Regex(@"(https?|ftp|file)(:\/\/[-_.!~*\'()a-zA-Z0-9;\/?:\@&=+\$,%#ぁ-んァ-ヴー一-龠]+)");

            Match match = regex.Match(text);
            if (match.Success) {
                return new int[] { match.Index, match.Index + match.Length };
            }

            // 正規表現で \\xxxx の形式、または [A-Za-z]:\\xxxx の形式の場合はファイルパスとみなし、StartとEndを返す
            regex = new Regex(@"(\\\\[a-zA-Z0-9_\-]+)+\\[a-zA-Z0-9_\-ぁ-んァ-ヴー一-龠]+");
            match = regex.Match(text);
            if (match.Success) {
                return new int[] { match.Index, match.Index + match.Length };
            }
            // それ以外はNullを返す。
            return null;
        }

        public static Action<ActionMessage> DefaultAction { get; } = (action) => {
            if (action.MessageType == ActionMessage.MessageTypes.Error) {
                Error(action.Message);
            } else {
                Info(action.Message);
            }

        };
    }
}
