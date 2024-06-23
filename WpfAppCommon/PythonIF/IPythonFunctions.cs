using System.Drawing;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using QAChat.Model;
using WpfAppCommon.Model;

namespace WpfAppCommon.PythonIF {

    public class MaskedData {
        public HashSet<MaskedEntity> Entities { get; set; } = [];
        public List<string> BeforeTextList { get; set; } = [];
        public List<string> AfterTextList { get; set; } = [];

        public MaskedData(List<string> beforeList) {

            foreach (var before in BeforeTextList) {
                BeforeTextList.Add(before);
            }
        }
    }
    public class MaskedEntity {
        public string Before { get; set; } = "";
        public string After { get; set; } = "";
        public string Label { get; set; } = "";

    }
    public interface IPythonFunctions {

        public enum VectorDBUpdateMode {
            update,
            delete
        }
        public class GitFileInfo(VectorDBUpdateMode mode, string relativePath, string workDirectory, string repositoryURL) {
            [JsonPropertyName("Mode")]
            public VectorDBUpdateMode Mode { get; set; } = mode;

            [JsonPropertyName("RelativePath")]
            public string RelativePath { get; set; } = relativePath;
            [JsonPropertyName("WorkDirectory")]
            public string WorkDirectory { get; set; } = workDirectory;
            [JsonPropertyName("RepositoryURL")]
            public string RepositoryURL { get; set; } = repositoryURL;

            // 絶対パス
            public string AbsolutePath {
                get {
                    return System.IO.Path.Combine(WorkDirectory, RelativePath);
                }
            }

            public string ToJson() {
                var options = new JsonSerializerOptions {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };
                return JsonSerializer.Serialize(this, options);
            }
        }
        public class ClipboardInfo(IPythonFunctions.VectorDBUpdateMode mode, string id, string content) {

            [JsonPropertyName("Id")]
            public string Id { get; set; } = id;
            [JsonPropertyName("Content")]
            public string Content { get; set; } = content;
            [JsonPropertyName("Mode")]
            public VectorDBUpdateMode Mode { get; set; } = mode;

            public string ToJson() {
                var options = new JsonSerializerOptions {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                    WriteIndented = true
                };
                return JsonSerializer.Serialize(this, options);
            }
        }

        public string ExtractText(string path);
        public string GetMaskedString(string spacyModel, string text);
        public string GetUnmaskedString(string spacyModel, string maskedText);

        public string ExtractTextFromImage(Image image, string tesseractExePath);

        public MaskedData GetMaskedData(string spacyModel, List<string> textList);

        public MaskedData GetUnMaskedData(string spacyModel, List<string> maskedTextList);


        public void OpenAIEmbedding(OpenAIProperties props, string text);

        public string RunScript(string script, string input);

        // 引数として渡した文字列をSpacyで処理してEntityを抽出する
        public HashSet<string> ExtractEntity(string SpacyModel, string text);

        
        public ChatResult OpenAIChat(OpenAIProperties props, ChatRequest chatController);


        public ChatResult LangChainChat(OpenAIProperties props, ChatRequest chatController);


        public void UpdateVectorDBIndex(OpenAIProperties props, GitFileInfo gitFileInfo,  VectorDBItem vectorDBItem);

        public void UpdateVectorDBIndex(OpenAIProperties props, ClipboardInfo contentInfo, VectorDBItem vectorDBItem);

        //テスト用
        public string HelloWorld();
    }
}
