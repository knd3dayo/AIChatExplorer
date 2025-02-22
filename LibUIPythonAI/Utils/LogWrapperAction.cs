using System.Text.RegularExpressions;
using LibPythonAI.Utils.Common;
using NLog;
using WpfAppCommon.Model;

namespace LibUIPythonAI.Utils {
    public class LogWrapperAction : ILogWrapperAction {

        public static CustomLogger Logger { get; } = LogManager.LogFactory.GetCurrentClassLogger<CustomLogger>();

        public void Debug(string message) {
            message = MaskAPIKey(message);
            Logger.Debug(message);
        }

        public void Info(string message) {
            message = MaskAPIKey(message);
            Logger.Info(message);
        }

        public void Warn(string message) {
            message = MaskAPIKey(message);
            Logger.Warn(message);
        }

        public void Error(string message) {
            message = MaskAPIKey(message);
            Logger.Error(message);
        }

        public void UpdateInProgress(bool value, string message = "") {

            StatusText.Instance.UpdateInProgress(true, message);
        }


        // OpenAIKey, api_key, OPENAI_API_KEYはログに出力しないよう、messageに含まれている場合はマスクする
        // 大文字小文字を区別しない
        // 例： api_key:"12345678" は api_key:"********" に変換
        private static string MaskAPIKey(string message) {
            string[] maskWords = { "OpenAIKey", "api_key", "OPENAI_API_KEY" };
            foreach (string maskWord in maskWords) {
                // 正規表現でマスクする, 大文字小文字を区別しない.後方参照を使う
                Regex regex = new($"({maskWord}[.:\"]+)[a-zA-Z0-9-]+", RegexOptions.IgnoreCase);
                message = regex.Replace(message, $"$1********");

            }
            return message;
        }



    }
}
