using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using LibPythonAI.Utils.Common;
using Markdig;
using Neo.Markdig.Xaml;
using System.Windows.Documents;
using System.Xml;
using System.Windows.Markup;

namespace LibUIPythonAI.Utils {
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
        //------------
        // 親フォルダのパスと子フォルダ名を連結する。ファイルシステム用
        public static string ConcatenateFileSystemPath(string parentPath, string childPath) {
            if (string.IsNullOrEmpty(parentPath))
                return childPath;
            if (string.IsNullOrEmpty(childPath))
                return parentPath;
            return Path.Combine(parentPath, childPath);
        }
        // ------------
        // Visibilityを切り替える TrueならVisible、FalseならCollapsed
        public static Visibility BoolToVisibility(bool value) {
            return value ? Visibility.Visible : Visibility.Collapsed;
        }


        [GeneratedRegex(@"<[^>]+>")]
        private static partial Regex MyRegex();
        [GeneratedRegex(@"(https?|ftp|file)(:\/\/[-_.!~*\'()a-zA-Z0-9;\/?:\@&=+\$,%#ぁ-んァ-ヴー一-龠]+)")]
        private static partial Regex MyRegex1();
        [GeneratedRegex(@"(\\\\[a-zA-Z0-9_\-]+)+\\[a-zA-Z0-9_\-ぁ-んァ-ヴー一-龠]+")]
        private static partial Regex MyRegex2();



    }
}
