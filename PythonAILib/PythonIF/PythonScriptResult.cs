using PythonAILib.Utils.Common;

namespace PythonAILib.PythonIF {
    public class PythonScriptResult {
        public string Output { get; set; } = "";
        public string Log { get; set; } = "";

        public string Error { get; set; } = "";

        public Dictionary<string, dynamic?> Parameters { get; protected set; } = [];

        public virtual void LoadFromJson(string json) {
            Parameters = JsonUtil.ParseJson(json);
            // レスポンスと詳細を取得
            if (Parameters.TryGetValue("output", out dynamic? outputValue)) {
                Output = outputValue ?? "";
            }
            // verboseがあれば取得
            if (Parameters.TryGetValue("log", out dynamic? logValue)) {
                Log = logValue ?? "";
            }
            // errorがあれば取得
            if (Parameters.TryGetValue("error", out dynamic? errorValue)) {
                Error = errorValue ?? "";
            }
        }
    }
}
