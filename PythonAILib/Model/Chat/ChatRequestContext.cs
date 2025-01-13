using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using PythonAILib.Common;
using PythonAILib.Model.AutoGen;
using PythonAILib.Model.VectorDB;
using PythonAILib.Utils.Python;

namespace PythonAILib.Model.Chat {
    // リクエストと共に送信するコンテキスト情報
    public class ChatRequestContext {

        public static JsonSerializerOptions options = new() {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
        };
        // ベクトルDBアイテム

        [JsonPropertyName("vector_db_items")]
        public List<VectorSearchProperty> VectorSearchProperties { get; set; } = [];

        // AutoGenProperties
        [JsonPropertyName("autogen_props")]
        public AutoGenProperties AutoGenProperties { get; set; } = new AutoGenProperties();

        // OpenAIProperties
        [JsonPropertyName("openai_props")]
        public OpenAIProperties OpenAIProperties { get; set; } = new OpenAIProperties();

        // ToDictList
        public Dictionary<string, object> ToDict() {
            Dictionary<string, object> dict = new() {
                { "vector_db_items", VectorSearchProperty.ToDictList(VectorSearchProperties) },
                { "autogen_props", AutoGenProperties.ToDict() },
                { "openai_props", OpenAIProperties.ToDict() },
            };
            return dict;
        }
        // ToJson
        public string ToJson() {
            return JsonSerializer.Serialize(ToDict(), options);
        }

        // CreateDefaultChatRequestContext 
        public static ChatRequestContext CreateDefaultChatRequestContext(List<VectorSearchProperty> vectorSearchProperties , AutoGenGroupChat? groupChat ) {
            PythonAILibManager libManager = PythonAILibManager.Instance;
            AutoGenProperties autoGenProperties;

            if (groupChat != null) {
                autoGenProperties = new AutoGenProperties() {
                    WorkDir = libManager.ConfigParams.GetAutoGenWorkDir(),
                    AutoGenGroupChat = groupChat,
                };
            } else {
                autoGenProperties = new AutoGenProperties() {
                    WorkDir = libManager.ConfigParams.GetAutoGenWorkDir(),
                };
            }

            ChatRequestContext chatRequestContext = new() {
                VectorSearchProperties = vectorSearchProperties,
                OpenAIProperties = libManager.ConfigParams.GetOpenAIProperties(),
                AutoGenProperties = autoGenProperties,
            };

            return chatRequestContext;
        }

    }
}
