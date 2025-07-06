using System.Text.Json;
using System.Text.Json.Serialization;
using LibPythonAI.PythonIF.Request;
using LibPythonAI.Resources;
using LibPythonAI.Utils.Common;

namespace LibPythonAI.Model.Chat {
    public class ChatSettings {
        public const string VECTOR_SEARCH_REQUESTS_KEY = "vector_search_requests";
        public const string VECTOR_SEARCH_REQUESTS_JSON_KEY = "vector_search_requests_json";
        public const string TEMPERATURE_KEY = "temperature";
        public const string RELATED_ITEMS_KEY = "related_items";
        public const string SEND_RELATED_ITEMS_ONLY_FIRST_REQUEST_KEY = "send_related_items_only_first_request";
        public const string AUTOGEN_PROPS_KEY = "autogen_props";
        public const string SPLIT_TOKEN_COUNT_KEY = "split_token_count";
        public const string PROMPT_TEMPLATE_TEXT_KEY = "prompt_template_text";
        public const string SPLIT_MODE_KEY = "split_mode";
        public const string SUMMARIZE_PROMPT_TEXT_KEY = "summarize_prompt_text";
        public const string RELATED_INFORMATION_PROMPT_TEXT_KEY = "related_information_prompt_text";
        public const string RAG_MODE_KEY = "rag_mode";
        public const string RAG_MODE_PROMPT_TEXT_KEY = "rag_mode_prompt_text";

        // ベクトル検索

        public List<VectorSearchRequest> VectorSearchRequests { get; set; } = [];

        // AutoGenPropsRequest
        public AutoGenPropsRequest? AutoGenPropsRequest { get; set; }

        // リクエストを分割するトークン数
        public int SplitTokenCount { get; set; } = 8000;

        // PromptTemplateText
        public string PromptTemplateText { get; set; } = "";

        // RAGを使用するかどうか
        public RAGModeEnum RAGMode { get; set; } = RAGModeEnum.None;

        // RAGを使用する場合のプロンプト
        public string RagModePromptText { get; set; } = "";


        public SplitModeEnum SplitMode = SplitModeEnum.None;

        public string SummarizePromptText = PromptStringResourceJa.Instance.SummarizePromptText;

        // Temperature
        public double Temperature { get; set; } = 0.5;

        // RelatedItems
        public ChatRelatedItems RelatedItems { 
            get; 
            set; 
        } = new();

        // SendRelatedItemsOnlyFirstRequest
        public bool SendRelatedItemsOnlyFirstRequest { get; set; } = true;

        // ToJson
        public string ToJson() {
            return System.Text.Json.JsonSerializer.Serialize(ToDict(), JsonUtil.JsonSerializerOptions);
        }
        // FromJson
        public static ChatSettings FromJson(string json) {
            Dictionary<string, dynamic?> dict = JsonUtil.ParseJson(json);
            if (dict == null) {
                return new ChatSettings();
            }
            return FromDict(dict);
        }
        // ToDict
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                [VECTOR_SEARCH_REQUESTS_JSON_KEY] = VectorSearchRequest.ToListJson(VectorSearchRequests),
                [SPLIT_TOKEN_COUNT_KEY] = SplitTokenCount,
                [PROMPT_TEMPLATE_TEXT_KEY] = PromptTemplateText,
                [RAG_MODE_KEY] = RAGMode.ToString(),
                [RAG_MODE_PROMPT_TEXT_KEY] = RagModePromptText,
                [SPLIT_MODE_KEY] = SplitMode.ToString(),
                [SUMMARIZE_PROMPT_TEXT_KEY] = SummarizePromptText,
                [TEMPERATURE_KEY] = Temperature,
                [RELATED_ITEMS_KEY] = RelatedItems.ToDict(),
                [SEND_RELATED_ITEMS_ONLY_FIRST_REQUEST_KEY] = SendRelatedItemsOnlyFirstRequest
            };
            if (AutoGenPropsRequest != null) {
                dict[AUTOGEN_PROPS_KEY] = AutoGenPropsRequest.ToDict();
            }
            return dict;
        }

        public static ChatSettings FromDict(Dictionary<string, dynamic?> dict) {
            ChatSettings chatSettings = new() {
                VectorSearchRequests = VectorSearchRequest.FromListJson(dict.GetValueOrDefault(VECTOR_SEARCH_REQUESTS_JSON_KEY, "[]")) ?? "[]",
                SplitTokenCount = Convert.ToInt32(dict.GetValueOrDefault(SPLIT_TOKEN_COUNT_KEY, 8000)),
                PromptTemplateText = dict.GetValueOrDefault(PROMPT_TEMPLATE_TEXT_KEY, "") ?? "",
                RAGMode = Enum.Parse<RAGModeEnum>(dict.GetValueOrDefault(RAG_MODE_KEY, RAGModeEnum.None)),
                RagModePromptText = dict.GetValueOrDefault(RAG_MODE_PROMPT_TEXT_KEY, null) ?? "",
                SplitMode = Enum.Parse<SplitModeEnum>(dict.GetValueOrDefault(SPLIT_MODE_KEY, SplitModeEnum.None)),
                SummarizePromptText = dict.GetValueOrDefault(SUMMARIZE_PROMPT_TEXT_KEY, null) ?? PromptStringResourceJa.Instance.SummarizePromptText,
                Temperature = Convert.ToDouble(dict.GetValueOrDefault(TEMPERATURE_KEY, 0.5)),
                RelatedItems = ChatRelatedItems.FromDict(dict.GetValueOrDefault(RELATED_ITEMS_KEY, new Dictionary<string, dynamic?>())),
                SendRelatedItemsOnlyFirstRequest = dict.GetValueOrDefault(SEND_RELATED_ITEMS_ONLY_FIRST_REQUEST_KEY, true)
            };

            if (dict.ContainsKey(AUTOGEN_PROPS_KEY)) {
                chatSettings.AutoGenPropsRequest = AutoGenPropsRequest.FromDict(dict[AUTOGEN_PROPS_KEY]);
            }
            return chatSettings;
        }

    }
}
