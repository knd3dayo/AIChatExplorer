using System.Text.RegularExpressions;
using System.Windows;
using WpfAppCommon.Model;

namespace WpfAppCommon.Utils {
    public partial class Tools {

        public static Window ActiveWindow { get; set; } = Application.Current.MainWindow;

        public static StatusText StatusText {
            get {
                return StatusText.GetStatusText(ActiveWindow);
            }
        }

        public static NLog.Logger Logger { get; } = NLog.LogManager.GetCurrentClassLogger();

        public static void Info(string message) {
            Application.Current.Dispatcher.Invoke(() => {
                Logger.Info(message);
                if (StatusText != null) {
                    StatusText.Text = message;
                }
            });
        }
        public static void Info(string message, bool showMessageBox) {
            Application.Current.Dispatcher.Invoke(() => {
                Logger.Info(message);
                if (showMessageBox) {
                    System.Windows.MessageBox.Show(ActiveWindow, message);
                }
            });
        }

        public static void Warn(string message) {
            Application.Current.Dispatcher.Invoke(() => {
                Logger.Warn(message);
                if (StatusText != null) {
                    StatusText.Text = message;
                }
                // 開発中はメッセージボックスを表示する
                System.Windows.MessageBox.Show(ActiveWindow, message);
            });
        }

        public static void Error(string message) {
            Application.Current.Dispatcher.Invoke(() => {
                Error(message);
                Logger.Error(message);
                if (StatusText != null) {
                    StatusText.Text = message;
                }
                System.Windows.MessageBox.Show(ActiveWindow, message);
            });
        }
        // Listの要素を要素 > 要素 ... の形式にして返す.最後の要素の後には>はつかない
        // Listの要素がNullの場合はNull > と返す
        public static string ListToString(List<string> list) {
            return list == null ? "Null" : string.Join(" > ", list);
        }

        public static int[] GetInAngleBracketPosition(string text) {
            // int[0] = start、int[1] = end
            // < > で囲まれた文字列のStartとEndを返す。< >は含まない。
            Regex regex = MyRegex();
            Match match = regex.Match(text);
            if (match.Success) {
                return [match.Index + 1, match.Index + match.Length - 1];
            }
            return [-1, -1];
        }
        public static int[]? GetURLPosition(string text) {
            // int[0] = start、int[1] = end
            // 正規表現でURLにマッチした場合にStartとEndを返す.URLには日本語が含まれることがある。
            Regex regex = MyRegex1();

            Match match = regex.Match(text);
            if (match.Success) {
                return [match.Index, match.Index + match.Length];
            }

            // 正規表現で \\xxx の形式、または [A-Za-z]:\\xxx の形式の場合はファイルパスとみなし、StartとEndを返す
            regex = MyRegex2();
            match = regex.Match(text);
            if (match.Success) {
                return [match.Index, match.Index + match.Length];
            }
            // それ以外はNullを返す。
            return null;
        }

        public static Action<ActionMessage> DefaultAction {
            get {
                return (action) => {
                    if (action.MessageType == ActionMessage.MessageTypes.Error) {
                        Error(action.Message);

                    } else {
                        Info(action.Message);
                    }

                };
            }
        }

        [GeneratedRegex(@"<[^>]+>")]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"(https?|ftp|file)(:\/\/[-_.!~*\'()a-zA-Z0-9;\/?:\@&=+\$,%#ぁ-んァ-ヴー一-龠]+)")]
        private static partial Regex MyRegex1();
        [GeneratedRegex(@"(\\\\[a-zA-Z0-9_\-]+)+\\[a-zA-Z0-9_\-ぁ-んァ-ヴー一-龠]+")]
        private static partial Regex MyRegex2();
    }
}
