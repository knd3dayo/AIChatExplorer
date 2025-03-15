using LibPythonAI.PythonIF.Response;
using PythonAILib.Utils;

namespace PythonAILib.Model.Chat {
    public class ChatResult : PythonScriptResult {

        public List<string> PageContentList { get; set; } = [];
        public List<Dictionary<string, string>> PageSourceList { get; set; } = [];

        public long TotalTokens { get; set; } = 0;

        public ChatResult() { }

        public override void LoadFromJson(string json) {
            base.LoadFromJson(json);
            // total_tokensがあれば取得
            if (Parameters.TryGetValue("total_tokens", out dynamic? value)) {
                TotalTokens = value ?? 0;
            }
            // page_source_listがあれば取得
            if (Parameters.TryGetValue("page_source_list", out dynamic? pageSourceListValue)) {
                PageSourceList = pageSourceListValue ?? new List<Dictionary<string, string>>();
            }
            // page_content_listがあれば取得
            if (Parameters.TryGetValue("page_content_list", out dynamic? pageContentListValue)) {
                PageContentList = pageContentListValue ?? new List<string>();
            }
        }
    }

}
