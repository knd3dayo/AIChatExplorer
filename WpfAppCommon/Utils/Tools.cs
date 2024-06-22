using System.Text.RegularExpressions;
using System.Windows;
using NLog;
using WpfAppCommon.Model;

namespace WpfAppCommon.Utils {
    public partial class Tools {

        private static Window _ActiveWindow = Application.Current.MainWindow;
        public static Window ActiveWindow {
            get {
                return _ActiveWindow;
            }
            set {
                // CustomLoggerのActiveWindowを設定する
                CustomLogger.ActiveWindow = value;
                _ActiveWindow = value;
            }
        }

        public static StatusText StatusText {
            get {
                return StatusText.GetStatusText(ActiveWindow); ;
            }
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
                        LogWrapper.Error(action.Message);

                    } else {
                        LogWrapper.Info(action.Message);
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
