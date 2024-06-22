using PythonLib.Model;

namespace PythonLib.PythonIF {

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

        public string ExtractText(string path);
        public string GetMaskedString(string spacyModel, string text);
        public string GetUnmaskedString(string spacyModel, string maskedText);

        public string ExtractTextFromImage(Image image, string tesseractExePath);

        public MaskedData GetMaskedData(string spacyModel, List<string> textList);

        public MaskedData GetUnMaskedData(string spacyModel, List<string> maskedTextList);


        public void OpenAIEmbedding(string text);

        public string RunScript(string script, string input);

        // 引数として渡した文字列をSpacyで処理してEntityを抽出する
        public HashSet<string> ExtractEntity(string SpacyModel, string text);

        public ChatResult OpenAIChat(Dictionary<string, string> props, string requestJson);

        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory);

        public ChatResult OpenAIChat(string prompt, IEnumerable<ChatItem> chatHistory, Dictionary<string, string> props);

        public ChatResult OpenAIChatWithVision(string prompt, IEnumerable<string> imageFileNames);

        public ChatResult OpenAIChatWithVision(string prompt, IEnumerable<string> imageFileNames, Dictionary<string, string> props);

        public ChatResult LangChainChat(Dictionary<string, string> props, IEnumerable<VectorDBItem> vectorDBItems, string request_json);

        public ChatResult LangChainChat(IEnumerable<VectorDBItem> vectorDBItems, string prompt, IEnumerable<ChatItem> chatHistory);

        public ChatResult LangChainChat(Dictionary<string, string> props, IEnumerable<VectorDBItem> vectorDBItems, string prompt, IEnumerable<ChatItem> chatHistory);

        public void UpdateVectorDBIndex(FileStatus fileStatus, string workingDirPath, string repositoryURL, VectorDBItem vectorDBItem);

        public void UpdateVectorDBIndex(VectorDBUpdateMode mode, ClipboardItem clipboardItem, VectorDBItem vectorDBItem);

        //テスト用
        public string HelloWorld();
    }
}
