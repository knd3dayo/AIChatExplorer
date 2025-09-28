using System.Text.Json;
using System.Text.Json.Serialization;
using LibMain.Common;
using LibMain.PythonIF.Request;
using LibMain.Resources;
using LibMain.Utils.Common;

namespace LibMain.Model.Chat {
    public class ChatSettings {
        public const string SPLIT_MODE_KEY = "split_mode";
        public const string SPLIT_TOKEN_COUNT_KEY = "split_token_count";
        public const string MAX_IMAGES_PER_REQUEST_KEY = "max_images_per_request";
        public const string PROMPT_TEMPLATE_TEXT_KEY = "prompt_template_text";
        public const string SUMMARIZE_PROMPT_TEXT_KEY = "summarize_prompt_text";

        public const string VECTOR_SEARCH_REQUESTS_JSON_KEY = "vector_search_requests_json";
        public const string VECTOR_SEARCH_REQUESTS_KEY = "vector_search_requests";

        public const string RELATED_ITEMS_KEY = "related_items";
        public const string SEND_RELATED_ITEMS_ONLY_FIRST_REQUEST_KEY = "send_related_items_only_first_request";
        public const string RELATED_INFORMATION_PROMPT_TEXT_KEY = "related_information_prompt_text";

        public const string RAG_MODE_KEY = "rag_mode";
        public const string RAG_MODE_PROMPT_TEXT_KEY = "rag_mode_prompt_text";

        public const string TEMPERATURE_KEY = "temperature";

        public ChatSettings() {
            var props = PythonAILibManager.Instance.ConfigParams.GetOpenAIProperties();
            Model = props.OpenAICompletionModel;
        }

        public string Model { get; private set; }

        protected int MaxTokens { get; set; } = 0;

        public double Temperature { get; set; } = 0.5;

        public string ContentText { get; set; } = "";

        // ImageのURLのリスト. data:image/{formatText};base64,{base64String}という形式の文字列のリスト
        public List<string> ImageURLs { get; set; } = [];
        public bool JsonMode { get; set; } = false;


        public List<ChatMessage> ChatHistory { get; set; } = [];


        // ベクトル検索

        public List<VectorSearchRequest> VectorSearchRequests { get; set; } = [];

        // リクエストを分割するトークン数
        public int SplitTokenCount { get; set; } = 8000;

        // MaxImagesPerRequest
        public int MaxImagesPerRequest { get; set; } = 4;

        // PromptTemplateText
        public string PromptTemplateText { get; set; } = "";

        // RAGを使用するかどうか
        public RAGModeEnum RAGMode { get; set; } = RAGModeEnum.None;

        // RAGを使用する場合のプロンプト
        public string RagModePromptText { get; set; } = "";


        public SplitModeEnum SplitMode = SplitModeEnum.None;

        public string SummarizePromptText = PromptStringResourceJa.Instance.SummarizePromptText;

        
        // RelatedItems
        public ChatRelatedItems RelatedItems { 
            get; 
            set; 
        } = new();

        // SendRelatedItemsOnlyFirstRequest
        public bool SendRelatedItemsOnlyFirstRequest { get; set; } = true;


        // CreateEntriesDictList
        public List<Dictionary<string, object>> ToDictVectorDBRequestDict() {
            return RAGMode != RAGModeEnum.None ? VectorSearchRequest.ToDictList(VectorSearchRequests) : [];
        }

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
                [SPLIT_MODE_KEY] = SplitMode.ToString(),
                [SPLIT_TOKEN_COUNT_KEY] = SplitTokenCount,
                [MAX_IMAGES_PER_REQUEST_KEY] = MaxImagesPerRequest,
                [PROMPT_TEMPLATE_TEXT_KEY] = PromptTemplateText,
                [RAG_MODE_KEY] = RAGMode.ToString(),
                [VECTOR_SEARCH_REQUESTS_JSON_KEY] = VectorSearchRequest.ToListJson(VectorSearchRequests),
                [RAG_MODE_PROMPT_TEXT_KEY] = RagModePromptText,
                [SUMMARIZE_PROMPT_TEXT_KEY] = SummarizePromptText,
                [TEMPERATURE_KEY] = Temperature,
                [RELATED_ITEMS_KEY] = RelatedItems.ToDict(),
                [SEND_RELATED_ITEMS_ONLY_FIRST_REQUEST_KEY] = SendRelatedItemsOnlyFirstRequest,
            };

            return dict;
        }

        public static ChatSettings FromDict(Dictionary<string, dynamic?> dict) {
            ChatSettings chatSettings = new() {
                VectorSearchRequests = VectorSearchRequest.FromListJson(dict.GetValueOrDefault(VECTOR_SEARCH_REQUESTS_JSON_KEY, "[]")) ?? "[]",
                SplitTokenCount = Convert.ToInt32(dict.GetValueOrDefault(SPLIT_TOKEN_COUNT_KEY, 8000)),
                MaxImagesPerRequest = Convert.ToInt32(dict.GetValueOrDefault(MAX_IMAGES_PER_REQUEST_KEY, 4)),
                PromptTemplateText = dict.GetValueOrDefault(PROMPT_TEMPLATE_TEXT_KEY, "") ?? "",
                RAGMode = Enum.Parse<RAGModeEnum>(dict.GetValueOrDefault(RAG_MODE_KEY, RAGModeEnum.None)),
                RagModePromptText = dict.GetValueOrDefault(RAG_MODE_PROMPT_TEXT_KEY, null) ?? "",
                SplitMode = Enum.Parse<SplitModeEnum>(dict.GetValueOrDefault(SPLIT_MODE_KEY, SplitModeEnum.None)),
                SummarizePromptText = dict.GetValueOrDefault(SUMMARIZE_PROMPT_TEXT_KEY, null) ?? PromptStringResourceJa.Instance.SummarizePromptText,
                Temperature = Convert.ToDouble(dict.GetValueOrDefault(TEMPERATURE_KEY, 0.5)),
                RelatedItems = ChatRelatedItems.FromDict(dict.GetValueOrDefault(RELATED_ITEMS_KEY, new Dictionary<string, dynamic?>())),
                SendRelatedItemsOnlyFirstRequest = dict.GetValueOrDefault(SEND_RELATED_ITEMS_ONLY_FIRST_REQUEST_KEY, true),

            };

            return chatSettings;
        }

    }
}
